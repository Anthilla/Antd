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
using System.Linq;
using antdlib.Common;

namespace antdlib.CCTable {
    public class CCTableConf {

        public enum DataType : byte {
            Boolean = 1, //specificare il tipo yes/no true/false True/False
            String = 2,
            StringArray = 3,
            None = 98,
            Other = 99
        }

        public class Mapping {
            /// <summary>
            /// Char* = definisce un carattere speciale
            /// Permits* = definisce se il file permette un certo comportamento
            /// Verb* = stringa che definisce un certo comportamento
            /// </summary>
            public class TextMapModel {
                public char CharComment { get; set; } = ' ';
                public bool PermitsInclude { get; set; }
                public string VerbInclude { get; set; } = "";
                public bool PermitsSection { get; set; }
                public char CharSectionOpen { get; set; } = ' ';
                public char CharSectionClose { get; set; } = ' ';
                public char CharKevValueSeparator { get; set; } = ' ';
                public bool PermitsBlock { get; set; }
                public char CharBlockOpen { get; set; } = ' ';
                public char CharBlockClose { get; set; } = ' ';
                public char CharEndOfLine { get; set; } = ' ';
            }

            public class LineMapModel {
                public int Number { get; set; }
                public DataType Type { get; set; }
                public Tuple<string, string> BooleanPair { get; set; }
            }

            public class FileMapModel {
                public string _Id { get; set; }
                public string Guid { get; set; }
                public string FilePath { get; set; }
                public TextMapModel TextMap { get; set; }
                public List<LineMapModel> LinesMap { get; set; } = new List<LineMapModel>();
            }

            public class Repository {

                public static void Create(string guid,
                    string filePath,
                    char comment,
                    bool hasInclude, string include,
                    bool hasSection, char sectionOpen, char sectionClose,
                    char dataSeparator,
                    bool hasBlock, char blockOpen, char blockClose,
                    char endOfLine) {
                    var textMap = new TextMapModel {
                        CharComment = comment,
                        CharKevValueSeparator = dataSeparator,
                        CharEndOfLine = endOfLine
                    };
                    if (hasInclude) {
                        textMap.PermitsInclude = true;
                        textMap.VerbInclude = include;
                    }
                    if (hasSection) {
                        textMap.PermitsSection = true;
                        textMap.CharSectionOpen = sectionOpen;
                        textMap.CharSectionClose = sectionClose;
                    }
                    if (hasBlock) {
                        textMap.PermitsBlock = true;
                        textMap.CharBlockOpen = blockOpen;
                        textMap.CharBlockClose = blockClose;
                    }
                    var map = new FileMapModel {
                        _Id = Guid.NewGuid().ToString(),
                        Guid = guid,
                        FilePath = filePath,
                        TextMap = textMap
                    };
                    DeNSo.Session.New.Set(map);
                }

                public static void AddLine(string guid, int number, DataType type, Tuple<string, string> boolPair = null) {
                    var map = DeNSo.Session.New.Get<FileMapModel>(m => m.Guid == guid).FirstOrDefault();
                    var line = new LineMapModel {
                        Number = number,
                        Type = type
                    };
                    if (type == DataType.Boolean && boolPair != null) {
                        line.BooleanPair = boolPair;
                    }
                    if (map == null)
                        return;
                    map.LinesMap.Add(line);
                    DeNSo.Session.New.Set(map);
                }

                public static FileMapModel GetMapByGuid(string guid) {
                    return DeNSo.Session.New.Get<FileMapModel>(m => m != null && m.Guid == guid).FirstOrDefault();
                }

                public static FileMapModel GetMapByFilePath(string filePath) {
                    return DeNSo.Session.New.Get<FileMapModel>(m => m != null && m.FilePath == filePath).FirstOrDefault();
                }

                public static bool CheckMapForFile(string filePath) {
                    return (GetMapByFilePath(filePath) != null);
                }

                public static DataType ConvertToDataType(string type) {
                    switch (type) {
                        case "boolean":
                            return DataType.Boolean;
                        case "string":
                            return DataType.String;
                        case "array":
                            return DataType.StringArray;
                        case "none":
                            return DataType.None;
                        default:
                            return DataType.Other;
                    }
                }
            }
        }

        public class Reading {
            public class KeyValueTypeModel {
                public string Key { get; set; }
                public string Value { get; set; }
                public DataType Type { get; set; }
            }

            public class BlockModel {
                public string Name { get; set; } = "";
                public List<KeyValueTypeModel> KeyValueList { get; set; } = new List<KeyValueTypeModel>();
            }

            public class ConfModel {
                public string _Id { get; set; }
                public string Guid { get; set; }
                public string FilePath { get; set; }
                public List<string> IncludeList { get; set; } = new List<string>();
                public List<string> SectionList { get; set; } = new List<string>();
                public List<KeyValueTypeModel> KeyValueList { get; set; } = new List<KeyValueTypeModel>();
                public List<BlockModel> BlockList { get; set; } = new List<BlockModel>();
            }

            public static void Convert(string filePath) {
                Mapping.Repository.GetMapByFilePath(filePath);
                FileSystem.ReadFile(filePath);
            }
        }
    }
}
