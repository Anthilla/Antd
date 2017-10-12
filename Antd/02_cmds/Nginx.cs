using System.Collections.Generic;
using System.IO;
using anthilla.core;
using Antd.parsing;
using Antd.models;

namespace Antd.cmds {

    public class Nginx {

        private const string ServiceName = "nginx.service";
        private const string MainFilePath = "/etc/nginx/nginx.conf";
        private const string MainFilePathBackup = "/etc/nginx/.named.conf";

        public static void Parse() {
            if(!File.Exists(MainFilePath)) {
                return;
            }
            var content = File.ReadAllText(MainFilePath);
            var model = new NginxModel { Active = false };
            model = NginxParser.ParseOptions(model, content);
            model = NginxParser.ParseEventsOptions(model, content);
            var upstreams = NginxParser.ParseUpstream(content);
            model.Upstreams = upstreams;
            var http = NginxParser.ParseHttpProtocol(content);
            model.Http = http;
            var servers = NginxParser.ParseServer(content);
            model.Servers = servers;
            ConsoleLogger.Log("[dhcpd] import existing configuration");
        }

        public static void Apply() {
            var options = Application.CurrentConfiguration.Services.Nginx;
            if(options == null) {
                return;
            }
            Stop();
            #region [    named.conf generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string>();

            if(!string.IsNullOrEmpty(options.User) && !string.IsNullOrEmpty(options.Group)) { lines.Add($"user {options.User} {options.Group};"); }
            if(!string.IsNullOrEmpty(options.Processes)) { lines.Add($"worker_processes {options.Processes};"); }
            if(!string.IsNullOrEmpty(options.FileLimit)) { lines.Add($"worker_rlimit_nofile {options.FileLimit};"); }
            if(!string.IsNullOrEmpty(options.ErrorLog)) { lines.Add($"error_log {options.ErrorLog};"); }
            lines.Add("");
            lines.Add("events {");
            if(!string.IsNullOrEmpty(options.EventsWorkerConnections)) { lines.Add($"    worker_connections {options.EventsWorkerConnections};"); }
            if(!string.IsNullOrEmpty(options.EventsMultiAccept)) { lines.Add($"    multi_accept {options.EventsMultiAccept};"); }
            if(!string.IsNullOrEmpty(options.EventsUse)) { lines.Add($"    use {options.EventsUse};"); }
            if(!string.IsNullOrEmpty(options.EventsAcceptMutex)) { lines.Add($"    accept_mutex {options.EventsAcceptMutex};"); }
            lines.Add("}");
            lines.Add("");
            lines.Add("http {");
            if(!string.IsNullOrEmpty(options.Http.Aio)) { lines.Add($"    aio {options.Http.Aio};"); }
            if(!string.IsNullOrEmpty(options.Http.Directio)) { lines.Add($"    directio {options.Http.Directio};"); }
            if(!string.IsNullOrEmpty(options.Http.AccessLog)) { lines.Add($"    access_log {options.Http.AccessLog};"); }
            if(!string.IsNullOrEmpty(options.Http.KeepaliveTimeout)) { lines.Add($"    keepalive_timeout {options.Http.KeepaliveTimeout};"); }
            if(!string.IsNullOrEmpty(options.Http.KeepaliveRequests)) { lines.Add($"    keepalive_requests {options.Http.KeepaliveRequests};"); }
            if(!string.IsNullOrEmpty(options.Http.Sendfile)) { lines.Add($"    sendfile {options.Http.Sendfile};"); }
            if(!string.IsNullOrEmpty(options.Http.SendfileMaxChunk)) { lines.Add($"    sendfile_max_chunk {options.Http.SendfileMaxChunk};"); }
            if(!string.IsNullOrEmpty(options.Http.TcpNopush)) { lines.Add($"    tcp_nopush {options.Http.TcpNopush};"); }
            if(!string.IsNullOrEmpty(options.Http.TcpNodelay)) { lines.Add($"    tcp_nodelay {options.Http.TcpNodelay};"); }
            if(!string.IsNullOrEmpty(options.Http.Include)) { lines.Add($"    include {options.Http.Include};"); }
            if(!string.IsNullOrEmpty(options.Http.DefaultType)) { lines.Add($"    default_type {options.Http.DefaultType};"); }
            if(!string.IsNullOrEmpty(options.Http.LogFormat)) { lines.Add($"    log_format {options.Http.LogFormat};"); }
            if(!string.IsNullOrEmpty(options.Http.RequestPoolSize)) { lines.Add($"    request_pool_size {options.Http.RequestPoolSize};"); }
            if(!string.IsNullOrEmpty(options.Http.OutputBuffers)) { lines.Add($"    output_buffers 1 {options.Http.OutputBuffers};"); }
            if(!string.IsNullOrEmpty(options.Http.PostponeOutput)) { lines.Add($"    postpone_output {options.Http.PostponeOutput};"); }
            if(!string.IsNullOrEmpty(options.Http.ResetTimedoutConnection)) { lines.Add($"    reset_timedout_connection {options.Http.ResetTimedoutConnection};"); }
            if(!string.IsNullOrEmpty(options.Http.SendTimeout)) { lines.Add($"    send_timeout {options.Http.SendTimeout};"); }
            if(!string.IsNullOrEmpty(options.Http.ServerTokens)) { lines.Add($"    server_tokens {options.Http.ServerTokens};"); }
            if(!string.IsNullOrEmpty(options.Http.ClientHeaderBufferSize)) { lines.Add($"    client_header_buffer_size {options.Http.ClientHeaderBufferSize};"); }
            if(!string.IsNullOrEmpty(options.Http.ClientHeaderTimeout)) { lines.Add($"    client_header_timeout {options.Http.ClientHeaderTimeout};"); }
            if(!string.IsNullOrEmpty(options.Http.ClientBodyBufferSize)) { lines.Add($"    client_body_buffer_size {options.Http.ClientBodyBufferSize};"); }
            if(!string.IsNullOrEmpty(options.Http.ClientBodyTimeout)) { lines.Add($"    client_body_timeout {options.Http.ClientBodyTimeout};"); }
            if(!string.IsNullOrEmpty(options.Http.LargeClientHeaderBuffers)) { lines.Add($"    large_client_header_buffers {options.Http.LargeClientHeaderBuffers};"); }
            if(!string.IsNullOrEmpty(options.Http.Gzip)) { lines.Add($"    gzip {options.Http.Gzip};"); }
            if(!string.IsNullOrEmpty(options.Http.GzipMinLength)) { lines.Add($"    gzip_min_length {options.Http.GzipMinLength};"); }
            if(!string.IsNullOrEmpty(options.Http.GzipProxied)) { lines.Add($"    gzip_proxied {options.Http.GzipProxied};"); }
            if(!string.IsNullOrEmpty(options.Http.GzipTypes)) { lines.Add($"    gzip_types {options.Http.GzipTypes};"); }
            if(!string.IsNullOrEmpty(options.Http.GzipBuffers)) { lines.Add($"    gzip_buffers 256 {options.Http.GzipBuffers};"); }
            if(!string.IsNullOrEmpty(options.Http.GzipCompLevel)) { lines.Add($"    gzip_comp_level {options.Http.GzipCompLevel};"); }
            if(!string.IsNullOrEmpty(options.Http.GzipHttpVersion)) { lines.Add($"    gzip_http_version {options.Http.GzipHttpVersion};"); }
            if(!string.IsNullOrEmpty(options.Http.GzipVary)) { lines.Add($"    gzip_vary {options.Http.GzipHttpVersion};"); }
            if(!string.IsNullOrEmpty(options.Http.GzipDisable)) { lines.Add($"    gzip_disable \"{options.Http.GzipDisable}\";"); }
            if(!string.IsNullOrEmpty(options.Http.OpenFileCacheMax) && !string.IsNullOrEmpty(options.Http.OpenFileCacheInactive)) { lines.Add($"    open_file_cache max={options.Http.OpenFileCacheMax} inactive={options.Http.OpenFileCacheInactive};"); }
            if(!string.IsNullOrEmpty(options.Http.OpenFileCacheValid)) { lines.Add($"    open_file_cache_valid {options.Http.OpenFileCacheValid};"); }
            if(!string.IsNullOrEmpty(options.Http.OpenFileCacheMinUses)) { lines.Add($"    open_file_cache_min_uses {options.Http.OpenFileCacheMinUses};"); }
            if(!string.IsNullOrEmpty(options.Http.OpenFileCacheErrors)) { lines.Add($"    open_file_cache_min_uses {options.Http.OpenFileCacheErrors};"); }
            if(!string.IsNullOrEmpty(options.Http.ServerNamesHashBucketSize)) { lines.Add($"    server_names_hash_bucket_size {options.Http.ServerNamesHashBucketSize};"); }
            lines.Add("");
            foreach(var upstr in options.Upstreams) {
                lines.Add($"upstream {upstr.Name} {{ server {upstr.Server} fail_timeout=0; }}");
            }
            lines.Add("");
            foreach(var s in options.Servers) {
                lines.Add("server {");
                if(!string.IsNullOrEmpty(s.Listen)) { lines.Add($"    listen {s.Listen};"); }
                if(!string.IsNullOrEmpty(s.ServerName)) { lines.Add($"    server_name {s.ServerName};"); }
                if(!string.IsNullOrEmpty(s.ServerTokens)) { lines.Add($"    server_tokens {s.ServerTokens};"); }
                if(!string.IsNullOrEmpty(s.Root)) { lines.Add($"    root {s.Root};"); }
                if(!string.IsNullOrEmpty(s.ClientMaxBodySize)) { lines.Add($"    client_max_body_size {s.ClientMaxBodySize};"); }
                if(!string.IsNullOrEmpty(s.ReturnRedirect)) { lines.Add($"    return {s.ReturnRedirect};"); }
                if(!string.IsNullOrEmpty(s.SslCertificate)) { lines.Add($"    ssl_certificate {s.SslCertificate};"); }
                if(!string.IsNullOrEmpty(s.SslTrustedCertificate)) { lines.Add($"    ssl_trusted_certificate {s.SslTrustedCertificate};"); }
                if(!string.IsNullOrEmpty(s.SslCertificateKey)) { lines.Add($"    ssl_certificate_key {s.SslCertificateKey};"); }
                if(!string.IsNullOrEmpty(s.AccessLog)) { lines.Add($"    access_log {s.AccessLog};"); }
                if(!string.IsNullOrEmpty(s.ErrorLog)) { lines.Add($"    error_log {s.ErrorLog};"); }
                lines.Add("");
                foreach(var l in s.Locations) {
                    lines.Add($"    location {l.Path} {{");
                    if(!string.IsNullOrEmpty(l.ProxyPass)) { lines.Add($"        proxy_pass {l.ProxyPass};"); }
                    if(!string.IsNullOrEmpty(l.ProxyPassHeader)) { lines.Add($"        proxy_pass_header {l.ProxyPassHeader};"); }
                    if(!string.IsNullOrEmpty(l.ProxyReadTimeout)) { lines.Add($"        proxy_read_timeout {l.ProxyReadTimeout};"); }
                    if(!string.IsNullOrEmpty(l.ProxyConnectTimeout)) { lines.Add($"        proxy_connect_timeout {l.ProxyConnectTimeout};"); }
                    lines.Add("        proxy_set_header Host $host;");
                    lines.Add("        proxy_set_header X-Frame-Options SAMEORIGIN;");
                    lines.Add("        proxy_set_header X-Real-IP $remote_addr;");
                    lines.Add("        proxy_set_header X-Forwarded-Proto $scheme;");
                    lines.Add("        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;");
                    lines.Add("        proxy_set_header Connection \"\";");
                    if(!string.IsNullOrEmpty(l.ProxyBuffering)) { lines.Add($"        proxy_buffering {l.ProxyBuffering};"); }
                    if(!string.IsNullOrEmpty(l.ClientMaxBodySize)) { lines.Add($"        client_max_body_size {l.ClientMaxBodySize};"); }
                    if(!string.IsNullOrEmpty(l.ProxyRedirect)) { lines.Add($"        proxy_redirect {l.ProxyRedirect};"); }
                    if(!string.IsNullOrEmpty(l.ProxyHttpVersion)) { lines.Add($"        proxy_http_version {l.ProxyHttpVersion};"); }
                    if(!string.IsNullOrEmpty(l.Aio)) { lines.Add($"        aio {l.Aio};"); }
                    if(!string.IsNullOrEmpty(l.ProxySslSessionReuse)) { lines.Add($"        proxy_ssl_session_reuse {l.ProxySslSessionReuse};"); }
                    lines.Add("    }");
                    lines.Add("");
                }
                lines.Add("}");
            }
            lines.Add("}");
            FileWithAcl.WriteAllLines(MainFilePath, lines, "644", "nginx", "nginx");
            SetParametersFiles();
            #endregion
            Start();
        }

        public static void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[nginx] stop");
        }

        public static void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[nginx] start");
        }

        private static void SetParametersFiles() {
            //var fastcgiconf = "/etc/nginx/fastcgi.conf";
            //if(!File.Exists(fastcgiconf)) {
            //    var fastcgiconfLines = new List<string>();
            //    FileWithAcl.WriteAllLines(fastcgiconf, fastcgiconfLines, "644", "nginx", "nginx");
            //}
        }
    }
}