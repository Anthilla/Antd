using System.IO;

namespace Antd.cmds {
    public class Sshd {

        private const string sshdEtcFile = "/etc/ssh/sshd_config";
        private const string sshdService = "sshd.service";

        public static bool Set() {
            var current = Application.CurrentConfiguration.Services.Sshd;
            if(!current.Active) {
                return true;
            }
            var lines = new string[] {
                $"Port {current.Port}",
                $"PermitRootLogin {current.PermitRootLogin}",
                $"PermitTunnel {current.PermitTunnel}",
                $"MaxAuthTries {current.MaxAuthTries}",
                $"MaxSessions {current.MaxSessions}",
                $"RsaAuthentication {current.RsaAuthentication}",
                $"PubkeyAuthentication {current.PubkeyAuthentication}",
                $"UsePam {current.UsePam}",
                "#AddressFamily any",
                "#ListenAddress 0.0.0.0",
                "#ListenAddress ::",
                "#Protocol 2",
                "#HostKey /etc/ssh/ssh_host_key",
                "#HostKey /etc/ssh/ssh_host_rsa_key",
                "#HostKey /etc/ssh/ssh_host_dsa_key",
                "#HostKey /etc/ssh/ssh_host_ecdsa_key",
                "#HostKey /etc/ssh/ssh_host_ed25519_key",
                "#KeyRegenerationInterval 1h",
                "#ServerKeyBits 1024",
                "#RekeyLimit default none",
                "#SyslogFacility AUTH",
                "#LogLevel INFO",
                "#LoginGraceTime 2m",
                "#StrictModes yes",
                "#AuthorizedKeysFile .ssh/authorized_keys",
                "#AuthorizedPrincipalsFile none",
                "#AuthorizedKeysCommand none",
                "#AuthorizedKeysCommandUser nobody",
                "#RhostsRSAAuthentication no",
                "#HostbasedAuthentication no",
                "#IgnoreUserKnownHosts no",
                "#IgnoreRhosts yes",
                "PasswordAuthentication no",
                "#PermitEmptyPasswords no",
                "#ChallengeResponseAuthentication yes",
                "#KerberosAuthentication no",
                "#KerberosOrLocalPasswd yes",
                "#KerberosTicketCleanup yes",
                "#KerberosGetAFSToken no",
                "#GSSAPIAuthentication no",
                "#GSSAPICleanupCredentials yes",
                "#AllowAgentForwarding yes",
                "#AllowTcpForwarding yes",
                "#GatewayPorts no",
                "#X11Forwarding no",
                "#X11DisplayOffset 10",
                "#X11UseLocalhost yes",
                "#PermitTTY yes",
                "PrintMotd no",
                "PrintLastLog no",
                "#TCPKeepAlive yes",
                "#UseLogin no",
                "#UsePrivilegeSeparation sandbox",
                "#PermitUserEnvironment no",
                "#Compression delayed",
                "#ClientAliveInterval 0",
                "#ClientAliveCountMax 3",
                "#UseDNS no",
                "#PidFile /run/sshd.pid",
                "#MaxStartups 10:30:100",
                "#PermitTunnel no",
                "#ChrootDirectory none",
                "#VersionAddendum none",
                "#Banner none",
                "#UseLPK yes",
                "#LpkLdapConf /etc/ldap.conf",
                "#LpkServers ldap://10.1.7.1/ ldap://10.1.7.2/",
                "#LpkUserDN ou=users,dc=phear,dc=org",
                "#LpkGroupDN ou=groups,dc=phear,dc=org",
                "#LpkBindDN cn=Manager,dc=phear,dc=org",
                "#LpkBindPw secret",
                "#LpkServerGroup mail",
                "#LpkFilter (hostAccess=master.phear.org)",
                "#LpkForceTLS no",
                "#LpkSearchTimelimit 3",
                "#LpkBindTimelimit 3",
                "#LpkPubKeyAttr sshPublicKey",
                "Subsystem sftp /usr/lib64/misc/sftp",
                "AcceptEnv LANG LC_*",
            };
            File.WriteAllLines(sshdEtcFile, lines);
            Systemctl.Restart(sshdService);
            return true;
        }
    }
}
