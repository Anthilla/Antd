namespace Antd.Vpn {
    public class VpnConfigurationModel {
        public bool IsActive { get; set; }
        public string RemoteHost { get; set; } = "";
        public VpnPointModel LocalPoint { get; set; } = new VpnPointModel { Address = "172.21.1.1", Range = "24" };
        public VpnPointModel RemotePoint { get; set; } = new VpnPointModel { Address = "172.21.1.2", Range = "24" };
    }

    public class VpnPointModel {
        public string Address { get; set; }
        public string Range { get; set; }
    }
}