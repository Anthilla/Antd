using System.Collections.Generic;

namespace antdlib.models {
    public class PageUsersModel {
        public string Master { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
