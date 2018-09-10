using anthilla.core;
using System;
using System.IO;

namespace Antd.cmds {
    public class Group {

        private const string groupFileLocation = "/etc/group";
        private const string groupaddFileLocation = "/usr/sbin/groupadd";

        public static SystemGroup[] Get() {
            if(!File.Exists(groupFileLocation)) {
                return new SystemGroup[0];
            }
            var result = File.ReadAllLines(groupFileLocation);
            var groups = new SystemGroup[result.Length];
            for(var i = 0; i < result.Length; i++) {
                var currentLine = result[i];
                var currentLineData = currentLine.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                groups[i] = new SystemGroup() {
                    Active = true,
                    Alias = currentLineData[0]
                };
            }
            return groups;
        }

        public static bool Set() {
            if(Const.IsUnix == false) {
                return false;
            }
            var current = Application.CurrentConfiguration.Users.SystemGroups;
            for(var i = 0; i < current.Length; i++) {
                AddGroup(current[i].Alias);
            }
            return true;
        }

        public static void AddGroup(string group) {
            CommonProcess.Do(groupaddFileLocation, group);
        }
    }
}
