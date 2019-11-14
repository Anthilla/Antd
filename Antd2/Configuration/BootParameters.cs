namespace Antd2.Configuration {
    /// <summary>
    /// Raccoglie alcuni valori che devono essere applicati in fase di avvio di antd
    /// </summary>
    public class BootParameters {
        /// <summary>
        /// Parametri di sistema (sysctl)
        /// </summary>
        public string[] Sysctl { get; set; } = new string[0];

        /// <summary>
        /// Moduli
        /// </summary>
        public string[] ActiveModules { get; set; } = new string[0];
        public string[] InactiveModules { get; set; } = new string[0];

        /// <summary>
        /// Servizi di systemctl
        /// Quelli attivi vengono abilitati e avviati
        /// Quelli inattivi vengono fermati
        /// Quelli disabilitati vengono fermati e disabilitati
        /// Quelli bloccati vengono fermati e disabilitati e mascherati
        /// </summary>
        public string[] ActiveServices { get; set; } = new string[0];
        public string[] InactiveServices { get; set; } = new string[0];
        public string[] DisabledServices { get; set; } = new string[0];
        public string[] BlockedServices { get; set; } = new string[0];
    }
}
