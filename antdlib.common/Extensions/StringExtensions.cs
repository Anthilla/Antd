using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace antdlib.common {
    public static class StringExtensions {

        public static string ToTitleCase(this string input) {
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(input);
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
            Guid guid;
            return Guid.TryParse(str, out guid) ? guid : Guid.Empty;
        }

        public static List<string> SplitToList(this string str, string separator = ",", StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries) {
            return str.Split(new[] { separator }, options).ToList();
        }

        public static T ToEnum<T>(this string str) {
            if(str == null)
                return default(T);
            return (T)Enum.Parse(typeof(T), str);
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
    }
}
