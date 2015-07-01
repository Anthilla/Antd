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

using Antd.CCTable;
using Antd.CommandManagement;
using Nancy;
using Nancy.Security;
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
                if (tbl != "") {
                    CCTableRepository.CreateTable(tbl, context);
                }
                return Response.AsRedirect("/cctable");
            };

            Post["/row"] = x => {
                string table = (string)this.Request.Form.TableGuid;
                string tableName = (string)this.Request.Form.TableName;
                string label = (string)this.Request.Form.Label;
                string inputType = (string)this.Request.Form.InputType.Value;
                string inputValue = (string)this.Request.Form.InputLabel;
                string inputCommand = (string)this.Request.Form.InputCommand;
                string notes = (string)this.Request.Form.Notes;
                string osi = (string)this.Request.Form.FlagOSI.Value;
                string func = (string)this.Request.Form.FlagFunction.Value;
                CCTableRepository.CreateRow(table, tableName, label, inputType, inputValue, inputCommand, 
                    notes, CCTableRepository.GetOsiLevel(osi), CCTableRepository.GetCommandFunction(func));

                string commandNone = this.Request.Form.CCTableCommandNone;
                string commandText = this.Request.Form.CCTableCommandText;
                string commandBoolean = this.Request.Form.CCTableCommandBoolean;



                //string inputid = "New" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + label.UppercaseAllFirstLetters().RemoveWhiteSpace();
                //string inputlocation = "CCTable" + this.Request.Form.TableName;
                //CommandDB.Create(inputid, command, command, inputlocation, notes);

                return Response.AsRedirect("/cctable");
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
        }
    }
}