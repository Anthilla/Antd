using System.Text;

namespace antd.core {
    public class CommonString {
        public static string Append(string s1, string s2) {
            var sb = new StringBuilder();
            sb.Append(s1);
            sb.Append(s2);
            return sb.ToString();
        }

        public static string Append(string s1, string s2, string s3) {
            var sb = new StringBuilder();
            sb.Append(s1);
            sb.Append(s2);
            sb.Append(s3);
            return sb.ToString();
        }

        public static string Append(string s1, string s2, string s3, string s4) {
            var sb = new StringBuilder();
            sb.Append(s1);
            sb.Append(s2);
            sb.Append(s3);
            sb.Append(s4);
            return sb.ToString();
        }

        public static string Append(string s1, string s2, string s3, string s4, string s5) {
            var sb = new StringBuilder();
            sb.Append(s1);
            sb.Append(s2);
            sb.Append(s3);
            sb.Append(s4);
            sb.Append(s5);
            return sb.ToString();
        }

        public static string Append(params string[] args) {
            var sb = new StringBuilder();
            for(int i = 0; i < args.Length; i++) {
                sb.Append(args[i]);
            }
            return sb.ToString();
        }

        public static string Build(string[] args) {
            var sb = new StringBuilder();
            for(int i = 0; i < args.Length; i++) {
                sb.Append(args[i]);
            }
            return sb.ToString();
        }

        public static string Build(string[] args, char separator) {
            var sb = new StringBuilder();
            for(int i = 0; i < args.Length; i++) {
                sb.Append(args[i]);
                sb.Append(separator);
            }
            return sb.ToString().TrimEnd(separator);
        }

        public static string Build(string[] args, string separator) {
            var sb = new StringBuilder();
            for(int i = 0; i < args.Length; i++) {
                sb.Append(args[i]);
                if(i != args.Length - 1) {
                    sb.Append(separator);
                }
            }
            return sb.ToString();
        }

        public static bool AreEquals(string s1, string s2) {
            return string.CompareOrdinal(s1, s2) == 0;
        }

        public static bool AreEquals(string s1, object s2) {
            return string.CompareOrdinal(s1, s2.ToString()) == 0;
        }

        public static bool AreEquals(object s1, string s2) {
            return string.CompareOrdinal(s1.ToString(), s2) == 0;
        }

        public static bool AreEquals(object s1, object s2) {
            return string.CompareOrdinal(s1.ToString(), s2.ToString()) == 0;
        }

        public static string Substring(string value, int s, int e) {
            if(s < 0) {
                s = 0;
            }
            if(s > value.Length - 1) {
                return string.Empty;
            }
            if(e > value.Length) {
                e = value.Length;
            }
            return value.Substring(s, e);
        }
    }
}