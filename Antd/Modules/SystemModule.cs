//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.Dynamic;
using System.Linq;
using antdlib.Boot;
using antdlib.CCTable;
using antdlib.Certificate;
using antdlib.Common;
using antdlib.Info;
using antdlib.MountPoint;
using antdlib.Status;
using Antd.ViewHelpers;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class SystemModule : CoreModule {
        private const string CctableContextName = "system";

        public SystemModule() {
            this.RequiresAuthentication();

            Post["/system/cctable"] = x => {
                var label = (string)Request.Form.Label;
                var inputLabel = (string)Request.Form.InputLabel;
                var notes = (string)Request.Form.Notes;
                var inputId = "New" + CctableContextName.UppercaseAllFirstLetters().RemoveWhiteSpace() + label.UppercaseAllFirstLetters().RemoveWhiteSpace();
                string inputLocation = "CCTable" + Request.Form.TableName;
                switch ((string)Request.Form.InputType.Value) {
                    case "hidden":
                        var directCommand = (string)Request.Form.CommandDirect;
                        CCTableRepository.New.CreateRowForDirectCommand(CctableContextName, label, inputLabel, directCommand,
                            notes, inputId, inputLocation);
                        break;
                    case "text":
                        var setCommand = (string)Request.Form.CommandSet;
                        var getCommand = (string)Request.Form.CommandGet;
                        CCTableRepository.New.CreateRowForTextInputCommand(CctableContextName, label, inputLabel, setCommand,
                            getCommand, notes, inputId, inputLocation);
                        break;
                    case "checkbox":
                        var enableCommand = (string)Request.Form.CommandTrue;
                        var disableCommand = (string)Request.Form.CommandFalse;
                        CCTableRepository.New.CreateRowForBooleanPairCommand(CctableContextName, label, inputLabel,
                            enableCommand, disableCommand, notes, inputId, inputLocation);
                        break;
                }
                return Response.AsRedirect("/system");
            };

            Get["/system/auth/disable"] = x => {
                CoreParametersConfig.DisableT2Fa();
                return Response.AsJson(true);
            };

            Get["/system/auth/enable"] = x => {
                CoreParametersConfig.EnableT2Fa();
                return Response.AsJson(true);
            };

            Get["/system/mounts"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["_page-system-mounts", vmod];
            };

            Post["/system/mount/unit"] = x => {
                var guid = Request.Form.Guid;
                string unit = Request.Form.Unit;
                var unitsSplit = unit.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                if (unitsSplit.Length > 0) {
                    MountRepository.AddUnit(guid, unitsSplit);
                }
                return Response.AsRedirect("/system/mounts");
            };

            Delete["/system/mount/unit"] = x => {
                var guid = Request.Form.Guid;
                var unit = Request.Form.Unit;
                MountRepository.RemoveUnit(guid, unit);
                return Response.AsJson(true);
            };

            Get["/system/sysctl"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.Sysctl = VhStatus.Sysctl(Sysctl.Stock, Sysctl.Running, Sysctl.Antd);
                return View["_page-system-sysctl", vmod];
            };

            Post["/system/sysctl/{param}/{Value}"] = x => {
                string param = x.param;
                string value = x.value;
                var output = Sysctl.Config(param, value);
                return Response.AsJson(output);
            };

            Post["/system/import/info"] = x => {
                SystemInfo.Import();
                return Response.AsJson(true);
            };
        }
    }
}