using Kvpbase;
using System.Collections.Generic;

namespace antdlib.models {
    public class PageVfsModel {
        public Settings Settings { get; set; }
        public Kvpbase.Topology Topology { get; set; }
        public List<ApiKey> ApiKeys { get; set; }
        public List<ApiKeyPermission> ApiKeyPermissions { get; set; }
        public List<UserMaster> UserMasters { get; set; }
    }
}
