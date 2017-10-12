using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class VirshModule : NancyModule {

        public VirshModule() : base("/virsh") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Services.Virsh);
            };

            //Get["/running"] = x => {
            //    return JsonConvert.SerializeObject(Application.RunningConfiguration.Host);
            //};

            //Post["/save"] = x => {
            //    string data = Request.Form.Data;
            //    var objects = JsonConvert.DeserializeObject<Host>(data);
            //    Application.CurrentConfiguration.Host = objects;
            //    ConfigRepo.Save();
            //    return HttpStatusCode.OK;
            //};

            //Post["/apply"] = x => {
            //    Hostnamectl.Apply();
            //    Application.RunningConfiguration.Host = Hostnamectl.Get();
            //    return HttpStatusCode.OK;
            //};

            Post["/destroy"] = x => {
                string data = Request.Form.Data;
                Virsh.Destroy(data);
                return HttpStatusCode.OK;
            };

            Post["/reboot"] = x => {
                string data = Request.Form.Data;
                Virsh.Reboot(data);
                return HttpStatusCode.OK;
            };

            Post["/reset"] = x => {
                string data = Request.Form.Data;
                Virsh.Reset(data);
                return HttpStatusCode.OK;
            };

            Post["/restore"] = x => {
                string data = Request.Form.Data;
                Virsh.Restore(data);
                return HttpStatusCode.OK;
            };

            Post["/resume"] = x => {
                string data = Request.Form.Data;
                Virsh.Resume(data);
                return HttpStatusCode.OK;
            };

            Post["/shutdown"] = x => {
                string data = Request.Form.Data;
                Virsh.Shutdown(data);
                return HttpStatusCode.OK;
            };

            Post["/start"] = x => {
                string data = Request.Form.Data;
                Virsh.Start(data);
                return HttpStatusCode.OK;
            };

            Post["/suspend"] = x => {
                string data = Request.Form.Data;
                Virsh.Suspend(data);
                return HttpStatusCode.OK;
            };

            Post["/dompmsuspend"] = x => {
                string data = Request.Form.Data;
                Virsh.Dompmsuspend(data);
                return HttpStatusCode.OK;
            };

            Post["/dompmwakeup"] = x => {
                string data = Request.Form.Data;
                Virsh.Dompmwakeup(data);
                return HttpStatusCode.OK;
            };
        }
    }
}