using System.Text.RegularExpressions;

namespace Antd {
    public class Help {
        public static string CaptureGroup(string sourceText, string pattern) {
            if(string.IsNullOrEmpty(sourceText)) {
                return string.Empty;
            }
            var regex = new Regex(pattern, RegexOptions.Multiline);
            var matchedGroups = regex.Match(sourceText).Groups;
            var capturedText = matchedGroups[1].Value;
            return capturedText;
        }
    }
}
