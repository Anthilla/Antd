using System;
using System.Collections.Generic;

namespace antdlib.models {
    public class RemoteCommand {
        public DateTime Date { get; set; }
        public string CommandCode { get; set; }
        public string Command { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        public bool Executed { get; set; } = false;
    }
}