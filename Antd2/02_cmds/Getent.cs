using anthilla.core;
using System.Collections.Generic;
using System.Linq;
using SSO = System.StringSplitOptions;

namespace Antd2.cmds {
    public class Getent {

        private const string getentCommand = "getent";
        private const string useraddCommand = "useradd";
        private const string usermodCommand = "usermod";
        private const string groupaddCommand = "groupadd";
        private const string shadowArg = "shadow";
        private const string passwdArg = "passwd";
        private const string groupArg = "group";
        private const string mkpasswdFileLocation = "/usr/bin/mkpasswd";

        public static IEnumerable<string> GetGroups() {
            return Bash.Execute($"{getentCommand} {groupArg}")
                .Select(_ => _.Split(new[] { ':' }, SSO.RemoveEmptyEntries).FirstOrDefault());
        }

        public static IEnumerable<string> GetUsers() {
            return Bash.Execute($"{getentCommand} {passwdArg}")
                .Select(_ => _.Split(new[] { ':' }, SSO.RemoveEmptyEntries).FirstOrDefault());
        }

        public static void AddUser(string user) {
            Bash.Do($"{useraddCommand} {user}");
        }

        /// <summary>
        //usermod -a -G editorial olivia
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public static void AssignGroup(string user, string group) {
            Bash.Do($"{usermodCommand} -a -G {group} {user}");
        }

        public static void AddGroup(string group) {
            Bash.Do($"{groupaddCommand} {group}");
        }

        public static string HashPasswd(string input) {
            var arg = CommonString.Append("-m sha-512 '", input, "'");
            var result = CommonProcess.Execute(mkpasswdFileLocation, arg).ToArray();
            return result[0];
        }

        public static void SetPassword(string user, string password) {
            var args = CommonString.Append("-p '", password, "' ", user);
        }

    }
}
