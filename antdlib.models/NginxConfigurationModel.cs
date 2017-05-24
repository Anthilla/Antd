using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdlib.models {
    public class NginxConfigurationModel {
        public bool IsActive { get; set; }

        public string User { get; set; }
        public string Group { get; set; }
        public string Processes { get; set; }
        public string FileLimit { get; set; }
        public string ErrorLog { get; set; } = "/var/log/nginx/error.log crit";

        public string EventsWorkerConnections { get; set; }
        public string EventsMultiAccept { get; set; }
        public string EventsUse { get; set; }
        public string EventsAcceptMutex { get; set; }

        public NginxHttpProtocol Http { get; set; }

        public List<NginxUpstream> Upstreams { get; set; } = new List<NginxUpstream>();
        public List<NginxServer> Servers { get; set; } = new List<NginxServer>();
    }

    public class NginxHttpProtocol {
        public string Aio { get; set; }
        public string Directio { get; set; }
        public string AccessLog { get; set; }
        public string KeepaliveTimeout { get; set; }
        public string KeepaliveRequests { get; set; }
        public string Sendfile { get; set; }
        public string SendfileMaxChunk { get; set; }
        public string TcpNopush { get; set; }
        public string TcpNodelay { get; set; }
        public string Include { get; set; } = "/etc/nginx/mime.types";
        public string DefaultType { get; set; }
        public string LogFormat { get; set; } = "log_format main \'$remote_addr - $remote_user [$time_local] \"$request\" $status $bytes_sent \"$http_referer\" \"$http_user_agent\" \"$gzip_ratio\"\'";
        public string RequestPoolSize { get; set; }
        public string OutputBuffers { get; set; }
        public string PostponeOutput { get; set; }
        public string ResetTimedoutConnection { get; set; }
        public string SendTimeout { get; set; }
        public string ServerTokens { get; set; }
        public string ClientHeaderBufferSize { get; set; }
        public string ClientHeaderTimeout { get; set; }
        public string ClientBodyBufferSize { get; set; }
        public string ClientBodyTimeout { get; set; }
        public string LargeClientHeaderBuffers { get; set; }
        public string Gzip { get; set; }
        public string GzipMinLength { get; set; }
        public string GzipProxied { get; set; }
        public string GzipTypes { get; set; } = "text/plain text/css application/json application/x-javascript text/xml application/xml application/xml+rss text/javascript text/mathml application/atom+xml application/xhtml+xml image/svg+xml";
        public string GzipBuffers { get; set; }
        public string GzipCompLevel { get; set; }
        public string GzipHttpVersion { get; set; }
        public string GzipVary { get; set; }
        public string GzipDisable { get; set; } = "MSIE [1-6]\\.(?!.*SV1)";
        public string OpenFileCacheMax { get; set; }
        public string OpenFileCacheInactive { get; set; }
        public string OpenFileCacheValid { get; set; }
        public string OpenFileCacheMinUses { get; set; }
        public string OpenFileCacheErrors { get; set; }
        public string ServerNamesHashBucketSize { get; set; }
    }

    public class NginxUpstream {
        public string Name { get; set; }
        public string Server { get; set; }
    }

    public class NginxServer {
        public string Listen { get; set; }
        public string ServerName { get; set; }
        public string ServerTokens { get; set; }
        public string Root { get; set; }
        public string ClientMaxBodySize { get; set; }
        public string ReturnRedirect { get; set; }
        public string SslCertificate { get; set; }
        public string SslTrustedCertificate { get; set; }
        public string SslCertificateKey { get; set; }
        public string AccessLog { get; set; } = "/var/log/nginx/80_access.log main";
        public string ErrorLog { get; set; } = "/var/log/nginx/80_error.log info";
        public List<NginxLocation> Locations { get; set; } = new List<NginxLocation>();
    }

    public class NginxLocation {
        public string Autoindex { get; set; }
        public string Root { get; set; }
        public string Aio { get; set; } = "threads";
        public bool Mp4 { get; set; } //mp4;
        public string ProxyPass { get; set; }
        public string ProxyPassHeader { get; set; } = "Authorization";
        public string ProxyReadTimeout { get; set; } = "300";
        public string ProxyConnectTimeout { get; set; } = "300";
        public List<string> ProxySetHeader { get; set; } = new List<string>();
        public string ProxyBuffering { get; set; } = "off";
        public string ClientMaxBodySize { get; set; } = "0";
        public string ProxyRedirect { get; set; } = "off";
        public string ProxyHttpVersion { get; set; } = "1.1";

        //proxy_set_header Host $host;
        //proxy_set_header X-Frame-Options SAMEORIGIN;
        //proxy_set_header X-Real-IP $remote_addr;
        //proxy_set_header X-Forwarded-Proto $scheme;
        //proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        //proxy_set_header Connection "";
    }
}
