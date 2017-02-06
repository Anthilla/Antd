using System.Collections.Generic;

namespace antdlib.models {
    public class TimerConfigurationModel {
        public bool IsActive { get; set; }

        public List<TimerModel> Timers { get; set; } = new List<TimerModel>();
    }

    public class TimerModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Alias { get; set; }
        public string Time { get; set; }
        public string Command { get; set; }
        public string TimerStatus { get; set; }
        public bool IsEnabled { get; set; }
    }
}
