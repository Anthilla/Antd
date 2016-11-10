using System;
using System.Collections.Generic;
using System.Linq;

namespace antdlib.common.Tool {

    public static class ReplaceExtensions {

        private static readonly string Empty = string.Empty;
        private static readonly IEnumerable<string> EmptyList = new List<string>();

        public static string ReplaceX(this string input, string strToReplace, string newStr) {
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

        public static IEnumerable<string> ReplaceX(this IEnumerable<string> input, string strToReplace, string newStr) {
            try {
                var output = input.Select(_ => _.Replace(strToReplace, newStr));
                return output;
            }
            catch(Exception) {
                return EmptyList;
            }
        }

        public static T ReplaceX<T>(this T input, string strToReplace, string newStr) {
            try {
                if(typeof(T) == typeof(string)) {
                    var input2 = input.ToString();
                    if(string.IsNullOrEmpty(input2)) {
                        return default(T);
                    }
                    var output = input.ReplaceX(strToReplace, newStr);
                    return output;
                }
                if(typeof(T) == typeof(IEnumerable<string>)) {
                    var input2 = (IEnumerable<string>)input;
                    if(!input2.Any()) {
                        return default(T);
                    }
                    var output = input.ReplaceX(strToReplace, newStr);
                    return output;
                }
                return default(T);
            }
            catch(Exception) {
                return default(T);
            }
        }
    }
}
