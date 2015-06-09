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

using Antd.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.CommandManagement {
    public class CommandDB {
        public class CommandInputModel {
            public string _Id { get; set; }

            public string Guid { get; set; }

            public DateTime Date { get; set; }

            public string File { get; set; }

            public string Arguments { get; set; }

            public string Layout { get; set; }

            public string InputID { get; set; }

            public string InputLocation { get; set; }

            public string Notes { get; set; }
        }

        public static List<CommandInputModel> GetAll() {
            var list = DeNSo.Session.New.Get<CommandInputModel>(m => m != null).ToList();
            if (list == null) {
                return new List<CommandInputModel>() { };
            }
            else {
                return list;
            }
        }

        public static List<CommandInputModel> GetByString(string q) {
            return DeNSo.Session.New.Get<CommandInputModel>(m => m.File.Contains(q) || m.Arguments.Contains(q)).ToList();
        }

        public static CommandInputModel GetByGuid(string g) {
            return DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == g).FirstOrDefault();
        }

        public static void Create(string inputid, string command, string layout, string inputlocation, string notes) {
            var model = new CommandInputModel();
            model._Id = inputid;
            model.Guid = inputid;
            model.Date = DateTime.Now;
            model.File = command.GetFirstString();
            model.Arguments = command.GetAllStringsButFirst();
            model.Layout = layout;
            model.InputLocation = inputlocation;
            model.Notes = notes;
            DeNSo.Session.New.Set(model);
        }

        public static void Delete(string g) {
            var model = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == g).FirstOrDefault();
            DeNSo.Session.New.Delete(model);
        }

        public static void Launch(string guid) {
            var command = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == guid).FirstOrDefault();
            Command.Launch(command.File, command.Arguments);
        }

        public static string LaunchAndGetOutput(string inputid) {
            var command = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == inputid).FirstOrDefault();
            return Command.Launch(command.File, command.Arguments).output;
        }

        public static string LaunchAndGetOutputUsingNewValue(string inputid, string newValue) {
            var command = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == inputid).FirstOrDefault();
            var layout = command.Layout;
            var findReplace = "{" + inputid + "}";
            var newCommand = layout.Replace(findReplace, newValue);
            var newFile = newCommand.GetFirstString();
            var newArguments = newCommand.GetAllStringsButFirst();
            return Command.Launch(newFile, newArguments).output;
        }
    }
}
