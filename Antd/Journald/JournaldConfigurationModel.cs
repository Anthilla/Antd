namespace Antd.Journald {
    public class JournaldConfigurationModel {
        public bool IsActive { get; set; } = true;
        public string Storage { get; set; } = "persistent";
        public string Compress { get; set; } = " yes";
        public string Seal { get; set; } = "yes";
        public string SplitMode { get; set; } = "uid";
        public string SyncIntervalSec { get; set; } = "5m";
        public string RateLimitInterval { get; set; } = "30s";
        public string RateLimitBurst { get; set; } = "1000";
        public string SystemMaxUse { get; set; } = "#";
        public string SystemKeepFree { get; set; } = "#";
        public string SystemMaxFileSize { get; set; } = "#";
        public string RuntimeMaxUse { get; set; } = "#";
        public string RuntimeKeepFree { get; set; } = "#";
        public string RuntimeMaxFileSize { get; set; } = "#";
        public string MaxRetentionSec { get; set; } = "#";
        public string MaxFileSec { get; set; } = "1month";
        public string ForwardToSyslog { get; set; } = "no";
        public string ForwardToKMsg { get; set; } = "no";
        public string ForwardToConsole { get; set; } = "no";
        public string ForwardToWall { get; set; } = "yes";
        public string TtyPath { get; set; } = "/dev/null";
        public string MaxLevelStore { get; set; } = "debug";
        public string MaxLevelSyslog { get; set; } = "debug";
        public string MaxLevelKMsg { get; set; } = "notice";
        public string MaxLevelConsole { get; set; } = "emerg";
        public string MaxLevelWall { get; set; } = "emerg";
    }
}