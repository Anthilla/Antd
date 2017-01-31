namespace antdlib.models {
    public class ZfsModel {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Used { get; set; }
        public string Available { get; set; }
        public string Refer { get; set; }
        public string Mountpoint { get; set; }

        public string Pool { get; set; }
        public bool HasSnapshot { get; set; }
        public string SnapshotGuid { get; set; } = "";
        public string Snapshot { get; set; } = "";
    }
}
