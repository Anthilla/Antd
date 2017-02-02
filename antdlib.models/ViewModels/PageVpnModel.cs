namespace antdlib.models {
    public class PageVpnModel {
        public bool VpnIsActive { get; set; }
        public VpnPointModel VpnLocalPoint { get; set; }
        public string VpnRemoteHost { get; set; }
        public VpnPointModel VpnRemotePoint { get; set; }
    }
}
