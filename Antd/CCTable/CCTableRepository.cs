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
using Antd.Common;
using System.Collections.Generic;
using System.Linq;

namespace Antd.CCTable {
    public class CCTableRepository {

        public static List<CCTableModel> GetAll() {
            var list = DeNSo.Session.New.Get<CCTableModel>(c => c != null).ToList();
            foreach (var item in list) {
                item.Content = GetRows(item.Guid);
            }
            return list;
        }

        public static CCTableModel GetByGuid(string guid) {
            var cc = DeNSo.Session.New.Get<CCTableModel>(c => c != null && c.Guid == guid).FirstOrDefault();
            cc.Content = GetRows(cc.Guid);
            return cc;
        }

        public static List<CCTableRowModel> GetRows(string guid) {
            var list = DeNSo.Session.New.Get<CCTableRowModel>(c => c != null && c.TableGuid == guid).ToList();
            return list;
        }

        public static void CreateTable(string alias) {
            var model = new CCTableModel();
            model._Id = Guid.NewGuid().ToString();
            model.Guid = Guid.NewGuid().ToString();
            model.Alias = alias.UppercaseAllFirstLetters();
            DeNSo.Session.New.Set(model);
        }

        public static void CreateRow(string tableGuid, string label, string inputType, string inputLabel, string notes) {
            var model = new CCTableRowModel();
            model._Id = Guid.NewGuid().ToString();
            model.Guid = Guid.NewGuid().ToString();
            model.TableGuid = tableGuid;
            model.Label = label;
            model.InputType = inputType;
            model.InputLabel = inputLabel;
            model.Notes = notes;
            model.HtmlInputID = "New" + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
            model.HtmlSumbitID = "Update" + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
            DeNSo.Session.New.Set(model);
        }

        public static void DeleteTable(string guid) {
            var cc = DeNSo.Session.New.Get<CCTableModel>(c => c != null && c.Guid == guid).FirstOrDefault();
            DeNSo.Session.New.Delete(cc);
        }

        public static void DeleteTableRow(string guid) {
            var cc = DeNSo.Session.New.Get<CCTableRowModel>(c => c != null && c.Guid == guid).FirstOrDefault();
            DeNSo.Session.New.Delete(cc);
        }
    }
}
