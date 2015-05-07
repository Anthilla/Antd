using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.ViewHelpers {
    public class Status {
        public static void Sysctl(List<SysctlModel> stockData, List<SysctlModel> runningData, List<SysctlModel> antdData) {
            HashSet<string> paramNames = new HashSet<string>() { };
            HashSet<string> valueNames = new HashSet<string>() { };

            foreach (SysctlModel data in stockData) {
                paramNames.Add(data.param);
            }
            foreach (SysctlModel data in runningData) {
                paramNames.Add(data.param);
            }
            foreach (SysctlModel data in antdData) {
                paramNames.Add(data.param);
            }
        }
    }
}
