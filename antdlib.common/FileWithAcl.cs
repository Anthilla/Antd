using System.Collections.Generic;
using System.IO;

namespace antdlib.common {
    public class DirectoryWithAcl {
        public static void CreateDirectory(string directory, string acl = "", string owner = "", string group = "") {
            System.IO.Directory.CreateDirectory(directory);
            //if(!string.IsNullOrEmpty(acl)) {
            //    Bash.Execute($"chmod {acl} {directory}");

            //}
            //if(!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(group)) {
            //    Bash.Execute($"chown {owner}:{group} {directory}");
            //}
        }
    }

    public class FileWithAcl {

        public static void WriteAllText(string file, string text, string acl = "", string owner = "", string group = "") {
            if(File.Exists(file)) {
                File.Copy(file, $"{file}.bck", true);
            }
            File.WriteAllText(file, text);
            if(!string.IsNullOrEmpty(acl)) {
                Bash.Execute($"chmod {acl} {file}");

            }
            if(!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(group)) {
                Bash.Execute($"chown {owner}:{group} {file}");
            }
        }

        public static void WriteAllLines(string file, IEnumerable<string> lines, string acl = "", string owner = "", string group = "") {
            if(File.Exists(file)) {
                File.Copy(file, $"{file}.bck", true);
            }
            File.WriteAllLines(file, lines);
            if(!string.IsNullOrEmpty(acl)) {
                Bash.Execute($"chmod {acl} {file}");

            }
            if(!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(group)) {
                Bash.Execute($"chown {owner}:{group} {file}");
            }
        }

        public static void AppendAllText(string file, string text, string acl = "", string owner = "", string group = "") {
            if(File.Exists(file)) {
                File.Copy(file, $"{file}.bck", true);
            }
            File.AppendAllText(file, text);
            if(!string.IsNullOrEmpty(acl)) {
                Bash.Execute($"chmod {acl} {file}");

            }
            if(!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(group)) {
                Bash.Execute($"chown {owner}:{group} {file}");
            }
        }

        public static void AppendAllLines(string file, IEnumerable<string> lines, string acl = "", string owner = "", string group = "") {
            if(File.Exists(file)) {
                File.Copy(file, $"{file}.bck", true);
            }
            File.AppendAllLines(file, lines);
            if(!string.IsNullOrEmpty(acl)) {
                Bash.Execute($"chmod {acl} {file}");

            }
            if(!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(group)) {
                Bash.Execute($"chown {owner}:{group} {file}");
            }
        }
    }
}
