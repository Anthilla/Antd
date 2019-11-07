using Nancy;

namespace Antd2.Modules {
    public class NetworkGetModule : NancyModule {

        public NetworkGetModule() : base("/network") {

            Get("/primarydomain", x => Response.AsJson((object)Application.CurrentConfiguration.Network.PrimaryDomain));

            Get("/knowndns", x => Response.AsJson((object)Application.CurrentConfiguration.Network.KnownDns));

            Get("/knownhosts", x => Response.AsJson((object)Application.CurrentConfiguration.Network.KnownHosts));

            Get("/knownnetworks", x => Response.AsJson((object)Application.CurrentConfiguration.Network.KnownNetworks));

            Get("/internalnetwork", x => Response.AsJson((object)Application.CurrentConfiguration.Network.InternalNetwork));

            Get("/externalnetwork", x => Response.AsJson((object)Application.CurrentConfiguration.Network.ExternalNetwork));

            Get("/networkinterfaces", x => Response.AsJson((object)Application.CurrentConfiguration.Network.NetworkInterfaces));

            Get("/routingtables", x => Response.AsJson((object)Application.CurrentConfiguration.Network.RoutingTables));

            Get("/routing", x => Response.AsJson((object)Application.CurrentConfiguration.Network.Routing));

            //Get("/devices", x => Response.AsJson((object)cmds.Network.GetAllNames()));

            //Get("/devices/addr", x => Response.AsJson((object)cmds.Network.GetAllLocalAddress()));

            Get("/interfaces", x => Response.AsJson((object)Application.CurrentConfiguration.Network.NetworkInterfaces));

            //Get("/default/hosts", x => Response.AsJson(Dns.DefaultHosts));

            //Get("/default/networks", x => Response.AsJson(Dns.DefaultNetworks);

            Get("/tuns", x => Response.AsJson((object)Application.CurrentConfiguration.Network.Tuns));

            Get("/taps", x => Response.AsJson((object)Application.CurrentConfiguration.Network.Taps));
        }
    }
}