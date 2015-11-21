using System;
using System.Dynamic;
using System.Linq;
using antdlib.CCTable;
using antdlib.Common;
using antdlib.Log;
using antdlib.Terminal;
using Nancy;
using Nancy.Security;
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

namespace Antd.Modules {

    public class CcTableModule : NancyModule {

        public CcTableModule()
            : base("/cctable") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.list = CCTableRepository.GetAll();
                return View["_page-cctable", vmod];
            };

            Post["/"] = x => {
                var tbl = (string)Request.Form.Alias;
                var context = (string)Request.Form.Context;
                var tblType = (string)Request.Form.TableType;
                if (tbl.RemoveWhiteSpace().Length > 0) {
                    CCTableRepository.CreateTable(tbl, tblType, context);
                }
                var redirect = context.RemoveWhiteSpace().Length > 0 ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/row"] = x => {
                var tableGuid = (string)Request.Form.TableGuid;
                var tableName = (string)Request.Form.TableName;
                var label = (string)Request.Form.Label;
                var inputType = (string)Request.Form.InputType.Value;
                var inputLabel = (string)Request.Form.InputLabel;
                var notes = (string)Request.Form.Notes;
                var osi = (string)Request.Form.FlagOSI.Value;
                var func = (string)Request.Form.FlagFunction.Value;
                var vOsi = CCTableRepository.GetOsiLevel(osi);
                var vFunc = CCTableRepository.GetCommandFunction(func);
                var inputId = "New" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + label.UppercaseAllFirstLetters().RemoveWhiteSpace();
                string inputLocation = "CCTable" + Request.Form.TableName;
                string command;
                var commandGet = (string)Request.Form.InputCommand;
                switch (inputType) {
                    case "hidden":
                        command = Request.Form.CCTableCommandNone;
                        CCTableRepository.CreateRowForDirectCommand(tableGuid, tableName, label, inputLabel, command, notes, vOsi, vFunc, inputId, inputLocation);
                        break;
                    case "text":
                        command = Request.Form.CCTableCommandText;
                        CCTableRepository.CreateRowForTextInputCommand(tableGuid, tableName, label, inputLabel, command, commandGet, notes, vOsi, vFunc, inputId, inputLocation);
                        break;
                    case "checkbox":
                        string commandTrue = Request.Form.CCTableCommandBooleanTrue;
                        string commandFalse = Request.Form.CCTableCommandBooleanFalse;
                        CCTableRepository.CreateRowForBooleanPairCommand(tableGuid, tableName, label, inputLabel, commandTrue, commandFalse, notes, vOsi, vFunc, inputId, inputLocation);
                        break;
                }

                var context = (string)Request.Form.Context;
                var redirect = context.RemoveWhiteSpace().Length > 0 ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/row/dataview"] = x => {
                var table = (string)Request.Form.TableGuid;
                var tableName = (string)Request.Form.TableName;
                var label = (string)Request.Form.Label;

                var commandString = (string)Request.Form.Command;
                var resultString = (string)Request.Form.Result;
                ConsoleLogger.Log(commandString);
                if (commandString != "") {
                    var thisResult = resultString == "" ? Terminal.Execute(commandString) : resultString;
                    CCTableRepository.CreateRowDataView(table, tableName, label, commandString, thisResult);
                }
                ConsoleLogger.Log(commandString);

                var context = (string)Request.Form.Context;
                var redirect = context.RemoveWhiteSpace().Length > 0 ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/row/mapdata"] = x => {
                var rowGuid = (string)Request.Form.ItemGuid;
                var labelArray = (string)Request.Form.MapLabel;
                var indexArray = (string)Request.Form.MapLabelIndex;
                CCTableRepository.SaveMapData(rowGuid, labelArray, indexArray);
                var context = (string)Request.Form.Context;
                var redirect = context.RemoveWhiteSpace().Length > 0 ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/row/refresh"] = x => {
                var guid = (string)Request.Form.Guid;
                CCTableRepository.Refresh(guid);
                return Response.AsJson(true);
            };

            Get["/delete/table/{guid}"] = x => {
                string guid = x.guid;
                CCTableRepository.DeleteTable(guid);
                return Response.AsJson("CCTable deleted");
            };

            Get["/delete/row/{guid}"] = x => {
                string guid = x.guid;
                CCTableRepository.DeleteTableRow(guid);
                return Response.AsJson("CCTable Row deleted");
            };

            Get["/edit/row/{guid}/{cmd*}"] = x => {
                string guid = x.guid;
                string cmd = x.cmd;
                CCTableRepository.EditTableRow(guid, cmd);
                return Response.AsJson("CCTable Row deleted");
            };

            Post["/Launch"] = x => {
                var commandType = (string)Request.Form.Type;
                var rowGuid = (string)Request.Form.RowGuid;
                var newValue = (string)Request.Form.NewValue;
                var boolSelected = (string)Request.Form.BoolSelected;

                var row = CCTableRepository.GetRow(rowGuid);

                switch (commandType) {
                    case "direct":
                        Terminal.Execute(row.CommandDirect);
                        break;
                    case "text":
                        Terminal.Execute(row.CommandSet.Replace("{Value}", newValue));
                        break;
                    case "bool":
                        if (boolSelected == "true") {
                            Terminal.Execute(row.CommandTrue);
                        }
                        else if (boolSelected == "false") {
                            Terminal.Execute(row.CommandFalse);
                        }
                        else {
                            Terminal.Execute("echo COMMAND NOT FOUND");
                        }
                        break;
                }

                return Response.AsJson(true);
            };

            Post["/row/conf"] = x => {
                var table = (string)Request.Form.TableGuid;
                var tableName = (string)Request.Form.TableName;
                var file = (string)Request.Form.File;

                var type = file.EndsWith(".conf") ? CCTableFlags.ConfType.File : CCTableFlags.ConfType.Directory;
                if (file != "") {
                    CCTableRepository.CreateRowConf(table, tableName, file, type);
                }
                var context = (string)Request.Form.Context;
                var redirect = context.RemoveWhiteSpace().Length > 0 ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Get["/conf/files"] = x => Response.AsJson(CCTableRepository.GetEtcConfs());

            Post["/update/conf"] = x => {
                var file = (string)Request.Form.FileName;
                var text = (string)Request.Form.FileText;
                CCTableRepository.UpdateConfFile(file, text);
                var context = (string)Request.Form.Context;
                var redirect = context.RemoveWhiteSpace().Length > 0 ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/map/conf"] = x => {
                var guid = Guid.NewGuid().ToString();
                var commentInput = ((string)Request.Form.CharComment).ToCharArray();
                var comment = commentInput.Length > 0 ? commentInput[0] : ' ';
                var filePath = (string)Request.Form.FilePath;
                bool hasInclude = Request.Form.PermitsInclude.HasValue;
                var include = (string)Request.Form.VerbInclude;
                bool hasSection = Request.Form.PermitsSection.HasValue;
                var sectionOpenInput = ((string)Request.Form.CharSectionOpen).ToCharArray();
                var sectionOpen = sectionOpenInput.Length > 0 ? sectionOpenInput[0] : ' ';
                var sectionCloseInput = ((string)Request.Form.CharSectionClose).ToCharArray();
                var sectionClose = sectionCloseInput.Length > 0 ? sectionCloseInput[0] : ' ';
                var dataSeparatorInput = ((string)Request.Form.CharKevValueSeparator).ToCharArray();
                var dataSeparator = dataSeparatorInput.Length > 0 ? dataSeparatorInput[0] : ' ';
                bool hasBlock = Request.Form.PermitsBlock.HasValue;
                var blockOpenInput = ((string)Request.Form.CharBlockOpen).ToCharArray();
                var blockOpen = blockOpenInput.Length > 0 ? blockOpenInput[0] : ' ';
                var blockCloseInput = ((string)Request.Form.CharBlockClose).ToCharArray();
                var blockClose = blockCloseInput.Length > 0 ? blockCloseInput[0] : ' ';
                var endOfLineInput = ((string)Request.Form.CharEndOfLine).ToCharArray();
                var endOfLine = endOfLineInput.Length > 0 ? endOfLineInput[0] : '\n';

                CCTableConf.Mapping.Repository.Create(guid, filePath, comment, hasInclude, include, hasSection, sectionOpen, sectionClose, dataSeparator, hasBlock, blockOpen, blockClose, endOfLine);

                var number = (string)Request.Form.LineNumber;
                var numbers = number.Split(new[] { "," }, StringSplitOptions.None).ToIntArray();
                var type = (string)Request.Form.LineType;
                var types = type.Split(new[] { "," }, StringSplitOptions.None).ToArray();
                //linee e typi dovrebbero essere uguali
                if (numbers.Length == types.Length) {
                    var l = (numbers.Length + types.Length) / 2;
                    for (var i = 0; i < l; i++) {
                        var ti = CCTableConf.Mapping.Repository.ConvertToDataType(types[i]);
                        CCTableConf.Mapping.Repository.AddLine(guid, numbers[i], ti);
                    }
                }

                var context = (string)Request.Form.Context;
                var redirect = context.RemoveWhiteSpace().Length > 0 ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };
        }
    }
}