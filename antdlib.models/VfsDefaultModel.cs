using anthilla.core;
using System;
using System.Collections.Generic;

namespace antdlib.models {

    public class VfsDefaults {
        /// <summary>
        /// (de)Serializzato in List<>
        /// </summary>
        public class UserMaster {
            public int UserMasterId { get; set; } = 1;
            public int NodeId { get; set; } = 0;
            public string FirstName { get; set; } = "First";
            public string LastName { get; set; } = "Last";
            public string CompanyName { get; set; } = "Default Company";
            public string Email { get; set; } = "default@default.com";
            public string Password { get; set; } = "default";
            public string Cellphone { get; set; } = "";
            public string Address1 { get; set; } = "";
            public string City { get; set; } = "";
            public string State { get; set; } = "";
            public string PostalCode { get; set; } = "";
            public string Country { get; set; } = "";
            public int IsAdmin { get; set; } = 1;
            public string Guid { get; set; } = "default";
            public int Active { get; set; } = 1;
            public string Created { get; set; } = DateTime.Now.ToString();
            public string LastUpdate { get; set; } = DateTime.Now.ToString();
            public string Expiration { get; set; } = DateTime.Now.AddYears(100).ToString();
        }

        /// <summary>
        /// (de)Serializzato in List<>
        /// </summary>
        public class ApiKey {
            public int ApiKeyId { get; set; } = 1;
            public int UserMasterId { get; set; } = 1;
            public string Guid { get; set; } = "default";
            public string Notes { get; set; } = "Created by Antd...";
            public int Active { get; set; } = 1;
            public string Created { get; set; } = DateTime.Now.ToString();
            public string LastUpdate { get; set; } = DateTime.Now.ToString();
            public string Expiration { get; set; } = DateTime.Now.AddYears(100).ToString();
        }

        /// <summary>
        /// (de)Serializzato in List<>
        /// </summary>
        public class ApiKeyPermission {
            public int ApiKeyPermissionId { get; set; } = 1;
            public int UserMasterId { get; set; } = 1;
            public int ApiKeyId { get; set; } = 1;
            public string Notes { get; set; } = "Created by Antd...";
            public int AllowReadObject { get; set; } = 1;
            public int AllowReadContainer { get; set; } = 1;
            public int AllowWriteObject { get; set; } = 1;
            public int AllowWriteContainer { get; set; } = 1;
            public int AllowDeleteObject { get; set; } = 1;
            public int AllowDeleteContainer { get; set; } = 1;
            public int AllowSearch { get; set; } = 1;
            public string Guid { get; set; } = "default";
            public int Active { get; set; } = 1;
            public string Created { get; set; } = DateTime.Now.ToString();
            public string LastUpdate { get; set; } = DateTime.Now.ToString();
            public string Expiration { get; set; } = DateTime.Now.AddYears(100).ToString();
        }

        public class Node {
            public int NodeId { get; set; }
            public string Name { get; set; }
            public string DnsHostname { get; set; }
            public int Port { get; set; }
            public int Ssl { get; set; }
            public int NumFailures { get; set; }
            public int[] Neighbors { get; set; }
        }

        public class ListTopology {
            public int CurrNodeId { get; set; } = 1;
            public List<Node> Nodes { get; set; } = new List<Node>() {
                new Node() {
                    NodeId= 1,
                    Name="localhost",
                    DnsHostname="127.0.0.1",
                    Port= 7080,
                    Ssl = 0,
                    NumFailures = 0,
                    Neighbors = new int[] { 0 }
                }
            };
        }

        public class Files {
            public string Topology { get; set; }
            public string UserMaster { get; set; }
            public string ApiKey { get; set; }
            public string Permission { get; set; }
        }

        public class Server {
            public string HeaderApiKey { get; set; }
            public string HeaderEmail { get; set; }
            public string HeaderPassword { get; set; }
            public string HeaderToken { get; set; }
            public string HeaderVersion { get; set; }
            public string AdminApiKey { get; set; }
            public int TokenExpirationSec { get; set; }
            public int FailedRequestsIntervalSec { get; set; }
        }

        public class Redirection {
            public string ReadRedirectionMode { get; set; }
            public int ReadRedirectHttpStatus { get; set; }
            public string ReadRedirectString { get; set; }
            public string SearchRedirectionMode { get; set; }
            public int SearchRedirectHttpStatus { get; set; }
            public string SearchRedirectString { get; set; }
            public string WriteRedirectionMode { get; set; }
            public int WriteRedirectHttpStatus { get; set; }
            public string WriteRedirectString { get; set; }
            public string DeleteRedirectionMode { get; set; }
            public int DeleteRedirectHttpStatus { get; set; }
            public string DeleteRedirectString { get; set; }
        }

        public class Topology {
            public int RefreshSec { get; set; }
        }

        public class Perfmon {
            public int Enable { get; set; }
            public int RefreshSec { get; set; }
            public int Syslog { get; set; }
        }

        public class Storage {
            public string Directory { get; set; }
            public int MaxObjectSize { get; set; }
            public int GatewayMode { get; set; }
            public int DefaultCompress { get; set; }
            public int DefaultEncrypt { get; set; }
        }

        public class Messages {
            public string Directory { get; set; }
            public int RefreshSec { get; set; }
        }

        public class Expiration {
            public string Directory { get; set; }
            public int RefreshSec { get; set; }
            public int DefaultExpirationSec { get; set; }
        }

        public class Replication {
            public string Directory { get; set; }
            public string ReplicationMode { get; set; }
            public int RefreshSec { get; set; }
        }

        public class Bunker {
            public int Enable { get; set; }
            public string Directory { get; set; }
            public int RefreshSec { get; set; }
        }

        public class PublicObj {
            public string Directory { get; set; }
            public int RefreshSec { get; set; }
            public int DefaultExpirationSec { get; set; }
        }

        public class Tasks {
            public string Directory { get; set; }
            public int RefreshSec { get; set; }
        }

        public class Logger {
            public int RefreshSec { get; set; }
        }

        public class Syslog {
            public string ServerIp { get; set; }
            public int ServerPort { get; set; }
            public string Header { get; set; }
            public int MinimumLevel { get; set; }
            public int LogHttpRequests { get; set; }
            public int LogHttpResponses { get; set; }
            public int ConsoleLogging { get; set; }
        }

        public class Email {
            public int SmtpPort { get; set; }
            public int SmtpSsl { get; set; }
            public int EmailExceptions { get; set; }
        }

        public class Encryption {
            public string Mode { get; set; }
            public int Port { get; set; }
            public int Ssl { get; set; }
            public string Passphrase { get; set; }
            public string Salt { get; set; }
            public string Iv { get; set; }
        }

        public class Rest {
            public int UseWebProxy { get; set; }
            public int AcceptInvalidCerts { get; set; }
        }

        public class Mailgun {
        }

        public class Debug {
            public int DebugCompression { get; set; }
            public int DebugEncryption { get; set; }
        }

        public class DefaltSystem {
            public string ProductName { get; set; } = "antd_+_kvpbase";
            public string ProductVersion { get; set; } = "1.1.1";
            public string DocumentationUrl { get; set; } = "";
            public string Environment { get; set; } = "linux";
            public string LogoUrl { get; set; } = "";
            public string HomepageUrl { get; set; } = "";
            public string SupportEmail { get; set; } = "";
            public int EnableConsole { get; set; } = 0;
            public Files Files { get; set; } = new Files() {
                Topology = $"{Parameter.AntdCfgVfs}/topology.json",
                UserMaster = $"{Parameter.AntdCfgVfs}/userMaster.json",
                ApiKey = $"{Parameter.AntdCfgVfs}/apiKey.json",
                Permission = $"{Parameter.AntdCfgVfs}/apiKeyPermission.json"
            };
            public Server Server { get; set; } = new Server() {
                HeaderApiKey = "x-api-key",
                HeaderEmail = "x-email",
                HeaderPassword = "x-password",
                HeaderToken = "x-token",
                HeaderVersion = "x-version",
                AdminApiKey = "kvpbaseadmin",
                TokenExpirationSec = 86400,
                FailedRequestsIntervalSec = 60
            };
            public Redirection Redirection { get; set; } = new Redirection() {
                ReadRedirectionMode = "proxy",
                ReadRedirectHttpStatus = 301,
                ReadRedirectString = "Moved Permanently",
                SearchRedirectionMode = "proxy",
                SearchRedirectHttpStatus = 301,
                SearchRedirectString = "Moved Permanently",
                WriteRedirectionMode = "proxy",
                WriteRedirectHttpStatus = 301,
                WriteRedirectString = "Moved Permanently",
                DeleteRedirectionMode = "proxy",
                DeleteRedirectHttpStatus = 301,
                DeleteRedirectString = "Moved Permanently",
            };
            public Topology Topology { get; set; } = new Topology() { RefreshSec = 10 };
            public Perfmon Perfmon { get; set; } = new Perfmon() {
                Enable = 1,
                RefreshSec = 10,
                Syslog = 0
            };
            public Storage Storage { get; set; } = new Storage() {
                Directory = $"{Parameter.AntdCfgVfs}/storage/",
                MaxObjectSize = 512000000,
                GatewayMode = 1,
                DefaultCompress = 0,
                DefaultEncrypt = 0
            };
            public Messages Messages { get; set; } = new Messages() {
                Directory = $"{Parameter.AntdCfgVfs}/messages/",
                RefreshSec = 10
            };
            public Expiration Expiration { get; set; } = new Expiration() {
                Directory = $"{Parameter.AntdCfgVfs}/expiration/",
                RefreshSec = 10,
                DefaultExpirationSec = 0
            };
            public Replication Replication { get; set; } = new Replication() {
                Directory = $"{Parameter.AntdCfgVfs}/expiration/",
                ReplicationMode = "sync",
                RefreshSec = 10
            };
            public Bunker Bunker { get; set; } = new Bunker() {
                Enable = 0,
                Directory = $"{Parameter.AntdCfgVfs}/bunker/",
                RefreshSec = 30
            };
            public PublicObj PublicObj { get; set; } = new PublicObj() {
                Directory = $"{Parameter.AntdCfgVfs}/pubfiles/",
                RefreshSec = 600,
                DefaultExpirationSec = 7776000
            };
            public Tasks Tasks { get; set; } = new Tasks() {
                Directory = $"{Parameter.AntdCfgVfs}/tasks/",
                RefreshSec = 10
            };
            public Logger Logger { get; set; } = new Logger() {
                RefreshSec = 10
            };
            public Syslog Syslog { get; set; } = new Syslog() {
                ServerIp = "127.0.0.1",
                ServerPort = 514,
                Header = "antd_storage_server",
                MinimumLevel = 1,
                LogHttpRequests = 0,
                LogHttpResponses = 0,
                ConsoleLogging = 1
            };
            public Email Email { get; set; } = new Email() {
                SmtpPort = 0,
                SmtpSsl = 0,
                EmailExceptions = 0
            };
            public Encryption Encryption { get; set; } = new Encryption() {
                Mode = "local",
                Port = 0,
                Ssl = 0,
                Passphrase = "0000000000000000",
                Salt = "0000000000000000",
                Iv = "0000000000000000"
            };
            public Rest Rest { get; set; } = new Rest() {
                UseWebProxy = 0,
                AcceptInvalidCerts = 0
            };
            public Mailgun Mailgun { get; set; } = new Mailgun();
            public Debug Debug { get; set; } = new Debug() {
                DebugCompression = 0,
                DebugEncryption = 0
            };
        }
    }
}
