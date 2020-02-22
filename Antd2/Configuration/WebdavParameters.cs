namespace Antd2.Configuration {
    public class WebdavParameters {

        public bool Enable { get; set; } = true;
        public string Target { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }

        public System.Collections.Generic.List<string> Users { get; set; } = new System.Collections.Generic.List<string>();

        [Nett.TomlIgnore]
        public System.Collections.Generic.List<SystemUser> MappedUsers { get; set; } = new System.Collections.Generic.List<SystemUser>();
        [Nett.TomlIgnore]
        public bool IsActive { get; set; }
    }
}
