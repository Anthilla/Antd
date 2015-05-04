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
                vmod.ETC = "fai cose per ottenere la lista completa di etc";
                vmod.PROCS = "non so se si deve mappare la cartella o il comando ps-aef";
                return View["page-status", vmod];
            };
        }
    }
}
