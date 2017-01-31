using System.Globalization;

namespace antdlib.common {
    public static class StringExtensions {

        public static string ToTitleCase(this string input) {
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(input);
        }

        public static byte ToBinaryValue(this char input) {
            return input == '1' ? (byte)1 : (byte)0;
        }
    }
}
