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
using System.Dynamic;
using System.Linq;
using antdlib;
using antdlib.Boot;
using antdlib.CCTable;
using antdlib.Certificate;
using antdlib.Config;
using antdlib.Contexts;
using antdlib.Firewall;
using antdlib.Log;
using antdlib.Network;
using antdlib.Status;
using antdlib.Terminal;
using antdlib.Users;
using Nancy.Security;

namespace Antd.Modules {
    public class HomeModule : CoreModule {
        private const string CctableContextName = "system";

        public HomeModule() {
            this.RequiresAuthentication();
            Before += x => {
                if (CCTableRepository.GetByContext(CctableContextName) == null) {
                    CCTableRepository.CreateTable("System Configuration", "4", CctableContextName);
                }
                return null;
            };

            Get["/"] = x => {
                dynamic viewModel = new ExpandoObject();

                viewModel.AntdContext = new[] {
                    "Info",
                    "Config",
                    "Network",
                    "DnsClient",
                    "Firewall",
                    "DnsServer",
                    "Proxy",
                    "Storage",
                    "Mount",
                    "Rsync",
                    "Users"
                };

                viewModel.Meminfo = Meminfo.GetMappedModel();
                viewModel.VersionOS = Terminal.Execute("uname -a");
                viewModel.VersionAOS = Terminal.Execute("cat /etc/aos-release");

                var activeKernel = Terminal.Execute("ls -l /mnt/cdrom/Kernel | grep active | awk '{print $9 \" : \" $11;}'").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                viewModel.ActiveKernel = activeKernel;
                viewModel.RecoveryKernel = Terminal.Execute("ls -l /mnt/cdrom/Kernel | grep recovery | awk '{print $9 \" : \" $11;}'").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                viewModel.ActiveSystem = Terminal.Execute("ls -l /mnt/cdrom/System | grep active | awk '{print $9 \" : \" $11;}'").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                viewModel.RecoverySystem = Terminal.Execute("ls -l /mnt/cdrom/System | grep recovery | awk '{print $9 \" : \" $11;}'").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                viewModel.Cpuinfo = Cpuinfo.Get();

                viewModel.NetworkPhysicalIf = NetworkInterface.Physical;
                viewModel.NetworkVirtualIf = NetworkInterface.Virtual;
                viewModel.NetworkBondIf = NetworkInterface.Bond;
                viewModel.NetworkBridgeIf = NetworkInterface.Bridge;

                viewModel.FirewallCommands = NfTables.GetNftCommandsBundle();

                viewModel.Mounts = antdlib.MountPoint.MountRepository.Get();

                viewModel.UserEntities = UserEntity.Repository.GetAll();

                //todo check next parameters
                viewModel.SSHPort = "22";
                viewModel.AuthStatus = CoreParametersConfig.GetT2Fa();

                viewModel.CCTableContext = CctableContextName;
                var table = CCTableRepository.GetByContext2(CctableContextName);
                viewModel.CommandDirect = table.Content.Where(_ => _.CommandType == CCTableCommandType.Direct);
                viewModel.CommandText = table.Content.Where(_ => _.CommandType == CCTableCommandType.TextInput);
                viewModel.CommandBool = table.Content.Where(_ => _.CommandType == CCTableCommandType.BooleanPair);
                return View["antd/page-antd", viewModel];
            };

            Get["/log"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.AntdContext = new[] {
                    "AntdLog",
                    "SystemLog",
                    "LogReport",
                };

                viewModel.LOGS = Logger.GetAll();
                viewModel.LogReports = Journalctl.Report.Get();
                return View["antd/page-log", viewModel];
            };

            Get["/ca"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.AntdContext = new[] {
                    "Manage",
                };

                viewModel.SslStatus = "Enabled";
                viewModel.SslStatusAction = "Disable";
                if (CoreParametersConfig.GetSsl() == "no") {
                    viewModel.SslStatus = "Disabled";
                    viewModel.SslStatusAction = "Enable";
                }
                viewModel.CertificatePath = CoreParametersConfig.GetCertificatePath();
                viewModel.CaStatus = "Enabled";
                if (CoreParametersConfig.GetCa() == "no") {
                    viewModel.CaStatus = "Disabled";
                }
                viewModel.CaIsActive = CertificateAuthority.IsActive;
                viewModel.Certificates = CertificateRepository.GetAll();

                return View["antd/page-ca", viewModel];
            };

            Get["/cfg"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.ValueBundle = ConfigManagement.GetValuesBundle();
                vmod.EnabledCommandBundle = ConfigManagement.GetCommandsBundle().Where(_ => _.IsEnabled).OrderBy(_=>_.Index);
                vmod.DisabledCommandBundle = ConfigManagement.GetCommandsBundle().Where(_ => _.IsEnabled == false).OrderBy(_ => _.Index);
                return View["antd/page-cfg", vmod];
            };
        }
    }
}