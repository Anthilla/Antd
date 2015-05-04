using System;
using System.Collections.Generic;
using System.Linq;
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
                HashSet<string> procDirectories = new DirectoryLister("/proc", false).FullList;
                vmod.PROCS.LIST = procDirectories;
                List<ProcModel> procs = Proc.All;
                vmod.PROCS.CMD = procs;
                return View["page-status", vmod];
            };
        }
    }
}
