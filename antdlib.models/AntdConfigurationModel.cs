using System.Collections.Generic;

namespace antdlib.models {
    public class AntdConfigurationModel {
        public string Name { get; set; }
        public string Path { get; set; }
        public IEnumerable<string> LoadModules { get; set; } = new List<string>();
        public IEnumerable<string> LoadServices { get; set; } = new List<string>();
        public Dictionary<string, string> LoadOsParameters { get; set; } = new Dictionary<string, string>();
    }
}
