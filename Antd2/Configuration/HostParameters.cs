﻿using Nett;
using System;

namespace Antd2.Configuration {
    /// <summary>
    /// Raccoglie i parametri per definire l'identità della macchina
    /// </summary>
    public class HostParameters {

        public string Uid { get; set; }
        /// <summary>
        /// Definisce il nome dell'host (anche visto dalle altre macchine)
        /// </summary>
        public string Name { get; set; } = "box01";
        public string Chassis { get; set; } = "server";
        public string Deployment { get; set; } = "developement";
        public string Location { get; set; } = "onEarth";

        [TomlIgnoreAttribute]
        public string RunningName { get; set; }
        [TomlIgnoreAttribute]
        public string RunningChassis { get; set; }
        [TomlIgnoreAttribute]
        public string RunningDeployment { get; set; }
        [TomlIgnoreAttribute]
        public string RunningLocation { get; set; }


        [TomlIgnoreAttribute]
        public string User { get; set; }
        [TomlIgnoreAttribute]
        public string Password { get; set; }
    }
}
