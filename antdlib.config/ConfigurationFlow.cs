using antdlib.models;
using System.Collections.Generic;

namespace antdlib.config {

    /// <summary>
    /// Questa classe (popolata eccetera) sarà salvata come json nel file (Path)
    /// </summary>
    public class ConfigurationFlow {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<Control> Controls { get; set; }
    }
}
