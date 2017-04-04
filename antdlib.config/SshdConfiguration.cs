using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using IoDir = System.IO.Directory;

namespace antdlib.config {
    public class SshdConfiguration {

        private readonly SshdConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/sshd.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/sshd.conf.bck";
        private const string ServiceName = "sshd.service";
        private const string MainFilePath = "/etc/ssh/sshd_config";
        private const string MainFilePathBackup = "/etc/ssh/.sshd_config";

        public SshdConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new SshdConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<SshdConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new SshdConfigurationModel();
                }

            }
        }

        public void Save(SshdConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
            ConsoleLogger.Log("[sshd] configuration saved");
        }

        public void Set() {
            Enable();
            Stop();
            #region [    named.conf generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string>();
            var options = _serviceModel;
            lines.Add($"Port {options.Port}");
            lines.Add($"PermitRootLogin {options.PermitRootLogin}");
            lines.Add($"PermitTunnel {options.PermitTunnel}");
            lines.Add($"MaxAuthTries {options.MaxAuthTries}");
            lines.Add($"MaxSessions {options.MaxSessions}");
            lines.Add($"RsaAuthentication {options.RsaAuthentication}");
            lines.Add($"PubkeyAuthentication {options.PubkeyAuthentication}");
            lines.Add($"UsePam {options.UsePam}");
            lines.Add("#AddressFamily any");
            lines.Add("#ListenAddress 0.0.0.0");
            lines.Add("#ListenAddress ::");
            lines.Add("#Protocol 2");
            lines.Add("#HostKey /etc/ssh/ssh_host_key");
            lines.Add("#HostKey /etc/ssh/ssh_host_rsa_key");
            lines.Add("#HostKey /etc/ssh/ssh_host_dsa_key");
            lines.Add("#HostKey /etc/ssh/ssh_host_ecdsa_key");
            lines.Add("#HostKey /etc/ssh/ssh_host_ed25519_key");
            lines.Add("#KeyRegenerationInterval 1h");
            lines.Add("#ServerKeyBits 1024");
            lines.Add("#RekeyLimit default none");
            lines.Add("#SyslogFacility AUTH");
            lines.Add("#LogLevel INFO");
            lines.Add("#LoginGraceTime 2m");
            lines.Add("#StrictModes yes");
            lines.Add("#AuthorizedKeysFile .ssh/authorized_keys");
            lines.Add("#AuthorizedPrincipalsFile none");
            lines.Add("#AuthorizedKeysCommand none");
            lines.Add("#AuthorizedKeysCommandUser nobody");
            lines.Add("#RhostsRSAAuthentication no");
            lines.Add("#HostbasedAuthentication no");
            lines.Add("#IgnoreUserKnownHosts no");
            lines.Add("#IgnoreRhosts yes");
            lines.Add("PasswordAuthentication no");
            lines.Add("#PermitEmptyPasswords no");
            lines.Add("#ChallengeResponseAuthentication yes");
            lines.Add("#KerberosAuthentication no");
            lines.Add("#KerberosOrLocalPasswd yes");
            lines.Add("#KerberosTicketCleanup yes");
            lines.Add("#KerberosGetAFSToken no");
            lines.Add("#GSSAPIAuthentication no");
            lines.Add("#GSSAPICleanupCredentials yes");
            lines.Add("#AllowAgentForwarding yes");
            lines.Add("#AllowTcpForwarding yes");
            lines.Add("#GatewayPorts no");
            lines.Add("#X11Forwarding no");
            lines.Add("#X11DisplayOffset 10");
            lines.Add("#X11UseLocalhost yes");
            lines.Add("#PermitTTY yes");
            lines.Add("PrintMotd no");
            lines.Add("PrintLastLog no");
            lines.Add("#TCPKeepAlive yes");
            lines.Add("#UseLogin no");
            lines.Add("#UsePrivilegeSeparation sandbox");
            lines.Add("#PermitUserEnvironment no");
            lines.Add("#Compression delayed");
            lines.Add("#ClientAliveInterval 0");
            lines.Add("#ClientAliveCountMax 3");
            lines.Add("#UseDNS no");
            lines.Add("#PidFile /run/sshd.pid");
            lines.Add("#MaxStartups 10:30:100");
            lines.Add("#PermitTunnel no");
            lines.Add("#ChrootDirectory none");
            lines.Add("#VersionAddendum none");
            lines.Add("#Banner none");
            lines.Add("#UseLPK yes");
            lines.Add("#LpkLdapConf /etc/ldap.conf");
            lines.Add("#LpkServers ldap://10.1.7.1/ ldap://10.1.7.2/");
            lines.Add("#LpkUserDN ou=users,dc=phear,dc=org");
            lines.Add("#LpkGroupDN ou=groups,dc=phear,dc=org");
            lines.Add("#LpkBindDN cn=Manager,dc=phear,dc=org");
            lines.Add("#LpkBindPw secret");
            lines.Add("#LpkServerGroup mail");
            lines.Add("#LpkFilter (hostAccess=master.phear.org)");
            lines.Add("#LpkForceTLS no");
            lines.Add("#LpkSearchTimelimit 3");
            lines.Add("#LpkBindTimelimit 3");
            lines.Add("#LpkPubKeyAttr sshPublicKey");
            lines.Add("Subsystem sftp /usr/lib64/misc/sftp");
            lines.Add("AcceptEnv LANG LC_*");
            File.WriteAllLines(MainFilePath, lines);
            #endregion
            Start();
        }

        public bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public SshdConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[sshd] enabled");
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[sshd] disabled");
        }

        public void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[sshd] stop");
        }

        public void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[sshd] start");
        }
    }
}
