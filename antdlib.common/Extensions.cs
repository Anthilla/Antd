using anthilla.core;
using System.Collections.Generic;
using System.Linq;

namespace antdlib.common {
    public static class Extensions {
        public static string JoinToString(this IEnumerable<string> val, string div) {
            return CommonString.Build(val.ToArray(), div);
        }
    }
}
