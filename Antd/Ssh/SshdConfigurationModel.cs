namespace Antd.Ssh {
    public class SshdConfigurationModel {
        public bool IsActive { get; set; }

        public string Port { get; set; } = "22";
        public string PermitRootLogin { get; set; } = "prohibit-password";
        public string PermitTunnel { get; set; } = "yes";
        public string MaxAuthTries { get; set; } = "6";
        public string MaxSessions { get; set; } = "10";
        public string RsaAuthentication { get; set; } = "yes";
        public string PubkeyAuthentication { get; set; } = "yes";
        public string UsePam { get; set; } = "yes";

        #region [    Frozen    ]
        public string AddressFamily { get; set; } = "any";
        public string[] ListenAddress { get; set; } = { "0.0.0.0", "::" };
        public string Protocol { get; set; } = "2";
        public string[] HostKey { get; set; } = { "/etc/ssh/ssh_host_key", "/etc/ssh/ssh_host_rsa_key", "/etc/ssh/ssh_host_dsa_key", "/etc/ssh/ssh_host_ecdsa_key" };
        public string KeyRegenerationInterval { get; set; } = "1h";
        public string ServerKeyBits { get; set; } = "1024";
        public string RekeyLimit { get; set; } = "default none";
        public string SyslogFacility { get; set; } = "AUTH";
        public string LogLevel { get; set; } = "INFO";
        public string LoginGraceTime { get; set; } = "2m";
        public string AuthorizedKeysFile { get; set; } = ".ssh/authorized_keys";
        public string StrictModes { get; set; } = "yes";
        public string AuthorizedPrincipalsFile { get; set; } = "none";
        public string AuthorizedKeysCommand { get; set; } = "none";
        public string AuthorizedKeysCommandUser { get; set; } = "nobody";
        public string RhostsRsaAuthentication { get; set; } = "no";
        public string HostbasedAuthentication { get; set; } = "no";
        public string IgnoreUserKnownHosts { get; set; } = "no";
        public string IgnoreRhosts { get; set; } = "yes";
        public string PasswordAuthentication { get; set; } = "no";
        public string PermitEmptyPasswords { get; set; } = "no";
        public string ChallengeResponseAuthentication { get; set; } = "yes";
        public string KerberosAuthentication { get; set; } = "no";
        public string KerberosOrLocalPasswd { get; set; } = "yes";
        public string KerberosTicketCleanup { get; set; } = "yes";
        public string KerberosGetAfsToken { get; set; } = "no";
        public string GssapiAuthentication { get; set; } = "no";
        public string GssapiCleanupCredentials { get; set; } = "yes";
        public string AllowAgentForwarding { get; set; } = "yes";
        public string AllowTcpForwarding { get; set; } = "yes";
        public string GatewayPorts { get; set; } = "no";
        public string X11Forwarding { get; set; } = "no";
        public string X11DisplayOffset { get; set; } = "10";
        public string X11UseLocalhost { get; set; } = "yes";
        public string PermitTty { get; set; } = "yes";
        public string PrintMotd { get; set; } = "no";
        public string PrintLastLog { get; set; } = "no";
        public string TcpKeepAlive { get; set; } = "yes";
        public string UseLogin { get; set; } = "no";
        public string UsePrivilegeSeparation { get; set; } = "sandbox";
        public string PermitUserEnvironment { get; set; } = "no";
        public string Compression { get; set; } = "delayed";
        public string ClientAliveInterval { get; set; } = "0";
        public string ClientAliveCountMax { get; set; } = "3";
        public string UseDns { get; set; } = "no";
        public string PidFile { get; set; } = "/run/sshd.pid";
        public string MaxStartups { get; set; } = "10:30:100";
        public string ChrootDirectory { get; set; } = "none";
        public string VersionAddendum { get; set; } = "none";
        public string Banner { get; set; } = "none";
        public string UseLpk { get; set; } = "yes";
        public string LpkLdapConf { get; set; } = "/etc/ldap.conf";
        public string LpkServers { get; set; } = "ldap://10.1.7.1/ ldap://10.1.7.2/";
        public string LpkUserDn { get; set; } = "ou=users,dc=phear,dc=org";
        public string LpkGroupDn { get; set; } = "ou=groups,dc=phear,dc=org";
        public string LpkBindDn { get; set; } = "cn=Manager,dc=phear,dc=org";
        public string LpkBindPw { get; set; } = "secret";
        public string LpkServerGroup { get; set; } = "mail";
        public string LpkFilter { get; set; } = "(hostAccess=master.phear.org)";
        public string LpkForceTls { get; set; } = "no";
        public string LpkSearchTimelimit { get; set; } = "3";
        public string LpkBindTimelimit { get; set; } = "3";
        public string LpkPubKeyAttr { get; set; } = "sshPublicKey";
        public string Subsystem { get; set; } = "sftp /usr/lib64/misc/sftp-server";
        public string AcceptEnv { get; set; } = "LANG LC_*";
        #endregion
    }
}