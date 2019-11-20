using Antd2.Configuration;
using Antd2.Jobs;
using Nancy;
using Newtonsoft.Json;
using System.Text;

namespace Antd2.Modules {
    public class SchedulerModule : NancyModule {

        public SchedulerModule() : base("/scheduler") {

            Get("/", x => ApiGet());

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());
        }

        private dynamic ApiGet() {
            var a = ConfigManager.Config.Saved.Cron;
            var jsonString = JsonConvert.SerializeObject(a);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostSave() {
            string json = Request.Form.Data;
            var model = JsonConvert.DeserializeObject<ScheduledCommand[]>(json);
            ConfigManager.Config.Saved.Cron = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            foreach (var job in ConfigManager.Config.Saved.Cron) {
                Cron.Jobs.Add(job.Name, job.Command, job.Time);
            }
            return HttpStatusCode.OK;
        }
    }
}