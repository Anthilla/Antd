namespace antd.core {
    public class CommonBool {
        public static bool Parse(string value) {
            if(!bool.TryParse(value, out var f)) {
                return false;
            }
            return f;
        }

        public static bool Parse(byte value) {
            return value != 0;
        }

        public static bool Parse(char value) {
            return value != 0;
        }

        public static bool Parse(decimal value) {
            return value != 0;
        }

        public static bool Parse(double value) {
            return value != 0;
        }

        public static bool Parse(float value) {
            return value != 0;
        }

        public static bool Parse(int value) {
            return value != 0;
        }

        public static bool Parse(long value) {
            return value != 0;
        }

        public static bool Parse(sbyte value) {
            return value != 0;
        }

        public static bool Parse(uint value) {
            return value != 0;
        }

        public static bool Parse(ulong value) {
            return value != 0;
        }

        public static bool Parse(ushort value) {
            return value != 0;
        }
    }
}