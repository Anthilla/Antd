namespace antdlib.models {
    public class PageMonitorModel {
        public string Hostname { get; set; }
        public UptimeModel Uptime { get; set; }
        public int MemoryUsage { get; set; }
        public int DiskUsage { get; set; }
    }
}
