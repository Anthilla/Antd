namespace antdlib.models {
    public class PartitionModel {

        public PartitionModel(LsblkJsonModel.Partition source) {
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
}