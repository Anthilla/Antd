using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd.Systemd {
    public class Units {

        private static List<UnitModel> GetAllUnits() {
            CommandModel command = Command.Launch("systemctl", "--no-pager list-unit-files");
            var output = JsonConvert.SerializeObject(command.output);
            List<UnitModel> units = MapUnitJson(output);
            units.RemoveAt(units.ToArray().Length - 1);
            units.RemoveAt(0);
            return units;
        }

        public static List<UnitModel> All { get { return GetAllUnits(); } }

        public static List<UnitModel> MapUnitJson(string _unitJson) {
            string unitJson = _unitJson;
            unitJson = System.Text.RegularExpressions.Regex.Replace(_unitJson, @"\s{2,}", " ").Replace("\"", "");
            string[] rowDivider = new String[] { "\\n" };
            string[] unitJsonRow = new string[] { };
            unitJsonRow = unitJson.Split(rowDivider, StringSplitOptions.None).ToArray();
            List<UnitModel> units = new List<UnitModel>() { };
            foreach (string rowJson in unitJsonRow) {
                if (rowJson != null && rowJson != "") {
                    string[] unitJsonCell = new string[] { };
                    string[] cellDivider = new String[] { " " };
                    unitJsonCell = rowJson.Split(cellDivider, StringSplitOptions.None).ToArray();
                    UnitModel unit = MapUnit(unitJsonCell);
                    units.Add(unit);
                }
            }
            return units;
        }

        public static UnitModel MapUnit(string[] _unitJsonCell) {
            string[] unitJsonCell = _unitJsonCell;
            UnitModel unit = new UnitModel();
            unit.name = unitJsonCell[0];
            if (unitJsonCell[1] != null) {
                unit.status = unitJsonCell[1];
            }
            return unit;
        }
    }
}
