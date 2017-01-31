using System.Collections.Generic;
using System.Linq;
using antdlib.common.Tool;

namespace Antd.Storage {
    public class DiskModel {
        public DiskModel(LsblkJsonModel.Blockdevice source) {
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

            var list = new List<PartitionModel>();
            if(source.children != null) {
                foreach(var c in source.children) {
                    var p = new PartitionModel(c);
                    list.Add(p);
                }
            }
            Partitions = list;

            if(Partitions.Any()) {
                HasPartition = true;
            }
            var bash = new Bash();
            Partprobe = bash.Execute($"partprobe -s /dev/{Name}");
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
        public List<PartitionModel> Partitions { get; set; } = new List<PartitionModel>();

        public bool HasPartition { get; set; }
        public string Partprobe { get; set; } = "";
        public string PartitionTable { get; set; } = "";
        public bool HasPartitionTable { get; set; }
    }
}