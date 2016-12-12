using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace Antd.Websocket.Connections {
    public class HttpConnection : IConnection {
        private readonly NetworkStream _networkStream;
        private readonly string _path;
        private readonly string _webRoot;

        public HttpConnection(NetworkStream networkStream, string path, string webRoot) {
            _networkStream = networkStream;
            _path = path;
            _webRoot = webRoot;
        }

        private static bool IsDirectory(string file) {
            if (!Directory.Exists(file)) return false;
            var attr = File.GetAttributes(file);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public void Respond() {
            var file = GetSafePath(_path);
            if (IsDirectory(file)) {
                file += "index.html";
            }
            var fi = new FileInfo(file);
            if (fi.Exists) {
                var ext = fi.Extension.ToLower();
                string contentType;
                if (MimeTypes.Instance.TryGetValue(ext, out contentType)) {
                    var bytes = File.ReadAllBytes(fi.FullName);
                    RespondSuccess(contentType, bytes.Length);
                    _networkStream.Write(bytes, 0, bytes.Length);
                    Trace.TraceInformation("Served file: " + file);
                }
                else {
                    RespondMimeTypeFailure(file);
                }
            }
            else {
                RespondNotFoundFailure(file);
            }
        }

        private string GetSafePath(string path) {
            path = path.Trim().Replace("/", "\\");
            if (path.Contains("..") || !path.StartsWith("\\") || path.Contains(":")) {
                return string.Empty;
            }
            var file = _webRoot + path;
            return file;
        }

        public void RespondMimeTypeFailure(string file) {
            HttpHelper.WriteHttpHeader("415 Unsupported Media Type", _networkStream);
            Trace.TraceWarning("File extension not found MimeTypes.config: " + file);
        }

        public void RespondNotFoundFailure(string file) {
            HttpHelper.WriteHttpHeader("HTTP/1.1 404 Not Found", _networkStream);
            Trace.TraceInformation("File not found: " + file);
        }

        public void RespondSuccess(string contentType, int contentLength) {
            var response = "HTTP/1.1 200 OK" + Environment.NewLine +
                              "Content-Type: " + contentType + Environment.NewLine +
                              "Content-Length: " + contentLength + Environment.NewLine +
                              "Connection: close";
            HttpHelper.WriteHttpHeader(response, _networkStream);
        }

        public void Dispose() {
        }
    }
}
