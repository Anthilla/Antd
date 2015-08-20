
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
                string tbl = (string)this.Request.Form.Alias;
                string context = (string)this.Request.Form.Context;
                string tblType = (string)this.Request.Form.TableType;
                if (tbl.RemoveWhiteSpace().Length > 0) {
                    CCTableRepository.CreateTable(tbl, tblType, context);
                }
                string redirect = (context.RemoveWhiteSpace().Length > 0) ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/row"] = x => {
                string table = (string)Request.Form.TableGuid;
                string tableName = (string)Request.Form.TableName;
                string label = (string)Request.Form.Label;
                string inputType = (string)Request.Form.InputType.Value;
                string inputValue = (string)Request.Form.InputLabel;
                string inputCommand = (string)Request.Form.InputCommand;
                string notes = (string)Request.Form.Notes;
                string osi = (string)Request.Form.FlagOSI.Value;
                string func = (string)Request.Form.FlagFunction.Value;
                CCTableRepository.CreateRow(table, tableName, label, inputType, inputValue, inputCommand,
                    notes, CCTableRepository.GetOsiLevel(osi), CCTableRepository.GetCommandFunction(func));

                string command;
                switch (inputType) {
                    case "hidden":
                        command = Request.Form.CCTableCommandNone;
                        break;
                    case "text":
                        command = Request.Form.CCTableCommandText;
                        break;
                    case "checkbox":
                        //todo: il comando in realtà è doppio, uno per true e uno per false
                        command = Request.Form.CCTableCommandBoolean;
                        break;
                    default:
                        command = "echo error during command assignment";
                        break;
                }
                ConsoleLogger.Info(command);

                string inputid = "New" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + label.UppercaseAllFirstLetters().RemoveWhiteSpace();
                string inputlocation = "CCTable" + Request.Form.TableName;
                CommandRepository.Create(inputid, command, command, inputlocation, notes);

                string context = (string)Request.Form.Context;
                string redirect = (context.RemoveWhiteSpace().Length > 0) ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };

            Post["/row/dataview"] = x => {
                string table = (string)Request.Form.TableGuid;
                string tableName = (string)Request.Form.TableName;
                string label = (string)this.Request.Form.Label;

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

            //todo: IMPORTANTE - questa api lancia un comando
            //ma è solamente un caso particolare, ovvero se c'è un input:text con un valore dentro
            //e va a sostituire il valore nel comando da lanciare
            //bisogna fare anche il comando per:
            // 1 - hidden (senza valore)
            // 2a - bool:true (input:check:checked)
            // 2b - bool:false (input:check:unchecked)
            //OVVIAMENTE bisgogna aggiustare anche gli script jquery e l'html
            //ad esempio in input:bool il valore Name è uguale sia per il true che per il false
            Post["/launch/{inputid}/{value}"] = x => {
                string inputid = (string)this.Request.Form.Input;
                string value = (string)this.Request.Form.Value;
                var r = CommandRepository.LaunchAndGetOutputUsingNewValue(inputid, value);
                return Response.AsJson(r);
            };

            Post["/row/conf"] = x => {
                string table = (string)this.Request.Form.TableGuid;
                string tableName = (string)this.Request.Form.TableName;
                string file = (string)this.Request.Form.File;

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

                string context = (string)this.Request.Form.Context;
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
                char comment = (char)Request.Form.CharComment;
                string filePath = (string)Request.Form.FilePath;
                bool hasInclude = Request.Form.PermitsInclude.HasValue;
                string include = (string)Request.Form.VerbInclude;
                bool hasSection = Request.Form.PermitsSection.HasValue;
                char sectionOpen = (char)Request.Form.CharSectionOpen;
                char sectionClose = (char)Request.Form.CharSectionClose;
                char dataSeparator = (char)Request.Form.CharKevValueSeparator;
                bool hasBlock = Request.Form.PermitsBlock.HasValue;
                char blockOpen = (char)Request.Form.CharBlockOpen;
                char blockClose = (char)Request.Form.CharBlockClose;
                char endOfLine = (char)Request.Form.CharEndOfLine;

                CCTableConf.MapRepository.Create(guid, filePath, comment, hasInclude, include, hasSection, sectionOpen, sectionClose, dataSeparator, hasBlock, blockOpen, blockClose, endOfLine);

                //qui mappare le righe di testo
                var number = (string)Request.Form.LineNumber;
                var numbers = number.Split(new String[] { "," }, StringSplitOptions.None).ToIntArray();
                var type = (string)Request.Form.LineType;
                var types = type.Split(new String[] { "," }, StringSplitOptions.None).ToArray();
                //linee e typi dovrebbero essere uguali
                if (numbers.Length == types.Length) {
                    var l = (numbers.Length + types.Length) / 2;
                    for (int i = 0; i < l; i++) {
                        var ti = CCTableConf.MapRepository.ConvertToDataType(types[i]);
                        CCTableConf.MapRepository.AddLine(guid, numbers[i], ti);
                    }
                }

                string context = (string)this.Request.Form.Context;
                string redirect = (context.RemoveWhiteSpace().Length > 0) ? context : "/cctable";
                return Response.AsRedirect(redirect);
            };
        }
    }
}