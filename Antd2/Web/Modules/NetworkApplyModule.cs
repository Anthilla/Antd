using Nancy;

namespace Antd2.Modules {
    public class NetworkApplyModule : NancyModule {

        public NetworkApplyModule() : base("/network") {

            //Dns.Set();
            Post("/apply/knowndns", x => HttpStatusCode.OK);

            //Dns.Set();
            Post("/apapply/knownhostsply", x => HttpStatusCode.OK);

            //Dns.Set();
            Post("/apply/knownnetworks", x => HttpStatusCode.OK);

            Post("/apply/internalnetwork", x => HttpStatusCode.OK);

            Post("/apply/externalnetwork", x => HttpStatusCode.OK);

            //cmds.Network.Set();
            Post("/apply", x => HttpStatusCode.OK);

            //Route.SetRoutingTable();
            Post("/apply", x => HttpStatusCode.OK);

            //Route.Set();
            Post("/apply", x => HttpStatusCode.OK);

            //cmds.Network.Set();
            Post("/apply", x => HttpStatusCode.OK);

            //cmds.Network.SetTuns();
            Post("/apply/tuns", x => HttpStatusCode.OK);

            //cmds.Network.SetTaps();
            Post("/apply/taps", x => HttpStatusCode.OK);
        }
    }
}