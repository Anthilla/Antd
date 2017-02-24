using System.Collections.Generic;
using antdlib.common;
using antdlib.models;
using anthilla.commands;
using Antd.Info;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class AntdServicesModule : NancyModule {

        public AntdServicesModule() {
            Get["/services"] = x => {
                var model = new PageServicesModel();
                var machineInfo = new MachineInfo();
                var services = machineInfo.GetUnits("service");
                var mounts = machineInfo.GetUnits("mount");
                var targets = machineInfo.GetUnits("target");
                var timers = machineInfo.GetUnits("timer");
                services.AddRange(mounts);
                services.AddRange(targets);
                services.AddRange(timers);
                model.Units = services;
                return JsonConvert.SerializeObject(model);
            };

            Get["/services/log"] = x => {
                string unit = Request.Query.unit;
                var launcher = new CommandLauncher();
                var model = launcher.Launch("journactl-service", new Dictionary<string, string> { { "$service", unit } });
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
