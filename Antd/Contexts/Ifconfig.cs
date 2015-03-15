using System.Linq;

namespace Antd {

    public class Ifconfig {

        public static string GetEther() {
            string dir = "/sys/devices";
            CommandModel find = Command.Launch("find", "./ -name address", dir);
            if (find.isError()) {
                return find.error;
            } else {
                string row = (from i in find.outputTable
                              where i.Contains("eth")
                              select i).FirstOrDefault();
                CommandModel cat = Command.Launch("cat", row.Replace("\"", ""), dir);
                return cat.outputTable.FirstOrDefault();
            }
        }
    }
}
