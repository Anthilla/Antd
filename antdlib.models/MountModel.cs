namespace antdlib.models {

    public enum MountStatus : byte {
        Mounted = 1,
        Unmounted = 2,
        MountedTmp = 3,
        DifferentMount = 4,
        MountedReadOnly = 5,
        MountedReadWrite = 6,
        Error = 99
    }

    public enum MountContext : byte {
        Core = 1,
        External = 2,
        Other = 99
    }

    public enum MountEntity : byte {
        Directory = 1,
        File = 2,
        Other = 99
    }

    public class MountModel {
        public MountEntity Entity { get; set; }
        public MountContext Context { get; set; }
        public MountStatus Status { get; set; }
        public string SystemPath { get; set; }
        public string RepoDirsPath { get; set; }
    }
}
