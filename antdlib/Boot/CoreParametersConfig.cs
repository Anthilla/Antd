
using antdlib;
using System;
using System.IO;
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

namespace antdlib.Boot {

    public class CoreParametersConfig {
        private static string coreFileName = "antdConfig";
        private static string[] _files = new string[] {
                coreFileName + "Current",
                coreFileName + "001",
                coreFileName + "002"
            };

        public static XmlWriter xmlWriter = new XmlWriter(_files);

        public static void WriteDefaults() {
            if (File.Exists(_files[0])) {
                xmlWriter.Write(Label.Root, Folder.Root);
                xmlWriter.ReadValue(Label.Root);
                if (xmlWriter.CheckValue(Label.Port) == false) {
                    xmlWriter.Write(Label.Port, Port.Antd);
                }
                if (xmlWriter.CheckValue(Label.Database) == false) {
                    xmlWriter.Write(Label.Database, Folder.Database);
                }
                //if (xmlWriter.CheckValue(Label.Files) == false) {
                //    xmlWriter.Write(Label.Files, Folder.FileRepository);
                //}
            }
        }

        public static string GetPort() {
            try {
                return (xmlWriter.CheckValue(Label.Port) == true) ? xmlWriter.ReadValue(Label.Port) : Port.Antd;
            }
            catch (Exception ex) {
                return "7777";
            }
        }

        public static string GetDb() {
            try {
                return (xmlWriter.CheckValue(Label.Database) == true) ? xmlWriter.ReadValue(Label.Database) : Folder.Database;
            }
            catch (Exception ex) {
                return "/Data/antd";
            }
        }

        //public static string GetFileRepo() {
        //    return (xmlWriter.CheckValue(Label.Files) == true) ? xmlWriter.ReadValue(Label.Files) : Folder.FileRepository;
        //}

        public static string GetHostUri() {
            try {
                if (xmlWriter.CheckValue(Label.Port) == false) {
                    return Uri.Antd;
                }
                else {
                    var port = xmlWriter.ReadValue(Label.Port);
                    return "http://+:" + port + "/";
                }
            }
            catch (Exception ex) {
                return "http://+:7777/";
            }
        }
    }
}