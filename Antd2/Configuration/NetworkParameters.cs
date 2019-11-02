using Antd.models;
using anthilla.core;
using System;
using System.Linq;

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
        /// Configurazione delle interfacce di rete
        ///     i valori di default verranno popolati all'avvio dopo aver importato i dati esistenti
        /// </summary>
        public NetInterface[] Interfaces { get; set; } = new NetInterface[0];

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
    }

    public class NetTun {
        /// <summary>
        /// Nome dell'interfaccia virtuale
        /// </summary>
        public string Name { get; set; }
    }

    public class NetTap {
        /// <summary>
        /// Nome dell'interfaccia virtuale
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Configurazione di interfacce Bridge
    /// </summary>
    public class NetBridge {
        /// <summary>
        /// Nome del bridge
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Interfacce collegate
        /// </summary>
        public string[] Lower { get; set; } = new string[0];
    }

    /// <summary>
    /// Configurazione di interfacce Bond
    /// </summary>
    public class NetBond {
        /// <summary>
        /// Nome del bond
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Interfacce collegate
        /// </summary>
        public string[] Lower { get; set; } = new string[0];
    }

    /// <summary>
    /// Configurazione dell'interfaccia di rete
    /// </summary>
    public class NetInterface {

        /// <summary>
        /// Nome dell'interfaccia di rete
        /// </summary>
        public string Name { get; set; } = string.Empty;

        public string Ip { get; set; } = string.Empty;

        /// <summary>
        /// Classe della rete di appartenenza
        /// </summary>
        public string Range { get; set; } = string.Empty;

        /// <summary>
        /// Indirizzo IP primario
        ///     deve appartenere alla rete definita da NetworkClass
        /// </summary>
        public string[] Conf { get; set; } = Array.Empty<string>();
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
