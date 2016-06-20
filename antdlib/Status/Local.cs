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
using antdlib.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.common;

namespace antdlib.Status {
    public class Local {
        private static string GetSystemVersion() {
            var sq = Terminal.Execute("losetup | grep /dev/loop0");
            var sqSplt = sq.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (sqSplt.Length <= 1)
                return "";
            var file = sqSplt[sqSplt.Length - 1];
            return new Regex("_.+_").Matches(file)[0].Value.Replace("_", "");
        }

        public static string SystemVersion => GetSystemVersion();

        private static IEnumerable<SystemComponentModel> GetActiveSystemComponents() {
            var list = new List<SystemComponentModel>();
            var activeLinkData = Terminal.Execute($"find {Parameter.Repo} -type l | grep active");
            ConsoleLogger.Warn(activeLinkData);
            var activeLinks = activeLinkData.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (var link in activeLinks) {
                ConsoleLogger.Warn(link);
                var linkInfoData = Terminal.Execute($"file {link}");
                ConsoleLogger.Warn(linkInfoData);
                var linkInfos = linkInfoData.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var sc = new SystemComponentModel() {
                    active = linkInfos[0].Replace(":", "").Trim(),
                    link = linkInfos[1].Replace("symbolic link to", "").Trim()
                };
                list.Add(sc);
            }
            return list;
        }

        public static IEnumerable<SystemComponentModel> ActiveSystemComponents => GetActiveSystemComponents();
    }
}
