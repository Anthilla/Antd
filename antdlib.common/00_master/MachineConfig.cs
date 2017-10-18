using Antd.models;
using anthilla.core;
using System;
using System.Linq;

namespace Antd {

    [Serializable]
    public class MachineConfig {
        /// <summary>
        /// Definisce l'identità della macchina
        /// </summary>
        public Host Host { get; set; } = new Host();

        /// <summary>
        /// Definizione dei riferimenti orari
        /// </summary>
        public TimeDate TimeDate { get; set; } = new TimeDate();

        /// <summary>
        /// Configurazione da applicare in fase di avvio di antd
        ///     - parametri di sistema (sysctl)
        ///     - moduli
        ///     - servizi (oltre quelli configurabili)
        /// </summary>
        public Boot Boot { get; set; } = new Boot();

        /// <summary>
        /// Utenti di sistema
        /// </summary>
        public Users Users { get; set; } = new Users();

        /// <summary>
        /// Configurazione di rete
        /// </summary>
        public Network Network { get; set; } = new Network();

        /// <summary>
        /// Configurazione del web service di antd (Nancy)
        /// </summary>
        public WebService WebService { get; set; } = new WebService();

        /// <summary>
        /// Configurazione di /etc/nsswitch.conf
        /// La configurazione di default la carico una volta all'avvio di antd
        /// </summary>
        public NsSwitch NsSwitch { get; set; } = new NsSwitch();

        /// <summary>
        /// Comandi "custom" da lanciare all'avvio
        /// La configurazione di default la carico una volta all'avvio di antd
        /// </summary>
        public Command[] SetupCommands { get; set; } = new Command[0];

        /// <summary>
        /// Servizi
        /// </summary>
        public Service Services { get; set; } = new Service();

        /// <summary>
        /// Configurazione e stato dello storage
        /// </summary>
        public Storage Storage { get; set; } = new Storage();

        /// <summary>
        /// Configurazione dei parametri del cluster e informazioni sugli altri nodi
        /// </summary>
        public Cluster Cluster { get; set; } = new Cluster();

        /// <summary>
        /// Applicazioni di Anthilla gestite da antd
        /// </summary>
        public Applications Applications { get; set; } = new Applications();
    }

    [Serializable]
    public class MachineStatus : MachineConfig {

        /// <summary>
        /// Informazioni sullo stato della macchina (ex MachineInfo)
        /// </summary>
        public Info Info { get; set; } = new Info();
    }

    /// <summary>
    /// Informazioni sullo stato della macchina (ex MachineInfo)
    /// </summary>
    public class Info {
        public UptimeModel Uptime { get; set; } = new UptimeModel();

        public CpuInfoModel[] CpuInfo { get; set; } = new CpuInfoModel[0];

        public MemInfoModel[] MemInfo { get; set; } = new MemInfoModel[0];

        public FreeModel[] Free { get; set; } = new FreeModel[0];

        public LosetupModel[] Losetup { get; set; } = new LosetupModel[0];

        public DiskUsageModel[] DiskUsage { get; set; } = new DiskUsageModel[0];

        /// <summary>
        /// Informazioni sulle versioni di applicazione (antd) e del sistema operativo (system + kernel)
        /// </summary>
        public Versions Versions { get; set; } = new Versions();
    }

    /// <summary>
    /// Informazioni sulle versioni di applicazione (antd) e del sistema operativo (system + kernel)
    /// </summary>
    public class Versions {
        public VersionElement System { get; set; } = new VersionElement();

        public VersionElement Firmware { get; set; } = new VersionElement();
        public VersionElement Modules { get; set; } = new VersionElement();
        public VersionElement SystemMap { get; set; } = new VersionElement();
        public VersionElement Initrd { get; set; } = new VersionElement();
        public VersionElement Kernel { get; set; } = new VersionElement();

        public VersionElement Antd { get; set; } = new VersionElement();
        public VersionElement AntdGui { get; set; } = new VersionElement();
        public VersionElement Antdsh { get; set; } = new VersionElement();
    }

    public class VersionElement {
        /// <summary>
        /// Etichetta dell'elemento
        /// </summary>
        public string Ver { get; set; } = string.Empty;

        /// <summary>
        /// Indica se la versione coincide con l'ultima più aggiornata (usare antdsh)
        /// </summary>
        public bool Uptodate { get; set; } = true;
    }

    /// <summary>
    /// Raccoglie i parametri per definire l'identità della macchina
    /// </summary>
    public class Host {
        public Guid MachineUid { get; set; } = Guid.Empty;
        public Guid SerialNumber { get; set; } = Guid.Empty;
        public Guid PartNumber { get; set; } = Guid.Empty;

        /// <summary>
        /// Definisce il nome dell'host (anche visto dalle altre macchine)
        /// </summary>
        public string HostName { get; set; } = "box01";
        public string HostChassis { get; set; } = "server";
        public string HostDeployment { get; set; } = "developement";
        public string HostLocation { get; set; } = "onEarth";
    }

    /// <summary>
    /// Raccoglie i parametri che gestiscono la configurazione temporale della macchina
    /// </summary>
    public class TimeDate {
        public string Timezone { get; set; } = "Europe/Rome";

        /// <summary>
        /// Switch per attivare/disattivare la sincronizzazione oraria locale
        ///     hwclock -s
        ///     hwclock -w
        /// </summary>
        public bool SyncLocalClock { get; set; } = true;

        /// <summary>
        /// Switch per attivare/disattivare la sincronizzazione oraria da un server remoto
        ///     RemoteNtpServer deve essere configurato
        ///     -1 = ignora
        /// </summary>
        public bool SyncFromRemoteServer { get; set; } = false;

        /// <summary>
        /// Server remoto da cui ottenere l'orario
        ///     SyncFromRemoteServer deve essere active
        /// </summary>
        public string RemoteNtpServer { get; set; } = string.Empty;
    }

    /// <summary>
    /// Raccoglie alcuni valori che devono essere applicati in fase di avvio di antd
    /// </summary>
    public class Boot {
        /// <summary>
        /// Parametri di sistema (sysctl)
        /// </summary>
        public SystemParameter[] Parameters { get; set; } = new SystemParameter[] {
            new SystemParameter() { Key = "/proc/sys/fs/file-max", Value = "1024000" },
            new SystemParameter() { Key = "/proc/sys/net/bridge/bridge-nf-call-arptables", Value = "0" },
            new SystemParameter() { Key = "/proc/sys/net/bridge/bridge-nf-call-ip6tables", Value = "0" },
            new SystemParameter() { Key = "/proc/sys/net/bridge/bridge-nf-call-iptables", Value = "0" },
            new SystemParameter() { Key = "/proc/sys/net/bridge/bridge-nf-filter-pppoe-tagged", Value = "0" },
            new SystemParameter() { Key = "/proc/sys/net/bridge/bridge-nf-filter-vlan-tagged", Value = "0" },
            new SystemParameter() { Key = "/proc/sys/net/core/netdev_max_backlog", Value = "300000" },
            new SystemParameter() { Key = "/proc/sys/net/core/optmem_max", Value = "40960" },
            new SystemParameter() { Key = "/proc/sys/net/core/rmem_max", Value = "268435456" },
            new SystemParameter() { Key = "/proc/sys/net/core/somaxconn", Value = "65536" },
            new SystemParameter() { Key = "/proc/sys/net/core/wmem_max", Value = "268435456" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/conf/all/accept_local", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/conf/all/accept_redirects", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/conf/all/accept_source_route", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/conf/all/rp_filter", Value = "0" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/conf/all/forwarding", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/conf/default/rp_filter", Value = "0" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/ip_forward", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/ip_local_port_range", Value = "1024 65000" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/ip_no_pmtu_disc", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_congestion_control", Value = "htcp" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_fin_timeout", Value = "40" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_max_syn_backlog", Value = "3240000" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_max_tw_buckets", Value = "1440000" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_moderate_rcvbuf", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_mtu_probing", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_rmem", Value = "4096 87380 134217728" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_slow_start_after_idle", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_tw_recycle", Value = "0" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_tw_reuse", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_window_scaling", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/net/ipv4/tcp_wmem", Value = "4096 65536 134217728" },
            new SystemParameter() { Key = "/proc/sys/net/ipv6/conf/all/disable_ipv6", Value = "1" },
            new SystemParameter() { Key = "/proc/sys/vm/swappiness", Value = "0" }
        };

        /// <summary>
        /// Moduli
        /// </summary>
        public SystemModule[] Modules { get; set; } = new SystemModule[] {
            new SystemModule { Module = "tun", Arguments= "", Active= true },
            new SystemModule { Module = "br_netfilter", Arguments= "", Active= true }
        };

        /// <summary>
        /// Servizi
        /// </summary>
        public SystemService[] Services { get; set; } = new SystemService[] {
            new SystemService { Service = "systemd-networkd.service", Active = false, Start= false, Masking  = true },
            new SystemService { Service = "systemd-resolved.service", Active = false, Start= false, Masking  = true }
        };
    }

    public class SystemParameter {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class SystemModule {
        public string Module { get; set; } = string.Empty;

        /// <summary>
        /// Parametri ulteriori da passare al modulo in fase di caricamento
        /// </summary>
        public string Arguments { get; set; } = string.Empty;

        /// <summary>
        /// Indica se il modulo deve essere caricato
        /// </summary>
        public bool Active { get; set; } = false;

        /// <summary>
        /// Indica se il modulo va rimosso
        /// </summary>
        public bool Remove { get; set; } = false;

        /// <summary>
        /// Indica se il modulo deve essere messo in blacklist
        /// </summary>
        public bool Blacklist { get; set; } = false;
    }

    public class SystemService {

        public SystemctlType Type { get; set; } = SystemctlType.none;

        public string Service { get; set; } = string.Empty;

        /// <summary>
        /// Definisce se il servizio deve essere abilitato o meno
        ///     1 enable
        ///     0 disable
        ///     -1 = ignora
        /// </summary>
        public bool Active { get; set; } = false;

        /// <summary>
        /// Definisce se il servizio deve avviarsi o meno
        ///     1 stop
        ///     0 start
        ///     -1 = ignora
        /// </summary>
        public bool Start { get; set; } = false;

        /// <summary>
        /// Forza il restart, senza controllare lo stato
        ///     -1 = ignora
        /// </summary>
        public bool ForceRestart { get; set; } = false;

        /// <summary>
        /// Definisce se la unit del servizion deve essere mascherata
        ///     1 mask
        ///     0 unmask
        ///     -1 = ignora
        /// </summary>
        public bool Masking { get; set; } = false;
    }

    /// <summary>
    /// Informazioni su utenti di sistema e utenti applicativi
    /// </summary>
    public class Users {
        public SystemUser[] SystemUsers { get; set; } = new SystemUser[0];

        public ApplicativeUser[] ApplicativeUsers { get; set; } = new ApplicativeUser[0];
    }

    /// <summary>
    /// Informazioni sugli utenti di sistema, da applicare al boot della macchina
    /// </summary>
    public class SystemUser {
        /// <summary>
        /// Definisce se l'utente è abilitato
        ///     default = 1
        /// </summary>
        public bool Active { get; set; } = true;

        public string Alias { get; set; } = string.Empty;

        /// <summary>
        /// La password salvata deve essere come hashing compatibile con shadow e passwd
        /// Se l'utente non è abilitato la password viene settata vuota
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Informazioni sulle utenze applicative, le applicazioni faranno il confronto su questi dati
    /// Questo repository sarà una mappatura dei vari meccanismi di autenticazione integrati
    /// 
    /// L'esito dell'autenticazione verrà confrontato sulla corrispondenza di Id + Password
    /// Dove Id e Password, dal lato applicativo, possono essere una "qualsiasi" coppia di informazioni
    /// 
    /// NB: nel framework di Nancy verrà mappato con un oggetto che eredita IUserIdentity
    /// </summary>
    public class ApplicativeUser {
        /// <summary>
        /// Indica se l'utenza è attiva o meno
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Tipo di autenticazione da effettuare
        /// </summary>
        public AuthenticationType Type { get; set; } = AuthenticationType.simple;

        /// <summary>
        /// Identificativo dell'utente (?)
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Dati dell'utente da confrontare
        /// Saranno oscurati, es: hashing
        /// </summary>
        public string[] Claims { get; set; } = new string[0];
    }

    /// <summary>
    /// Configurazione di rete
    /// </summary>
    public class Network {

        public string PrimaryDomain { get; set; } = string.Empty;

        /// <summary>
        /// Configurazione di /etc/resolv.conf
        /// </summary>
        public DnsClientConfiguration KnownDns { get; set; } = new DnsClientConfiguration();

        /// <summary>
        /// Configurazione di /etc/hosts
        ///     contiene i dati degli host che saranno configurati nel cluster
        /// NON contiene gli eventuali default
        /// </summary>
        public KnownHost[] KnownHosts { get; set; } = new KnownHost[0];

        /// <summary>
        /// Configurazione di /etc/networks
        ///     contiene i dati della configurazione di network
        /// NON contiene gli eventuali default
        /// </summary>
        public KnownNetwork[] KnownNetworks { get; set; } = new KnownNetwork[0];

        public NetTun[] Tuns { get; set; } = new NetTun[0];

        public NetTap[] Taps { get; set; } = new NetTap[0];

        public NetBridge[] Bridges { get; set; } = new NetBridge[0];

        public NetBond[] Bonds { get; set; } = new NetBond[0];

        /// <summary>
        /// Configurazione della rete interna
        /// Utilizzo QUESTI parametri per configurare l'ip dell'interfaccia principale
        /// </summary>
        public SubNetwork InternalNetwork { get; set; } = new SubNetwork();

        /// <summary>
        /// Configrazione della rete esterna
        /// </summary>
        public SubNetwork ExternalNetwork { get; set; } = new SubNetwork(false);

        /// <summary>
        /// Configurazione delle interfacce di rete
        ///     i valori di default verranno popolati all'avvio dopo aver importato i dati esistenti
        /// </summary>
        public NetInterface[] NetworkInterfaces { get; set; } = new NetInterface[0];

        /// <summary>
        /// Preset delle configurazioni dei Gateway da applicare al routing
        /// </summary>
        public NetGateway[] Gateways { get; set; } = new NetGateway[0];

        /// <summary>
        /// Configurazione delle tabelle di routing
        /// </summary>
        public NetRoutingTable[] RoutingTables { get; set; } = new NetRoutingTable[0];

        /// <summary>
        /// Configurazione delle rotte
        /// </summary>
        public NetRoute[] Routing { get; set; } = new NetRoute[0];

        /// <summary>
        /// Configurazione Wi-Fi
        /// </summary>
        public WpaSupplicant WpaSupplicant { get; set; } = new WpaSupplicant();
    }

    public class DnsClientConfiguration {
        public string[] Nameserver { get; set; } = new string[] { "8.8.8.8", "8.8.4.4" };
        public string Search { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;

        public override string ToString() {
            return CommonString.Append(CommonString.Build(this.Nameserver), this.Search, this.Domain);
        }
    }

    public class KnownHost {
        public string IpAddr { get; set; } = string.Empty;
        public string[] CommonNames { get; set; } = new string[0];

        public override string ToString() {
            return CommonString.Append(this.IpAddr, CommonString.Build(this.CommonNames));
        }
    }

    public class KnownNetwork {
        public string Label { get; set; } = string.Empty;
        public string NetAddr { get; set; } = string.Empty;

        public override string ToString() {
            return CommonString.Append(this.Label, this.NetAddr);
        }
    }

    public class NetTun {
        /// <summary>
        /// Nome dell'interfaccia virtuale
        /// </summary>
        public string Id { get; set; }
    }

    public class NetTap {
        /// <summary>
        /// Nome dell'interfaccia virtuale
        /// </summary>
        public string Id { get; set; }
    }

    /// <summary>
    /// Configurazione di interfacce Bridge
    /// </summary>
    public class NetBridge {
        /// <summary>
        /// Nome del bridge
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Interfacce collegate
        /// </summary>
        public string[] Lower { get; set; } = new string[0];

        public override string ToString() {
            if(this == null) {
                return string.Empty;
            }
            return CommonString.Append(this.Id, CommonString.Build(this.Lower));
        }
    }

    /// <summary>
    /// Configurazione di interfacce Bond
    /// </summary>
    public class NetBond {
        /// <summary>
        /// Nome del bond
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Interfacce collegate
        /// </summary>
        public string[] Lower { get; set; } = new string[0];

        public override string ToString() {
            if(this == null) {
                return string.Empty;
            }
            return CommonString.Append(this.Id, CommonString.Build(this.Lower));
        }
    }

    public class SubNetwork {

        public SubNetwork() { }
        public SubNetwork(bool active) { Active = active; }

        public bool Active { get; set; } = true;

        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Dominio di riferimento per la sottorete, se non specificato si fa riferimento a Network.PrimaryDomain
        /// </summary>
        public string Domain { get; set; } = string.Empty;

        /// <summary>
        /// Definizione del gateway di default per la sottorete, l'indirizzo deve appartenere alla stessa classe della sottorete
        /// </summary>
        public string DefaultGateway { get; set; } = string.Empty;

        /// <summary>
        /// Interfaccia di rete principale
        /// L'etichetta deve appartenere a un'interfaccia presente in Network.NetworkAdapters
        /// </summary>
        public string NetworkAdapter { get; set; } = string.Empty;

        /// <summary>
        /// Configurazione statica dell'ip
        ///  se false usa dhclient
        /// </summary>
        public bool StaticAddress { get; set; } = true;

        /// <summary>
        /// Indirizzo ip della rete
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

        public byte NetworkRange { get; set; } = (byte)0;

        /// <summary>
        /// Definisce la classe della rete (es: 10.1.0.0)
        /// Va in /etc/networks
        /// </summary>
        public string NetworkClass { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configurazione dell'interfaccia di rete
    /// </summary>
    public class NetInterface {
        /// <summary>
        /// Definisce se l'interfaccia deve essere abilitata o meno
        ///     default = 1 up
        ///     0 = down
        ///     -1 = ignora configurazione
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Id dell'interfaccia di rete
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nome dell'interfaccia di rete
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Tipo di interfaccia di rete
        /// Al momento il default è models.NetworkAdapterType.Physical perché non viene gestito nella creazione dell'interfaccia
        /// In questo modo "almeno" si prende l'ip configurato
        /// TODO: gestire in fase di creazione della configurazione dell'interfaccia il tipo
        /// </summary>
        public models.NetworkAdapterType Type { get; set; } = models.NetworkAdapterType.Physical;

        /// <summary>
        /// Definisce se appartiene alla rete interna o esterna
        /// </summary>
        public NetworkAdapterMembership Membership { get; set; } = NetworkAdapterMembership.none;

        /// <summary>
        /// Classe della rete di appartenenza
        ///     ereditato da SubNetwork
        /// </summary>
        public string NetworkClass { get; set; } = string.Empty;

        /// <summary>
        /// Indirizzo IP primario
        ///     deve appartenere alla rete definita da NetworkClass
        /// </summary>
        public NetworkAdapterInfo.AddressConfiguration PrimaryAddressConfiguration { get; set; } = new NetworkAdapterInfo.AddressConfiguration();

        /// <summary>
        /// Indirizzamanti secondari
        ///     non sono tenuti ad appartenere alla rete definita da NetworkClass
        /// </summary>
        public NetworkAdapterInfo.AddressConfiguration[] SecondaryAddressConfigurations { get; set; } = new NetworkAdapterInfo.AddressConfiguration[0];

        public NetworkAdapterInfo.HardwareConfiguration HardwareConfiguration { get; set; } = new NetworkAdapterInfo.HardwareConfiguration();

        /// <summary>
        /// Identifica se l'interfaccia di rete è a sua volta un aggregatore di interfacce (bridge o bond)
        /// </summary>
        public bool InterfaceAggregator { get; set; } = false;

        /// <summary>
        /// Interfacce aggregate
        /// </summary>
        public string[] LowerInterfaces { get; set; } = new string[0];

        public override string ToString() {
            return CommonString.Append(this.Id, this.HardwareConfiguration.ToString(), this.PrimaryAddressConfiguration.ToString(), CommonString.Build(this.SecondaryAddressConfigurations.Select(_ => _.ToString()).ToArray()));
        }
    }

    /// <summary>
    /// Configurazione dei Gateway
    /// Vanno poi utilizzati da NetRoute 
    /// Sono dei "preset"
    /// </summary>
    public class NetGateway {
        /// <summary>
        /// Nome/etichetta del gateway
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Indirizzo del Gateway
        /// Questo parametro va passato alla configurazione di NetRoute
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configurazione delle tabelle di routing
    /// </summary>
    public class NetRoutingTable {
        /// <summary>
        /// Indice della tabella
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nome della tabella
        /// </summary>
        public string Alias { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configurazione delle rotte
    /// </summary>
    public class NetRoute {
        /// <summary>
        /// Rotta di default
        ///     -1 ignora
        /// Può esserci solo 1 rotta di default
        /// </summary>
        public bool Default { get; set; } = false;

        /// <summary>
        /// Destinazione della rotta, se Default = = true allora Destination = "default"
        /// </summary>
        public string Destination { get; set; } = string.Empty;

        public string Gateway { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;

        public override string ToString() {
            return CommonString.Append(Destination, Gateway, Device);
        }
    }

    /// <summary>
    /// Configurazione Wi-Fi 
    /// </summary>
    public class WpaSupplicant {
        public bool Active { get; set; } = false;

        public string Interface { get; set; } = string.Empty;

        public string Ssid { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configurazione del web service di antd (Nancy)
    /// </summary>
    public class WebService {
        /// <summary>
        /// Definisce se lanciare il web service di interfaccia grafica
        /// </summary>
        public bool GuiWebServiceActive { get; set; } = true;

        public int Port { get; set; } = 8084;
        public int GuiWebServicePort { get; set; } = 8086;

        public string Cloud { get; set; } = "http://api.anthilla.com";

        public string MasterPassword { get; set; } = "252975977253103541671893814013814116237132841698924515721578242135731312536863201";

        /// <summary>
        /// Protocollo di comunicazione tra antd e antdui
        /// </summary>
        public string Protocol { get; set; } = "http";

        public string Host { get; set; } = "127.0.0.1";
    }

    /// <summary>
    /// Configurazione di /etc/nsswitch.conf
    /// </summary>
    public class NsSwitch {
        public string Aliases { get; set; } = string.Empty;
        public string Ethers { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Hosts { get; set; } = string.Empty;
        public string Initgroups { get; set; } = string.Empty;
        public string Netgroup { get; set; } = string.Empty;
        public string Networks { get; set; } = string.Empty;
        public string Passwd { get; set; } = string.Empty;
        public string Protocols { get; set; } = string.Empty;
        public string Publickey { get; set; } = string.Empty;
        public string Rpc { get; set; } = string.Empty;
        public string Services { get; set; } = string.Empty;
        public string Shadow { get; set; } = string.Empty;
        public string Netmasks { get; set; } = string.Empty;
        public string Bootparams { get; set; } = string.Empty;
        public string Automount { get; set; } = string.Empty;

        public override string ToString() {
            return CommonString.Append(this.Aliases, this.Automount, this.Bootparams, this.Ethers, this.Group, this.Hosts, this.Initgroups, this.Netgroup, this.Netmasks, this.Passwd, this.Protocols, this.Publickey, this.Rpc, this.Services, this.Shadow);
        }
    }

    /// <summary>
    /// Comando "custom" da lanciare all'avvio
    /// </summary>
    public class Command {
        public string BashCommand { get; set; } = string.Empty;

        /// <summary>
        /// Comando da lanciare prima di BashCommand e se il risultato contiene ControlResult 
        /// </summary>
        public string ControlBashCommand { get; set; } = string.Empty;
        public string ControlResult { get; set; } = string.Empty;
    }

    /// <summary>
    /// Servizi
    /// Dove possibile il modello della configurazione sarà contestuale al servizio stesso
    /// es: service = ServiceModel
    /// </summary>
    public class Service {
        public ServiceSsh Ssh { get; set; } = new ServiceSsh();

        public SshdModel Sshd { get; set; } = new SshdModel();

        public FirewallModel Firewall { get; set; } = new FirewallModel();

        public DhcpdModel Dhcpd { get; set; } = new DhcpdModel();

        public BindModel Bind { get; set; } = new BindModel();

        public NginxModel Nginx { get; set; } = new NginxModel();

        public SambaModel Samba { get; set; } = new SambaModel();

        public SyslogNgModel SyslogNg { get; set; } = new SyslogNgModel();

        public TorModel Tor { get; set; } = new TorModel();

        public CaModel Ca { get; set; } = new CaModel();

        public RsyncModel Rsync { get; set; } = new RsyncModel();

        public VirshModel Virsh { get; set; } = new VirshModel();
    }

    public class ServiceSsh {
        public string PublicKey { get; set; } = string.Empty;

        public string PrivateKey { get; set; } = string.Empty;

        public AuthorizedKey[] AuthorizedKey { get; set; } = new AuthorizedKey[0];
    }

    public class AuthorizedKey {
        public string User { get; set; } = string.Empty;

        public string Host { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configurazione e stato dello storage
    /// </summary>
    public class Storage {
        /// <summary>
        /// Punti di montaggio configurati
        /// </summary>
        public MountElement[] Mounts { get; set; } = new MountElement[0];

        public ZpoolModel[] Zpools { get; set; } = new ZpoolModel[0];

        public ZfsDatasetModel[] ZfsDatasets { get; set; } = new ZfsDatasetModel[0];

        public ZfsSnapshotModel[] ZfsSnapshots { get; set; } = new ZfsSnapshotModel[0];

        public RemoteVfsServer Server { get; set; } = new RemoteVfsServer();
    }

    /// <summary>
    /// Configurazione di un punto di montaggio
    /// </summary>
    public class MountElement {

        /// <summary>
        /// Indica se il punto di montaggio configurato è attivo
        ///     -1 = ignora
        /// </summary>
        public bool Active { get; set; } = false;

        /// <summary>
        /// Indica l'opzione bind nel comando mount
        /// </summary>
        public bool Bind { get; set; } = false;

        /// <summary>
        /// Cartella sorgente da montare
        /// </summary>
        public string Folder { get; set; } = string.Empty;

        /// <summary>
        /// Punto di montaggio nel file system
        /// </summary>
        public string MountPoint { get; set; } = string.Empty;

        public string Device { get; set; } = string.Empty;
        public string FileSystem { get; set; } = string.Empty;
        public string MountOptions { get; set; } = string.Empty;
    }

    public class RemoteVfsServer {
        public bool Active { get; set; } = false;
        public int Port { get; set; }
    }

    /// <summary>
    /// Configurazione dei parametri del cluster e informazioni sugli altri nodi
    /// </summary>
    public class Cluster {
        public bool Active { get; set; } = false;

        public Guid Id { get; set; } = Guid.Empty;
        public string Label { get; set; } = string.Empty;

        public ClusterNode[] Nodes { get; set; } = new ClusterNode[0];

        /// <summary>
        /// Condivisione della rete -> haproxy e keepalived
        /// </summary>
        public ClusterNetwork SharedNetwork { get; set; } = new ClusterNetwork();

        /// <summary>
        /// Sincronizzazione di servizi
        /// </summary>
        public ClusterService SharedService { get; set; } = new ClusterService();

        /// <summary>
        /// Condivisione dei dati sui dischi -> gluster e rsync
        /// </summary>
        public ClusterFs SharedFs { get; set; } = new ClusterFs();
    }

    public class ClusterNode {
        public string MachineUid { get; set; }
        public string Hostname { get; set; }
        public string PublicIp { get; set; }
        public string EntryPoint { get; set; }
        public ClusterNodeService[] Services { get; set; } = new ClusterNodeService[0];
        public GlusterVolumeModel[] Volumes { get; set; } = new GlusterVolumeModel[0];
    }

    public class ClusterNodeService {
        public string ServiceType { get; set; }
        public string ControlURL { get; set; }
        public string Name { get; set; }
        public long Port { get; set; }
    }

    public class ClusterNetwork {
        public bool Active { get; set; } = false;
        public string NetworkInterface { get; set; }
        public string VirtualIpAddress { get; set; }
        public PortMapping[] PortMapping { get; set; } = new PortMapping[0];
    }

    /// <summary>
    /// Sincronizzazione di servizi
    /// Se il servizio è true allora viene sincronizzato tra i vari nodi del cluster
    /// NB  probabilmente ogni servizio avrà un metodo di sincronizzazione personalizzato
    ///     non tutti i servizi potranno essere sincronizzati al 100%
    /// </summary>
    public class ClusterService {
        public bool Ssh { get; set; } = false;
        public bool Sshd { get; set; } = false;
        public bool Firewall { get; set; } = false;
        public bool Dhcpd { get; set; } = false;
        public bool Bind { get; set; } = false;
        public bool Nginx { get; set; } = false;
        public bool Samba { get; set; } = false;
        public bool SyslogNg { get; set; } = false;
        public bool Tor { get; set; } = false;
        public bool Ca { get; set; } = false;
        public bool Gluster { get; set; } = false;
        public bool Rsync { get; set; } = false;
        public bool Virsh { get; set; } = false;
    }

    public class PortMapping {
        public string ServiceName { get; set; }
        public string ServicePort { get; set; }
        public string VirtualPort { get; set; }
    }

    public class ClusterFs {
        public bool Active { get; set; } = false;

        public ClusterFsDirectory[] SyncDirectories { get; set; } = new ClusterFsDirectory[0];

        public ClusterFsFile[] SyncFiles { get; set; } = new ClusterFsFile[0];

        /// <summary>
        /// Lista delle etichette dei volumi configurati
        /// per ognuno di questi valori vado a prendere le informazioni dei volumi configurati in Cluster.Nodes.Volumes
        /// in modo da avere: nome del volume, percorso del brick in ogni nodo e infine il mountpoint
        /// </summary>
        public string[] VolumesLabels { get; set; } = new string[0];
    }

    public class ClusterFsDirectory {
        public string Path { get; set; } = string.Empty;
    }

    public class ClusterFsFile {
        public string Path { get; set; } = string.Empty;
    }

    /// <summary>
    /// Applicazioni di Anthilla gestite da antd
    /// </summary>
    public class Applications {

    }
}
