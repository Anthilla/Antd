using Nancy;
using Nancy.Security;

namespace Antd {

    public class FirewallModule : NancyModule {

        public FirewallModule()
            : base("/firewall") {
            this.RequiresAuthentication();

            Get["/home"] = x => {
                return View["page-firewall-home"];
            };

            Get["/manage"] = x => {
                return View["page-firewall-mgmt"];
            };

            Get["/util"] = x => {
                return View["page-firewall-util"];
            };

            Get["/hspot"] = x => {
                return View["page-firewall-hspot"];
            };

            Get["/filter"] = x => {
                return View["page-firewall-filter"];
            };

            Get["/mail"] = x => {
                return View["page-firewall-mail"];
            };

            Get["/mail/domains"] = x => {
                return View["page-firewall-mldmns"];
            };

            Get["/user/acl"] = x => {
                return View["page-firewall-user"];
            };

            Get["/network"] = x => {
                return View["page-firewall-net"];
            };

            Get["/network/server"] = x => {
                return View["page-firewall-netsrv"];
            };

            Get["/network/vpn"] = x => {
                return View["page-firewall-vpn"];
            };

            Get["/log"] = x => {
                return View["page-firewall-log"];
            };

            Get["/setup"] = x => {
                return View["page-firewall-setup"];
            };
        }
    }
}