using Antd.Status;
using Nancy;

namespace Antd.Modules {

    public class InfoModule : NancyModule {

        public InfoModule()
            : base("/info") {
            Get["/loadaverage"] = x => {
                return Response.AsJson(Uptime.LoadAverage);
            };

            Get["/disk"] = x => {
                return Response.AsJson(Uptime.LoadAverage);
            };
        }
    }
}