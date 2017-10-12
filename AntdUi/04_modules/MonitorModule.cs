using anthilla.core;
using Nancy;
using Nancy.Security;

namespace AntdUi.Modules {
    public class MonitorModule : NancyModule {

        /// <summary>
        /// Ottiene le informazioni base sullo stato macchina: 
        /// - hostname
        /// - uptime
        /// - load average
        /// - memory usage
        /// - disk usage
        /// </summary>
        public MonitorModule() : base("/monitor") {
            this.RequiresAuthentication();
            Get["/"] = x => {
                return ApiConsumer.GetJson(CommonString.Append(Application.ServerUrl, Request.Path));
            };
        }
    }
}