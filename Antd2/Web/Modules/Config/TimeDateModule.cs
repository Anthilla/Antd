using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using System.Text;

namespace Antd2.Modules {
    public class TimeDateModule : NancyModule {

        public TimeDateModule() : base("/timedate/config") {

            Get("/", x => ApiGet());

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());
        }

        private dynamic ApiGet() {
            var time = ConfigManager.Config.Saved.Time;
            time.NtpServerTxt = string.Join("\n", time.NtpServer);
            var jsonString = JsonConvert.SerializeObject(time);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostSave() {
            string json = Request.Form.Data;
            var model = JsonConvert.DeserializeObject<TimeDateParameters>(json);
            model.NtpServer = model.NtpServerTxt.Split('\n');
            ConfigManager.Config.Saved.Time = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            if (ConfigManager.Config.Saved.Time.EnableNtpSync &&
                ConfigManager.Config.Saved.Time.NtpServer.Length > 0 &&
                !string.IsNullOrEmpty(ConfigManager.Config.Saved.Time.NtpServer[0])) {

                Ntpdate.SyncFromRemoteServer(ConfigManager.Config.Saved.Time.NtpServer[0]);
            }
            if (!string.IsNullOrEmpty(ConfigManager.Config.Saved.Time.Timezone)) {
                Timedatectl.SetTimezone(ConfigManager.Config.Saved.Time.Timezone);
            }
            return HttpStatusCode.OK;
        }
    }
}