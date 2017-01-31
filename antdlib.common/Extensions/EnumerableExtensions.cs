using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace antdlib.common {
    public static class EnumerableExtensions {

        public static string JoinToString(this IEnumerable<string> stringList, string separator = ",") {
            if(stringList == null)
                return string.Empty;
            stringList = stringList.ToList();
            return !stringList.Any() ? string.Empty : string.Join(separator, stringList.ToList());
        }

        public static int[] ToIntArray(this string[] arr) {
            return arr.Select(s => Convert.ToInt32(s)).ToArray();
        }

        public static string ToHex(this byte[] bytes) {
            var value = GetString(bytes);
            var chars = value.ToCharArray();
            var stringBuilder = new StringBuilder();
            foreach(var c in chars)
                stringBuilder.Append(((short)c).ToString(""));
            return stringBuilder.ToString();
        }

        public static string GetString(this byte[] bytes) {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> input) {
            var list = new HashSet<T>();
            foreach(var i in input)
                list.Add(i);
            return list;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length) {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
