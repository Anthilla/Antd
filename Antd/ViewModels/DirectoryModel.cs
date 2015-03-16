using System.Collections.Generic;

namespace Antd.ViewModels {
    public class DirectoryModel {
        public IEnumerable<string> parents { get; set; }
        public IEnumerable<string> children { get; set; }
    }
}
