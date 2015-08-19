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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdlib.CCTable {
    public static class CCTableExtensions {

        public static CCTableConfModel[] GetConfFiles(this string[] array) {
            var list = new HashSet<CCTableConfModel>() { };
            for (int i = 0; i < array.Length; i++) {
                var conf = array[i];
                if (array[i].Contains("\\")) {
                    conf = array[i].Replace("\\", "/");
                }
                var confNameSplit = conf.Split(new String[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                if (confNameSplit.Length < 3) { //so che è un file perché non c'è niente dopo
                    if (conf.EndsWith(".conf")) { //e so che è un file .conf
                        var m = new CCTableConfModel() {
                            Name = confNameSplit[1],
                            Path = $"/etc/{confNameSplit[1]}",
                            Type = CCTableFlags.ConfType.File
                        };
                        list.Add(m);
                    }
                }
            }
            return list.ToArray();
        }

        public static CCTableConfModel[] GetServices(this string[] array) {
            var list = new HashSet<CCTableConfModel>() { };
            for (int i = 0; i < array.Length; i++) {
                var conf = array[i];
                if (array[i].Contains("\\")) {
                    conf = array[i].Replace("\\", "/");
                }
                var confNameSplit = conf.Split(new String[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                if (confNameSplit.Length >= 3) { //so che è una cartella perché c'è altro dopo
                    if (!confNameSplit[1].EndsWith(".d")) { //e so che non è una cartella con dentro delle conf da non toccare
                        if (list.Where(l=>l.Name == confNameSplit[1]).Count() < 1) { //nella mia lista non c'è già un entry con il nome uguale (a confNameSplit[1])
                            var m = new CCTableConfModel() {
                                Name = confNameSplit[1],
                                Path = $"/etc/{confNameSplit[1]}",
                                Type = CCTableFlags.ConfType.Directory
                            };
                            list.Add(m);
                        }
                    }
                }
            }
            return list.ToArray();
        }
    }
}
