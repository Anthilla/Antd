using System.Collections.Generic;
using System.IO;
using antdlib.common;
using antdlib.models;

namespace Antd.License {
    public class LicenseManagement {

        private readonly string _licensePath = $"{Parameter.AntdCfg}/license.lic";
        private readonly ApiConsumer _api = new ApiConsumer();

        public void Download(string appName, string machineUid) {
            if(File.Exists(_licensePath))
                return;
            var dict = new Dictionary<string, string> {
                { "AppName", appName },
                { "Uid", machineUid }
            };
            var lic = _api.Post<string>($"{Parameter.Cloud}license/create", dict);
            File.WriteAllText(_licensePath, lic);
        }

        public CheckStatusModel Check(string appName, string machineUid) {
            var dict = new Dictionary<string, string> {
                { "AppName", appName },
                { "Uid", machineUid }
            };
            var status = _api.Post<CheckStatusModel>($"{Parameter.Cloud}license/check", dict);
            return status;
        }
    }
}
