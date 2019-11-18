using System;
using System.Globalization;

namespace antd.core {
    public class CommonFloat {
        public static float Parse(string value) {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            if(!float.TryParse(value, NumberStyles.Any, ci, out var f)) {
                return default(float);
            }
            return f;
        }

        public static float Parse(string value, string decimalSeparator = ".") {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = decimalSeparator;
            if(!float.TryParse(value, NumberStyles.Any, ci, out var f)) {
                return default(float);
            }
            return f;
        }

        public static float Parse(bool value) {
            return value ? 1f : 0f;
        }

        public static float Parse(byte value) {
            return Convert.ToSingle(value);
        }

        public static float Parse(char value) {
            return Convert.ToSingle(value);
        }

        public static float Parse(decimal value) {
            return Convert.ToSingle(value);
        }

        public static float Parse(double value) {
            return Convert.ToSingle(value);
        }

        public static float Parse<T>(T value) where T : struct, IConvertible {
            if(!typeof(T).IsEnum) {
                return -1f;
            }
            return Convert.ToInt32(value);
        }

        public static float Parse(int value) {
            return Convert.ToSingle(value);
        }

        public static float Parse(long value) {
            return Convert.ToSingle(value);
        }

        public static float Parse(sbyte value) {
            return Convert.ToSingle(value);
        }

        public static float Parse(short value) {
            return Convert.ToSingle(value);
        }

        public static float Parse(uint value) {
            return Convert.ToSingle(value);
        }

        public static float Parse(ulong value) {
            return Convert.ToSingle(value);
        }

        public static float Parse(ushort value) {
            return Convert.ToSingle(value);
        }
    }
}