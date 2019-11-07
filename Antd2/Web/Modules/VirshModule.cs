using Antd2.cmds;
using Nancy;

namespace Antd2.Modules {
    public class VirshModule : NancyModule {

        public VirshModule() : base("/virsh") {

            Get("/", x => ApiGet());

            Post("/save", x => ApiPostSave());

            Post("/destroy", x => ApiPostDestroy());

            Post("/reboot", x => ApiPostReboot());

            Post("/reset", x => ApiPostReset());

            Post("/restore", x => ApiPostRestore());

            Post("/resume", x => ApiPostResume());

            Post("/shutdown", x => ApiPostShutdown());

            Post("/start", x => ApiPostStart());

            Post("/suspend", x => ApiPostSuspend());

            Post("/dompmsuspend", x => ApiPostDomsuspend());

            Post("/dompmwakeup", x => ApiPostDomwakeup());

        }

        private dynamic ApiGet() {
            return Response.AsJson((object)Application.RunningConfiguration.Services.Virsh);
        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<VirshModel>(data);
            //Application.CurrentConfiguration.Services.Virsh = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostDestroy() {
            string data = Request.Form.Data;
            Virsh.Destroy(data);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostReboot() {
            string data = Request.Form.Data;
            Virsh.Reboot(data);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostReset() {
            string data = Request.Form.Data;
            Virsh.Reset(data);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostRestore() {
            string data = Request.Form.Data;
            Virsh.Restore(data);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostResume() {
            string data = Request.Form.Data;
            Virsh.Resume(data);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostShutdown() {
            string data = Request.Form.Data;
            Virsh.Shutdown(data);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostStart() {
            string data = Request.Form.Data;
            Virsh.Start(data);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostSuspend() {
            string data = Request.Form.Data;
            Virsh.Suspend(data);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostDomsuspend() {
            string data = Request.Form.Data;
            Virsh.Dompmsuspend(data);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostDomwakeup() {
            string data = Request.Form.Data;
            Virsh.Dompmwakeup(data);
            return HttpStatusCode.OK;
        }

    }
}