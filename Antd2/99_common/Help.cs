using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace Antd {
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
    }
}
