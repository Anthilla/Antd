using System;
using System.IO;

namespace antdlib.common.Tool {

    public class Replace {

        public enum ExitCode {
            ReplaceOk = 0,
            NoReplaces = 1,
            ReplaceFailed = -1
        }

        private int _replaces;

        public ExitCode InFile(string filename, string strtoreplace, string newstr) {
            var tempfilename = Path.GetTempFileName();
            try {
                using(var srcfs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                using(var tempfs = new FileStream(tempfilename, FileMode.Open, FileAccess.Write))
                using(var srcsr = new StreamReader(srcfs))
                using(var tmpwr = new StreamWriter(tempfs)) {
                    string srccurrline;
                    while((srccurrline = srcsr.ReadLine()) != null) {
                        var deststr = ReplaceSubStr(srccurrline, strtoreplace, newstr);
                        tmpwr.WriteLine(deststr);
                    }
                }
                if(_replaces > 0) {
                    File.Copy(filename, filename + ".bak", true);
                    File.Copy(tempfilename, filename, true);
                    File.Delete(tempfilename);
                    return ExitCode.ReplaceOk;
                }
                File.Delete(tempfilename);
                return ExitCode.NoReplaces;
            }
            catch(Exception) {
                return ExitCode.ReplaceFailed;
            }
        }

        private string ReplaceSubStr(string srcstr, string strtoreplace, string newstr) {
            var deststr = srcstr.Replace(strtoreplace, newstr);
            if(deststr != srcstr) {
                _replaces++;
            }
            return deststr;
        }
    }
}
