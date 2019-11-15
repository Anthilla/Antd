using System;
using System.Linq;

namespace Antd2.cmds {
    public class Acl {

        private const string getfaclCommand = "getfacl";
        private const string setfaclCommand = "setfacl";

        /// <summary>
        /// Esempio:
        ///   # file: Data/Data02/storage/recordings
        ///   # owner: root
        ///   # group: root
        ///   user::rwx
        ///   group::r-x
        ///   other::r-x
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static (string Owner, string Group, string[] Acl) Get(string file) {
            var lines = Bash.Execute($"{getfaclCommand} {file}").ToArray();
            var owner = lines[1].Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            var group = lines[2].Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            return (owner, owner, lines.Skip(3).ToArray());
        }

        public static void Set(string file) {
            throw new NotImplementedException();
        }
    }
}
