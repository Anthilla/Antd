using System.Collections.Generic;

namespace antdlib.models {
    public class PageDhcpdModel {
        public bool DhcpdIsActive { get; set; }
        public DhcpdConfigurationModel DhcpdOptions { get; set; }
        public IEnumerable<DhcpConfigurationClassModel> DhcpdClass { get; set; }
        public IEnumerable<DhcpConfigurationPoolModel> DhcpdPools { get; set; }
        public IEnumerable<DhcpConfigurationReservationModel> DhcpdReservation { get; set; }
    }
}
