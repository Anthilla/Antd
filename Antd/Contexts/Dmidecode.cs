using System.Collections.Generic;
using System.Linq;

namespace Antd {

    public class Dmidecode {

        public static string GetUUID(List<string> inputTable) {
            string row = (from i in inputTable
                          where i.Contains("UUID:")
                          select i).FirstOrDefault();
            var array = row.Split(new[] { ' ' }, 2);
            string uuid = array[1];
            return uuid;
        }
    }
}