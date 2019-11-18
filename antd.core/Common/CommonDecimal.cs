using System;
using System.Globalization;

namespace antd.core {
    public class CommonDecimal{
        public static decimal Parse(string value) {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            if(!decimal.TryParse(value, NumberStyles.Any, ci, out var f)) {
                return default(decimal);
            }
            return f;
        }

        public static decimal Parse(string value, string decimalSeparator = ".") {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = decimalSeparator;
            if(!decimal.TryParse(value, NumberStyles.Any, ci, out var f)) {
                return default(decimal);
            }
            return f;
        }

        public static decimal Parse(short value) {
            return Convert.ToDecimal(value);
        }

        public static decimal Parse(ushort value) {
            return Convert.ToDecimal(value);
        }

        public static decimal Parse(int value) {
            return Convert.ToDecimal(value);
        }

        public static decimal Parse(uint value) {
            return Convert.ToDecimal(value);
        }

        public static decimal Parse(long value) {
            return Convert.ToDecimal(value);
        }

        public static decimal Parse(ulong value) {
            return Convert.ToDecimal(value);
        }

        public static decimal Parse(decimal value) {
            return Convert.ToDecimal(value);
        }
    }
}