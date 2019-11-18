using antd.core;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Antd2.models {

    public class AuthenticationDataModel {
        public string Id { get; set; }
        public string[] Claims { get; set; }
    }

    public class UptimeModel {
        public string Uptime { get; set; }
        public string Users { get; set; }
        public string LoadAverage { get; set; }
    }

    public class CpuInfoModel {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class MemInfoModel {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class DiskUsageModel {
        public string Filesystem { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Used { get; set; }
        public string Avail { get; set; }
        public string UsePercentage { get; set; }
        public string MountedOn { get; set; }
    }

    public class FreeModel {
        public string Name { get; set; }
        public string Total { get; set; }
        public string Used { get; set; }
        public string Free { get; set; }
        public string Shared { get; set; }
        public string BuffCache { get; set; }
        public string Available { get; set; }
    }

    public class FreeStatus {
        public string Name { get; set; }
        public int Total { get; set; }
        public int Used { get; set; }
        public int Free { get; set; }
        public int Shared { get; set; }
        public int BuffCache { get; set; }
        public int Available { get; set; }
    }

    public class DiskUsageStatus {
        public string Device { get; set; }
        public int Used { get; set; }
    }

    public class DateStatus {
        public DateTime Date1 { get; set; }
        public string Date2 { get; set; }
    }

    public class ServiceStatus {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class MachineIdStatus {
        public Guid PartNumber { get; set; }
        public Guid SerialNumber { get; set; }
        public Guid MachineUid { get; set; }
    }

    public class CurrentMachineStatus {
        public string PublicIp { get; set; }
        public MachineIdStatus MachineId { get; set; }
        public DateStatus Date { get; set; }
        public FreeStatus[] Free { get; set; }
        public DiskUsageStatus[] DiskUsage { get; set; }
        public ServiceStatus[] Services { get; set; }
    }

    public class LosetupModel {
        public string Name { get; set; }
        public string Sizelimit { get; set; }
        public string Offset { get; set; }
        public string Autoclear { get; set; }
        public string Readonly { get; set; }
        public string Backfile { get; set; }
        public string Dio { get; set; }
        public string Hash { get; set; }
    }

    public class SshdModel {
        public bool Active { get; set; } = false;

        public string Port { get; set; } = "22";
        public string PermitRootLogin { get; set; } = "prohibit-password";
        public string PermitTunnel { get; set; } = "yes";
        public string MaxAuthTries { get; set; } = "6";
        public string MaxSessions { get; set; } = "10";
        public string RsaAuthentication { get; set; } = "yes";
        public string PubkeyAuthentication { get; set; } = "yes";
        public string UsePam { get; set; } = "yes";

        #region [    Static Configuration   ]
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

    public class CaModel {
        public bool Active { get; set; } = false;
        public string KeyPassout { get; set; } = "";
        public string RootCountryName { get; set; } = "IT";
        public string RootStateOrProvinceName { get; set; } = "";
        public string RootLocalityName { get; set; } = "";
        public string RootOrganizationName { get; set; } = "";
        public string RootOrganizationalUnitName { get; set; } = "";
        public string RootCommonName { get; set; } = "default";
        public string RootEmailAddress { get; set; } = "";
    }

    public class BindModel {
        public bool Active { get; set; } = false;
        public string Notify { get; set; } = "no";
        public string MaxCacheSize { get; set; } = "128M";
        public string MaxCacheTtl { get; set; } = "108000";
        public string MaxNcacheTtl { get; set; } = "3";
        public string[] Forwarders { get; set; } = new string[0]; // { "8.8.8.8", "8.8.4.4" };
        public string[] AllowNotify { get; set; } = new string[0]; // { "iif", "inet" };
        public string[] AllowTransfer { get; set; } = new string[0]; //  { "iif", "inet" };
        public string Recursion { get; set; } = "yes";
        public string TransferFormat { get; set; } = "many-answers";
        public string QuerySourceAddress { get; set; } = "*";
        public string QuerySourcePort { get; set; } = "*";
        public string Version { get; set; } = "none";
        public string[] AllowQuery { get; set; } = new string[0]; //  { "loif", "iif", "oif", "lonet", "inet", "onet" };
        public string[] AllowRecursion { get; set; } = new string[0]; // { "loif", "iif", "oif", "lonet", "inet", "onet" };
        public string IxfrFromDifferences { get; set; } = "yes";
        public string[] ListenOnV6 { get; set; } = new string[0]; //  { "none" };
        public string[] ListenOnPort53 { get; set; } = new string[0]; //  { "loif", "iif", "oif" };
        public string DnssecEnabled { get; set; } = "yes";
        public string DnssecValidation { get; set; } = "yes";
        public string DnssecLookaside { get; set; } = "auto";
        public string AuthNxdomain { get; set; } = "yes";

        public string KeyName { get; set; } = string.Empty;
        public string KeySecret { get; set; } = string.Empty;

        public string ControlIp { get; set; } = string.Empty;
        public string ControlPort { get; set; } = "953";
        public string[] ControlAllow { get; set; } = new string[0]; //  { "loif", "iif", "oif" };
        public string[] ControlKeys { get; set; } = new string[0]; //  { "updbindkey" };

        public string SyslogSeverity { get; set; } = "info";
        public string SyslogPrintCategory { get; set; } = "yes";
        public string SyslogPrintSeverity { get; set; } = "yes";
        public string SyslogPrintTime { get; set; } = "yes";

        public string TrustedKeys { get; set; } = string.Empty;

        public BindAclModel[] AclList { get; set; } = new BindAclModel[0];
        public List<BindZoneModel> Zones { get; set; } = new List<BindZoneModel>();
        public string[] IncludeFiles { get; set; } = new string[0];
    }

    public class BindAclModel {
        public string Name { get; set; }
        public string[] InterfaceList { get; set; } = new string[0];
    }

    public class BindZoneModel {
        public string Name { get; set; }
        /// <summary>
        /// Valore preso da Bind.Verbs.ZoneType
        /// </summary>
        public string Type { get; set; } = string.Empty;
        public string File { get; set; } = string.Empty;

        public BindZoneRecord[] Records { get; set; } = new BindZoneRecord[0];
    }

    public class BindZoneRecord {
        public string Name { get; set; } = string.Empty;
        public string Ttl { get; set; } = string.Empty;
        /// <summary>
        /// Valore preso da Bind.Verbs.ZoneRecordType
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// A (ipaddress), AAAA (ipaddress), CAA (value), CNAME (domain), MX (domain), SRV (target), TXT (text)
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Valore preso da Bind.Verbs.ZoneRecordCaaFlag
        /// </summary>
        public string CaaFlag { get; set; } = string.Empty;

        /// <summary>
        /// Valore preso da Bind.Verbs.ZoneRecordCaaTag
        /// </summary>
        public string CaaTag { get; set; } = string.Empty;

        /// <summary>
        /// MX, SRV
        /// </summary>
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// SRV
        /// </summary>
        public string Weight { get; set; } = string.Empty;

        /// <summary>
        /// SRV
        /// </summary>
        public string Port { get; set; } = string.Empty;
    }

    public class DhcpdModel {
        public bool Active { get; set; } = false;

        public List<string> Allow { get; set; } = new List<string>(); //{ "client-updates", "unknown-clients" };

        public string UpdateStaticLeases { get; set; } = "on";
        public string UpdateConflictDetection { get; set; } = "false";
        public string UseHostDeclNames { get; set; } = "on";
        public string DoForwardUpdates { get; set; } = "on";
        public string DoReverseUpdates { get; set; } = "on";
        public string LogFacility { get; set; } = "local7";
        public string ZoneName { get; set; } = "";
        public string ZonePrimaryAddress { get; set; } = "";
        public string ZonePrimaryKey { get; set; } = "";
        public string DdnsUpdateStyle { get; set; } = "interim";
        public string DdnsUpdates { get; set; } = "on";
        public string DdnsDomainName { get; set; } = "";
        public string DdnsRevDomainName { get; set; } = "in-addr.arpa.";
        public string DefaultLeaseTime { get; set; } = "7200";
        public string MaxLeaseTime { get; set; } = "7200";
        public string OptionRouters { get; set; } = "7200";
        public string OptionLocalProxy { get; set; } = "7200";
        public string OptionDomainName { get; set; } = "7200";
        public string KeyName { get; set; } = "updbindkey";
        public string KeySecret { get; set; } = "";

        public List<DhcpdSubnetModel> Subnets { get; set; } = new List<DhcpdSubnetModel>();
        public List<DhcpdClassModel> Classes { get; set; } = new List<DhcpdClassModel>();
        public List<DhcpdReservationModel> Reservations { get; set; } = new List<DhcpdReservationModel>();
    }

    public class DhcpdSubnetModel {
        public string SubnetIpFamily { get; set; } = "";
        public string SubnetIpMask { get; set; } = "255.255.0.0";
        public string SubnetOptionRouters { get; set; } = "";
        public string SubnetNtpServers { get; set; } = "";
        public string SubnetTimeServers { get; set; } = "";
        public string SubnetDomainNameServers { get; set; } = "";
        public string SubnetBroadcastAddress { get; set; } = "";
        public string SubnetMask { get; set; } = "255.255.0.0";
        public string ZoneName { get; set; } = "";
        public string ZonePrimaryAddress { get; set; } = "";
        public string ZonePrimaryKey { get; set; } = "";
        public string PoolDynamicRangeStart { get; set; } = "";
        public string PoolDynamicRangeEnd { get; set; } = "";
        public List<DhcpdPoolModel> Pools { get; set; } = new List<DhcpdPoolModel>();
    }

    public class DhcpdPoolModel {
        public string ClassName { get; set; } = "";
        public string PoolRangeStart { get; set; } = "";
        public string PoolRangeEnd { get; set; } = "";
    }

    public class DhcpdClassModel {
        public string Name { get; set; }
        public string VendorMacAddress { get; set; }
    }

    public class DhcpdReservationModel {
        public string HostName { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
    }

    public class NginxModel {
        public bool Active { get; set; } = false;

        public string User { get; set; }
        public string Group { get; set; }
        public string Processes { get; set; }
        public string FileLimit { get; set; }
        public string ErrorLog { get; set; } = "/var/log/nginx/error.log crit";

        public string EventsWorkerConnections { get; set; }
        public string EventsMultiAccept { get; set; }
        public string EventsUse { get; set; }
        public string EventsAcceptMutex { get; set; }

        public NginxHttpProtocolModel Http { get; set; }

        public List<NginxUpstreamModel> Upstreams { get; set; } = new List<NginxUpstreamModel>();
        public List<NginxServerModel> Servers { get; set; } = new List<NginxServerModel>();
    }

    public class NginxHttpProtocolModel {
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
        public string LogFormat { get; set; } = "main \'$remote_addr - $remote_user [$time_local] \"$request\" $status $bytes_sent \"$http_referer\" \"$http_user_agent\" \"$gzip_ratio\"\'";
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

    public class NginxUpstreamModel {
        public string Name { get; set; }
        public string Server { get; set; }
    }

    public class NginxServerModel {
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
        public List<NginxLocationModel> Locations { get; set; } = new List<NginxLocationModel>();
    }

    public class NginxLocationModel {
        public string Path { get; set; }
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
        public string ProxySslSessionReuse { get; set; }
    }

    public class FirewallModel {
        /// <summary>
        /// Indica se il servizio di firewall è attivo
        /// La configurazione può esistere ma il primo controllo per avviare il servizio viene fatto su questo parametro
        /// </summary>
        public bool Active { get; set; } = false;
        /// <summary>
        /// Set di parametri, da utilizzare nelle Tables
        /// Quindi qui vengono creati i set ma sono POI utilizzati nelle tabelle
        /// </summary>
        public FirewallSetModel[] Sets { get; set; } = new FirewallSetModel[0];
        /// <summary>
        /// Tabelle del Firewall
        /// </summary>
        public FirewallTableModel[] Tables { get; set; } = new FirewallTableModel[0];
    }

    /// <summary>
    /// Convenzione/Regola
    /// Tutte le chain sono raggruppate a seconda del proprio tipo (es. filter o nat)
    /// Quindi, creando prima la tabella le assegnerò il Type, che verrà ereditato dalle sue chain
    /// </summary>
    public class FirewallTableModel {
        /// <summary>
        /// Prende il valore da Firewall.Verbs.TableFamily
        /// </summary>
        public string Family { get; set; } = string.Empty;
        /// <summary>
        /// Nome della tabella
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Tipo di chain gestite
        /// Prende il valore da Firewall.Verbs.ChainType
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// Set di parametri presi da FirewallModel.Sets
        /// Idealmente non creo oggetti nuovi per questo valore ma li prendo dalla lista
        /// </summary>
        public FirewallSetModel[] Sets { get; set; } = new FirewallSetModel[0];
        public FirewallChainModel[] Chains { get; set; } = new FirewallChainModel[0];

        public override string ToString() {
            var setStr = new string[this.Sets.Length];
            for (var i = 0; i < this.Sets.Length; i++) {
                setStr[i] = this.Sets[i].ToString();
            }
            var chainStr = new string[this.Chains.Length];
            for (var i = 0; i < this.Chains.Length; i++) {
                chainStr[i] = this.Chains[i].ToString();
            }
            return CommonString.Append(this.Family, this.Name, CommonString.Build(setStr, ""), CommonString.Build(chainStr, ""));
        }
    }

    public class FirewallSetModel {
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Prende il valore da Firewall.Verbs.SetType
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// Prende il valore da Firewall.Verbs.SetFlag
        /// </summary>
        public string Flags { get; set; } = string.Empty;
        /// <summary>
        /// Oggetti che compongono il set
        /// </summary>
        public string[] Elements { get; set; } = new string[0];

        public override string ToString() {
            return CommonString.Append(this.Name, this.Type, this.Flags, CommonString.Build(this.Elements, ""));
        }
    }

    /// <summary>
    /// TODO gestire prior + default
    /// </summary>
    public class FirewallChainModel {
        //public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Prende il valore da Firewall.Verbs.ChainType
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// Prende il valore da Firewall.Verbs.ChainHook
        /// </summary>
        public string Hook { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// Regola di default
        /// Prende il valore da Firewall.Verbs.RuleJump
        /// </summary>
        public string DefaultJump { get; set; } = string.Empty;

        public FirewallRuleModel[] Rules { get; set; } = new FirewallRuleModel[0];

        public override string ToString() {
            var ruleStr = new string[this.Rules.Length];
            for (var i = 0; i < this.Rules.Length; i++) {
                ruleStr[i] = this.Rules[i].ToString();
            }
            return CommonString.Append(this.Type, this.Hook, CommonString.Build(ruleStr, ""));
        }
    }

    public class FirewallRuleModel {
        /// <summary>
        /// Prende il valore da Firewall.Verbs.RuleMatch
        /// </summary>
        public string Match { get; set; } = string.Empty;
        /// <summary>
        /// Prende il valore da Firewall.Verbs.RuleMatch{$Match}Arg
        /// </summary>
        public string MatchArgument { get; set; } = string.Empty;
        /// <summary>
        /// Oggetto filtrato dalla regola, può essere un oggetto singolo o un set
        /// </summary>
        public string Object { get; set; } = string.Empty;
        /// <summary>
        /// Prende il valore da Firewall.Verbs.RuleJump
        /// </summary>
        public string Jump { get; set; } = string.Empty;
        /// <summary>
        /// Definisce se il pacchetto filtrato deve essere loggato
        /// </summary>
        public bool Log { get; set; } = false;
        /// <summary>
        /// Se Log == true
        /// Definisce la stringa da appendere al pacchetto filtrato
        /// </summary>
        public string LogString { get; set; } = string.Empty;

        public override string ToString() {
            return CommonString.Append(this.Match, this.MatchArgument, this.Object, this.Jump);
        }
    }

    public class SambaModel {
        public bool Active { get; set; } = false;

        public string DosCharset { get; set; } = "";
        public string Workgroup { get; set; } = "";
        public string ServerString { get; set; } = "";
        public string MapToGuest { get; set; } = "";
        public string ObeyPamRestrictions { get; set; } = "";
        public string GuestAccount { get; set; } = "";
        public string PamPasswordChange { get; set; } = "";
        public string PasswdProgram { get; set; } = "";
        public string UnixPasswordSync { get; set; } = "";
        public string ResetOnZeroVc { get; set; } = "";
        public string HostnameLookups { get; set; } = "";
        public string LoadPrinters { get; set; } = "";
        public string PrintcapName { get; set; } = "";
        public string DisableSpoolss { get; set; } = "";
        public string TemplateShell { get; set; } = "";
        public string WinbindEnumUsers { get; set; } = "";
        public string WinbindEnumGroups { get; set; } = "";
        public string WinbindUseDefaultDomain { get; set; } = "";
        public string WinbindNssInfo { get; set; } = "";
        public string WinbindRefreshTickets { get; set; } = "";
        public string WinbindNormalizeNames { get; set; } = "";
        public string RecycleTouch { get; set; } = "";
        public string RecycleKeeptree { get; set; } = "";
        public string RecycleRepository { get; set; } = "";
        public string Nfs4Chown { get; set; } = "";
        public string Nfs4Acedup { get; set; } = "";
        public string Nfs4Mode { get; set; } = "";
        public string ShadowFormat { get; set; } = "";
        public string ShadowLocaltime { get; set; } = "";
        public string ShadowSort { get; set; } = "";
        public string ShadowSnapdir { get; set; } = "";
        public string RpcServerDefault { get; set; } = "";
        public string RpcServerSvcctl { get; set; } = "";
        public string RpcServerSrvsvc { get; set; } = "";
        public string RpcServerEventlog { get; set; } = "";
        public string RpcServerNtsvcs { get; set; } = "";
        public string RpcServerWinreg { get; set; } = "";
        public string RpcServerSpoolss { get; set; } = "";
        public string RpcDaemonSpoolssd { get; set; } = "";
        public string RpcServerTcpip { get; set; } = "";
        public string IdmapConfigBackend { get; set; } = "";
        public string ReadOnly { get; set; } = "";
        public string GuestOk { get; set; } = "";
        public string AioReadSize { get; set; } = "";
        public string AioWriteSize { get; set; } = "";
        public string EaSupport { get; set; } = "";
        public string DirectoryNameCacheSize { get; set; } = "";
        public string CaseSensitive { get; set; } = "";
        public string MapReadonly { get; set; } = "";
        public string StoreDosAttributes { get; set; } = "";
        public string WideLinks { get; set; } = "";
        public string DosFiletimeResolution { get; set; } = "";
        public string VfsObjects { get; set; } = "";

        public List<SambaResourceModel> Resources { get; set; } = new List<SambaResourceModel>();
    }

    public class SambaResourceModel {
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Path { get; set; }
    }

    public class SyslogNgModel {
        public bool Active { get; set; } = true;
        public string RootPath { get; set; } = "/cfg/antd/log";
        public string PortLevelApplication { get; set; } = "511";
        public string PortLevelSecurity { get; set; } = "512";
        public string PortLevelSystem { get; set; } = "513";
    }

    public class TorModel {
        public bool Active { get; set; } = false;
        public List<TorServiceModel> Services { get; set; } = new List<TorServiceModel>();
    }

    public class TorServiceModel {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string TorPort { get; set; }
    }

    public class ZpoolModel {
        public string Name { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Alloc { get; set; } = string.Empty;
        public string Free { get; set; } = string.Empty;
        public string Expandsz { get; set; } = string.Empty;
        public string Frag { get; set; } = string.Empty;
        public string Cap { get; set; } = string.Empty;
        public string Dedup { get; set; } = string.Empty;
        public string Health { get; set; } = string.Empty;
        public string Altroot { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class ZfsDatasetModel {
        public string Name { get; set; }
        public string Used { get; set; }
        public string Available { get; set; }
        public string Refer { get; set; }
        public string Mountpoint { get; set; }
    }

    public class ZfsSnapshotModel {
        public string Name { get; set; }
        public string Used { get; set; }
        public string Available { get; set; }
        public string Refer { get; set; }
        public string Mountpoint { get; set; }
        public bool IsEmpty { get; set; }

        public int Index { get; set; }
        public DateTime Created { get; set; }
        public long Dimension { get; set; }
    }

    public class RsyncModel {
        public bool Active { get; set; }
        public RsyncObjectModel[] Directories { get; set; } = new RsyncObjectModel[0];
    }

    public class RsyncObjectModel {
        public string Type { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
    }

    public class MonitorModel {
        public string Hostname { get; set; }
        public string Uptime { get; set; }
        public string LoadAverage { get; set; }
        public string MemoryUsage { get; set; }
        public string DiskUsage { get; set; }
    }

    public class NetworkAdapterInfo {

        public class AddressConfiguration {
            /// <summary>
            /// Configurazione statica dell'ip
            ///  se false usa dhclient
            /// </summary>
            public bool StaticAddress { get; set; } = true;

            public string IpAddr { get; set; } = string.Empty;
            public byte NetworkRange { get; set; } = (byte)0;

            public override string ToString() {
                return CommonString.Append(this.StaticAddress.ToString(), this.IpAddr, this.NetworkRange.ToString());
            }
        }

        public class HardwareConfiguration {
            public long Mtu { get; set; } = 6000;
            public long Txqueuelen { get; set; } = 10000;
            /// <summary>
            ///     1 promisc on
            ///     0 promisc off
            ///     -1 ignora configurazione
            /// </summary>
            public bool Promisc { get; set; } = true;
            public string MacAddress { get; set; } = string.Empty;

            public override string ToString() {
                return CommonString.Append(this.Mtu.ToString(), this.Txqueuelen.ToString());
            }
        }
    }

    [XmlRoot(ElementName = "specVersion")]
    public class SpecVersion {
        [XmlElement(ElementName = "major")]
        public string Major { get; set; }
        [XmlElement(ElementName = "minor")]
        public string Minor { get; set; }
    }

    [XmlRoot(ElementName = "service")]
    public class Service {
        [XmlElement(ElementName = "serviceType")]
        public string ServiceType { get; set; }
        [XmlElement(ElementName = "serviceId")]
        public string ServiceId { get; set; }
        [XmlElement(ElementName = "controlURL")]
        public string ControlURL { get; set; }
        [XmlElement(ElementName = "eventSubURL")]
        public string EventSubURL { get; set; }
        [XmlElement(ElementName = "SCPDURL")]
        public string SCPDURL { get; set; }
    }

    [XmlRoot(ElementName = "serviceList")]
    public class ServiceList {
        [XmlElement(ElementName = "service")]
        public List<Service> Service { get; set; }
    }

    [XmlRoot(ElementName = "device")]
    public class Device {
        [XmlElement(ElementName = "deviceType")]
        public string DeviceType { get; set; }
        [XmlElement(ElementName = "friendlyName")]
        public string FriendlyName { get; set; }
        [XmlElement(ElementName = "manufacturer")]
        public string Manufacturer { get; set; }
        [XmlElement(ElementName = "manufacturerURL")]
        public string ManufacturerURL { get; set; }
        [XmlElement(ElementName = "modelDescription")]
        public string ModelDescription { get; set; }
        [XmlElement(ElementName = "modelName")]
        public string ModelName { get; set; }
        [XmlElement(ElementName = "modelNumber")]
        public string ModelNumber { get; set; }
        [XmlElement(ElementName = "modelURL")]
        public string ModelURL { get; set; }
        [XmlElement(ElementName = "serialNumber")]
        public string SerialNumber { get; set; }
        [XmlElement(ElementName = "UDN")]
        public string UDN { get; set; }
        [XmlElement(ElementName = "UPC")]
        public string UPC { get; set; }
        [XmlElement(ElementName = "serviceList")]
        public ServiceList ServiceList { get; set; }
    }

    [XmlRoot(ElementName = "deviceList")]
    public class DeviceList {
        [XmlElement(ElementName = "device")]
        public Device Device { get; set; }
    }

    [XmlRoot(ElementName = "root")]
    public class ServiceDiscoveryModel {
        [XmlElement(ElementName = "specVersion")]
        public SpecVersion SpecVersion { get; set; }
        [XmlElement(ElementName = "device")]
        public Device Device { get; set; }
    }

    public class DeviceUidsModel {
        public string Uuid { get; set; }
        public string Upnp { get; set; }
        public string Pnp { get; set; }
        public string Urn { get; set; }
        public string Device { get; set; }
    }

    public class NodeModel {
        public string RawUid { get; set; }
        public string Upnp { get; set; }
        public string Pnp { get; set; }
        public string Urn { get; set; }
        public string Device { get; set; }
        public string MachineUid { get; set; }
        public string DescriptionLocation { get; set; }
        public string DeviceType { get; set; }
        public string Hostname { get; set; }
        public string PublicIp { get; set; }
        public string Manufacturer { get; set; }
        public string ModelName { get; set; }
        public string ModelDescription { get; set; }
        public string ModelNumber { get; set; }
        public string ModelUrl { get; set; }
        public string SerialNumber { get; set; }
    }

    public class VirshModel {
        public bool Active { get; set; } = false;
        public VirshDomainModel[] Domains { get; set; } = new VirshDomainModel[0];
    }

    public class VirshDomainModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }

        public string UUID { get; set; }
        public string OsType { get; set; }
        public string Cpus { get; set; }
        public string CpuTime { get; set; }
        public string MaxMemory { get; set; }
        public string UsedMemory { get; set; }
    }

    /// <summary>
    /// Check list di elementi da controllare localmente
    /// Ogni parametro sarà un byte valorizzato a 0 o 1
    /// dove 0 => OK
    ///      1 => ci sono problemi
    /// </summary>
    public class MachineStatusChecklistModel {
        /// <summary>
        /// Prova a raggiungere "internet" tramite un IP
        /// es  8.8.8.8
        /// </summary>
        public byte InternetReach { get; set; }
        /// <summary>
        /// Prova a raggiungere "internet" tramite un indirizzo dns
        /// es  www.google.com
        /// </summary>
        public byte InternetDnsReach { get; set; }

        public override string ToString() {
            return CommonString.Append(this.InternetReach.ToString(), this.InternetDnsReach.ToString());
        }
    }

    /// <summary>
    /// Check list di elementi da controllare
    /// Relativamente a un nodo
    /// Es: da nodo01 a nodo02 cosa riesco o non riesco a raggiungere/vedere
    /// Ogni parametro sarà un byte valorizzato a 0 o 1
    /// dove 0 => OK
    ///      1 => ci sono problemi
    /// </summary>
    public class ClusterNodeChecklistModel {
        /// <summary>
        /// UID del nodo analizzato
        /// </summary>
        public string TargetNodeMachineUid { get; set; } = string.Empty;
        /// <summary>
        /// Hostname del nodo analizzato
        /// </summary>
        public string Hostname { get; set; } = string.Empty;
        /// <summary>
        /// Prova a raggiungere "internet" tramite un IP
        /// es  8.8.8.8
        /// </summary>
        public byte InternetReach { get; set; } = 1;
        /// <summary>
        /// Prova a raggiungere "internet" tramite un indirizzo dns
        /// es  www.google.com
        /// </summary>
        public byte InternetDnsReach { get; set; } = 1;
        /// <summary>
        /// Prova a raggiungere l'ip conosciuto, configurato nelle impostazioni del cluster ed esposto da rssdp
        /// </summary>
        public byte KnownPublicIpReach { get; set; } = 1;
        /// <summary>
        /// Prova a raggiungere tutti gli ip impostati sul nodo
        /// </summary>
        public ClusterNodeIpStatusModel[] DiscoveredIpsReach { get; set; } = new ClusterNodeIpStatusModel[0];
        /// <summary>
        /// Raggiungibilità di antd
        /// </summary>
        public byte ServiceReach { get; set; } = 1;
        /// <summary>
        /// Uptime di antd
        /// </summary>
        public string ApplicationUptime { get; set; } = string.Empty;

        /// <summary>
        /// Stato del servizio: Virsh
        /// </summary>
        public VirshModel VirshService { get; set; } = new VirshModel();

        public override string ToString() {
            var dlines = new string[this.DiscoveredIpsReach.Length];
            for (var i = 0; i < this.DiscoveredIpsReach.Length; i++) {
                dlines[i] = this.DiscoveredIpsReach[i].ToString();
            }
            var dict = CommonString.Build(dlines, "");
            return CommonString.Append(this.TargetNodeMachineUid,
                this.InternetReach.ToString(),
                this.InternetDnsReach.ToString(),
                this.KnownPublicIpReach.ToString(),
                dict);
        }
    }

    public class ClusterNodeIpStatusModel {
        public string IpAddress { get; set; } = string.Empty;
        public byte Status { get; set; } = 1;

        public override string ToString() {
            return CommonString.Append(this.IpAddress, this.Status.ToString());
        }
    }

    public class ClusterNodeStatusModel {
        public ClusterNode Node { get; set; } = new ClusterNode();
        public ClusterNodeChecklistModel Status { get; set; } = new ClusterNodeChecklistModel();
    }

    public class StorageServerFileMetadata {
        public string FilePath { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public long Size { get; set; }
    }

    public class StorageServerFolder {
        public string FolderPath { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public StorageServerFolder[] Folders { get; set; }
        public StorageServerFileMetadata[] Files { get; set; }
    }
}
