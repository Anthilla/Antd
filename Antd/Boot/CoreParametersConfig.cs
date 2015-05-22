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

namespace Antd.Boot {

    public class CoreParametersConfig {
        private static string cfgAlias = "antdControlSet";

        private static string[] paths = new string[] {
                cfgAlias + "Current",
                cfgAlias + "001",
                cfgAlias + "002"
            };

        private static Anth_ParamWriter config = new Anth_ParamWriter(paths, "antdCore");

        public static void WriteDefaults() {
            string root;
            config.Write("root", "/antd");
            root = config.ReadValue("root");

            if (config.CheckValue("antdport") == false) {
                config.Write("antdport", "7777");
            }

            if (config.CheckValue("antddb") == false) {
                config.Write("antddb", "/Data/Data01Vol01/antdData");
            }

            if (config.CheckValue("antdfr") == false) {
                config.Write("antdfr", "/Data/Data01Vol01/antdData");
            }

            if (config.CheckValue("sysd") == false) {
                config.Write("sysd", "/etc/systemd/system");
            }
        }

        public static string GetAntdPort() {
            return config.ReadValue("antdport");
        }

        public static string GetAntdDb() {
            return config.ReadValue("antddb");
        }

        public static string GetAntdRepo() {
            return config.ReadValue("antdfr");
        }

        public static string GetUnitDir() {
            return config.ReadValue("sysd");
        }

        public static string GetAntdUri() {
            if (config.CheckValue("antddb") == false) {
                return "http://+:7777/";
            }
            else {
                var port = config.ReadValue("antdport");
                return "http://+:" + port + "/";
            }
        }
    }
}