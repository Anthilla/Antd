using System;
using System.Collections.Generic;
using System.Linq;

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

        //public static T ReplacesX<T>(this T input, string strToReplace, string newStr) {
        //    try {
        //        if(typeof(T) == typeof(string)) {
        //            var input2 = input.ToString();
        //            if(string.IsNullOrEmpty(input2)) {
        //                return default(T);
        //            }
        //            var output = input2.ReplaceInString(strToReplace, newStr);
        //            return output;
        //        }
        //        if(typeof(T) == typeof(IEnumerable<string>)) {
        //            var input2 = (IEnumerable<string>)input;
        //            if(!input2.Any()) {
        //                return default(T);
        //            }
        //            var output = input2.ReplaceInList(strToReplace, newStr);
        //            return output;
        //        }
        //        return default(T);
        //    }
        //    catch(Exception) {
        //        return default(T);
        //    }
        //}
    }
}
