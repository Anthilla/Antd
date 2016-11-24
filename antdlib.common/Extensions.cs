//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using antdlib.common.Tool;
using Newtonsoft.Json;

namespace antdlib.common {
    public static class Extensions {

        private static readonly Bash Bash = new Bash();

        public static void DosToUnix(this string file, string otherFile = "") {
            var fileToConvert = otherFile.Length > 0 ? otherFile : file;
            Bash.Execute($"dos2unix {fileToConvert}", false);
        }

        public static bool ToNnBoolean(this string str) {
            if(string.IsNullOrEmpty(str))
                return false;
            return Convert.ToBoolean(str.ToLower());
        }

        public static bool? ToBoolean(this string str) {
            if(string.IsNullOrEmpty(str))
                return null;
            return Convert.ToBoolean(str.ToLower());
        }

        public static Guid ToGuid(this string str) {
            Guid guid;
            Guid.TryParse(str, out guid);
            return guid;
        }

        public static string JoinToString(this IEnumerable<string> stringList, string separator = ",") {
            if(stringList == null) {
                return string.Empty;
            }
            stringList = stringList.ToList();
            return !stringList.Any() ? string.Empty : string.Join(separator, stringList.ToList());
        }

        public static List<string> SplitToList(this string str, string separator = ",", StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries) {
            return str.Split(new[] { separator }, options).ToList();
        }

        public static T ToEnum<T>(this string str) {
            if(str == null)
                return default(T);
            return (T)Enum.Parse(typeof(T), str);
        }

        public static Guid ToGuid(this Guid? source) {
            return source ?? Guid.Empty;
        }

        public static string ToJson<T>(this T obj) {
            if(obj == null)
                return string.Empty;
            return JsonConvert.SerializeObject(obj);
        }

        public static T ToObject<T>(this string json) {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string GetFirstString(this string str) {
            var arr = str.Split(' ');
            return arr.Length > 0 ? arr[0] : string.Empty;
        }

        public static string GetFirstString(this string str, char div) {
            var arr = str.Split(div);
            return arr.Length > 0 ? arr[0] : string.Empty;
        }

        public static string GetAllStringsButFirst(this string str) {
            var arr = str.Split(' ');
            return arr.Length > 1 ? string.Join($"", arr.Skip(1).ToArray()) : string.Empty;
        }

        public static string GetAllStringsButFirst(this string str, char div) {
            var arr = str.Split(div).Skip(1).ToArray();
            return arr.Length > 1 ? string.Join($"", arr) : string.Empty;
        }

        public static string GetAllStringsButLast(this string str, char div) {
            var arr = str.Split(div);
            var arr2 = arr.SubArray(0, arr.Length - 1);
            return arr2.Length > 1 ? string.Join(div.ToString(), arr.ToArray()) : string.Empty;
        }

        public static string UppercaseAllFirstLetters(this string str) {
            var arr = str.Split(' ');
            var newList = new List<string>();
            newList.AddRange(arr.Select(a => a.UppercaseFirstLetter()));
            return string.Join("", newList.ToArray());
        }

        public static string UppercaseAllFirstLetters(this string str, char div) {
            var arr = str.Split(' ');
            var newList = new List<string>();
            newList.AddRange(arr.Select(a => a.UppercaseFirstLetter()));
            return string.Join(div.ToString(), newList.ToArray());
        }

        public static string UppercaseFirstLetter(this string str) {
            if(string.IsNullOrEmpty(str)) {
                return string.Empty;
            }
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string AsJson(this object str) {
            return JsonConvert.SerializeObject(str);
        }

        public static int[] ToIntArray(this string[] arr) {
            return arr.Select(s => Convert.ToInt32(s)).ToArray();
        }

        public static string ToHex(this string value) {
            var chars = value.ToCharArray();
            var stringBuilder = new StringBuilder();
            foreach(var c in chars) {
                stringBuilder.Append(((short)c).ToString(""));
            }
            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes) {
            var value = GetString(bytes);
            var chars = value.ToCharArray();
            var stringBuilder = new StringBuilder();
            foreach(var c in chars) {
                stringBuilder.Append(((short)c).ToString(""));
            }
            return stringBuilder.ToString();
        }

        public static byte[] GetBytes(this String str) {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(this byte[] bytes) {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static IEnumerable<string> SplitAndGetTextBetween(this string input, char start, char end) {
            var r = new Regex(Regex.Escape(start.ToString()) + "(.*?)" + Regex.Escape(end.ToString()));
            var matches = r.Matches(input);
            return from Match match in matches select match.Groups[1].Value;
        }

        public static IEnumerable<string> SplitAndGetTextBetween(this string input, string start, string end) {
            var r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
            var matches = r.Matches(input);
            return from Match match in matches select match.Groups[1].Value;
        }

        public static IEnumerable<string> SplitAndGetTextBetween(this string input, char start, string end) {
            var r = new Regex(Regex.Escape(start.ToString()) + "(.*?)" + Regex.Escape(end));
            var matches = r.Matches(input);
            return from Match match in matches select match.Groups[1].Value;
        }

        public static IEnumerable<string> SplitAndGetTextBetween(this string input, string start, char end) {
            var r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end.ToString()));
            var matches = r.Matches(input);
            return from Match match in matches select match.Groups[1].Value;
        }

        public static string RemoveTextBetween(this string input, char start, char end) {
            var r = new Regex(Regex.Escape(start.ToString()) + "(.*?)" + Regex.Escape(end.ToString()));
            var matches = r.Matches(input);
            var list = (from Match match in matches where match.Groups[1].Value.Length > 0 select match.Groups[1].Value).ToList();
            var output = input;
            if(list.Count <= 0)
                return output;
            var removeThis = list.ToArray()[list.Count - 1];
            var t = input;
            if(removeThis.Length > 0) {
                t = input.Replace(removeThis, "");
            }
            output = t.RemoveTextBetween(start, end);
            return output;
        }

        public static string ReplaceAllTextBetweenWith(this string input, char start, char end, string replacement) {
            var memReplace = input;
            var regex = new Regex(Regex.Escape(start.ToString()) + "(.*?)" + Regex.Escape(end.ToString()));
            var matches = regex.Matches(input);
            if(matches.Count <= 0)
                return memReplace;
            for(var i = 0; i < matches.Count; i++) {
                memReplace = memReplace.Replace(matches[i].Value, replacement);
            }
            return memReplace;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> input) {
            var list = new HashSet<T>();
            foreach(var i in input) {
                list.Add(i);
            }
            return list;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length) {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static byte[] ToKey(this Guid guid) {
            return Encryption.Hash(guid.ToString());
        }

        public static byte[] ToVector(this Guid guid) {
            var coreVector = new byte[16];
            Array.Copy(Encryption.Hash(guid.ToString()), 0, coreVector, 0, coreVector.Length);
            return coreVector;
        }

        public static string RemoveWhiteSpace(this string input) {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

        public static string RemoveWhiteSpaceFromStart(this string input) {
            var s = input;
            if(char.IsWhiteSpace(s[0])) {
                s = string.Join("", s.ToCharArray().Skip(1));
                RemoveWhiteSpaceFromStart(s);
            }
            return s;
        }

        public static string RemoveDoubleSpace(this string input) {
            var line = input.Replace("\t", " ");
            while(line.IndexOf("  ", StringComparison.InvariantCulture) > 0) {
                line = line.Replace("  ", " ");
            }
            return line;
        }

        public static string Replace(this string input, string[] values, string rep) {
            var i = input;
            foreach(var val in values) {
                i = i.Replace(val, rep);
            }
            return i;
        }

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictA, Dictionary<TKey, TValue> dictB)
    where TValue : class {
            return dictA.Keys.Union(dictB.Keys).ToDictionary(k => k, k => dictA.ContainsKey(k) ? dictA[k] : dictB[k]);
        }
    }
}