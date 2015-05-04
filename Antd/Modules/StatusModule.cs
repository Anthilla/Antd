using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.Security;

namespace Antd {

    public class StatusModule : NancyModule {
        public StatusModule()
            : base("/status") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                return View["page-status"];
            };
        }
    }
}
