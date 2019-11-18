using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace antd.core {
    public static class StringExtensions {

        public static string ToTitleCase(this string input) {
            return string.IsNullOrEmpty(input) ? null : new CultureInfo("en-US", false).TextInfo.ToTitleCase(input);
        }

        public static byte ToBinaryValue(this char input) {
            return input == '1' ? (byte)1 : (byte)0;
        }

        public static bool ToBoolean(this string str) {
            return !string.IsNullOrEmpty(str) && Convert.ToBoolean(str.ToLower());
        }

        public static bool? ToNullableBoolean(this string str) {
            return string.IsNullOrEmpty(str) ? null : (bool?)Convert.ToBoolean(str.ToLower());
        }

        public static Guid ToGuid(this string str) {
            return Guid.TryParse(str, out var guid) ? guid : Guid.Empty;
        }

        public static string[] SplitToArray(this string str, string separator = ",", StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries) {
            if(str.Length < 1) {
                return new string[0];
            }
            return str.Split(new[] { separator }, options);
        }

        public static T ToEnum<T>(this string str) where T : struct, IComparable {
            var isenum = Enum.TryParse(str, out T e);
            return isenum ? e : default(T);
        }

        public static T ToObject<T>(this string json) {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToHex(this string value) {
            var chars = value.ToCharArray();
            var stringBuilder = new StringBuilder();
            foreach(var c in chars)
                stringBuilder.Append(((short)c).ToString(""));
            return stringBuilder.ToString();
        }

        public static byte[] GetBytes(this string str) {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string RemoveWhiteSpace(this string input) {
            return new string(input.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        public static bool ContainsAny(this string text, IEnumerable<string> values) {
            return values.Any(text.Contains);
        }

        public static List<string> TextToList(string text) {
            var rowDivider = new[] { "\n" };
            var rowList = text.Split(rowDivider, StringSplitOptions.None).ToArray();
            return rowList.Where(row => !string.IsNullOrEmpty(row)).ToList();
        }

        public static decimal ToDecimal(this string text) {
            return decimal.TryParse(text, out var number) ? number : 0;
        }

        public static DateTime ToDateTime(this string text) {
            return DateTime.TryParse(text, out var dt) ? dt : DateTime.MinValue;
        }

        public static DateTime ToDateTime(this string text, string format) {
            return DateTime.ParseExact(text, format, CultureInfo.InvariantCulture);
        }

        public static byte ToByte(this string input) {
            var i = int.TryParse(input, out var o);
            if(!i) {
                return 0;
            }
            if(o < 0) {
                return 0;
            }
            if(o > 255) {
                return 0;
            }
            return (byte)o;
        }

        public static int ToInt32(this string str) {
            if(string.IsNullOrEmpty(str))
                return 0;
            return int.Parse(str);
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
    }
}
