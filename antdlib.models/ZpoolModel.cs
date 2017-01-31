namespace Antd.Storage {
    public class ZpoolModel {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Alloc { get; set; }
        public string Free { get; set; }
        public string Expandsz { get; set; }
        public string Frag { get; set; }
        public string Cap { get; set; }
        public string Dedup { get; set; }
        public string Health { get; set; }
        public string Altroot { get; set; }
        public string Status { get; set; }

        public bool HasSnapshot { get; set; }
        public string SnapshotGuid { get; set; } = "";
        public string Snapshot { get; set; } = "";
    }
}
