using System.Collections.Generic;
using System.IO;
using System.Linq;
using anthilla.commands;
using anthilla.core;

namespace antdlib.config {
    public class SystemUser {
        private const string FilePath = "/etc/shadow";

        public List<string> GetAll() {
            return File.ReadAllLines(FilePath).Select(_ => _.SplitToList(":").FirstOrDefault()).ToList();
        }

        public void Create(string user) {
            Bash.Execute($"useradd {user}", false);
        }

        public string HashPasswd(string input) {
            var output = CommandLauncher.Launch("mkpasswd", new Dictionary<string, string> { { "$password", input } }).FirstOrDefault();
            return output;
        }

        public void SetPassword(string user, string password) {
            if(string.IsNullOrEmpty(user)) {
                return;
            }
            if(string.IsNullOrEmpty(password)) {
                return;
            }
            Bash.Execute($"usermod -p '{password}' {user}", false);
        }
    }
}