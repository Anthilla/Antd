using Nancy;

namespace Antd2.Modules {
    public class NsswitchModule : NancyModule {

        public NsswitchModule() : base("/nsswitch") {

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());

        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<NsSwitch>(data);
            //Application.CurrentConfiguration.NsSwitch = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //NS.Switch.Set();
            return HttpStatusCode.OK;
        }
    }
}