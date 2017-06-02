using antdlib.config.shared;
using antdlib.models;
using System.Collections.Generic;
using System.IO;
using System.Text;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace Antd.License {
    public class LicenseManagement {

        private readonly string _licensePath = $"{Parameter.AntdCfg}/license.lic";
        private readonly ApiConsumer _api = new ApiConsumer();

        public void Download(string appName, MachineIdsModel machineUid, byte[] publicKey) {
            if(File.Exists(_licensePath))
                return;
            var cloudaddress = new AppConfiguration().Get().CloudAddress;
            if(string.IsNullOrEmpty(cloudaddress)) {
                return;
            }
            if(cloudaddress.Contains("localhost"))  {
                return;
            }
            if(!cloudaddress.EndsWith("/")) {
                cloudaddress = cloudaddress + "/";
            }
            var pk = Encoding.ASCII.GetString(publicKey);
            var dict = new Dictionary<string, string> {
                { "AppName", appName },
                { "PartNumber", machineUid.PartNumber },
                { "SerialNumber", machineUid.SerialNumber },
                { "Uid", machineUid.MachineUid },
                { "PublicKey", pk}
            };
            var lic = _api.Post<string>($"{cloudaddress}license/create", dict);
            if(lic != null) {
                FileWithAcl.WriteAllText(_licensePath, lic, "644", "root", "wheel");
            }
        }

        public ResponseLicenseStatusModel Check(string appName, MachineIdsModel machineUid, byte[] publicKey) {
            var cloudaddress = new AppConfiguration().Get().CloudAddress;
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
                { "PartNumber", machineUid.PartNumber },
                { "SerialNumber", machineUid.SerialNumber },
                { "Uid", machineUid.MachineUid },
                { "PublicKey", pk }
            };
            var status = _api.Post<ResponseLicenseStatusModel>($"{cloudaddress}license/check", dict);
            return status;
        }
    }
}
