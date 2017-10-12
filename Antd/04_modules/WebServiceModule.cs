using anthilla.core;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class WebServiceModule : NancyModule {

        public WebServiceModule() : base("/webservice") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.WebService);
            };

            Get["/running"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.WebService);
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<Antd.WebService>(data);
                Application.CurrentConfiguration.WebService = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/apply"] = x => {
                return HttpStatusCode.OK;
            };

            Post["/set/users"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SystemUser[]>(data);
                Application.CurrentConfiguration.Users.SystemUsers = objects;
                return HttpStatusCode.OK;
            };

            Post["/set/master"] = x => {
                string data = Request.Form.Data;
                data = Encryption.XHash(data);
                Application.CurrentConfiguration.WebService.MasterPassword = data;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}