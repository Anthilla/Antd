using antd.core;
using System;
using System.Collections.Generic;
using System.Linq;
using SSO = System.StringSplitOptions;

namespace Antd2.cmds {
    public class Getent {

        private const string getentCommand = "getent";
        private const string useraddCommand = "useradd";
        private const string usermodCommand = "usermod";
        private const string groupaddCommand = "groupadd";
        private const string groupmodCommand = "groupmod";
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
        /// usermod -a -G [GROUP] [USER]
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public static void AssignGroup(string user, string group) {
            Bash.Do($"{usermodCommand} -a -G {group} {user}");
        }

        public static void AddGroup(string group) {
            Bash.Do($"{groupaddCommand} {group}");
        }

        /// <summary>
        /// usermod -u [NEW_USER_ID] [USERNAME]
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newId"></param>
        public static void ChangeUserUid(string user, string newId) {
            Bash.Do($"{usermodCommand} -u {newId} {user}");
        }

        /// <summary>
        /// usermod -c "[NEW_USER_DESCRIPTION]" [USERNAME]
        /// </summary>
        /// <param name="user"></param>
        /// <param name="description"></param>
        public static void ChangeUserDescription(string user, string description) {
            Bash.Do($"{usermodCommand} -c \\\"{description}\\\" {user}");
        }

        /// <summary>
        /// groupmod -g [NEW_GROUP_ID] [GROUP]
        /// </summary>
        /// <param name="group"></param>
        /// <param name="newId"></param>
        public static void ChangeGroupUid(string group, string newId) {
            Bash.Do($"{groupmodCommand} -g {newId} {group}");
        }

        public static string HashPasswd(string input) {
            var arg = CommonString.Append("-m sha-512 '", input, "'");
            var result = CommonProcess.Execute(mkpasswdFileLocation, arg).ToArray();
            return result[0];
        }

        public static void SetPassword(string user, string password) {
            var args = CommonString.Append("-p '", password, "' ", user);
        }

        /// <summary>
        /// deus:x:701:701:AOS_UserStack_deus:
        /// xctr:x:702:702:AOS_UserStack_executor:
        /// obsi:x:703:703:AOS_UserStack_obsequium_internum:
        /// obse:x:704:704:AOS_UserStack_obsequium_exterioris:
        /// visor:x:1000:1000:AOS_UserStack_supervisor:
        /// 
        /// wheel:x:10:root,deus,xctr,visor
        /// deus:x:701:deus
        /// xctr:x:702:xctr
        /// obsi:x:703:
        /// obse:x:704:
        /// visor:x:1000:visor
        /// </summary>
        public static void AddAOSDefaults() {
            foreach (var (User, Uid, Description) in new List<(string User, string Uid, string Description)> {
                ("deus", "701", "AOS_UserStack_deus"),
                ("xctr", "702", "AOS_UserStack_executor"),
                ("obsi", "703", "AOS_UserStack_obsequium_internum"),
                ("obse", "704", "AOS_UserStack_obsequium_exterioris"),
                ("visor", "1000", "AOS_UserStack_supervisor"),
            }) {
                AddUser(User);
                ChangeUserUid(User, Uid);
                ChangeUserDescription(User, Description);
            }

            foreach (var (Group, Uid, Users) in new List<(string Group, string Uid, string[] Users)> {
                ("wheel", "10", new[] { "root", "deus", "xctr", "visor" }),
                ("deus", "701", new[] { "deus" }),
                ("xctr", "702", new[] { "xctr" }),
                ("obsi", "703", Array.Empty<string>()),
                ("obse", "704", Array.Empty<string>()),
                ("visor", "1000", new[] { "visor" }),
            }) {
                AddGroup(Group);
                ChangeGroupUid(Group, Uid);
                foreach (var user in Users)
                    AssignGroup(user, Group);
            }
        }
    }
}
