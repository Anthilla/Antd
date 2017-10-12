using System.Collections.Generic;

namespace Antd {
    public class Dicts {

        public static Dictionary<string, string[]> DirsAndServices = new Dictionary<string, string[]> {
            {"DIR_etc_bind", new []{ "named.service" } },
            {"DIR_etc_dhcp", new []{ "dhcpd4.service" } },
            {"DIR_etc_nginx", new []{ "nginx.service" } },
            {"DIR_etc_samba", new []{ "smbd.service", "smbd.socket" } },
            {"DIR_etc_squid", new []{ "squid.service" } },
            {"DIR_etc_squidGuard", new []{ "squid.service" } },
            {"DIR_etc_ssh", new []{ "sshd.service" } },
            {"DIR_etc_systemd_network", new []{ "systemd-networkd.service" } },
            {"DIR_var_cache_bind", new []{ "named.service" } },
            {"DIR_var_log_bind", new []{ "named.service" } },
            {"DIR_var_log_journal", new []{ "systemd-journald.service" } },
            {"DIR_var_log_nginx", new []{ "nginx.service" } },
            {"DIR_var_log_squidGuard", new []{ "squid.service" } },
            {"FILE_etc_systemd_journald.conf", new []{ "systemd-journald.service" } }
        };
    }
}
