using System;

namespace Antd2.Configuration {
    /// <summary>
    /// Raccoglie i parametri per definire l'identità della macchina
    /// </summary>
    public class HostParameters {
        /// <summary>
        /// Definisce il nome dell'host (anche visto dalle altre macchine)
        /// </summary>
        public string Name { get; set; } = "box01";
        public string Chassis { get; set; } = "server";
        public string Deployment { get; set; } = "developement";
        public string Location { get; set; } = "onEarth";
    }
}
