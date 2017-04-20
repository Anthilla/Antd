using antdlib.common;
using antdlib.config.shared;
using antdlib.models;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Antd.License {
    public class LicenseManagement {

        private readonly string _licensePath = $"{Parameter.AntdCfg}/license.lic";
        private readonly ApiConsumer _api = new ApiConsumer();

        public void Download(string appName, string machineUid, byte[] publicKey) {
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
                { "Uid", machineUid },
                { "PublicKey", pk}
            };
            var lic = _api.Post<string>($"{cloudaddress}license/create", dict);
            if(lic != null) {
                File.WriteAllText(_licensePath, lic);
            }
        }

        public ResponseLicenseStatusModel Check(string appName, string machineUid, byte[] publicKey) {
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
                { "Uid", machineUid },
                { "PublicKey", pk }
            };
            var status = _api.Post<ResponseLicenseStatusModel>($"{cloudaddress}license/check", dict);
            return status;
        }
    }
}
