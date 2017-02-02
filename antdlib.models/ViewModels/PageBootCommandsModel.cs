using System.Collections.Generic;

namespace antdlib.models {
    public class PageBootCommandsModel {
        public bool HasConfiguration { get; set; }
        public IEnumerable<Control> Controls { get; set; }
    }
}
