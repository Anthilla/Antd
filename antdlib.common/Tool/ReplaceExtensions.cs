using System;
using System.Collections.Generic;

namespace antdlib.common.Tool {

    public static class ReplaceExtensions {

        private static readonly string Empty = string.Empty;
        private static readonly IEnumerable<string> EmptyList = new List<string>();

        public static string ReplaceInString(this string input, string strToReplace, string newStr) {
            try {
                if(string.IsNullOrEmpty(input)) {
                    return Empty;
                }
                var output = input.Replace(strToReplace, newStr);
                return output;
            }
            catch(Exception) {
                return Empty;
            }
        }

        public static IEnumerable<string> ReplaceInList(this IEnumerable<string> input, string strToReplace, string newStr) {
            try {
                var output = new List<string>();
                foreach(var l in input) {
                    var r = l.Replace(strToReplace, newStr);
                    output.Add(r);
                }
                return output;
            }
            catch(Exception) {
                return EmptyList;
            }
        }
    }
}
