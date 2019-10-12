using anthilla.core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kvpbase {
    public class Settings {
        #region Public-Members-and-Nested-Classes

        public string ProductName;
        public string ProductVersion;
        public string DocumentationUrl;
        public string Environment;
        public string LogoUrl;
        public string HomepageUrl;
        public string SupportEmail;
        public int EnableConsole;
        public int DefaultRefreshSec;

        public SettingsServer Server;
        public SettingsRedirection Redirection;
        public SettingsPerfmon Perfmon;
        public SettingsStorage Storage;
        public SettingsMessages Messages;
        public SettingsExpiration Expiration;
        public SettingsReplication Replication;
        public SettingsBunker Bunker;
        public SettingsPublicObj PublicObj;
        public SettingsTasks Tasks;
        public SettingsLogger Logger;
        public SettingsEncryption Encryption;
        public SettingsRest Rest;

        public class SettingsServer {
            public string HeaderApiKey;
            public string HeaderEmail;
            public string HeaderPassword;
            public string HeaderToken;
            public string HeaderVersion;

            public string AdminApiKey;
            public int TokenExpirationSec;
            public int FailedRequestsIntervalSec;
        }

        public class SettingsPerfmon {
            public int Enable;
            public int RefreshSec;
            public int Syslog;
        }

        public class SettingsStorage {
            public string Directory;
            public int MaxObjectSize;
            public int GatewayMode;
            public int DefaultCompress;
            public int DefaultEncrypt;
        }

        public class SettingsMessages {
            public string Directory;
            public int RefreshSec;
        }

        public class SettingsExpiration {
            public string Directory;
            public int RefreshSec;
            public int DefaultExpirationSec;
        }

        public class SettingsReplication {
            public string Directory;
            public string ReplicationMode;
            public int RefreshSec;
        }

        public class SettingsBunker {
            public int Enable;
            public string Directory;
            public int RefreshSec;
            public List<BunkerNode> Nodes;
        }

        public class SettingsPublicObj {
            public string Directory;
            public int RefreshSec;
            public int DefaultExpirationSec;
        }

        public class BunkerNode {
            public int Enable;
            public string Name;
            public string Vendor;
            public string Version;
            public string UserGuid;
            public string Url;
            public string ApiKey;
        }

        public class SettingsTasks {
            public string Directory;
            public int RefreshSec;
        }

        public class SettingsLogger {
            public int RefreshSec;
        }

        public class SettingsRedirection {
            public string ReadRedirectionMode;
            public int ReadRedirectHttpStatus;
            public string ReadRedirectString;
            public string SearchRedirectionMode;
            public int SearchRedirectHttpStatus;
            public string SearchRedirectString;
            public string WriteRedirectionMode;
            public int WriteRedirectHttpStatus;
            public string WriteRedirectString;
            public string DeleteRedirectionMode;
            public int DeleteRedirectHttpStatus;
            public string DeleteRedirectString;
        }

        public class SettingsEncryption {
            public string Mode;

            public string Server;
            public int Port;
            public int Ssl;
            public string ApiKeyHeader;
            public string ApiKeyValue;

            public string Passphrase;
            public string Salt;
            public string Iv;
        }

        public class SettingsRest {
            public int UseWebProxy;
            public string WebProxyUrl;
            public int AcceptInvalidCerts;
        }

        #endregion

        #region Constructors-and-Factories

        public Settings() {

        }

        public static Settings FromFile(string filename) {
            if(String.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));
            if(!Common.FileExists(filename))
                throw new FileNotFoundException(nameof(filename));

            ConsoleLogger.Log("Reading " + filename);
            string contents = Common.ReadTextFile(@filename);

            if(String.IsNullOrEmpty(contents)) {
                Common.ExitApplication("Settings", "Unable to read contents of " + filename, -1);
                return null;
            }

            ConsoleLogger.Log("Deserializing System.json");
            Settings ret = null;

            try {
                ret = Common.DeserializeJson<Settings>(contents);
                if(ret == null) {
                    Common.ExitApplication("Settings", "Unable to deserialize " + filename + " (null)", -1);
                    return null;
                }
            }
            catch(Exception e) {
                ConsoleLogger.Warn(("Settings", "Deserialization issue with " + filename, e));
                Common.ExitApplication("Settings", "Unable to deserialize " + filename + " (exception)", -1);
                return null;
            }

            return ret;
        }

        public static Settings Default(string dir) {
            return new Settings() {
                ProductName = "",
                ProductVersion = "",
                DocumentationUrl = "www.anthilla.com",
                Environment = "",
                LogoUrl = "",
                HomepageUrl = "www.anthilla.com",
                SupportEmail = "info@anthilla.com",
                EnableConsole = 0,
                DefaultRefreshSec = 10,
                Server = new SettingsServer() {
                    HeaderApiKey = "x-api-key",
                    HeaderEmail = "x-email",
                    HeaderPassword = "x-password",
                    HeaderToken = "x-token",
                    HeaderVersion = "x-version",
                    AdminApiKey = "kvpbaseadmin",
                    TokenExpirationSec = 86400,
                    FailedRequestsIntervalSec = 60
                },
                Redirection = new SettingsRedirection() {
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
                    DeleteRedirectString = "Moved Permanently"
                },
                Perfmon = new SettingsPerfmon() {
                    Enable = 1,
                    RefreshSec = 10,
                    Syslog = 0
                },
                Storage = new SettingsStorage() {
                    Directory = dir,
                    MaxObjectSize = 512000000,
                    GatewayMode = 1,
                    DefaultCompress = 0,
                    DefaultEncrypt = 0
                },
                Messages = new SettingsMessages() {
                    Directory = dir,
                    RefreshSec = 10
                },
                Expiration = new SettingsExpiration() {
                    Directory = dir,
                    RefreshSec = 10,
                    DefaultExpirationSec = 0
                },
                Replication = new SettingsReplication() {
                    Directory = dir,
                    ReplicationMode = "sync",
                    RefreshSec = 10
                },
                Bunker = new SettingsBunker() {
                    Enable = 0,
                    Directory = dir,
                    RefreshSec = 30
                },
                PublicObj = new SettingsPublicObj() {
                    Directory = dir,
                    RefreshSec = 600,
                    DefaultExpirationSec = 7776000
                },
                Tasks = new SettingsTasks() {
                    Directory = dir,
                    RefreshSec = 10
                },
                Encryption = new SettingsEncryption() {
                    Mode = "local",
                    Port = 0,
                    Ssl = 0,
                    Passphrase = "0000000000000000",
                    Salt = "0000000000000000",
                    Iv = "0000000000000000"
                },
                Rest = new SettingsRest() {
                    UseWebProxy = 0,
                    AcceptInvalidCerts = 0
                },
            };
        }

        #endregion
    }
}
