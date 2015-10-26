using System.Collections.Generic;

namespace antdlib.Models {
    public class TokenUserRelation {
        public string username { get; set; }
        public string tokenID { get; set; }
    }

    public class GlobalUserModel {
        public string _Id  { get; set; }
        public string GlobalUID  { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Identities { get; set; } = new HashSet<KeyValuePair<string, string>>() { };
    }
}
