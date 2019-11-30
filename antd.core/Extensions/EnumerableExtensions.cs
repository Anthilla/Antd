using System;
using System.Collections.Generic;
using System.Text;

namespace antd.core {
    public static class EnumerableExtensions {

        public static string JoinToString(this string[] stringList, string separator = ",") {
            return CommonString.Build(stringList, separator);
        }

        public static int[] ToIntArray(this string[] arr) {
            if(arr.Length == 0) {
                return new int[0];
            }
            var ints = new int[arr.Length];
            for(var a = 0;a < arr.Length; a++) {
                var isnum = int.TryParse(arr[a], out var i);
                if(isnum) {
                    ints[a] = i;
                }
                else {
                    ints[a] = 0;
                }
            }
            return ints;
        }

        public static string ToHex(this byte[] bytes) {
            var value = GetString(bytes);
            var chars = value.ToCharArray();
            var stringBuilder = new StringBuilder();
            for (var b = 0;  b < bytes.Length; b++) {
                var h = Convert.ToInt16(bytes[b]);
                stringBuilder.Append(h.ToString(""));
            }
            return stringBuilder.ToString();
        }

        public static string GetString(this byte[] bytes) {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        //public static HashSet<T> ToHashSet<T>(this IEnumerable<T> input) {
        //    return new HashSet<T>(input);
        //}

        public static IEnumerable<T> Merge<T>(this IEnumerable<IEnumerable<T>> input) {
            var list = new HashSet<T>();
            foreach(var i in input)
                foreach(var a in i)
                    list.Add(a);
            return list;
        }


        public static T[] MoveToFront<T>(this T[] mos, Predicate<T> match) {
            if(mos.Length == 0) {
                return new T[0];
            }
            var idx = Array.FindIndex(mos, match);
            if(idx == -1) {
                return new T[0];
            }
            var tmp = mos[idx];
            Array.Copy(mos, 0, mos, 1, idx);
            mos[0] = tmp;
            return mos;
        }
    }
}
