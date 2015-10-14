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
using System.Collections.Generic;
using System.Linq;

namespace antdlib.CCTable {
    public class Flag {
        public CCTableFlags.OsiLevel Osi { get; set; }

        public CCTableFlags.CommandFunction Status { get; set; }
    }

    public enum Type : byte {
        Direct = 0,
        GetSet = 1,
        GetSetWithList = 2,
        Bool = 3,
        Other = 99
    }

    public class Lines {
        public IEnumerable<string> LinesForDirectInput { get; set; } = new List<string>() { };

        public IEnumerable<string> LinesForGetValue { get; set; } = new List<string>() { };

        public IEnumerable<string> LinesForSetValue { get; set; } = new List<string>() { };

        public IEnumerable<string> LinesForBoolTrue { get; set; } = new List<string>() { };

        public IEnumerable<string> LinesForBoolFalse { get; set; } = new List<string>() { };

        public IEnumerable<string> ListValues { get; set; } = new List<string>() { };
    }

    public class CCTableImportModel {
        public string Context { get; set; }

        public string Table { get; set; }

        public string TableGuid { get; set; } = string.Empty;

        public string Label { get; set; }

        public string Guid { get; set; }

        public Type Type { get; set; }

        public Lines Lines { get; set; }

        public Flag Flags { get; set; }

        public string Notes { get; set; }
    }

    public class CCTableImport {

        public class FromDatabase {
        }

        public class FromJson {
            public static void Save() {


            }
        }
    }
}
