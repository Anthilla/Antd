namespace antdlib.models {
    public class PageFirewallModel {
        public bool FirewallIsActive { get; set; }
        public FirewallConfigurationModel FirewallOptions { get; set; }
        public FirewallTable FwIp4Filter { get; set; }
        public FirewallTable FwIp4Nat { get; set; }
        public FirewallTable FwIp6Filter { get; set; }
        public FirewallTable FwIp6Nat { get; set; }
    }
}
