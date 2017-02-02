using System.Collections.Generic;

namespace antdlib.models {
    public class PageAclModel {
        public bool AclIsActive { get; set; }
        public IEnumerable<AclPersistentSettingModel> Acl { get; set; }
    }
}
