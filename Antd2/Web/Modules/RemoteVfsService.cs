using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class RemoteVfsService : NancyModule {

        public RemoteVfsService() : base("/remotevfs") {

            Post["/set"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<RemoteVfsServer>(data);
                Application.CurrentConfiguration.Storage.Server = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };
        }
    }
}