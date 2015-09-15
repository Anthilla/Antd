
using antdlib;
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
using antdlib.CCTable;
using antdlib.CommandManagement;
using Nancy;
using Nancy.Security;
using System;
using System.Dynamic;
using System.Linq;

namespace Antd {

    public class CCTableModule : NancyModule {

        public CCTableModule()
            : base("/cctable") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.list = CCTableRepository.GetAll();
                return View["_page-cctable", vmod];
            };

            Post["/"] = x => {
                string tbl = (string)Request.Form.Alias;
                string context = (string)Request.Form.Context;
                string tblType = (string)Request.Form.TableType;
                if (tbl.RemoveWhiteSpace().Length > 0) {
                    CCTableRepository.CreateTable(tbl, tblType, context);
                }
                string redirect = (context.RemoveWhiteSpace().Length > 0) ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/row"] = x => {
                string tableGuid = (string)Request.Form.TableGuid;
                string tableName = (string)Request.Form.TableName;
                string label = (string)Request.Form.Label;
                string inputType = (string)Request.Form.InputType.Value;
                string inputLabel = (string)Request.Form.InputLabel;
                string notes = (string)Request.Form.Notes;
                string osi = (string)Request.Form.FlagOSI.Value;
                string func = (string)Request.Form.FlagFunction.Value;
                var vOSI = CCTableRepository.GetOsiLevel(osi);
                var vFUNC = CCTableRepository.GetCommandFunction(func);

                string inputId = "New" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + label.UppercaseAllFirstLetters().RemoveWhiteSpace();
                string inputLocation = "CCTable" + Request.Form.TableName;

                string command, commandTrue, commandFalse = "";
                string commandGet = (string)Request.Form.InputCommand;
                switch (inputType) {
                    case "hidden":
                        command = Request.Form.CCTableCommandNone;
                        CCTableRepository.CreateRowForDirectCommand(tableGuid, tableName, label, inputLabel, command, notes, vOSI, vFUNC, inputId, inputLocation);
                        break;
                    case "text":
                        command = Request.Form.CCTableCommandText;
                        CCTableRepository.CreateRowForTextInputCommand(tableGuid, tableName, label, inputLabel, command, commandGet, notes, vOSI, vFUNC, inputId, inputLocation);
                        break;
                    case "checkbox":
                        commandTrue = Request.Form.CCTableCommandBooleanTrue;
                        commandFalse = Request.Form.CCTableCommandBooleanFalse;
                        CCTableRepository.CreateRowForBooleanPairCommand(tableGuid, tableName, label, inputLabel, commandTrue, commandFalse, notes, vOSI, vFUNC, inputId, inputLocation);
                        break;
                    default:
                        break;
                }

                string context = (string)Request.Form.Context;
                string redirect = (context.RemoveWhiteSpace().Length > 0) ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/row/dataview"] = x => {
                string table = (string)Request.Form.TableGuid;
                string tableName = (string)Request.Form.TableName;
                string label = (string)Request.Form.Label;

                string commandString = (string)Request.Form.Command;
                string resultString = (string)Request.Form.Result;
                ConsoleLogger.Log(commandString);
                if (commandString != "") {
                    string thisResult = (resultString == "") ? Terminal.Execute(commandString) : resultString;
                    CCTableRepository.CreateRowDataView(table, tableName, label, commandString, thisResult);
                }
                ConsoleLogger.Info(commandString);

                string context = (string)Request.Form.Context;
                string redirect = (context.RemoveWhiteSpace().Length > 0) ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/row/mapdata"] = x => {
                string rowGuid = (string)Request.Form.ItemGuid;
                string result = (string)Request.Form.ItemResult;

                string labelArray = (string)Request.Form.MapLabel;
                string indexArray = (string)Request.Form.MapLabelIndex;
                CCTableRepository.SaveMapData(rowGuid, labelArray, indexArray);

                string context = (string)Request.Form.Context;
                string redirect = (context.RemoveWhiteSpace().Length > 0) ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/row/refresh"] = x => {
                string guid = (string)Request.Form.Guid;
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

            Post["/launch"] = x => {
                string commandType = (string)Request.Form.Type;
                string rowGuid = (string)Request.Form.RowGuid;
                string newValue = (string)Request.Form.NewValue;
                string boolSelected = (string)Request.Form.BoolSelected;

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
                    default:
                        break;
                }

                return Response.AsJson(true);
            };

            Post["/row/conf"] = x => {
                string table = (string)Request.Form.TableGuid;
                string tableName = (string)Request.Form.TableName;
                string file = (string)Request.Form.File;

                CCTableFlags.ConfType type;
                if (file.EndsWith(".conf")) {
                    type = CCTableFlags.ConfType.File;
                }
                else {
                    type = CCTableFlags.ConfType.Directory;
                }

                if (file != "") {
                    CCTableRepository.CreateRowConf(table, tableName, file, type);
                }

                string context = (string)Request.Form.Context;
                string redirect = (context.RemoveWhiteSpace().Length > 0) ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Get["/conf/files"] = x => {
                return Response.AsJson(CCTableRepository.GetEtcConfs());
            };

            Post["/update/conf"] = x => {
                string file = (string)Request.Form.FileName;
                string text = (string)Request.Form.FileText;
                CCTableRepository.UpdateConfFile(file, text);
                string context = (string)Request.Form.Context;
                string redirect = (context.RemoveWhiteSpace().Length > 0) ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/map/conf"] = x => {
                var guid = Guid.NewGuid().ToString();
                var commentInput = ((string)Request.Form.CharComment).ToCharArray();
                var comment = (commentInput.Length > 0) ? commentInput[0] : ' ';
                var filePath = (string)Request.Form.FilePath;
                bool hasInclude = Request.Form.PermitsInclude.HasValue;
                var include = (string)Request.Form.VerbInclude;
                bool hasSection = Request.Form.PermitsSection.HasValue;
                var sectionOpenInput = ((string)Request.Form.CharSectionOpen).ToCharArray();
                var sectionOpen = (sectionOpenInput.Length > 0) ? sectionOpenInput[0] : ' ';
                var sectionCloseInput = ((string)Request.Form.CharSectionClose).ToCharArray();
                var sectionClose = (sectionCloseInput.Length > 0) ? sectionCloseInput[0] : ' ';
                var dataSeparatorInput = ((string)Request.Form.CharKevValueSeparator).ToCharArray();
                var dataSeparator = (dataSeparatorInput.Length > 0) ? dataSeparatorInput[0] : ' ';
                bool hasBlock = Request.Form.PermitsBlock.HasValue;
                var blockOpenInput = ((string)Request.Form.CharBlockOpen).ToCharArray();
                var blockOpen = (blockOpenInput.Length > 0) ? blockOpenInput[0] : ' ';
                var blockCloseInput = ((string)Request.Form.CharBlockClose).ToCharArray();
                var blockClose = (blockCloseInput.Length > 0) ? blockCloseInput[0] : ' ';
                var endOfLineInput = ((string)Request.Form.CharEndOfLine).ToCharArray();
                var endOfLine = (endOfLineInput.Length > 0) ? endOfLineInput[0] : '\n';

                CCTableConf.Mapping.Repository.Create(guid, filePath, comment, hasInclude, include, hasSection, sectionOpen, sectionClose, dataSeparator, hasBlock, blockOpen, blockClose, endOfLine);

                var number = (string)Request.Form.LineNumber;
                var numbers = number.Split(new String[] { "," }, StringSplitOptions.None).ToIntArray();
                var type = (string)Request.Form.LineType;
                var types = type.Split(new String[] { "," }, StringSplitOptions.None).ToArray();
                //linee e typi dovrebbero essere uguali
                if (numbers.Length == types.Length) {
                    var l = (numbers.Length + types.Length) / 2;
                    for (int i = 0; i < l; i++) {
                        var ti = CCTableConf.Mapping.Repository.ConvertToDataType(types[i]);
                        CCTableConf.Mapping.Repository.AddLine(guid, numbers[i], ti);
                    }
                }

                string context = (string)Request.Form.Context;
                string redirect = (context.RemoveWhiteSpace().Length > 0) ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };
        }
    }
}