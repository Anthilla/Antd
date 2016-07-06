using System.Collections.Generic;

namespace Antd.Storage {
    public class Partition {
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

    public class Blockdevice {
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
        public List<Partition> Children { get; set; }
    }

    public class Lsblk {
        public List<Blockdevice> Blockdevices { get; set; }
    }
}
