using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Antd.Sysctl {
    public class Sysctl {

        private static List<SysctlModel> GetAllSysctls() {
            CommandModel command = Command.Launch("sysctl", "--all");
            var output = JsonConvert.SerializeObject(command.output);
            List<SysctlModel> sysctls = MapSysctlJson(output);
            return sysctls;
        }

        public static List<SysctlModel> All { get { return GetAllSysctls(); } }

        public static List<SysctlModel> MapSysctlJson(string _sysctlJson) {
            string sysctlJson = _sysctlJson;
            sysctlJson = Regex.Replace(_sysctlJson, @"\s{2,}", " ").Replace("\"", "").Replace("\\n", "\n").Replace("\t", " ");
            string[] rowDivider = new String[] { "\n" };
            string[] sysctlJsonRow = new string[] { };
            sysctlJsonRow = sysctlJson.Split(rowDivider, StringSplitOptions.None).ToArray();
            List<SysctlModel> sysctls = new List<SysctlModel>() { };
            foreach (string rowJson in sysctlJsonRow) {
                if (rowJson != null && rowJson != "") {
                    string[] sysctlJsonCell = new string[] { };
                    string[] cellDivider = new String[] { " = " };
                    sysctlJsonCell = rowJson.Split(cellDivider, StringSplitOptions.None).ToArray();
                    SysctlModel sysctl = MapSysctl(sysctlJsonCell);
                    sysctls.Add(sysctl);
                }
            }
            return sysctls;
        }

        public static SysctlModel MapSysctl(string[] _sysctlJsonCell) {
            string[] sysctlJsonCell = _sysctlJsonCell;
            SysctlModel sysctl = new SysctlModel();
            sysctl.param = sysctlJsonCell[0];
            sysctl.value = sysctlJsonCell[1];
            return sysctl;
        }
    }
}
