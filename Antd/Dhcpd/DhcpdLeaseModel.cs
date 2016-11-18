using System;

namespace Antd.Dhcpd {
    public class DhcpdLeaseModel {
        public string Reference { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsExpired => DateTime.Now > EndTime;
        public string StartTimeStr => StartTime.ToString("yyyy/MM/dd HH:mm:ss");
        public string EndTimeStr => EndTime.ToString("yyyy/MM/dd HH:mm:ss");
        public string Hardware { get; set; }
        public string ClientHostname { get; set; }
    }
}
