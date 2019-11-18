using System;
using System.Globalization;

namespace antd.core {
    public class CommonInt16 {
        public static short Parse(string value) {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            if(!short.TryParse(value, NumberStyles.Any, ci, out var f)) {
                return default(short);
            }
            return f;
        }

        public static short Parse(string value, string decimalSeparator = ".") {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = decimalSeparator;
            if(!short.TryParse(value, NumberStyles.Any, ci, out var f)) {
                return default(short);
            }
            return f;
        }

        public static short Parse(bool value) {
            return value ? (short)1 : (short)0;
        }

        public static short Parse(byte value) {
            return Convert.ToInt16(value);
        }

        public static short Parse(char value) {
            return Convert.ToInt16(value);
        }

        public static short Parse(decimal value) {
            if(value < short.MinValue) {
                return short.MinValue;
            }
            if(value > short.MaxValue) {
                return short.MaxValue;
            }
            return Convert.ToInt16(value);
        }

        public static short Parse(double value) {
            if(value < short.MinValue) {
                return short.MinValue;
            }
            if(value > short.MaxValue) {
                return short.MaxValue;
            }
            return Convert.ToInt16(value);
        }

        public static short Parse<T>(T value) where T : struct, IConvertible {
            if(!typeof(T).IsEnum) {
                return -1;
            }
            return Convert.ToInt16(value);
        }

        public static short Parse(float value) {
            if(value < short.MinValue) {
                return short.MinValue;
            }
            if(value > short.MaxValue) {
                return short.MaxValue;
            }
            return Convert.ToInt16(value);
        }

        public static short Parse(int value) {
            if(value < short.MinValue) {
                return short.MinValue;
            }
            if(value > short.MaxValue) {
                return short.MaxValue;
            }
            return Convert.ToInt16(value);
        }

        public static short Parse(long value) {
            if(value < short.MinValue) {
                return short.MinValue;
            }
            if(value > short.MaxValue) {
                return short.MaxValue;
            }
            return Convert.ToInt16(value);
        }

        public static short Parse(sbyte value) {
            return Convert.ToInt16(value);
        }

        public static short Parse(uint value) {
            if(value > short.MaxValue) {
                return short.MaxValue;
            }
            return Convert.ToInt16(value);
        }

        public static short Parse(ulong value) {
            return Convert.ToInt16(value);
        }

        public static short Parse(ushort value) {
            return Convert.ToInt16(value);
        }
    }
}