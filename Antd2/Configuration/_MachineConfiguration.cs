using System;

namespace Antd2.Configuration {

    public class MachineConfiguration {

        /// <summary>
        /// Configurazione da applicare in fase di avvio di antd
        ///     - parametri di sistema (sysctl)
        ///     - moduli
        ///     - servizi (oltre quelli configurabili)
        /// </summary>
        public BootParameters Boot { get; set; } = new BootParameters();

        /// <summary>
        /// Utenti di sistema
        /// </summary>
        public SystemUser[] Users { get; set; } = new SystemUser[0];

        public TimeDateParameters Time { get; set; } = new TimeDateParameters();
        public HostParameters Host { get; set; } = new HostParameters();
        public NetworkParameters Network { get; set; } = new NetworkParameters();


        /// <summary>
        /// Parametri applicativi
        /// </summary>
        public AppParameters App { get; set; } = new AppParameters();


        /// <summary>
        /// Comandi "custom" da lanciare all'avvio
        /// La configurazione di default la carico una volta all'avvio di antd
        /// </summary>
        public SetupCommandParameters Commands { get; set; } = new SetupCommandParameters();

        //public ScheduledCommand[] ScheduledCommands { get; set; } = Array.Empty<ScheduledCommand>();

    }
}
