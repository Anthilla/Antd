using System;
using System.IO;
using System.Linq;

namespace antdlib.common.Tool {
    internal enum ExitCode {
        ReplaceOk = 0,
        NoReplaces = 1,
        BadCmdLine = -1,
        ReplaceFailed = -2,
    }

    public class Replace {
        static int _replaces = 0;

        static int Main(string[] args) {
            if(!CmdLineOk(args))
                return (int)ExitCode.BadCmdLine;

            var filename = args[0];
            var strtoreplace = args[1];
            var newstr = args[2];
            var tempfilename = Path.GetTempFileName();

            Console.WriteLine("Replace in {0}: '{1}' -> '{2}'", filename, strtoreplace, newstr);

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

                Console.WriteLine();

                if(_replaces > 0) {
                    // делаем копию оригинала
                    File.Copy(sourceFileName: filename, destFileName: filename + ".bak", overwrite: true);
                    // удаляем оригинал
                    File.Delete(filename);
                    // переносим временный файл
                    File.Move(sourceFileName: tempfilename, destFileName: filename);

                    Console.WriteLine("{0} replaces OK!", _replaces);

                    return (int)ExitCode.ReplaceOk;
                }
                else {
                    // удаляем временный файл
                    File.Delete(tempfilename);

                    Console.WriteLine("OK. No replaces.");

                    return (int)ExitCode.NoReplaces;
                }
            }
            catch(Exception ex) {
                Console.WriteLine("Replace failed:");
                return (int)ExitCode.ReplaceFailed;
            }
        }

        static string ReplaceSubStr(string srcstr, string strtoreplace, string newstr) {
            var deststr = srcstr.Replace(strtoreplace, newstr);

            if(deststr != srcstr) {
                _replaces++;
                Console.Write("!");
            }
            else
                Console.Write(".");

            return deststr;
        }

        static bool CmdLineOk(string[] args) {
            if(args.Count() == 3) {
                return true;
            }
            else {
                Console.WriteLine(@"Use: replace.exe ""File name.ext"" ""string to replace"" ""new string""");
                Console.WriteLine("Or, if args ends with backsplash:");
                Console.WriteLine(@"Use: replace.exe ""File name.ext"" ""string to replace\\"" ""new string\\""");
                Console.WriteLine("Return result code:");
                Console.WriteLine(" 0 - Replace OK.");
                Console.WriteLine(" 1 - No replaces.");
                Console.WriteLine("-1 - Bad cmdline");
                Console.WriteLine("-2 - Replace failed.");

                return false;
            }
        }
    }
}
