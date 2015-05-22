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

using System.IO;
using Antd.Common;
using Antd.Scheduler;

namespace Antd.Boot {

    public class AntdBoot {

        public static string[] directories = new string[] { 
            "/framework/antd",
            "/framework/anthillasp",
            "/framework/anthillaas"
        };

        public static void CheckDirectories() {
            foreach (string path in directories) {
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
            }
        }

        public static void SetCoreParameters() {
            SelfConfig.WriteDefaults();
        }

        public static void StartNetworkd() {
            Antd.Status.Networkd.EnableRequiredServices();
            Antd.Status.Networkd.MountNetworkdDir();
            Antd.Status.Networkd.CreateFirstUnit();
            Antd.Status.Networkd.RestartNetworkdDir();
            ConsoleLogger.Log(Antd.Status.Networkd.StatusNetworkdDir());
        }

        public static void StartScheduler(bool loadFromDatabase) {
            JobScheduler.Start(loadFromDatabase);
        }

        public static void StartDatabase() {
            string[] databases;
            string root = SelfConfig.GetAntdDb();
            if (!Directory.Exists(root)) {
                Directory.CreateDirectory(root);
            }
            databases = new string[] { root };
            DatabaseBoot.Start(databases);
        }
    }
}