using System.Collections.Generic;
using System.IO;
using System.Text;
using anthilla.core;
using System.Net.NetworkInformation;
using System;

namespace Antd.cmds {
    public class LicenseManagement {

        private static readonly string _licensePath = $"{Parameter.AntdCfg}/license.lic";

        public static void Download(string appName, byte[] publicKey) {
            if(File.Exists(_licensePath))
                return;
            var cloudaddress = Application.CurrentConfiguration.WebService.Cloud;
            //try {
            //    var p = new Ping();
            //    var pingReply = p.Send(cloudaddress);
            //    if(pingReply?.Status != IPStatus.Success) {
            //        ConsoleLogger.Log("[cloud] unreachable");
            //        return;
            //    }
            //}
            //catch(Exception) {
            //    ConsoleLogger.Log("[cloud] unreachable");
            //    return;
            //}

            if(string.IsNullOrEmpty(cloudaddress)) {
                return;
            }
            if(cloudaddress.Contains("localhost")) {
                return;
            }
            if(!cloudaddress.EndsWith("/")) {
                cloudaddress = cloudaddress + "/";
            }
            var pk = Encoding.ASCII.GetString(publicKey);
            var dict = new Dictionary<string, string> {
                { "AppName", appName },
                { "PartNumber", Application.CurrentConfiguration.Host.PartNumber.ToString() },
                { "SerialNumber", Application.CurrentConfiguration.Host.SerialNumber.ToString() },
                { "Uid", Application.CurrentConfiguration.Host.MachineUid.ToString() },
                { "PublicKey", pk}
            };
            var lic = ApiConsumer.Post<string>($"{cloudaddress}license/create", dict);
            if(lic != null) {
                FileWithAcl.WriteAllText(_licensePath, lic, "644", "root", "wheel");
            }
        }

        public static ResponseLicenseStatusModel Check(string appName, byte[] publicKey) {
            var cloudaddress = Application.CurrentConfiguration.WebService.Cloud;
            try {
                var p = new Ping();
                var pingReply = p.Send(cloudaddress);
                if(pingReply?.Status != IPStatus.Success) {
                    ConsoleLogger.Log("[cloud] unreachable");
                    return null;
                }
            }
            catch(Exception) {
                ConsoleLogger.Log("[cloud] unreachable");
                return null;
            }
            if(string.IsNullOrEmpty(cloudaddress)) {
                return null;
            }
            if(cloudaddress.Contains("localhost")) {
                return null;
            }
            if(!cloudaddress.EndsWith("/")) {
                cloudaddress = cloudaddress + "/";
            }
            var pk = Encoding.ASCII.GetString(publicKey);
            var dict = new Dictionary<string, string> {
                { "AppName", appName },
                { "PartNumber", Application.CurrentConfiguration.Host.PartNumber.ToString() },
                { "SerialNumber", Application.CurrentConfiguration.Host.SerialNumber.ToString() },
                { "Uid", Application.CurrentConfiguration.Host.MachineUid.ToString() },
                { "PublicKey", pk }
            };
            var status = ApiConsumer.Post<ResponseLicenseStatusModel>($"{cloudaddress}license/check", dict);
            return status;
        }
    }
}
