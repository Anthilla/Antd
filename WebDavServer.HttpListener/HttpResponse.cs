﻿using NWebDav.Server.Http;
using System.IO;
using System.Net;

namespace NWebDav.Server.HttpListener {
    public class HttpResponse : IHttpResponse {
        private readonly HttpListenerResponse _response;

        internal HttpResponse(HttpListenerResponse response) {
            _response = response;
        }

        public int Status {
            get { return _response.StatusCode; }
            set { _response.StatusCode = value; }
        }

        public string StatusDescription {
            get { return _response.StatusDescription; }
            set { _response.StatusDescription = value; }
        }

        public void SetHeaderValue(string header, string value) {
            switch(header) {
                case "Content-Length":
                    _response.ContentLength64 = long.Parse(value);
                    break;

                default:
                    _response.Headers[header] = value;
                    break;
            }
        }

        public Stream Stream => _response.OutputStream;
    }
}