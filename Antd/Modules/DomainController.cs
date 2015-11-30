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

using System;
using System.IO;
using antdlib.Certificate;
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
                Terminal.Execute(!File.Exists("/etc/resolv.conf")
                    ? $"echo nameserver {domainHostip} > /etc/resolv.conf"
                    : $"echo nameserver {domainHostip} >> /etc/resolv.conf");
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

            Post["/dc/cert"] = x => {
                var domControllerGuid = (string)Request.Form.DomainControllerGuid;
                var domDnsName = (string)Request.Form.DomainDnsName;
                var domCrlDistributionPoint = (string)Request.Form.DomainCrlDistributionPoint;
                var domCaCountry = (string)Request.Form.DomainCaCountry;
                var domCaProvince = (string)Request.Form.DomainCaProvince;
                var domCaLocality = (string)Request.Form.DomainCaLocality;
                var domCaOrganization = (string)Request.Form.DomainCaOrganization;
                var domCaOrganizationalUnit = (string)Request.Form.DomainCaOrganizationalUnit;
                var domCaCommonName = (string)Request.Form.DomainCaCommonName;
                var domCaEmail = (string)Request.Form.DomainCaEmail;
                var domCaPassphrase = (string)Request.Form.DomainCaPassphrase;
                CertificateAuthority.DomainControllerCertificate.Create(domCrlDistributionPoint, domControllerGuid, domDnsName, domCaCountry, domCaProvince, domCaLocality, domCaOrganization, domCaOrganizationalUnit, domCaCommonName, domCaEmail, domCaPassphrase);
                return Response.AsRedirect("/");
            };

            Post["/sc/cert"] = x => {
                var userPrincipalName = (string)Request.Form.UserPrincipalName;
                var domainCrlDistributionPoint = (string)Request.Form.DomainCrlDistributionPoint;
                var smartCardCaCountry = (string)Request.Form.SmartCardCaCountry;
                var smartCardCaProvince = (string)Request.Form.SmartCardCaProvince;
                var smartCardCaLocality = (string)Request.Form.SmartCardCaLocality;
                var smartCardCaOrganization = (string)Request.Form.SmartCardCaOrganization;
                var smartCardCaOrganizationalUnit = (string)Request.Form.SmartCardCaOrganizationalUnit;
                var smartCardCaPassphrase = (string)Request.Form.SmartCardCaPassphrase;
                CertificateAuthority.SmartCardCertificate.Create(domainCrlDistributionPoint, userPrincipalName, smartCardCaCountry, smartCardCaProvince, smartCardCaLocality, smartCardCaOrganization, smartCardCaOrganizationalUnit, smartCardCaPassphrase);
                return Response.AsRedirect("/");
            };

            Post["/ca/cert"] = x => {
                var countryName = ((string)Request.Form.CountryName).Length < 1 ? "." : (string)Request.Form.CountryName;
                if (countryName.Length > 2) {
                    countryName = countryName.Substring(0, 2).ToUpper();
                }
                var stateProvinceName = ((string)Request.Form.StateProvinceName).Length < 1 ? "." : (string)Request.Form.StateProvinceName;
                var localityName = ((string)Request.Form.LocalityName).Length < 1 ? "." : (string)Request.Form.LocalityName;
                var organizationName = ((string)Request.Form.OrganizationName).Length < 1 ? "." : (string)Request.Form.OrganizationName;
                var organizationalUnitName = ((string)Request.Form.OrganizationalUnitName).Length < 1 ? "." : (string)Request.Form.OrganizationalUnitName;
                var commonName = ((string)Request.Form.CommonName).Length < 1 ? "*" : (string)Request.Form.CommonName;
                var emailAddress = ((string)Request.Form.EmailAddress).Length < 1 ? "." : (string)Request.Form.EmailAddress;
                var password = ((string)Request.Form.Password).Length < 1 ? "" : (string)Request.Form.Password;
                var bytesLength = ((string)Request.Form.BytesLength).Length < 1 ? "2048" : (string)Request.Form.BytesLength;
                var assignment = ((string)Request.Form.Assignment.Value).Length < 1 ? CertificateAssignment.User : DetectCertificateAssignment((string)Request.Form.Assignment.Value);
                var userGuid = ((string)Request.Form.UserGuid).Length < 1 ? "" : (string)Request.Form.UserGuid;
                var serviceGuid = ((string)Request.Form.ServiceGuid).Length < 1 ? "" : (string)Request.Form.ServiceGuid;
                var serviceAlias = ((string)Request.Form.ServiceAlias).Length < 1 ? "" : (string)Request.Form.ServiceAlias;
                CertificateAuthority.Certificate.Create(countryName, stateProvinceName, localityName, organizationName, organizationalUnitName, commonName, emailAddress, password, assignment, bytesLength, userGuid, serviceGuid, serviceAlias);
                return Response.AsRedirect("/");
            };
        }
    }
}