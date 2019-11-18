using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Nancy;

namespace anthilla.core {

    public static class NancyExtensions {

        public static Response FromPartialFile(this IResponseFormatter f, Request req, string path, string contentType, string headerKey = "Range") {
            //return f.FromPartialStream(req, new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), contentType, headerKey);
            return f.FromPartialStream(req, new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), contentType, headerKey);
        }

        public static Response FromPartialStream(this IResponseFormatter f, Request req, Stream stream, string contentType, string headerKey = "Range") {
            var len = stream.Length;
            var res = f.FromStream(stream, contentType)
                .WithHeader("Cache-Control", "no-cache")
                .WithHeader("Access-Control-Allow-Origin", "*")
                .WithHeader("connection", "keep-alive")
                .WithHeader("accept-ranges", "bytes");
            res.StatusCode = HttpStatusCode.PartialContent;
            long startI = 0;
            foreach(var s in req.Headers[headerKey]) {
                var start = s.Split('=')[1];
                var m = Regex.Match(start, @"(\d+)-(\d+)?");
                start = m.Groups[1].Value;
                var end = len - 1;
                if(m.Groups[2] != null && !string.IsNullOrWhiteSpace(m.Groups[2].Value)) {
                    end = Convert.ToInt64(m.Groups[2].Value);
                }

                startI = Convert.ToInt64(start);
                var length = len - startI;
                res.WithHeader("Cache-Control", "no-cache");
                res.WithHeader("Access-Control-Allow-Origin", "*");
                res.WithHeader("content-range", "bytes " + start + "-" + end + "/" + len);
                res.WithHeader("content-length", length.ToString(CultureInfo.InvariantCulture));
            }
            stream.Seek(startI, SeekOrigin.Begin);
            return res;
        }

        public static Response FromPartialFileAsIs(this IResponseFormatter f, Request req, string path, string contentType) {
            return f.FromPartialStreamAsIs(req, new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), contentType);
        }

        public static Response FromPartialStreamAsIs(this IResponseFormatter f, Request req, Stream stream, string contentType) {
            var len = stream.Length;
            var res = f.FromStream(stream, contentType)
                .WithHeader("Cache-Control", "no-cache")
                .WithHeader("Access-Control-Allow-Origin", "*")
                .WithHeader("connection", "keep-alive")
                .WithHeader("accept-ranges", "bytes");
            res.StatusCode = HttpStatusCode.PartialContent;
            long start = 0;
            var end = len - 1;
            var length = len - start;
            res.WithHeader("Cache-Control", "no-cache");
            res.WithHeader("Access-Control-Allow-Origin", "*");
            res.WithHeader("content-range", "bytes " + start + "-" + end + "/" + len);
            res.WithHeader("content-length", length.ToString(CultureInfo.InvariantCulture));
            stream.Seek(0, SeekOrigin.Begin);
            return res;
        }
    }
}
