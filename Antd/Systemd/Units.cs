///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using Antd.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd.Systemd {

    public class Units {

        private static List<UnitModel> GetAllUnits() {
            CommandModel command = Command.Launch("systemctl", "--no-pager list-unit-files");
            var output = JsonConvert.SerializeObject(command.output);
            if (output != null) {
                List<UnitModel> units = MapUnitJson(output);
                units.RemoveAt(units.ToArray().Length - 1);
                if (units.Any()) units.RemoveAt(0);
                return units;
            }
            return null;
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
                if (!string.IsNullOrEmpty(rowJson)) {
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
            if (unitJsonCell.Length > 1 && unitJsonCell[1] != null) {
                unit.status = unitJsonCell[1];
            }
            return unit;
        }
    }
}
