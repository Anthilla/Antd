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
using System.IO;
using antdlib.Common;

namespace antdlib.Boot {

    public class CoreParametersConfig {
        private static readonly string CoreFileName = "antdConfig";
        private static readonly string[] Files = {
                CoreFileName + "Current",
                CoreFileName + "001",
                CoreFileName + "002"
            };

        public static readonly ParameterXmlWriter Writer = new ParameterXmlWriter(Files);

        public static void WriteDefaults() {
            if (!File.Exists(Files[0]))
                return;
            Writer.Write(Label.Root, Folder.Root);
            Writer.ReadValue(Label.Root);
            if (Writer.CheckValue(Label.Port) == false) {
                Writer.Write(Label.Port, Port.Antd);
            }
            if (Writer.CheckValue(Label.Database) == false) {
                Writer.Write(Label.Database, Folder.Database);
            }
        }

        public static string GetPort() {
            try {
                return (Writer.CheckValue(Label.Port)) ? Writer.ReadValue(Label.Port) : Port.Antd;
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.Message);
                return "7777";
            }
        }

        public static string GetDb() {
            try {
                return (Writer.CheckValue(Label.Database)) ? Writer.ReadValue(Label.Database) : Folder.Database;
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.Message);
                return Folder.Database;
            }
        }

        public static string GetHostUri() {
            try {
                if (Writer.CheckValue(Label.Port) == false) {
                    return Uri.Antd;
                }
                return "http://+:" + Writer.ReadValue(Label.Port) + "/";
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.Message);
                return "http://+:7777/";
            }
        }
    }
}