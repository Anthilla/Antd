using System.Collections.Generic;
using antdlib.views;

namespace antdlib.models {
    public class PageSchedulerModel {
        public IEnumerable<TimerSchema> Jobs { get; set; }
    }
}
