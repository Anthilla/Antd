using System.Collections.Generic;

namespace antdlib.models {
    public class AclConfigurationModel {
        public bool IsActive { get; set; }
        public List<AclPersistentSettingModel> Settings { get; set; } = new List<AclPersistentSettingModel>();
    }

    public class AclPersistentSettingModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Path { get; set; }
        public string Acl { get; set; }
        public string AclText { get; set; } = "";
    }
}