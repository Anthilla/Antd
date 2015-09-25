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

using antdlib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace antdlib {

    public class Proc {

        private static List<ProcModel> GetAllAllProc() {
            CommandModel command = Terminal.Execute("ps -aef").ConvertCommandToModel();
            var output = JsonConvert.SerializeObject(command.output);
            List<ProcModel> procs = MapProcJson(output);
            return procs;
        }

        public static List<ProcModel> AllAll { get { return GetAllAllProc(); } }

        private static List<ProcModel> GetAllProc() {
            CommandModel command = Terminal.Execute("ps -aef").ConvertCommandToModel();
            var output = JsonConvert.SerializeObject(command.output);
            var list = MapProcJson(output);
            var procs = new List<ProcModel>() { };
            foreach (var p in list) {
                if (!p.CMD.Contains('[')) {
                    procs.Add(p);
                }
            }
            return procs;
        }

        public static List<ProcModel> All { get { return GetAllProc(); } }

        public static List<ProcModel> MapProcJson(string _procJson) {
            string procJson = _procJson;
            procJson = System.Text.RegularExpressions.Regex.Replace(_procJson, @"\s{2,}", " ").Replace("\"", "");
            string[] rowDivider = new[] { "\\n" };
            string[] procJsonRow = new string[] { };
            procJsonRow = procJson.Split(rowDivider, StringSplitOptions.None).ToArray();
            List<ProcModel> procs = new List<ProcModel>() { };
            foreach (string rowJson in procJsonRow) {
                if (!string.IsNullOrEmpty(rowJson)) {
                    var cellDivider = new[] { " " };
                    var procJsonCell = rowJson.Split(cellDivider, StringSplitOptions.None).ToArray();
                    var proc = MapProc(procJsonCell);
                    procs.Add(proc);
                }
            }
            return procs;
        }

        public static ProcModel MapProc(string[] _procJsonCell) {
            string[] procJsonCell = _procJsonCell;
            ProcModel proc = new ProcModel();
            proc.UID = procJsonCell[0];
            proc.PID = procJsonCell[1];
            proc.PPID = procJsonCell[2];
            proc.C = procJsonCell[3];
            proc.STIME = procJsonCell[4];
            proc.TTY = procJsonCell[5];
            if (procJsonCell.Length > 6) {
                proc.TIME = procJsonCell[6];
            }
            if (procJsonCell.Length > 8) {
                proc.CMD = procJsonCell[7] + " " + procJsonCell[8];
            }
            else if (procJsonCell.Length > 7) {
                proc.CMD = procJsonCell[7];
            }
            return proc;
        }

        public static string GetPID(string service) {
            List<ProcModel> procs = Proc.All;
            var proc = (from p in procs
                        where p.CMD.Contains(service)
                        select p).FirstOrDefault();
            return proc != null ? proc.PID : null;
        }
    }

    public class ProcModel {

        //UID PID PPID C STIME TTY TIME CMD
        public string UID { get; set; }

        public string PID { get; set; }

        public string PPID { get; set; }

        public string C { get; set; }

        public string STIME { get; set; }

        public string TTY { get; set; }

        public string TIME { get; set; }

        public string CMD { get; set; }
    }
}