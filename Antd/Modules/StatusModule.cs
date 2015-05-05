using System.Collections.Generic;
using Nancy;
using Nancy.Security;
using System.Dynamic;
using Antd.Systemd;

namespace Antd {

    public class StatusModule : NancyModule {
        public StatusModule()
            : base("/status") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.UNITS = Units.All;
                HashSet<string> etcDirectories = new DirectoryLister("/etc", false).FullList;
                vmod.ETC = etcDirectories;
                vmod.PROCS = Antd.Sysctl.Sysctl.All;
                return View["page-status", vmod];
            };
        }
    }
}