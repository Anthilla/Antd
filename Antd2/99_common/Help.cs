using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace Antd2 {
    public class Help {
        public static string CaptureGroup(string sourceText, string pattern) {
            if (string.IsNullOrEmpty(sourceText)) {
                return string.Empty;
            }
            var regex = new Regex(pattern, RegexOptions.Multiline);
            var matchedGroups = regex.Match(sourceText).Groups;
            var capturedText = matchedGroups[1].Value;
            return capturedText;
        }
        public static string[] ReadLine() {
            Console.Write("> ");
            return Console.ReadLine()?.Split() ?? Array.Empty<string>();
        }

        public static string RemoveWhiteSpace(string a) {
            return Regex.Replace(a, @"\s+", "");
        }

        public static (string Address, string Range) SplitAddressAndRange(string a) {
            var arr = a.Split(new[] { '/' });
            if (arr.Length >= 2)
                return (arr[0], arr[1]);
            else
                return (arr[0], string.Empty);
        }
    }
}
