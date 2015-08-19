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

using System.Collections.Generic;

namespace antdlib.CCTable {

    public class CCTableFlags {
        public enum TableType {
            Settings = 1,
            DataView = 2,
            Conf = 3,
            None = 99
        }

        public enum OsiLevel {
            Physical = 1,
            DataLink = 2,
            Network = 3,
            Transport = 4,
            Session = 5,
            Presentation = 6,
            Application = 7,
            None = 99
        }

        public enum CommandFunction {
            Stable = 0,
            Testing = 1,
            None = 99
        }

        public enum ConfType : byte {
            File = 0,
            Directory = 1
        }
    }

    public class CCTableModel {
        public string _Id { get; set; }

        public string Guid { get; set; }

        public string Alias { get; set; }

        public string Context { get; set; }

        public CCTableFlags.TableType Type { get; set; }

        public List<CCTableRowModel> Content { get; set; }

        public List<CCTableRowModel> DataViewContent { get; set; }
    }

    public class CCTableRowModel {
        public string _Id { get; set; }

        public string Guid { get; set; }

        public string TableGuid { get; set; }

        public string NUid { get; set; }

        public string Label { get; set; }

        public string File { get; set; }

        public string InputType { get; set; }

        public string InputLabel { get; set; }

        public string InputCommand { get; set; }

        public string ValueResult { get; set; }

        public string[] ValueResultArray { get; set; }

        public string Notes { get; set; }

        public string HtmlInputID { get; set; }

        public string HtmlSumbitID { get; set; }

        public CCTableFlags.OsiLevel FlagOsi { get; set; }

        public CCTableFlags.CommandFunction FlagCommandFunction { get; set; }

        public List<CCTableRowMap> MapRules { get; set; }

        public bool HasMap { get; set; }
    }

    public class CCTableRowMap {
        public string MapLabel { get; set; }

        public int[] MapIndex { get; set; }
    }

    public class CCTableRowMapped {
        public string Key { get; set; }

        public string Value { get; set; }
    }

    public class CCTableConfModel {
        public string Name { get; set; }
        
        public string Path { get; set; }

        public CCTableFlags.ConfType Type { get; set; }
    }
}
