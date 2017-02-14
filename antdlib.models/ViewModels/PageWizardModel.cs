using System.Collections.Generic;

namespace antdlib.models {
    public class PageWizardModel {
        public IEnumerable<string> Timezones { get; set; }
        public IEnumerable<string> NetworkInterfaceList { get; set; }
        public string DomainInt { get; set; }
        public string DomainExt { get; set; }
        public string Hosts { get; set; }
        public string Networks { get; set; }
        public string Resolv { get; set; }
        public string Nsswitch { get; set; }
    }
}
