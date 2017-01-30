//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using ICSharpCode.SharpZipLib.GZip;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;

namespace AntdUi {

    public class RssResponse : Response {
        public RssResponse(SyndicationFeed feed) {
            StatusCode = HttpStatusCode.OK;
            ContentType = "application/rss+xml";
            Contents = stream => {
                using (var writer = XmlWriter.Create(stream)) {
                    var formatter = new Atom10FeedFormatter(feed);
                    formatter.WriteTo(writer);
                }
            };
        }
    }

    public static class RequestHandling {

        public static Func<NancyContext, string, Response> AddDirectoryWithExpiresHeader(string requestedPath, string contentPath, TimeSpan expiresTimeSpan, params string[] allowedExtensions) {
            var responseBuilder = StaticContentConventionBuilder.AddDirectory(requestedPath, contentPath, allowedExtensions);
            return (context, root) => {
                var response = responseBuilder(context, root);
                if (response != null) {
                    response.Headers.Add("Expires", DateTime.Now.Add(expiresTimeSpan).ToString("R"));
                    response.Headers["Transfer-Encoding"] = "chunked";
                    response.Headers["Content-Encoding"] = "gzip";
                    response.Headers["Cache-Control"] = "private, max-age=0";
                    var contents = response.Contents;
                    response.Contents = responseStream => {
                        using (var compression = new GZipOutputStream(responseStream)) {
                            compression.SetLevel(9);
                            contents(compression);
                        }
                    };
                }
                return response;
            };
        }

        public static void RegisterCompressionCheck(this IPipelines pipelines) {
            pipelines.AfterRequest.AddItemToEndOfPipeline(CheckForCompression);
        }

        static void CheckForCompression(NancyContext context) {
            if (!RequestIsGzipCompatible(context.Request)) {
                return;
            }

            if (context.Response.StatusCode != HttpStatusCode.OK) {
                return;
            }

            if (!ResponseIsCompatibleMimeType(context.Response)) {
                return;
            }

            if (ContentLengthIsTooSmall(context.Response)) {
                return;
            }

            CompressResponse(context.Response);
        }

        private static void CompressResponse(Response response) {
            response.Headers.Add("Expires", DateTime.Now.Add(TimeSpan.FromDays(365)).ToString("R"));
            response.Headers["Transfer-Encoding"] = "chunked";
            response.Headers["Content-Encoding"] = "gzip";
            response.Headers["Cache-Control"] = "private, max-age=0";
            response.ContentType = "text/html; charset=utf-8";
            var contents = response.Contents;
            response.Contents = responseStream => {
                using (var compression = new GZipOutputStream(responseStream)) {
                    compression.SetLevel(9);
                    contents(compression);
                }
            };
        }

        private static bool ContentLengthIsTooSmall(Response response) {
            string contentLength;
            if (!response.Headers.TryGetValue("Content-Length", out contentLength)) return false;
            var length = long.Parse(contentLength);
            return length < 4096;
        }

        public static List<string> ValidMimes = new List<string> {
                                                    "text/css",
                                                    "text/html",
                                                    "text/plain",
                                                    "image/png",
                                                    "application/xml",
                                                    "application/json",
                                                    "application/xaml+xml",
                                                    "application/x-javascript",
                                                    "application/javascript",
                                                    "application/atom+xml",
                                                    "application/rss+xml"
                                                };

        private static bool ResponseIsCompatibleMimeType(Response response) {
            return ValidMimes.Any(x => x == response.ContentType);
        }

        private static bool RequestIsGzipCompatible(Request request) {
            return request.Headers.AcceptEncoding.Any(x => x.Contains("gzip"));
        }
    }
}
