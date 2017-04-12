using antdsh;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace apptest {
    public class Program {
        static void Main(string[] args) {
            if(args.Length == 0) {
                return;
            }
            if(args.Length != 2) {
                return;
            }
            var src = args[0];
            var dst = args[1];

            FileDownloader.DownloadFile(src, dst, 1000 * 60 * 60);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();


            Console.WriteLine(GetFileHash(dst));

            var repoLines = File.ReadAllLines(dst).Select(StringCompression.DecompressString);
            foreach(var r in repoLines) {
                Console.WriteLine(r);
            }

            Console.ReadLine();
        }

        public static string GetFileHash(string filePath) {
            using(var fileStreamToRead = File.OpenRead(filePath)) {
                return BitConverter.ToString(new SHA1Managed().ComputeHash(fileStreamToRead)).Replace("-", string.Empty);
            }
        }

        public static string xz(string file, bool redirectStandard = true) {
            try {
                var proc = new ProcessStartInfo {
                    FileName = @"xz.eze",
                    Arguments = $"-d {file}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                using(var p = new Process()) {
                    p.StartInfo = proc;
                    p.Start();
                    var output = string.Empty;
                    if(redirectStandard) {
                        using(var streamReader = p.StandardOutput) {
                            output = streamReader.ReadToEnd();
                        }
                    }
                    p.WaitForExit(1000 * 30);
                    return output;
                }
            }
            catch(FileNotFoundException fex) {
                Console.WriteLine($"404: {fex.FileName}");
                return "";
            }
            catch(Exception ex) {
                Console.WriteLine($"Failed to execute command: {ex.Message} {ex}");
                return "";
            }
        }
    }
}
