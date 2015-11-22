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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.Common;
using antdlib.Terminal;

namespace antdlib.CCTable {
    public class CCTableRepository {

        public static List<CCTableModel> GetAll() {
            var list = DeNSo.Session.New.Get<CCTableModel>(c => c != null).ToList();
            foreach (var item in list) {
                item.Content = GetRows(item.Guid);
            }
            return list;
        }

        public static List<CCTableModel> GetAllByContext(string context) {
            var list = DeNSo.Session.New.Get<CCTableModel>(c => c != null && c.Context == context).ToList();
            foreach (var item in list) {
                item.Content = GetRows(item.Guid);
            }
            return list;
        }

        public static CCTableModel GetByContext(string context) {
            var item = DeNSo.Session.New.Get<CCTableModel>(c => c != null && c.Context == context).FirstOrDefault();
            if (item != null) {
                item.Content = GetRows(item.Guid);
            }
            return item;
        }

        public static CCTableModel GetByContext2(string context) {
            var item = DeNSo.Session.New.Get<CCTableModel>(c => c != null && c.Context == context).FirstOrDefault();
            if (item != null) {
                item.Content = GetRows(context);
            }
            return item;
        }

        public static CCTableModel GetByGuid(string guid) {
            var cc = DeNSo.Session.New.Get<CCTableModel>(c => c != null && c.Guid == guid).FirstOrDefault();
            if (cc != null) {
                cc.Content = GetRows(cc.Guid);
            }
            return cc;
        }

        public static CCTableRowModel GetRow(string guid) {
            var rw = DeNSo.Session.New.Get<CCTableRowModel>(c => c != null && c.Guid == guid).FirstOrDefault();
            return rw;
        }

        public static List<CCTableRowModel> GetRows(string guid) {
            var list = DeNSo.Session.New.Get<CCTableRowModel>(c => c != null && c.TableGuid == guid).ToList();
            foreach (var i in list) {
                if (i.InputCommand == null)
                    continue;
                var f = i.InputCommand.GetFirstString();
                var a = i.InputCommand.GetAllStringsButFirst();
                if (a == null)
                    throw new ArgumentNullException(nameof(a));
                var c = a;
                i.ValueResult = Terminal.Terminal.Execute(f + " " + c);
            }
            return list;
        }

        public static void CreateTable(string alias, string type, string context) {
            var model = new CCTableModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                Alias = alias.UppercaseAllFirstLetters(),
                Context = context,
                Type = GetTableType(type)
            };
            DeNSo.Session.New.Set(model);
        }

        public static void CreateRowForDirectCommand(string tableGuid, string tableName, string label, string inputLabel, string command, string notes, CCTableFlags.OsiLevel flagOsi, CCTableFlags.CommandFunction flagFunction, string inputId, string inputLocation) {
            var model = new CCTableRowModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                NUid = Uid.ShortGuid,
                TableGuid = tableGuid,
                Label = label,
                InputType = "hidden",
                InputLabel = inputLabel,
                CommandDirect = command,
                CommandType = CCTableCommandType.Direct,
                Notes = notes,
                FlagOsi = flagOsi,
                FlagCommandFunction = flagFunction,
                CommandInputId = inputId,
                CommandInputLocation = inputLocation
            };
            model.HtmlInputId = "New" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
            model.HtmlSumbitId = "Update" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
            DeNSo.Session.New.Set(model);
        }

        public static void CreateRowForTextInputCommand(string tableGuid, string tableName, string label,
string inputLabel, string inputCommandSet, string inputCommandGet, string notes, CCTableFlags.OsiLevel flagOsi, CCTableFlags.CommandFunction flagFunction, string inputId, string inputLocation) {
            var model = new CCTableRowModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                NUid = Uid.ShortGuid,
                TableGuid = tableGuid,
                Label = label,
                InputType = "text",
                InputLabel = inputLabel,
                CommandSet = inputCommandSet,
                CommandGet = inputCommandGet,
                CommandType = CCTableCommandType.TextInput,
                Notes = notes,
                FlagOsi = flagOsi,
                FlagCommandFunction = flagFunction,
                CommandInputId = inputId,
                CommandInputLocation = inputLocation
            };
            model.HtmlInputId = "New" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
            model.HtmlSumbitId = "Update" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
            DeNSo.Session.New.Set(model);
        }

        public static void CreateRowForBooleanPairCommand(string tableGuid, string tableName, string label, string inputLabel, string inputCommandTrue, string inputCommandFalse, string notes, CCTableFlags.OsiLevel flagOsi, CCTableFlags.CommandFunction flagFunction, string inputId, string inputLocation) {
            var model = new CCTableRowModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                NUid = Uid.ShortGuid,
                TableGuid = tableGuid,
                Label = label,
                InputType = "checkbox",
                InputLabel = inputLabel,
                CommandTrue = inputCommandTrue,
                CommandFalse = inputCommandFalse,
                CommandType = CCTableCommandType.BooleanPair,
                Notes = notes,
                FlagOsi = flagOsi,
                FlagCommandFunction = flagFunction,
                CommandInputId = inputId,
                CommandInputLocation = inputLocation
            };
            model.HtmlInputId = "New" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
            model.HtmlSumbitId = "Update" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
            DeNSo.Session.New.Set(model);
        }

        public static void CreateRowDataView(string tableGuid, string tableName, string label, string inputCommand, string result) {
            var model = new CCTableRowModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                NUid = Uid.ShortGuid,
                TableGuid = tableGuid,
                Label = label,
                InputCommand = inputCommand,
                ValueResult = result,
                MapRules = new List<CCTableRowMap>(),
                HasMap = false,
                ValueResultArray = result.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray()
            };
            model.HtmlInputId = "New" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
            model.HtmlSumbitId = "Update" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
            DeNSo.Session.New.Set(model);
        }

        public static void CreateRowConf(string tableGuid, string tableName, string file, CCTableFlags.ConfType type) {
            string newPath;
            if (type == CCTableFlags.ConfType.File) {
                newPath = $"{Parameter.RepoDirs}/FILE{file.Replace("/", "_")}";
            }
            else {
                newPath = $"{Parameter.RepoDirs}/DIR{file.Replace("/", "_")}";
            }
            var model = new CCTableRowModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                NUid = Uid.ShortGuid,
                TableGuid = tableGuid,
                File = newPath,
                ConfType = type
            };
            DeNSo.Session.New.Set(model);
            if (type == CCTableFlags.ConfType.File) {
                SetConfFile(file, newPath);
            }
            else {
                SetConfDirectory(file, newPath);
            }
        }

        private static void SetConfFile(string source, string destination) {
            Terminal.Terminal.Execute($"cp {source} {destination}");
            //File.Copy(source, destination, true);
            Terminal.Terminal.Execute($"mount --bind {source} {destination}");
        }

        private static void SetConfDirectory(string source, string destination) {
            Terminal.Terminal.Execute($"cp {source} {destination}");
            FileSystem.CopyDirectory(source, destination);
            MountPoint.Mount.Dir(source);
        }

        public static void UpdateConfFile(string file, string text) {
            FileSystem.WriteFile(file, text);
        }

        public static CCTableConfModel[] GetEtcConfs() {
            var confs = Directory.EnumerateFiles("/etc", "*.conf", SearchOption.AllDirectories).Where(f => !f.Contains("portage")).ToArray();
            var files = confs.GetConfFiles();
            var dirs = confs.GetServices();
            return files.Concat(dirs).ToArray();
        }

        public static CCTableConfModel[] GetEtcConfs(string directory) {
            var list = new List<CCTableConfModel>();
            var confs = Directory.EnumerateFiles(directory, "*.conf", SearchOption.AllDirectories).Where(f => !f.Contains("portage")).ToArray();
            list.AddRange(confs.Select(conf => new CCTableConfModel {
                Name = conf.Replace("\\", "/"),
                Path = conf.Replace("\\", "/"),
                Type = CCTableFlags.ConfType.File
            }));
            return list.ToArray();
        }

        public static void DeleteTable(string guid) {
            var cc = DeNSo.Session.New.Get<CCTableModel>(c => c != null && c.Guid == guid).FirstOrDefault();
            DeNSo.Session.New.Delete(cc);
        }

        public static void DeleteTableRow(string guid) {
            var cc = DeNSo.Session.New.Get<CCTableRowModel>(c => c != null && c.Guid == guid).FirstOrDefault();
            DeNSo.Session.New.Delete(cc);
        }

        public static void EditTableRow(string guid, string command) {
            var row = DeNSo.Session.New.Get<CCTableRowModel>(c => c != null && c.Guid == guid).FirstOrDefault();
            if (row == null)
                return;
            var i = row.HtmlInputId;
            CommandRepository.Edit(i, command);
        }

        public static void Refresh(string guid) {
            var row = DeNSo.Session.New.Get<CCTableRowModel>(c => c != null && c.Guid == guid).FirstOrDefault();
            if (row != null) {
                var command = row.InputCommand;
                var result = Terminal.Terminal.Execute(command);
                row.ValueResult = result;
                row.ValueResultArray = result.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
            DeNSo.Session.New.Set(row);
        }

        public static void SaveMapData(string rowGuid, string labelArray, string indexArray) {
            var r = DeNSo.Session.New.Get<CCTableRowModel>(c => c != null && c.Guid == rowGuid).FirstOrDefault();
            var labelArraySplit = labelArray.Split(new[] { "," }, StringSplitOptions.None);
            var indexArraySplit = indexArray.Split(new[] { "," }, StringSplitOptions.None);
            for (var i = 0; i < labelArraySplit.Length; i++) {
                var map = new CCTableRowMap {
                    MapLabel = labelArraySplit[i],
                    MapIndex = indexArraySplit[i].Split(new[] { ";" }, StringSplitOptions.None).ToIntArray()
                };
                r?.MapRules.Add(map);
            }
            if (r == null)
                return;
            r.HasMap = true;
            DeNSo.Session.New.Set(r);
        }

        public static IEnumerable<CCTableRowMapped> MapData(string[] result, List<CCTableRowMap> mapList) {
            var resultArray = result;
            return (from map in mapList
                    let z = map.MapIndex.Aggregate("", (current, i) => current + (resultArray[i] + " "))
                    select new CCTableRowMapped {
                        Key = map.MapLabel,
                        Value = z
                    });
        }

        public static CCTableFlags.CommandFunction GetCommandFunction(string src) {
            switch (Convert.ToInt32(src)) {
                case 0:
                    return CCTableFlags.CommandFunction.Stable;
                case 1:
                    return CCTableFlags.CommandFunction.Testing;
                default:
                    return CCTableFlags.CommandFunction.None;
            }
        }

        public static CCTableFlags.OsiLevel GetOsiLevel(string src) {
            switch (Convert.ToInt32(src)) {
                case 1:
                    return CCTableFlags.OsiLevel.Physical;
                case 2:
                    return CCTableFlags.OsiLevel.DataLink;
                case 3:
                    return CCTableFlags.OsiLevel.Network;
                case 4:
                    return CCTableFlags.OsiLevel.Transport;
                case 5:
                    return CCTableFlags.OsiLevel.Session;
                case 6:
                    return CCTableFlags.OsiLevel.Presentation;
                case 7:
                    return CCTableFlags.OsiLevel.Application;
                default:
                    return CCTableFlags.OsiLevel.None;
            }
        }

        public static CCTableFlags.TableType GetTableType(string src) {
            switch (Convert.ToInt32(src)) {
                case 1:
                    return CCTableFlags.TableType.Settings;
                case 2:
                    return CCTableFlags.TableType.DataView;
                case 3:
                    return CCTableFlags.TableType.Conf;
                case 4:
                    return CCTableFlags.TableType.New;
                default:
                    return CCTableFlags.TableType.None;
            }
        }

        public class New {
            public static void CreateRowForDirectCommand(string tableName, string label, string inputLabel, string command, string notes, string inputId, string inputLocation) {
                var model = new CCTableRowModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    NUid = Uid.ShortGuid,
                    TableGuid = tableName,
                    Label = label,
                    InputType = "hidden",
                    InputLabel = inputLabel,
                    CommandDirect = command,
                    CommandType = CCTableCommandType.Direct,
                    Notes = notes,
                    CommandInputId = inputId,
                    CommandInputLocation = inputLocation
                };
                model.HtmlInputId = "New" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
                model.HtmlSumbitId = "Update" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
                DeNSo.Session.New.Set(model);
            }

            public static void CreateRowForTextInputCommand(string tableName, string label,
    string inputLabel, string inputCommandSet, string inputCommandGet, string notes, string inputId, string inputLocation) {
                var model = new CCTableRowModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    NUid = Uid.ShortGuid,
                    TableGuid = tableName,
                    Label = label,
                    InputType = "text",
                    InputLabel = inputLabel,
                    CommandSet = inputCommandSet,
                    CommandGet = inputCommandGet,
                    CommandType = CCTableCommandType.TextInput,
                    Notes = notes,
                    CommandInputId = inputId,
                    CommandInputLocation = inputLocation
                };
                model.HtmlInputId = "New" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
                model.HtmlSumbitId = "Update" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
                DeNSo.Session.New.Set(model);
            }

            public static void CreateRowForBooleanPairCommand(string tableName, string label, string inputLabel, string inputCommandTrue, string inputCommandFalse, string notes, string inputId, string inputLocation) {
                var model = new CCTableRowModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    NUid = Uid.ShortGuid,
                    TableGuid = tableName,
                    Label = label,
                    InputType = "checkbox",
                    InputLabel = inputLabel,
                    CommandTrue = inputCommandTrue,
                    CommandFalse = inputCommandFalse,
                    CommandType = CCTableCommandType.BooleanPair,
                    Notes = notes,
                    CommandInputId = inputId,
                    CommandInputLocation = inputLocation
                };
                model.HtmlInputId = "New" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
                model.HtmlSumbitId = "Update" + tableName.UppercaseAllFirstLetters().RemoveWhiteSpace() + model.Label.UppercaseAllFirstLetters().RemoveWhiteSpace();
                DeNSo.Session.New.Set(model);
            }
        }
    }
}
