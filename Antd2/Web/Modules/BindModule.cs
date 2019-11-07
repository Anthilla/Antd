using Nancy;

namespace Antd2.Modules {
    public class BindModule : NancyModule {

        public BindModule() : base("/bind") {

            Get("/", x => ApiGet());

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());

        }

        private dynamic ApiGet() {
            return Response.AsJson((object)Application.CurrentConfiguration.Services.Bind);
        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<BindModel>(data);
            //Application.CurrentConfiguration.Services.Bind = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //Bind.Apply();
            return HttpStatusCode.OK;
        }
    }
}