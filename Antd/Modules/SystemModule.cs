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
using antdlib.MountPoint;
using antdlib.Status;
using Antd.ViewHelpers;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class SystemModule : NancyModule {
        private const string CctableContextName = "system";

        public SystemModule()
            : base("/system") {
            this.RequiresAuthentication();

            Before += x => {
                if (CCTableRepository.GetByContext(CctableContextName) == null) {
                    CCTableRepository.CreateTable("System Configuration", "4", CctableContextName);
                }
                return null;
            };

            Post["/cctable"] = x => {
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

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.SSHPort = "22";
                vmod.AuthStatus = antdlib.Auth.T2FA.Config.IsEnabled;

                vmod.SslStatus = "Enabled";
                vmod.SslStatusAction = "Disable";
                if (CoreParametersConfig.GetSsl() == "no") {
                    vmod.SslStatus = "Disabled";
                    vmod.SslStatusAction = "Enable";
                }
                vmod.CertificatePath = CoreParametersConfig.GetCertificatePath();
                vmod.CaStatus = "Enabled";
                if (CoreParametersConfig.GetCa() == "no") {
                    vmod.SslStatus = "Disabled";
                }
                vmod.Certificates = CertificateAuthority.GetAllCertificates();

                vmod.CCTableContext = CctableContextName;
                var table = CCTableRepository.GetByContext2(CctableContextName);
                vmod.CommandDirect = table.Content.Where(_ => _.CommandType == antdlib.CCTableCommandType.Direct);
                vmod.CommandText = table.Content.Where(_ => _.CommandType == antdlib.CCTableCommandType.TextInput);
                vmod.CommandBool = table.Content.Where(_ => _.CommandType == antdlib.CCTableCommandType.BooleanPair);
                return View["_page-system", vmod];
            };

            Get["/auth/disable"] = x => {
                antdlib.Auth.T2FA.Config.Disable();
                return Response.AsJson(true);
            };

            Get["/auth/enable"] = x => {
                antdlib.Auth.T2FA.Config.Enable();
                return Response.AsJson(true);
            };

            Get["/mounts"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["_page-system-mounts", vmod];
            };

            Post["/mount/unit"] = x => {
                var guid = Request.Form.Guid;
                string unit = Request.Form.Unit;
                var unitsSplit = unit.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                if (unitsSplit.Length > 0) {
                    MountRepository.AddUnit(guid, unitsSplit);
                }
                return Response.AsRedirect("/system/mounts");
            };

            Delete["/mount/unit"] = x => {
                var guid = Request.Form.Guid;
                var unit = Request.Form.Unit;
                MountRepository.RemoveUnit(guid, unit);
                return Response.AsJson(true);
            };

            Get["/sysctl"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.Sysctl = VhStatus.Sysctl(Sysctl.Stock, Sysctl.Running, Sysctl.Antd);
                return View["_page-system-sysctl", vmod];
            };

            Post["/sysctl/{param}/{value}"] = x => {
                string param = x.param;
                string value = x.value;
                var output = Sysctl.Config(param, value);
                return Response.AsJson(output);
            };

            Get["/wizard"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["page-wizard", vmod];
            };
        }
    }
}