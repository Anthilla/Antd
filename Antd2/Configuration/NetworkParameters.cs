﻿using Nett;
using System;

namespace Antd2.Configuration {
    /// <summary>
    /// Configurazione di rete
    /// </summary>
    public class NetworkParameters {

        /// <summary>
        /// Configurazione di /etc/resolv.conf
        /// </summary>
        public DnsClientConfiguration Dns { get; set; } = new DnsClientConfiguration();

        public NetTun[] Tuns { get; set; } = new NetTun[0];

        public NetTap[] Taps { get; set; } = new NetTap[0];

        public NetBridge[] Bridges { get; set; } = new NetBridge[0];

        public NetBond[] Bonds { get; set; } = new NetBond[0];


        /// <summary>
        /// Configurazione delle tabelle di routing
        /// </summary>
        public NetRoutingTable[] RoutingTables { get; set; } = new NetRoutingTable[0];

        /// <summary>
        /// Configurazione delle interfacce di rete
        ///     i valori di default verranno popolati all'avvio dopo aver importato i dati esistenti
        /// </summary>
        public NetInterface[] Interfaces { get; set; } = new NetInterface[0];

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
    }

    public abstract class NetInterfaceParameter {

        public string[] PreUp { get; set; } = Array.Empty<string>();
        public string[] PostUp { get; set; } = Array.Empty<string>();
        public string[] PreDown { get; set; } = Array.Empty<string>();
        public string[] PostDown { get; set; } = Array.Empty<string>();


        [TomlIgnoreAttribute]
        public string PreUpTxt { get; set; } = string.Empty;
        [TomlIgnoreAttribute]
        public string PostUpTxt { get; set; } = string.Empty;
        [TomlIgnoreAttribute]
        public string PreDownTxt { get; set; } = string.Empty;
        [TomlIgnoreAttribute]
        public string PostDownTxt { get; set; } = string.Empty;
    }

    public class NetTun : NetInterfaceParameter {
        /// <summary>
        /// Nome dell'interfaccia virtuale
        /// </summary>
        public string Name { get; set; }
    }

    public class NetTap : NetInterfaceParameter {
        /// <summary>
        /// Nome dell'interfaccia virtuale
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Configurazione di interfacce Bridge
    /// </summary>
    public class NetBridge : NetInterfaceParameter {
        /// <summary>
        /// Nome del bridge
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Interfacce collegate
        /// </summary>
        public string[] Lower { get; set; } = new string[0];

        [TomlIgnoreAttribute]
        public string LowerTxt { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configurazione di interfacce Bond
    /// </summary>
    public class NetBond : NetInterfaceParameter {
        /// <summary>
        /// Nome del bond
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Interfacce collegate
        /// </summary>
        public string[] Lower { get; set; } = new string[0];

        [TomlIgnoreAttribute]
        public string LowerTxt { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configurazione dell'interfaccia di rete
    /// </summary>
    public class NetInterface : NetInterfaceParameter {

        /// <summary>
        /// Indica se l'interfaccia deve essere attivata all'avvio
        ///     up
        ///     down
        /// </summary>
        public string Auto { get; set; } = "up";

        /// <summary>
        /// Nome dell'interfaccia di rete
        /// </summary>
        public string Iface { get; set; } = string.Empty;

        /// <summary>
        /// Se c'è "/" splitta il valore e recupera il range da qui
        /// </summary>
        public string Address { get; set; } = string.Empty;

        public bool AutoBool { get; set; }

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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Regole da applicare
        ///     ip rule add xyz
        /// </summary>
        public string[] Rules { get; set; } = Array.Empty<string>();
        public string RulesTxt { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configurazione delle rotte
    /// </summary>
    public class NetRoute {
        /// <summary>
        /// Destinazione della rotta, se Default = = true allora Destination = "default"
        /// </summary>
        public string Destination { get; set; } = string.Empty;
        public string Gateway { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
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
}
