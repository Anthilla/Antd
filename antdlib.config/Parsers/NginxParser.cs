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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using antdlib.models;

namespace antdlib.config.Parsers {
    public class NginxParser {

        private static string CaptureGroup(string sourceText, string pattern) {
            var regex = new Regex(pattern);
            var matchedGroups = regex.Match(sourceText).Groups;
            var capturedText = matchedGroups[1].Value;
            return capturedText;
        }

        public static NginxConfigurationModel ParseOptions(NginxConfigurationModel nginxConfigurationModel, string text) {
            nginxConfigurationModel.User = CaptureGroup(text, "user[\\s]+([\\d\\w]+)");
            nginxConfigurationModel.Group = CaptureGroup(text, "user[\\s]+[\\d\\w]+[\\s]+([\\d\\w]+);");
            nginxConfigurationModel.Processes = CaptureGroup(text, "worker_processes[\\s]+[\\d]+;");
            nginxConfigurationModel.FileLimit = CaptureGroup(text, "worker_rlimit_nofile[\\s]+[\\d]+;");
            nginxConfigurationModel.ErrorLog = CaptureGroup(text, "error_log[\\s]+[\\w\\/.\\s\\d]+;");
            return nginxConfigurationModel;
        }

        //(events[\s]+{[\s]+[\w\s_;]+[\s]+})
        public static NginxConfigurationModel ParseEventsOptions(NginxConfigurationModel nginxConfigurationModel, string text) {
            var regex = new Regex("(events[\\s]+{[\\s]+[\\w\\s_;]+[\\s]+})");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            nginxConfigurationModel.EventsWorkerConnections = CaptureGroup(capturedText, "worker_connections[\\s]+[\\d]+;");
            nginxConfigurationModel.EventsMultiAccept = CaptureGroup(capturedText, "multi_accept[\\s]+[\\w]+;");
            nginxConfigurationModel.EventsUse = CaptureGroup(capturedText, "use[\\s]+[\\w]+;");
            nginxConfigurationModel.EventsAcceptMutex = CaptureGroup(capturedText, "accept_mutex[\\s]+[\\w]+;");
            return nginxConfigurationModel;
        }

        //(upstream[\s]+[\w\d]+ {[\s]+[\w\s\d.:=]+;[\s]+})
        public static List<NginxUpstream> ParseUpstream(string text) {
            var list = new List<NginxUpstream>();
            var regex = new Regex("(upstream[\\s]+[\\w\\d]+ {[\\s]+[\\w\\s\\d.:=]+;[\\s]+})");
            var matches = regex.Matches(text);
            foreach(var match in matches) {
                var name = CaptureGroup(match.ToString(), "upstream[\\s]+([\\w\\d]+) {");
                var server = CaptureGroup(match.ToString(), "upstream[\\s]+[\\w\\d]+[\\s]+{[\\s]+server[\\s]+([\\d.:]+)");
                var acl = new NginxUpstream {
                    Name = name,
                    Server = server
                };
                list.Add(acl);
            }
            return list;
        }

        //(http[\s]+{[\s\w\d\-.'$\[\]"+\\()\?\!*={}:#;\/]+)
        public static NginxHttpProtocol ParseHttpProtocol(string text) {
            var regex = new Regex("(http[\\s]+{[\\s\\w\\d\\-.\'$\\[\\]\"+\\\\()\\?\\!*={}:#;\\/]+)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            var httpProtocol = new NginxHttpProtocol {
                Aio = CaptureGroup(capturedText, "aio[\\s]+([\\w\\d]+);"),
                Directio = CaptureGroup(capturedText, "directio[\\s]+([\\w\\d]+);"),
                AccessLog = CaptureGroup(capturedText, "access_log[\\s]+([\\w\\d]+);"),
                KeepaliveTimeout = CaptureGroup(capturedText, "keepalive_timeout[\\s]+([\\w\\d]+);"),
                KeepaliveRequests = CaptureGroup(capturedText, "keepalive_requests[\\s]+([\\w\\d]+);"),
                Sendfile = CaptureGroup(capturedText, "sendfile[\\s]+([\\w\\d]+);"),
                SendfileMaxChunk = CaptureGroup(capturedText, "sendfile_max_chunk[\\s]+([\\w\\d]+);"),
                TcpNopush = CaptureGroup(capturedText, "tcp_nopush[\\s]+([\\w\\d]+);"),
                TcpNodelay = CaptureGroup(capturedText, "tcp_nodelay[\\s]+([\\w\\d]+);"),
                DefaultType = CaptureGroup(capturedText, "default_type[\\s]+([\\w\\d\\/.]+);"),
                RequestPoolSize = CaptureGroup(capturedText, "request_pool_size[\\s]+([\\w\\d\\/.]+);"),
                OutputBuffers = CaptureGroup(capturedText, "output_buffers 1[\\s]+([\\w\\d]+);"),
                PostponeOutput = CaptureGroup(capturedText, "postpone_output[\\s]+([\\w\\d]+);"),
                ResetTimedoutConnection = CaptureGroup(capturedText, "reset_timedout_connection[\\s]+([\\w\\d]+);"),
                SendTimeout = CaptureGroup(capturedText, "send_timeout[\\s]+([\\w\\d]+);"),
                ServerTokens = CaptureGroup(capturedText, "server_tokens[\\s]+([\\w\\d]+);"),
                ClientHeaderBufferSize = CaptureGroup(capturedText, "client_header_buffer_size[\\s]+([\\w\\d]+);"),
                ClientHeaderTimeout = CaptureGroup(capturedText, "client_header_timeout[\\s]+([\\w\\d]+);"),
                ClientBodyBufferSize = CaptureGroup(capturedText, "client_body_buffer_size[\\s]+([\\w\\d]+);"),
                ClientBodyTimeout = CaptureGroup(capturedText, "client_body_timeout[\\s]+([\\w\\d]+);"),
                LargeClientHeaderBuffers = CaptureGroup(capturedText, "large_client_header_buffers 4[\\s]+([\\w\\d]+);"),
                Gzip = CaptureGroup(capturedText, "gzip[\\s]+([\\w\\d]+);"),
                GzipMinLength = CaptureGroup(capturedText, "gzip_min_length[\\s]+([\\w\\d]+);"),
                GzipProxied = CaptureGroup(capturedText, "gzip_proxied[\\s]+([\\w\\d\\s\\-]+);"),
                GzipBuffers = CaptureGroup(capturedText, "gzip_buffers 256[\\s]+([\\w\\d]+);"),
                GzipCompLevel = CaptureGroup(capturedText, "gzip_comp_level[\\s]+([\\w\\d]+);"),
                GzipHttpVersion = CaptureGroup(capturedText, "gzip_http_version[\\s]+([\\w\\d]+);"),
                GzipVary = CaptureGroup(capturedText, "gzip_vary[\\s]+([\\w\\d]+);"),
                OpenFileCacheMax = CaptureGroup(capturedText, "open_file_cache max=([\\w\\d]+)"),
                OpenFileCacheInactive = CaptureGroup(capturedText, "open_file_cache max=[\\w\\d]+ inactive=([\\w\\d]+);"),
                OpenFileCacheValid = CaptureGroup(capturedText, "open_file_cache_valid[\\s]+([\\w\\d]+);"),
                OpenFileCacheMinUses = CaptureGroup(capturedText, "open_file_cache_min_uses[\\s]+([\\w\\d]+);"),
                OpenFileCacheErrors = CaptureGroup(capturedText, "open_file_cache_errors[\\s]+([\\w\\d]+);"),
                ServerNamesHashBucketSize = CaptureGroup(capturedText, "server_names_hash_bucket_size[\\s]+([\\w\\d]+);")
            };
            return httpProtocol;
        }

        //(server[\s]+{[\s]+[\w\s\d;.\/:$#]+location[\s]+[\w\/][\s]+{[\s]+[\w\s\d:\/;$\-".]+[\s]+[}]*[\s]+[location]*[\/\s\w]*[\s]*[{]*[\s]*[\w\s\d:\/;$\-".]*)
        public static List<NginxServer> ParseServer(string text) {
            var list = new List<NginxServer>();
            var regex = new Regex("(server[\\s]+{[\\s]+[\\w\\s\\d;.\\/:$#]+location[\\s]+[\\w\\/][\\s]+{[\\s]+[\\w\\s\\d:\\/;$\\-\".]+[\\s]+[}]*[\\s]+[location]*[\\/\\s\\w]*[\\s]*[{]*[\\s]*[\\w\\s\\d:\\/;$\\-\".]*)");
            var matches = regex.Matches(text);
            foreach(var match in matches) {
                var listen = CaptureGroup(match.ToString(), "listen[\\s]+([\\w\\d\\s]+);");
                var serverName = CaptureGroup(match.ToString(), "server_name[\\s]+([\\w\\d.]+);");
                var serverTokens = CaptureGroup(match.ToString(), "server_tokens[\\s]+([\\w\\d.]+);");
                var root = CaptureGroup(match.ToString(), "root[\\s]+([\\w\\d.\\/]+);");
                var clientMaxBodySize = CaptureGroup(match.ToString(), "client_max_body_size[\\s]+([\\w\\d.\\/]+);");
                var sslCertificate = CaptureGroup(match.ToString(), "ssl_certificate[\\s]+([\\w\\d.\\/]+);");
                var sslTrustedCertificate = CaptureGroup(match.ToString(), "ssl_trusted_certificate[\\s]+([\\w\\d.\\/]+);");
                var sslCertificateKey = CaptureGroup(match.ToString(), "ssl_certificate_key[\\s]+([\\w\\d.\\/]+);");
                var returnRedirect = CaptureGroup(match.ToString(), "");
                var server = new NginxServer {
                    Listen = listen,
                    ServerName = serverName,
                    ServerTokens = serverTokens,
                    Root = root,
                    ClientMaxBodySize = clientMaxBodySize,
                    SslCertificate = sslCertificate,
                    SslTrustedCertificate = sslTrustedCertificate,
                    SslCertificateKey = sslCertificateKey,
                    ReturnRedirect = returnRedirect,
                    Locations = ParseLocation(match.ToString())
                };
                list.Add(server);
            }
            return list;
        }

        //(location[\s]+[\/\w]+[\s]+{[\s]+[\w\s_\/:;$\-".]+[\s]+})
        public static List<NginxLocation> ParseLocation(string text) {
            var list = new List<NginxLocation>();
            var regex = new Regex("(location[\\s]+[\\/\\w]+[\\s]+{[\\s]+[\\w\\s_\\/:;$\\-\".]+[\\s]+})");
            var matches = regex.Matches(text);
            foreach(var match in matches) {
                var path = CaptureGroup(match.ToString(), "location[\\s]+[\\/\\w\\d]+[\\s]+{");
                var autoindex = CaptureGroup(match.ToString(), "autoindex[\\s]+([\\w\\d]+);");
                var root = CaptureGroup(match.ToString(), "root[\\s]+([\\w\\d\\/._-]+);");
                var mp4 = CaptureGroup(match.ToString(), "mp4;");
                var proxyPass = CaptureGroup(match.ToString(), "proxy_pass[\\s]+([\\w\\d\\/._\\-:]+);");
                var proxysslsession = CaptureGroup(match.ToString(), "proxy_ssl_session_reuse[\\s]+([\\w\\d\\/._\\-:]+);");
                var location = new NginxLocation {
                    Path = path,
                    Autoindex = autoindex,
                    Root = root,
                    Mp4 = !string.IsNullOrEmpty(mp4),
                    ProxyPass = proxyPass,
                    ProxySslSessionReuse = proxysslsession
                };

                list.Add(location);
            }
            return list;
        }
    }
}
