using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace antdlib.common {
    public static class GrepExtension {

        private static readonly IEnumerable<string> Empty = new List<string>();

        public static IEnumerable<string> Grep(this string input, string pattern, bool caseInsensitive = false) {
            if(string.IsNullOrEmpty(pattern)) {
                return Empty;
            }
            if(string.IsNullOrEmpty(input)) {
                return Empty;
            }
            try {
                var list = new List<string>();
                if(caseInsensitive) {
                    if(Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase)) {
                        list.Add(input);
                    }
                }
                else {
                    if(Regex.IsMatch(input, pattern)) {
                        list.Add(input);
                    }
                }
                return list;
            }
            catch(Exception) {
                return Empty;
            }
        }

        public static IEnumerable<string> Grep(this IEnumerable<string> inputLines, string pattern, bool caseInsensitive = false) {
            if(string.IsNullOrEmpty(pattern)) {
                return Empty;
            }
            var inputList = inputLines as IList<string> ?? inputLines.ToList();
            if(!inputList.Any()) {
                return Empty;
            }
            try {
                var list = new List<string>();
                foreach(var input in inputList) {
                    if(caseInsensitive) {
                        if(Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase)) {
                            list.Add(input);
                        }
                    }
                    else {
                        if(Regex.IsMatch(input, pattern)) {
                            list.Add(input);
                        }
                    }
                }
                return list;
            }
            catch(Exception) {
                return Empty;
            }
        }

        public static IEnumerable<string> GrepIgnore(this IEnumerable<string> inputLines, string pattern, bool caseInsensitive = false) {
            if(string.IsNullOrEmpty(pattern)) {
                return Empty;
            }
            var inputList = inputLines as IList<string> ?? inputLines.ToList();
            if(!inputList.Any()) {
                return Empty;
            }
            try {
                var list = new List<string>();
                foreach(var input in inputList) {
                    if(caseInsensitive) {
                        if(!Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase)) {
                            list.Add(input);
                        }
                    }
                    else {
                        if(!Regex.IsMatch(input, pattern)) {
                            list.Add(input);
                        }
                    }
                }
                return list;
            }
            catch(Exception) {
                return Empty;
            }
        }
    }
}
