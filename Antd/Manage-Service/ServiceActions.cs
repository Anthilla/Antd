using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.ServiceManagement {
    public class Service {
        public static string GetPID(string service) {
            List<ProcModel> procs = Proc.All;
            var proc = (from p in procs
                        where p.CMD.Contains(service)
                        select p).FirstOrDefault();
            if (proc != null) {
                return proc.PID;
            }
            else {
                return null;
            }
        }
    }
}
