using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using antdlib.config.Parsers;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public static class NginxConfiguration {

        private static NginxConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/nginx.conf";
        private const string ServiceName = "nginx.service";
        private const string MainFilePath = "/etc/nginx/nginx.conf";
        private const string MainFilePathBackup = "/etc/nginx/.named.conf";

        private static NginxConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new NginxConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<NginxConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new NginxConfigurationModel();
            }
        }

        public static void Save(NginxConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[nginx] configuration saved");
        }

        public static NginxConfigurationModel Get() {
            return Load();
        }

        public static void TryImport() {
            if(File.Exists(CfgFile)) {
                return;
            }
            if(!File.Exists(MainFilePath)) {
                return;
            }
            var text = File.ReadAllText(MainFilePath);
            var model = Parse(text);
            Save(model);
            ConsoleLogger.Log("[nginx] import existing configuration");
        }

        private static NginxConfigurationModel Parse(string text) {
            var model = new NginxConfigurationModel { IsActive = false };
            model = NginxParser.ParseOptions(model, text);
            model = NginxParser.ParseEventsOptions(model, text);
            var upstreams = NginxParser.ParseUpstream(text);
            model.Upstreams = upstreams;
            var http = NginxParser.ParseHttpProtocol(text);
            model.Http = http;
            var servers = NginxParser.ParseServer(text);
            model.Servers = servers;
            return model;
        }

        public static void Set() {
            Stop();
            #region [    named.conf generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var o = ServiceModel;
            var lines = new List<string>();

            if(!string.IsNullOrEmpty(o.User) && !string.IsNullOrEmpty(o.Group)) { lines.Add($"user {o.User} {o.Group};"); }
            if(!string.IsNullOrEmpty(o.Processes)) { lines.Add($"worker_processes {o.Processes};"); }
            if(!string.IsNullOrEmpty(o.FileLimit)) { lines.Add($"worker_rlimit_nofile {o.FileLimit};"); }
            if(!string.IsNullOrEmpty(o.ErrorLog)) { lines.Add($"error_log {o.ErrorLog};"); }
            lines.Add("");
            lines.Add("events {");
            if(!string.IsNullOrEmpty(o.EventsWorkerConnections)) { lines.Add($"    worker_connections {o.EventsWorkerConnections};"); }
            if(!string.IsNullOrEmpty(o.EventsMultiAccept)) { lines.Add($"    multi_accept {o.EventsMultiAccept};"); }
            if(!string.IsNullOrEmpty(o.EventsUse)) { lines.Add($"    use {o.EventsUse};"); }
            if(!string.IsNullOrEmpty(o.EventsAcceptMutex)) { lines.Add($"    accept_mutex {o.EventsAcceptMutex};"); }
            lines.Add("}");
            lines.Add("");
            lines.Add("http {");
            if(!string.IsNullOrEmpty(o.Http.Aio)) { lines.Add($"    aio {o.Http.Aio};"); }
            if(!string.IsNullOrEmpty(o.Http.Directio)) { lines.Add($"    directio {o.Http.Directio};"); }
            if(!string.IsNullOrEmpty(o.Http.AccessLog)) { lines.Add($"    access_log {o.Http.AccessLog};"); }
            if(!string.IsNullOrEmpty(o.Http.KeepaliveTimeout)) { lines.Add($"    keepalive_timeout {o.Http.KeepaliveTimeout};"); }
            if(!string.IsNullOrEmpty(o.Http.KeepaliveRequests)) { lines.Add($"    keepalive_requests {o.Http.KeepaliveRequests};"); }
            if(!string.IsNullOrEmpty(o.Http.Sendfile)) { lines.Add($"    sendfile {o.Http.Sendfile};"); }
            if(!string.IsNullOrEmpty(o.Http.SendfileMaxChunk)) { lines.Add($"    sendfile_max_chunk {o.Http.SendfileMaxChunk};"); }
            if(!string.IsNullOrEmpty(o.Http.TcpNopush)) { lines.Add($"    tcp_nopush {o.Http.TcpNopush};"); }
            if(!string.IsNullOrEmpty(o.Http.TcpNodelay)) { lines.Add($"    tcp_nodelay {o.Http.TcpNodelay};"); }
            if(!string.IsNullOrEmpty(o.Http.Include)) { lines.Add($"    include {o.Http.Include};"); }
            if(!string.IsNullOrEmpty(o.Http.DefaultType)) { lines.Add($"    default_type {o.Http.DefaultType};"); }
            if(!string.IsNullOrEmpty(o.Http.LogFormat)) { lines.Add($"    log_format {o.Http.LogFormat};"); }
            if(!string.IsNullOrEmpty(o.Http.RequestPoolSize)) { lines.Add($"    request_pool_size {o.Http.RequestPoolSize};"); }
            if(!string.IsNullOrEmpty(o.Http.OutputBuffers)) { lines.Add($"    output_buffers 1 {o.Http.OutputBuffers};"); }
            if(!string.IsNullOrEmpty(o.Http.PostponeOutput)) { lines.Add($"    postpone_output {o.Http.PostponeOutput};"); }
            if(!string.IsNullOrEmpty(o.Http.ResetTimedoutConnection)) { lines.Add($"    reset_timedout_connection {o.Http.ResetTimedoutConnection};"); }
            if(!string.IsNullOrEmpty(o.Http.SendTimeout)) { lines.Add($"    send_timeout {o.Http.SendTimeout};"); }
            if(!string.IsNullOrEmpty(o.Http.ServerTokens)) { lines.Add($"    server_tokens {o.Http.ServerTokens};"); }
            if(!string.IsNullOrEmpty(o.Http.ClientHeaderBufferSize)) { lines.Add($"    client_header_buffer_size {o.Http.ClientHeaderBufferSize};"); }
            if(!string.IsNullOrEmpty(o.Http.ClientHeaderTimeout)) { lines.Add($"    client_header_timeout {o.Http.ClientHeaderTimeout};"); }
            if(!string.IsNullOrEmpty(o.Http.ClientBodyBufferSize)) { lines.Add($"    client_body_buffer_size {o.Http.ClientBodyBufferSize};"); }
            if(!string.IsNullOrEmpty(o.Http.ClientBodyTimeout)) { lines.Add($"    client_body_timeout {o.Http.ClientBodyTimeout};"); }
            if(!string.IsNullOrEmpty(o.Http.LargeClientHeaderBuffers)) { lines.Add($"    large_client_header_buffers {o.Http.LargeClientHeaderBuffers};"); }
            if(!string.IsNullOrEmpty(o.Http.Gzip)) { lines.Add($"    gzip {o.Http.Gzip};"); }
            if(!string.IsNullOrEmpty(o.Http.GzipMinLength)) { lines.Add($"    gzip_min_length {o.Http.GzipMinLength};"); }
            if(!string.IsNullOrEmpty(o.Http.GzipProxied)) { lines.Add($"    gzip_proxied {o.Http.GzipProxied};"); }
            if(!string.IsNullOrEmpty(o.Http.GzipTypes)) { lines.Add($"    gzip_types {o.Http.GzipTypes};"); }
            if(!string.IsNullOrEmpty(o.Http.GzipBuffers)) { lines.Add($"    gzip_buffers 256 {o.Http.GzipBuffers};"); }
            if(!string.IsNullOrEmpty(o.Http.GzipCompLevel)) { lines.Add($"    gzip_comp_level {o.Http.GzipCompLevel};"); }
            if(!string.IsNullOrEmpty(o.Http.GzipHttpVersion)) { lines.Add($"    gzip_http_version {o.Http.GzipHttpVersion};"); }
            if(!string.IsNullOrEmpty(o.Http.GzipVary)) { lines.Add($"    gzip_vary {o.Http.GzipHttpVersion};"); }
            if(!string.IsNullOrEmpty(o.Http.GzipDisable)) { lines.Add($"    gzip_disable \"{o.Http.GzipDisable}\";"); }
            if(!string.IsNullOrEmpty(o.Http.OpenFileCacheMax) && !string.IsNullOrEmpty(o.Http.OpenFileCacheInactive)) { lines.Add($"    open_file_cache max={o.Http.OpenFileCacheMax} inactive={o.Http.OpenFileCacheInactive};"); }
            if(!string.IsNullOrEmpty(o.Http.OpenFileCacheValid)) { lines.Add($"    open_file_cache_valid {o.Http.OpenFileCacheValid};"); }
            if(!string.IsNullOrEmpty(o.Http.OpenFileCacheMinUses)) { lines.Add($"    open_file_cache_min_uses {o.Http.OpenFileCacheMinUses};"); }
            if(!string.IsNullOrEmpty(o.Http.OpenFileCacheErrors)) { lines.Add($"    open_file_cache_min_uses {o.Http.OpenFileCacheErrors};"); }
            if(!string.IsNullOrEmpty(o.Http.ServerNamesHashBucketSize)) { lines.Add($"    server_names_hash_bucket_size {o.Http.ServerNamesHashBucketSize};"); }
            lines.Add("");
            foreach(var upstr in o.Upstreams) {
                lines.Add($"upstream {upstr.Name} {{ server {upstr.Server} fail_timeout=0; }}");
            }
            lines.Add("");
            foreach(var s in o.Servers) {
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

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static void Enable() {
            var mo = ServiceModel;
            mo.IsActive = true;
            Save(mo);
            ConsoleLogger.Log("[nginx] enabled");
        }

        public static void Disable() {
            var mo = ServiceModel;
            mo.IsActive = false;
            Save(mo);
            ConsoleLogger.Log("[nginx] disabled");
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
