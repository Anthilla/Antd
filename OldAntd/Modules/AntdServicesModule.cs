using Antd.Info;
using antdlib.models;
using anthilla.commands;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using anthilla.core;

namespace Antd.Modules {
    public class AntdServicesModule : NancyModule {

        public AntdServicesModule() {
            Get["/services"] = x => {
                var model = new PageServicesModel();
                var services = MachineInfo.GetUnits("service");
                var mounts = MachineInfo.GetUnits("mount");
                var targets = MachineInfo.GetUnits("target");
                var timers = MachineInfo.GetUnits("timer");
                services.AddRange(mounts);
                services.AddRange(targets);
                services.AddRange(timers);
                model.Units = services;
                return JsonConvert.SerializeObject(model);
            };

            Get["/services/log"] = x => {
                string unit = Request.Query.unit;
                var model = CommandLauncher.Launch("journactl-service", new Dictionary<string, string> { { "$service", unit } });
                return JsonConvert.SerializeObject(model);
            };

            Post["/services/start"] = x => {
                string unit = Request.Form.Unit;
                Systemctl.Start(unit);
                return HttpStatusCode.OK;
            };

            Post["/services/restart"] = x => {
                string unit = Request.Form.Unit;
                Systemctl.Restart(unit);
                return HttpStatusCode.OK;
            };

            Post["/services/stop"] = x => {
                string unit = Request.Form.Unit;
                Systemctl.Stop(unit);
                return HttpStatusCode.OK;
            };

            Post["/services/enable"] = x => {
                string unit = Request.Form.Unit;
                Systemctl.Enable(unit);
                return HttpStatusCode.OK;
            };

            Post["/services/disable"] = x => {
                string unit = Request.Form.Unit;
                Systemctl.Disable(unit);
                return HttpStatusCode.OK;
            };
        }
    }
}
