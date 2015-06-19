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

using Antd.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd {

    public class Meminfo {

        public static string GetText() {
            string meminfoContent = "";
            meminfoContent = FileSystem.ReadFile("/proc/meminfo");
            string meminfoJson = JsonConvert.SerializeObject(meminfoContent);
            return meminfoJson;
        }

        public static List<MeminfoModel> GetModel() {
            string meminfoContent = "";
            meminfoContent = FileSystem.ReadFile("/proc/meminfo");
            var meminfo = ConvertMeminfo(meminfoContent);
            return meminfo;
        }

        public static List<MeminfoModel> ConvertMeminfo(string meminfoText) {
            List<MeminfoModel> meminfoList = new List<MeminfoModel>();
            string[] rowDivider = new String[] { "\n" };
            string[] cellDivider = new String[] { ": " };
            string[] rowList = meminfoText.Split(rowDivider, StringSplitOptions.None).ToArray();
            foreach (string row in rowList) {
                if (!string.IsNullOrEmpty(row)) {
                    string[] cellList = row.Split(cellDivider, StringSplitOptions.None).ToArray();
                    MeminfoModel meminfo = new MeminfoModel {
                        key = cellList[0],
                        value = cellList[1]
                    };
                    meminfoList.Add(meminfo);
                }
            }
            return meminfoList;
        }
    }
}
