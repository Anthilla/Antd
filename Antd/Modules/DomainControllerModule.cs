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

using System.IO;
using antdlib;
using antdlib.Certificate;
using antdlib.Log;
using antdlib.MountPoint;
using antdlib.Terminal;
using Antd.Helpers;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {

    public class DomainControllerModule : CoreModule {

        private readonly string[] _directories = {
            "/etc/samba",
            "/usr/lib64/python2.7/site-packages/samba",
            "/var/cache/samba",
            "/var/lib/samba",
            "/var/lock/samba",
            "/var/log/samba"
        };

        public DomainControllerModule() {
            this.RequiresAuthentication();

            Post["/dc/setup"] = x => {
                foreach (var dir in _directories) {
                    var mntDir = Mount.GetDirsPath(dir);
                    Terminal.Execute($"mkdir -p {mntDir}");
                    Terminal.Execute($"cp /mnt/livecd{dir} {mntDir}");
                    Mount.Dir(dir);
                }

                var domainName = (string)Request.Form.DomainName;
                var domainRealmname = (string)Request.Form.DomainRealmname;
                var domainHostname = (string)Request.Form.DomainHostname;
                var domainHostip = (string)Request.Form.DomainHostip;
                var domainAdminPassword = (string)Request.Form.DomainAdminPassword;

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

                const string sambaRealConf = "/etc/samba/smb.conf";
                var sambaConf = $"{Parameter.Resources}/smb.conf.template";
                const string workgroup = "$workgroup$";
                const string realm = "$realm$";
                const string netbiosName = "$netbiosName$";
                const string netlogonPath = "$netlogonPath$";
                var lowerRealm = domainRealmname.ToLower();
                var sambaCnfText = File.ReadAllText(sambaConf)
                    .Replace(workgroup, domainName.ToUpper())
                    .Replace(realm, domainRealmname.ToUpper())
                    .Replace(netbiosName, domainHostname.ToUpper())
                    .Replace(netlogonPath, $"/var/lib/samba/sysvol/{lowerRealm}/scripts");
                if (File.Exists(sambaRealConf)) {
                    File.Delete(sambaRealConf);
                }
                File.WriteAllText(sambaRealConf, sambaCnfText);

                Terminal.Execute("systemctl restart samba");

                Terminal.Execute("mkdir -p /var/lib/samba/private");
                var krbConf = $"{Parameter.Resources}/krb5.conf.template";
                const string realmAlt = "$realmalt$";
                var krbCnfText = File.ReadAllText(krbConf)
                    .Replace(realmAlt, lowerRealm)
                    .Replace(realm, domainRealmname.ToUpper());
                const string krbRealConf = "/etc/krb5.conf";
                if (File.Exists(krbRealConf)) {
                    File.Delete(krbRealConf);
                }
                File.WriteAllText(krbRealConf, krbCnfText);
                const string krbRealConfSamba = "/var/lib/samba/private/krb5.conf";
                if (File.Exists(krbRealConfSamba)) {
                    File.Delete(krbRealConfSamba);
                }
                File.WriteAllText(krbRealConfSamba, krbCnfText);

                ConsoleLogger.Log($"{domainName} references updated");

                return Response.AsRedirect("/");
            };

            Post["/dc/adduser"] = x => {
                var domainName = (string)Request.Form.DomainName;
                var username = (string)Request.Form.Username;
                var userPassword = (string)Request.Form.UserPassword;

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
                var certAssignment = (string)Request.Form.CertAssignment.Value;
                var certCountry = (string)Request.Form.CertCountry;
                var certProvince = (string)Request.Form.CertProvince;
                var certLocality = (string)Request.Form.CertLocality;
                var certOrganization = (string)Request.Form.CertOrganization;
                var certOrganizationalUnit = (string)Request.Form.CertOrganizationalUnit;
                var certCommonName = (string)Request.Form.CertCommonName;
                var certEmailAddress = (string)Request.Form.CertEmailAddress;
                var certPassphrase = (string)Request.Form.CertPassphrase;
                var certKeyLength = (string)Request.Form.CertKeyLength;
                var certUserAssignedGuid = (string)Request.Form.CertUserAssignedGuid;
                var certServiceAssignedGuid = (string)Request.Form.CertServiceAssignedGuid;
                var certServiceAssignedName = (string)Request.Form.CertServiceAssignedName;
                CertificateAuthority.Certificate.Create(certCountry, certProvince, certLocality, certOrganization, certOrganizationalUnit, certCommonName, certEmailAddress, certPassphrase, CertificateAssignmentType.Detect(certAssignment), certKeyLength, certUserAssignedGuid, certServiceAssignedGuid, certServiceAssignedName);
                return Response.AsRedirect("/");
            };
        }
    }
}