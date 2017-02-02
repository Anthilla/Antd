using System.Collections.Generic;

namespace antdlib.models {
    public class AppInfo {
        public string Name { get; set; }
        public string Repository { get; set; }
        public List<KeyValuePair<string, string>> Values { get; set; } = new List<KeyValuePair<string, string>>();
        public bool IsSetup { get; set; }
    }
}
