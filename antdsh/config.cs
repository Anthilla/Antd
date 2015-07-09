using antdlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    public class config {
        public class downloadDirectory {
            public static void Set(string text) {
                if (!Directory.Exists(text)) {
                    Console.WriteLine("> This directory '{0}' does not exist...", text);
                    return;
                }
                var path = Path.Combine(global.configDir, global.configFile);
                File.Delete(path);
                FileSystem.WriteFile(path, text);
            }

            public static string Get() {
                return FileSystem.ReadFile(Path.Combine(global.configDir, global.configFile));
            }
        }
    }
}
