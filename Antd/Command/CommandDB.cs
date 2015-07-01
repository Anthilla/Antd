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

using System;
using System.Collections.Generic;
using System.Linq;

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
            return DeNSo.Session.New.Get<CommandInputModel>(m => m != null).ToList();
        }

        public static List<CommandInputModel> GetByString(string q) {
            return DeNSo.Session.New.Get<CommandInputModel>(m => m.File.Contains(q) || m.Arguments.Contains(q)).ToList();
        }

        public static CommandInputModel GetByGuid(string g) {
            return DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == g).FirstOrDefault();
        }

        public static string GetCommandByGuid(string g) {
            var m = GetByGuid(g);
            return m.File + " " + m.Arguments;
        }

        public static void Create(string inputid, string command, string layout, string inputlocation, string notes) {
            var model = new CommandInputModel {
                _Id = inputid,
                Guid = inputid,
                Date = DateTime.Now,
                File = command.GetFirstString(),
                Arguments = command.GetAllStringsButFirst(),
                Layout = layout,
                InputLocation = inputlocation,
                Notes = notes
            };
            DeNSo.Session.New.Set(model);
        }

        public static void Delete(string g) {
            var model = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == g).FirstOrDefault();
            DeNSo.Session.New.Delete(model);
        }

        public static void Launch(string guid) {
            var command = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == guid).FirstOrDefault();
            if (command != null) Command.Launch(command.File, command.Arguments);
        }

        public static string LaunchAndGetOutput(string inputid) {
            var command = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == inputid).FirstOrDefault();
            if (command != null) return Command.Launch(command.File, command.Arguments).output;
            return null;
        }

        public static string LaunchAndGetOutputUsingNewValue(string inputid) {
            var command = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == inputid).FirstOrDefault();
            if (command != null) {
                var layout = command.Layout;
                var newFile = layout.GetFirstString();
                var newArguments = layout.GetAllStringsButFirst();
                return Command.Launch(newFile, newArguments).output;
            }
            return null;
        }

        public static string LaunchAndGetOutputUsingNewValue(string inputid, string newValue) {
            var command = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == inputid).FirstOrDefault();
            if (command != null) {
                var layout = command.Layout;
                var findReplace = "{" + inputid + "}";
                var newCommand = layout.Replace(findReplace, newValue);
                var newFile = newCommand.GetFirstString();
                var newArguments = newCommand.GetAllStringsButFirst();
                return Command.Launch(newFile, newArguments).output;
            }
            return null;
        }
    }
}