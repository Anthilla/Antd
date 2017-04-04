using antdlib.common;
using anthilla.commands;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antd.Users {
    public class SystemUser {
        private const string FilePath = "/etc/shadow";

        public List<string> GetAll() {
            return File.ReadAllLines(FilePath).Select(_ => _.SplitToList(":").FirstOrDefault()).ToList();
        }

        public void Create(string user) {
            var bash = new Bash();
            bash.Execute($"useradd {user}", false);
        }

        public string HashPasswd(string input) {
            var launcher = new CommandLauncher();
            var output = launcher.Launch("mkpasswd", new Dictionary<string, string> { { "$password", input } }).FirstOrDefault();
            return output;
        }

        public void SetPassword(string user, string password) {
            if(string.IsNullOrEmpty(user)) {
                return;
            }
            if(string.IsNullOrEmpty(password)) {
                return;
            }
            var bash = new Bash();
            bash.Execute($"usermod -p '{password}' {user}", false);
        }
    }
}