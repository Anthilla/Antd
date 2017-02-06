using System.Collections.Generic;

namespace antdlib.models {
    public class PageSchedulerModel {
        public IEnumerable<TimerModel> Jobs { get; set; }
    }
}
