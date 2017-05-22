using System.Collections.Generic;

namespace antdlib.config.Antd_SVCS.Bind {
    public class BindModel2 {

        public class Acl {
            public string Name { get; set; }
            public List<string> InterfaceList { get; set; } = new List<string>();
        }

        public List<Acl> AclList { get; set; } = new List<Acl>();

        public class Control {
            public string NetworkName { get; set; }
            public string NetworkAddress { get; set; }
            public string Port { get; set; }
            public List<string> InterfaceList { get; set; } = new List<string>();
            public string Key { get; set; }
        }

        public List<Control> ControlList { get; set; } = new List<Control>();

        public class Include {
            public string FilePath { get; set; }
        }

        public List<Include> IncludeList { get; set; } = new List<Include>();

        public class Zone {
            public string ArpaName { get; set; }
            public string Type { get; set; }
            public string FilePath { get; set; }
            public string SerialUpdateMethod { get; set; }
            public List<string> AllowUpdate { get; set; } = new List<string>();
            public List<string> AllowQuery { get; set; } = new List<string>();
            public List<string> AllowTransfer { get; set; } = new List<string>();
            public List<string> InterfaceList { get; set; } = new List<string>();
            public string Key { get; set; }
        }

        public List<Zone> ZoneList { get; set; } = new List<Zone>();
    }
}
