using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Nancy.ModelBinding;

namespace Antd2.Modules {
    public class TimeDateModule : NancyModule {

        public TimeDateModule() : base("/timedate/config") {

            Get("/", x => Response.AsJson((object)ConfigManager.Config.Saved.Time));

            Get("/save", x => ApiPostSave());

            Get("/apply", x => ApiPostApply());
        }

        private dynamic ApiPostSave() {
            var model = this.Bind<TimeDateParameters>();
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