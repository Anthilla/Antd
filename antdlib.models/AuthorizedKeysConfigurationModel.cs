using System.Collections.Generic;

namespace antdlib.models {
    public class AuthorizedKeysConfigurationModel {
        public bool IsActive { get; set; }

        public List<AuthorizedKeyModel> Keys { get; set; } = new List<AuthorizedKeyModel>();
    }

    public class AuthorizedKeyModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string RemoteUser { get; set; }
        public string User { get; set; }
        public string KeyValue { get; set; }
    }
}