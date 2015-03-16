using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd {
    public class Timestamp {
        private static String Get(string format) {
            return DateTime.Now.ToString(format);
        }

        public static String Now { get { return Get("yyyyMMddHHmmssfff"); } }

        //public static String ConsoleFormat { get { return Get("yyyyMMddHHmmssfff"); } }
    }
}
