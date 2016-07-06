using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using Newtonsoft.Json;

namespace Antd.Storage {
    public class Disks {
        public static List<Disk> List() {
            var str = new Terminal().Execute("lsblk -JO");
            var clean = str?.Replace("-", "_").Replace("maj:min", "maj_min");
            var ret = JsonConvert.DeserializeObject<Json.Lsblk>(clean);
            var result = ret.blockdevices.Select(_ => new Disk(_)).ToList();
            return result;
        }
    }

    public class Disk {
        public Disk(Json.Blockdevice source) {
            Guid = System.Guid.NewGuid().ToString();
            Name = source.name;
            Kname = source.kname;
            MajMin = source.maj_min;
            Fstype = source.fstype;
            Mountpoint = source.mountpoint;
            Label = source.label;
            Uuid = source.uuid;
            Parttype = source.parttype;
            Partlabel = source.partlabel;
            Partuuid = source.partuuid;
            Partflags = source.partflags;
            Ra = source.ra;
            Ro = source.ro;
            Rm = source.rm;
            Hotplug = source.hotplug;
            Model = source.model;
            Serial = source.serial;
            Size = source.size;
            State = source.state;
            Owner = source.owner;
            Group = source.group;
            Mode = source.mode;
            Alignment = source.alignment;
            MinIo = source.min_io;
            OptIo = source.opt_io;
            PhySec = source.phy_sec;
            LogSec = source.log_sec;
            Rota = source.rota;
            Sched = source.sched;
            RqSize = source.rq_size;
            Type = source.type;
            DiscAln = source.disc_aln;
            DiscGran = source.disc_gran;
            DiscMax = source.disc_max;
            DiscZero = source.disc_zero;
            Wsame = source.wsame;
            Wwn = source.wwn;
            Rand = source.rand;
            Pkname = source.pkname;
            Hctl = source.hctl;
            Tran = source.tran;
            Subsystems = source.subsystems;
            Rev = source.rev;
            Vendor = source.vendor;

            var list = new List<Partition>();
            if (source.children != null) {
                foreach (var c in source.children) {
                    var p = new Partition(c);
                    list.Add(p);
                }
            }
            Partitions = list;

            if (Partitions.Any()) {
                HasPartition = true;
            }
            Partprobe = new Terminal().Execute($"partprobe -s /dev/{Name}");
            var partitionTable = new Terminal().Execute($"parted /dev/{Name} print 2> /dev/null | grep \"Partition Table:\"");
            if (!string.IsNullOrEmpty(partitionTable)) {
                PartitionTable = partitionTable.Replace("Partition Table:", "").Trim();
                HasPartitionTable = partitionTable != "unknown";
            }
        }

        public string Guid { get; set; }
        public string Name { get; set; }
        public string Kname { get; set; }
        public string MajMin { get; set; }
        public string Fstype { get; set; }
        public string Mountpoint { get; set; }
        public object Label { get; set; }
        public object Uuid { get; set; }
        public object Parttype { get; set; }
        public object Partlabel { get; set; }
        public object Partuuid { get; set; }
        public object Partflags { get; set; }
        public string Ra { get; set; }
        public string Ro { get; set; }
        public string Rm { get; set; }
        public string Hotplug { get; set; }
        public string Model { get; set; }
        public string Serial { get; set; }
        public string Size { get; set; }
        public string State { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        public string Mode { get; set; }
        public string Alignment { get; set; }
        public string MinIo { get; set; }
        public string OptIo { get; set; }
        public string PhySec { get; set; }
        public string LogSec { get; set; }
        public string Rota { get; set; }
        public object Sched { get; set; }
        public string RqSize { get; set; }
        public string Type { get; set; }
        public string DiscAln { get; set; }
        public string DiscGran { get; set; }
        public string DiscMax { get; set; }
        public string DiscZero { get; set; }
        public string Wsame { get; set; }
        public object Wwn { get; set; }
        public string Rand { get; set; }
        public object Pkname { get; set; }
        public string Hctl { get; set; }
        public string Tran { get; set; }
        public string Subsystems { get; set; }
        public string Rev { get; set; }
        public string Vendor { get; set; }
        public List<Partition> Partitions { get; set; } = new List<Partition>();

        public bool HasPartition { get; set; }
        public string Partprobe { get; set; } = "";
        public string PartitionTable { get; set; } = "";
        public bool HasPartitionTable { get; set; }
    }

    public class Partition {

        public Partition(Json.Partition source) {
            Guid = System.Guid.NewGuid().ToString();
            Name = source.name;
            Kname = source.kname;
            MajMin = source.maj_min;
            Fstype = source.fstype;
            Mountpoint = source.mountpoint;
            Label = source.label;
            Uuid = source.uuid;
            Parttype = source.parttype;
            Partlabel = source.partlabel;
            Partuuid = source.partuuid;
            Partflags = source.partflags;
            Ra = source.ra;
            Ro = source.ro;
            Rm = source.rm;
            Hotplug = source.hotplug;
            Model = source.model;
            Serial = source.serial;
            Size = source.size;
            State = source.state;
            Owner = source.owner;
            Group = source.group;
            Mode = source.mode;
            Alignment = source.alignment;
            MinIo = source.min_io;
            OptIo = source.opt_io;
            PhySec = source.phy_sec;
            LogSec = source.log_sec;
            Rota = source.rota;
            Sched = source.sched;
            RqSize = source.rq_size;
            Type = source.type;
            DiscAln = source.disc_aln;
            DiscGran = source.disc_gran;
            DiscMax = source.disc_max;
            DiscZero = source.disc_zero;
            Wsame = source.wsame;
            Wwn = source.wwn;
            Rand = source.rand;
            Pkname = source.pkname;
            Hctl = source.hctl;
            Tran = source.tran;
            Subsystems = source.subsystems;
            Rev = source.rev;
            Vendor = source.vendor;
        }

        public string Guid { get; set; }
        public string Name { get; set; }
        public string Kname { get; set; }
        public string MajMin { get; set; }
        public string Fstype { get; set; }
        public string Mountpoint { get; set; }
        public string Label { get; set; }
        public string Uuid { get; set; }
        public string Parttype { get; set; }
        public string Partlabel { get; set; }
        public string Partuuid { get; set; }
        public object Partflags { get; set; }
        public string Ra { get; set; }
        public string Ro { get; set; }
        public string Rm { get; set; }
        public string Hotplug { get; set; }
        public object Model { get; set; }
        public object Serial { get; set; }
        public string Size { get; set; }
        public object State { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        public string Mode { get; set; }
        public string Alignment { get; set; }
        public string MinIo { get; set; }
        public string OptIo { get; set; }
        public string PhySec { get; set; }
        public string LogSec { get; set; }
        public string Rota { get; set; }
        public object Sched { get; set; }
        public string RqSize { get; set; }
        public string Type { get; set; }
        public string DiscAln { get; set; }
        public string DiscGran { get; set; }
        public string DiscMax { get; set; }
        public string DiscZero { get; set; }
        public string Wsame { get; set; }
        public object Wwn { get; set; }
        public string Rand { get; set; }
        public string Pkname { get; set; }
        public object Hctl { get; set; }
        public object Tran { get; set; }
        public string Subsystems { get; set; }
        public object Rev { get; set; }
        public object Vendor { get; set; }
    }

    public class Json {
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
