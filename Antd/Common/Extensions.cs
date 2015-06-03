using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.Common {
    public static class Extensions {

        public static string SplitAndGetFirstString(this String str, char div) {
            var arr = str.Split(div);
            if(arr.Length > 0){
                return arr[0].ToString();
            }
            else {
                return String.Empty;
            }
        }

        public static string SplitAndGetAllStringsButFirst(this String str, char div) {
            var arr = str.Split(div);
            arr.Skip(1).ToArray();
            if (arr.Length > 1) {
                return string.Join(" ", arr);
            }
            else {
                return String.Empty;
            }
        }
    }
}
