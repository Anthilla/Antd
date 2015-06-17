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

namespace Antd.Boot {

    public class CoreParametersConfig {
        private static string coreFileName = "antdConfig";

        private static string[] _files = new string[] {
                coreFileName + "Current",
                coreFileName + "001",
                coreFileName + "002"
            };

        private static XmlWriter xmlWriter = new XmlWriter(_files, "config");

        public static void WriteDefaults() {
            string root;
            xmlWriter.Write("root", "/antd");
            root = xmlWriter.ReadValue("root");

            if (xmlWriter.CheckValue("antdport") == false) {
                xmlWriter.Write("antdport", "7777");
            }

            if (xmlWriter.CheckValue("antddb") == false) {
                xmlWriter.Write("antddb", "/Data/Data01Vol01/antdData");
            }

            if (xmlWriter.CheckValue("antdfr") == false) {
                xmlWriter.Write("antdfr", "/Data/Data01Vol01/antdData");
            }

            if (xmlWriter.CheckValue("sysd") == false) {
                xmlWriter.Write("sysd", "/etc/systemd/system");
            }
        }

        public static string GetAntdPort() {
            return xmlWriter.ReadValue("antdport");
        }

        public static string GetAntdDb() {
            return xmlWriter.ReadValue("antddb");
        }

        public static string GetAntdRepo() {
            return xmlWriter.ReadValue("antdfr");
        }

        public static string GetUnitDir() {
            return xmlWriter.ReadValue("sysd");
        }

        public static string GetAntdUri() {
            if (xmlWriter.CheckValue("antddb") == false) {
                return "http://+:7777/";
            }
            else {
                var port = xmlWriter.ReadValue("antdport");
                return "http://+:" + port + "/";
            }
        }
    }
}