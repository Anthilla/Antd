using System;

namespace Antd2.Configuration {
    /// <summary>
    /// Comando "custom" da lanciare all'avvio
    /// </summary>
    public class SetupCommandParameters {
        public string[] Run { get; set; } = Array.Empty<string>();
        public ScheduledCommand[] Scheduled { get; set; } = Array.Empty<ScheduledCommand>();
    }

    public class ScheduledCommand {
        public string BashCommand { get; set; } = string.Empty;

        public int Time { get; set; } = 0;
    }
}
