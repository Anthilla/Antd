using System.Collections.Generic;

namespace antdlib.models {
    public class PageTimeModel {
        public string LocalTime { get; set; }
        public string UnivTime { get; set; }
        public string RtcTime { get; set; }
        public string Timezone { get; set; }
        public string Nettimeon { get; set; }
        public string Ntpsync { get; set; }
        public string Rtcintz { get; set; }
        public string NtpServer { get; set; }
        public string Ntpd { get; set; }
        public string NtpdEdit { get; set; }

        public IEnumerable<string> Timezones { get; set; }
    }
}
