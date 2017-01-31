using System.Collections.Generic;

namespace Antd.Storage {
    public class LsblkJsonModel {
        public class Lsblk {
            public List<Blockdevice> blockdevices { get; set; }
        }

        public class Blockdevice {
            public string name { get; set; }
            public string kname { get; set; }
            public string maj_min { get; set; }
            public string fstype { get; set; }
            public string mountpoint { get; set; }
            public object label { get; set; }
            public object uuid { get; set; }
            public object parttype { get; set; }
            public object partlabel { get; set; }
            public object partuuid { get; set; }
            public object partflags { get; set; }
            public string ra { get; set; }
            public string ro { get; set; }
            public string rm { get; set; }
            public string hotplug { get; set; }
            public string model { get; set; }
            public string serial { get; set; }
            public string size { get; set; }
            public string state { get; set; }
            public string owner { get; set; }
            public string group { get; set; }
            public string mode { get; set; }
            public string alignment { get; set; }
            public string min_io { get; set; }
            public string opt_io { get; set; }
            public string phy_sec { get; set; }
            public string log_sec { get; set; }
            public string rota { get; set; }
            public object sched { get; set; }
            public string rq_size { get; set; }
            public string type { get; set; }
            public string disc_aln { get; set; }
            public string disc_gran { get; set; }
            public string disc_max { get; set; }
            public string disc_zero { get; set; }
            public string wsame { get; set; }
            public object wwn { get; set; }
            public string rand { get; set; }
            public object pkname { get; set; }
            public string hctl { get; set; }
            public string tran { get; set; }
            public string subsystems { get; set; }
            public string rev { get; set; }
            public string vendor { get; set; }
            public List<Partition> children { get; set; }
        }

        public class Partition {
            public string name { get; set; }
            public string kname { get; set; }
            public string maj_min { get; set; }
            public string fstype { get; set; }
            public string mountpoint { get; set; }
            public string label { get; set; }
            public string uuid { get; set; }
            public string parttype { get; set; }
            public string partlabel { get; set; }
            public string partuuid { get; set; }
            public object partflags { get; set; }
            public string ra { get; set; }
            public string ro { get; set; }
            public string rm { get; set; }
            public string hotplug { get; set; }
            public object model { get; set; }
            public object serial { get; set; }
            public string size { get; set; }
            public object state { get; set; }
            public string owner { get; set; }
            public string group { get; set; }
            public string mode { get; set; }
            public string alignment { get; set; }
            public string min_io { get; set; }
            public string opt_io { get; set; }
            public string phy_sec { get; set; }
            public string log_sec { get; set; }
            public string rota { get; set; }
            public object sched { get; set; }
            public string rq_size { get; set; }
            public string type { get; set; }
            public string disc_aln { get; set; }
            public string disc_gran { get; set; }
            public string disc_max { get; set; }
            public string disc_zero { get; set; }
            public string wsame { get; set; }
            public object wwn { get; set; }
            public string rand { get; set; }
            public string pkname { get; set; }
            public object hctl { get; set; }
            public object tran { get; set; }
            public string subsystems { get; set; }
            public object rev { get; set; }
            public object vendor { get; set; }
        }
    }
}