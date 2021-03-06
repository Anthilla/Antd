﻿using Nett;

namespace Antd2.Configuration {
    /// <summary>
    /// Raccoglie i parametri che gestiscono la configurazione temporale della macchina
    /// </summary>
    public class TimeDateParameters {
        public string Timezone { get; set; } = "Europe/Rome";

        /// <summary>
        /// Switch per attivare/disattivare la sincronizzazione oraria da un server remoto
        ///     RemoteNtpServer deve essere configurato
        ///     -1 = ignora
        /// </summary>
        public bool EnableNtpSync { get; set; } = false;

        /// <summary>
        /// Server remoto da cui ottenere l'orario
        ///     SyncFromRemoteServer deve essere active
        /// </summary>
        public string[] NtpServer { get; set; } = System.Array.Empty<string>();

        [TomlIgnoreAttribute]
        public string NtpServerTxt { get; set; }
        [TomlIgnoreAttribute]
        public string RunningDate { get; set; }
        [TomlIgnoreAttribute]
        public string RunningTimezone { get; set; }
    }
}
