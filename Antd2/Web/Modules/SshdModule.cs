using Nancy;

namespace Antd2.Modules {
    public class SshdModule : NancyModule {

        public SshdModule() : base("/sshd") {

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());

        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<SshdModel>(data);
            //Application.CurrentConfiguration.Services.Sshd = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //Sshd.Set();
            return HttpStatusCode.OK;
        }
    }
}