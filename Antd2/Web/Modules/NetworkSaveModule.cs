using Nancy;

namespace Antd2.Modules {
    public class NetworkSaveModule : NancyModule {

        public NetworkSaveModule() : base("/network") {

            Post("/save/knownhosts", x => HttpStatusCode.OK);
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<KnownHost[]>(data);
            //Application.CurrentConfiguration.Network.KnownHosts = objects;
            //ConfigRepo.Save();

            Post("/save/knownnetworks", x => HttpStatusCode.OK);
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<KnownNetwork[]>(data);
            //Application.CurrentConfiguration.Network.KnownNetworks = objects;
            //ConfigRepo.Save();

            Post("/save/internalnetwork", x => HttpStatusCode.OK);
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<SubNetwork>(data);
            //Application.CurrentConfiguration.Network.InternalNetwork = objects;
            //ConfigRepo.Save();

            Post("/save/externalnetwork", x => HttpStatusCode.OK);
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<SubNetwork>(data);
            //Application.CurrentConfiguration.Network.ExternalNetwork = objects;
            //ConfigRepo.Save();

            Post("/save/networkinterfaces", x => HttpStatusCode.OK);
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<NetInterface[]>(data);
            //Application.CurrentConfiguration.Network.NetworkInterfaces = objects;
            //ConfigRepo.Save();

            Post("/save/routingtables", x => HttpStatusCode.OK);
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<NetRoutingTable[]>(data);
            //Application.CurrentConfiguration.Network.RoutingTables = objects;
            //ConfigRepo.Save();

            Post("/save/routing", x => HttpStatusCode.OK);
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<NetRoute[]>(data);
            //Application.CurrentConfiguration.Network.Routing = objects;
            //ConfigRepo.Save();

            Post("/save/interfaces", x => HttpStatusCode.OK);
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<NetInterface[]>(data);
            //Application.CurrentConfiguration.Network.NetworkInterfaces = objects;
            //ConfigRepo.Save();

            Post("/save/tuns", x => HttpStatusCode.OK);
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<NetTun[]>(data);
            //Application.CurrentConfiguration.Network.Tuns = objects;
            //ConfigRepo.Save();

            Post("/save/taps", x => HttpStatusCode.OK);
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<NetTap[]>(data);
            //Application.CurrentConfiguration.Network.Taps = objects;
            //ConfigRepo.Save();
        }
    }
}