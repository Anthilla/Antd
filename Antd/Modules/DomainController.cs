//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using antdlib.Log;
using antdlib.MountPoint;
using antdlib.Terminal;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {

    public class DomainController : CoreModule {
        public DomainController() {
            this.RequiresAuthentication();

            Post["/dc/setup"] = x => {
                var domainName = Request.Form.DomainName;
                var domainRealmname = Request.Form.DomainRealmname;
                var domainHostname = Request.Form.DomainHostname;
                var domainHostip = Request.Form.DomainHostip;
                var domainAdminPassword = Request.Form.DomainAdminPassword;

                if (string.IsNullOrEmpty(domainName) || string.IsNullOrEmpty(domainRealmname) ||
                string.IsNullOrEmpty(domainHostname) || string.IsNullOrEmpty(domainHostip) ||
                string.IsNullOrEmpty(domainAdminPassword)) {
                    return Response.AsText("error: a value is missing. go back.");
                }

                Terminal.Execute($"samba-tool domain provision --option=\"interfaces = lo br0\" --option=\"bind interfaces only = yes\" --use-rfc2307 --domain={domainName} --realm={domainRealmname} --host-name={domainHostname} --host-ip={domainHostip} --adminpass={domainAdminPassword} --dns-backend=SAMBA_INTERNAL --server-role=dc");
                ConsoleLogger.Log($"domain {domainName} created");

                if (!Mount.IsAlreadyMounted("/etc/hosts")) {
                    Mount.File("/etc/hosts");
                }
                Terminal.Execute("echo 127.0.0.1 localhost.localdomain localhost > /etc/hosts");
                Terminal.Execute($"echo {domainHostip} {domainHostname}.{domainRealmname} {domainHostname} >> /etc/hosts");

                if (!Mount.IsAlreadyMounted("/etc/resolv.conf")) {
                    Mount.File("/etc/resolv.conf");
                }
                Terminal.Execute($"echo nameserver {domainHostip} > /etc/resolv.conf");
                Terminal.Execute($"echo search {domainRealmname} >> /etc/resolv.conf");
                Terminal.Execute($"echo domain {domainRealmname} >> /etc/resolv.conf");
                ConsoleLogger.Log($"{domainName} references updated");
                return Response.AsRedirect("/");
            };

            Post["/dc/adduser"] = x => {
                var domainName = Request.Form.DomainName;
                var username = Request.Form.Username;
                var userPassword = Request.Form.UserPassword;

                if (string.IsNullOrEmpty(domainName) || string.IsNullOrEmpty(userPassword) || string.IsNullOrEmpty(username)) {
                    return Response.AsText("error: a value is missing. go back.");
                }

                Terminal.Execute($"samba-tool user create {username} --password={userPassword} --username={username} --mail-address={username}@{domainName} --given-name={username}");

                return Response.AsRedirect("/");
            };
        }
    }
}