using Antd.cmds;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class NetworkModule : NancyModule {

        public NetworkModule() : base("/network") {

            Get["/primarydomain"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.PrimaryDomain);
            };

            Get["/knowndns"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.KnownDns);
            };

            Get["/knownhosts"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.KnownHosts);
            };

            Get["/knownnetworks"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.KnownNetworks);
            };

            Get["/internalnetwork"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.InternalNetwork);
            };

            Get["/externalnetwork"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.ExternalNetwork);
            };

            Get["/networkinterfaces"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.NetworkInterfaces);
            };

            Get["/routingtables"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.RoutingTables);
            };

            Get["/routing"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.Routing);
            };

            Get["/devices"] = x => {
                return JsonConvert.SerializeObject(cmds.Network.GetAllNames());
            };

            Get["/devices/addr"] = x => {
                return JsonConvert.SerializeObject(cmds.Network.GetAllLocalAddress());
            };

            Get["/interfaces"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.NetworkInterfaces);
            };

            Get["/default/hosts"] = x => {
                return JsonConvert.SerializeObject(Dns.DefaultHosts);
            };

            Get["/default/networks"] = x => {
                return JsonConvert.SerializeObject(Dns.DefaultNetworks);
            };

            Get["/tuns"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.Tuns);
            };

            Get["/taps"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Network.Taps);
            };

            Post["/save/knowndns"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<DnsClientConfiguration>(data);
                Application.CurrentConfiguration.Network.KnownDns = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/knownhosts"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<KnownHost[]>(data);
                Application.CurrentConfiguration.Network.KnownHosts = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/knownnetworks"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<KnownNetwork[]>(data);
                Application.CurrentConfiguration.Network.KnownNetworks = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/internalnetwork"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SubNetwork>(data);
                Application.CurrentConfiguration.Network.InternalNetwork = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/externalnetwork"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SubNetwork>(data);
                Application.CurrentConfiguration.Network.ExternalNetwork = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/networkinterfaces"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NetInterface[]>(data);
                Application.CurrentConfiguration.Network.NetworkInterfaces = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/routingtables"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NetRoutingTable[]>(data);
                Application.CurrentConfiguration.Network.RoutingTables = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/routing"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NetRoute[]>(data);
                Application.CurrentConfiguration.Network.Routing = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/interfaces"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NetInterface[]>(data);
                Application.CurrentConfiguration.Network.NetworkInterfaces = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/tuns"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NetTun[]>(data);
                Application.CurrentConfiguration.Network.Tuns = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/taps"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<NetTap[]>(data);
                Application.CurrentConfiguration.Network.Taps = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/apply/knowndns"] = x => {
                Dns.Set();
                return HttpStatusCode.OK;
            };

            Post["/apply/knownhosts"] = x => {
                Dns.Set();
                return HttpStatusCode.OK;
            };

            Post["/apply/knownnetworks"] = x => {
                Dns.Set();
                return HttpStatusCode.OK;
            };

            Post["/apply/internalnetwork"] = x => {
                return HttpStatusCode.OK;
            };

            Post["/apply/externalnetwork"] = x => {
                return HttpStatusCode.OK;
            };

            Post["/apply/networkinterfaces"] = x => {
                cmds.Network.Set();
                return HttpStatusCode.OK;
            };

            Post["/apply/routingtables"] = x => {
                Route.SetRoutingTable();
                return HttpStatusCode.OK;
            };

            Post["/apply/routing"] = x => {
                Route.Set();
                return HttpStatusCode.OK;
            };

            Post["/apply/interfaces"] = x => {
                cmds.Network.Set();
                return HttpStatusCode.OK;
            };

            Post["/apply/tuns"] = x => {
                cmds.Network.SetTuns();
                return HttpStatusCode.OK;
            };

            Post["/apply/taps"] = x => {
                cmds.Network.SetTaps();
                return HttpStatusCode.OK;
            };
        }
    }
}