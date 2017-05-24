using System.Collections.Generic;

namespace antdlib.models {
    public class PageDhcpdModel {
        public bool DhcpdIsActive { get; set; }
        public DhcpdConfigurationModel DhcpdOptions { get; set; }
        public IEnumerable<DhcpdClass> DhcpdClass { get; set; }
        public IEnumerable<DhcpdPool> DhcpdPools { get; set; }
        public IEnumerable<DhcpdReservation> DhcpdReservation { get; set; }
    }
}
