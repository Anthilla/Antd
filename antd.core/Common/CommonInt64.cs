using System;
using System.Globalization;

namespace antd.core {
    public class CommonInt64 {
        public static long Parse(string value) {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            if(!long.TryParse(value, NumberStyles.Any, ci, out var f)) {
                return default(long);
            }
            return f;
        }

        public static long Parse(string value, string decimalSeparator = ".") {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = decimalSeparator;
            if(!long.TryParse(value, NumberStyles.Any, ci, out var f)) {
                return default(long);
            }
            return f;
        }

        public static long Parse(bool value) {
            return value ? 1 : 0;
        }

        public static long Parse(byte value) {
            return Convert.ToInt64(value);
        }

        public static long Parse(char value) {
            return Convert.ToInt64(value);
        }

        public static long Parse(decimal value) {
            if(value < long.MinValue) {
                return long.MinValue;
            }
            if(value > long.MaxValue) {
                return long.MaxValue;
            }
            return Convert.ToInt64(value);
        }

        public static long Parse(double value) {
            if(value < long.MinValue) {
                return long.MinValue;
            }
            if(value > long.MaxValue) {
                return long.MaxValue;
            }
            return Convert.ToInt64(value);
        }

        public static long Parse<T>(T value) where T : struct, IConvertible {
            if(!typeof(T).IsEnum) {
                return -1;
            }
            return Convert.ToInt64(value);
        }

        public static long Parse(float value) {
            if(value < long.MinValue) {
                return long.MinValue;
            }
            if(value > long.MaxValue) {
                return long.MaxValue;
            }
            return Convert.ToInt64(value);
        }

        public static long Parse(int value) {
            return Convert.ToInt64(value);
        }

        public static long Parse(sbyte value) {
            return Convert.ToInt64(value);
        }

        public static long Parse(short value) {
            return Convert.ToInt64(value);
        }

        public static long Parse(uint value) {
            return Convert.ToInt64(value);
        }

        public static long Parse(ulong value) {
            return Convert.ToInt64(value);
        }

        public static long Parse(ushort value) {
            return Convert.ToInt64(value);
        }
    }
}