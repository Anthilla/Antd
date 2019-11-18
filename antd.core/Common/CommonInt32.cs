using System;
using System.Globalization;

namespace antd.core {
    public class CommonInt32 {
        public static int Parse(string value) {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            if(!int.TryParse(value, NumberStyles.Any, ci, out var f)) {
                return default(int);
            }
            return f;
        }

        public static int Parse(string value, string decimalSeparator = ".") {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = decimalSeparator;
            if(!int.TryParse(value, NumberStyles.Any, ci, out var f)) {
                return default(int);
            }
            return f;
        }

        public static int Parse(bool value) {
            return value ? 1 : 0;
        }

        public static int Parse(byte value) {
            return Convert.ToInt32(value);
        }

        public static int Parse(char value) {
            return Convert.ToInt32(value);
        }

        public static int Parse(decimal value) {
            if(value < int.MinValue) {
                return int.MinValue;
            }
            if(value > int.MaxValue) {
                return int.MaxValue;
            }
            return Convert.ToInt32(value);
        }

        public static int Parse(double value) {
            if(value < int.MinValue) {
                return int.MinValue;
            }
            if(value > int.MaxValue) {
                return int.MaxValue;
            }
            return Convert.ToInt32(value);
        }

        public static int Parse<T>(T value) where T : struct, IConvertible {
            if(!typeof(T).IsEnum) {
                return -1;
            }
            return Convert.ToInt32(value);
        }

        public static int Parse(float value) {
            if(value < int.MinValue) {
                return int.MinValue;
            }
            if(value > int.MaxValue) {
                return int.MaxValue;
            }
            return Convert.ToInt32(value);
        }

        public static int Parse(long value) {
            if(value < int.MinValue) {
                return int.MinValue;
            }
            if(value > int.MaxValue) {
                return int.MaxValue;
            }
            return Convert.ToInt32(value);
        }

        public static int Parse(sbyte value) {
            return Convert.ToInt32(value);
        }

        public static int Parse(short value) {
            return Convert.ToInt32(value);
        }

        public static int Parse(uint value) {
            return Convert.ToInt32(value);
        }

        public static int Parse(ulong value) {
            if(value > int.MaxValue) {
                return int.MaxValue;
            }
            return Convert.ToInt32(value);
        }

        public static int Parse(ushort value) {
            return Convert.ToInt32(value);
        }

        public static int GetPercentage(int tot, int part) {
            if(tot == 0 || part == 0) {
                return 0;
            }
            var p = part * 100 / tot;
            return p <= 100 ? p : 0;
        }
    }
}