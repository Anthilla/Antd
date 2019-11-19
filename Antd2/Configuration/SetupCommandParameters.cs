﻿using System;

namespace Antd2.Configuration {
    /// <summary>
    /// Comando "custom" da lanciare all'avvio
    /// </summary>
    public class SetupCommandParameters {
        public string[] Run { get; set; } = Array.Empty<string>();
    }

    public class ScheduledCommand {
        public string Name { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public int Time { get; set; } = 0;
    }
}
