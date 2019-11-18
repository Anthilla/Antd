using System;
using System.Linq;

namespace antd.core {
    public class CommonMath {

        public static decimal Sum(decimal s1, decimal s2) {
            return s1 + s2;
        }

        public static decimal Sum(decimal s1, decimal s2, decimal s3) {
            return s1 + s2 + s3;
        }

        public static decimal Sum(decimal s1, decimal s2, decimal s3, decimal s4) {
            return s1 + s2 + s3 + s4;
        }

        public static decimal Sum(decimal s1, decimal s2, decimal s3, decimal s4, decimal s5) {
            return s1 + s2 + s3 + s4 + s5;
        }

        public static decimal Sum(params decimal[] args) {
            decimal sum = 0;
            for(int i = 0; i < args.Length; i++) {
                sum = sum + args[i];
            }
            return sum;
        }

        public static int Sum(params int[] args) {
            int sum = 0;
            for(int i = 0; i < args.Length; i++) {
                sum = sum + args[i];
            }
            return sum;
        }

        public static decimal Average(decimal[] values) {
            if(values.Length == 0) { return 0; }
            return values.Sum() / values.Length;
        }

        public static int Average(int[] values) {
            if(values.Length == 0) { return 0; }
            return values.Sum() / values.Length;
        }

        public static T Max<T>(T[] values) where T : struct, IComparable {
            return values.Max();
        }

        public static int Percentage(int partial, int total, bool limit = false) {
            if(total <= 0)
                return 0;
            var result = (partial * 100) / total;
            return limit == false ? result : result >= 100 ? 100 : result;
        }
    }
}