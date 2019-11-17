namespace Antd2.models {
    public class DiskModel {

        public DiskModel() {
        }

        public DiskModel(string name, string majMin, string rm, string size, string ro, string type, string mountpoint) {
            Name = name;
            if (!Name.StartsWith("/dev"))
                Name = "/dev/" + Name;
            MajMin = majMin;
            Rm = rm;
            Size = size;
            Ro = ro;
            Type = type;
            Mountpoint = mountpoint;
        }

        public string Name { get; set; }
        public string MajMin { get; set; }
        public string Rm { get; set; }
        public string Size { get; set; }
        public string Ro { get; set; }
        public string Type { get; set; }
        public string Mountpoint { get; set; }

        public string FsType { get; set; }
        public string Label { get; set; }

    }

    public class PartedDiskModel {
        public PartedDiskModel() {
        }

        public PartedDiskModel(string name, string size, string bus, string partitionTable, string model) {
            Name = name;
            Size = size;
            Bus = bus;
            PartitionTable = partitionTable;
            Model = model;
        }

        public string Name { get; set; }
        public string Size { get; set; }
        public string Bus { get; set; }
        public string PartitionTable { get; set; }
        public string Model { get; set; }


        public PartedPartitionModel[] Partitions { get; set; }
    }


    public class PartedPartitionModel {

        public PartedPartitionModel() {
        }

        public PartedPartitionModel(string name, string start, string end, string fsType) {
            Name = name;
            Start = start;
            End = end;
            FsType = fsType;
        }

        public string Name { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string FsType { get; set; }

        public bool IsVolume { get; set; }
        public string Mountpoint { get; set; }
        public string Label { get; set; }
        public string Size { get; set; }
        public string Used { get; set; }
        public bool WebdavRunning { get; set; }
        public System.Collections.Generic.List<string> SyncableVolumes { get; set; }

    }
}
