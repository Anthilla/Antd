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
using antdlib.common;
using Antd.Certificates;
using Nancy;
using Nancy.Responses;
using Nancy.Security;

namespace Antd.Modules {

    public class ServiceCaModule : NancyModule {
        public ServiceCaModule() {
            this.RequiresAuthentication();
            Post["/services/ca/set"] = x => {
                var caConfiguration = new CaConfiguration();
                caConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/services/ca/enable"] = x => {
                var caConfiguration = new CaConfiguration();
                caConfiguration.Enable();
                return HttpStatusCode.OK;
            };

            Post["/services/ca/disable"] = x => {
                var caConfiguration = new CaConfiguration();
                caConfiguration.Disable();
                return HttpStatusCode.OK;
            };

            Post["/services/ca/options"] = x => {
                string keyPassout = Request.Form.KeyPassout;
                string rootCountryName = Request.Form.RootCountryName;
                string rootStateOrProvinceName = Request.Form.RootStateOrProvinceName;
                string rootLocalityName = Request.Form.RootLocalityName;
                string rootOrganizationName = Request.Form.RootOrganizationName;
                string rootOrganizationalUnitName = Request.Form.RootOrganizationalUnitName;
                string rootCommonName = Request.Form.RootCommonName;
                string rootEmailAddress = Request.Form.RootEmailAddress;
                var model = new CaConfigurationModel {
                    KeyPassout = keyPassout,
                    RootCountryName = rootCountryName,
                    RootStateOrProvinceName = rootStateOrProvinceName,
                    RootLocalityName = rootLocalityName,
                    RootOrganizationName = rootOrganizationName,
                    RootOrganizationalUnitName = rootOrganizationalUnitName,
                    RootCommonName = rootCommonName,
                    RootEmailAddress = rootEmailAddress,
                };
                var caConfiguration = new CaConfiguration();
                caConfiguration.Save(model);
                return Response.AsRedirect("/ca");
            };

            Get["/services/ca/crl"] = x => {
                var crl = $"{Parameter.AntdCfg}/ca/intermediate/crl/intermediate.crl.pem";
                if(!File.Exists(crl)) {
                    return HttpStatusCode.ExpectationFailed;
                }
                var file = new FileStream(crl, FileMode.Open);
                const string fileName = "intermediate.crl.pem";
                var response = new StreamResponse(() => file, MimeTypes.GetMimeType(fileName));
                return response.AsAttachment(fileName);
            };

            Post["/services/ca/certificate/user"] = x => {
                string name = Request.Form.Name;
                string passphrase = Request.Form.Passphrase;
                string email = Request.Form.Email;
                string c = Request.Form.CountryName;
                string st = Request.Form.StateOrProvinceName;
                string l = Request.Form.LocalityName;
                string o = Request.Form.OrganizationName;
                string ou = Request.Form.OrganizationalUnitName;
                var caConfiguration = new CaConfiguration();
                caConfiguration.CreateUserCertificate(name, passphrase, email, c, st, l, o, ou);
                return Response.AsRedirect("/ca");
            };

            Post["/services/ca/certificate/server"] = x => {
                string name = Request.Form.Name;
                string passphrase = Request.Form.Passphrase;
                string email = Request.Form.Email;
                string c = Request.Form.CountryName;
                string st = Request.Form.StateOrProvinceName;
                string l = Request.Form.LocalityName;
                string o = Request.Form.OrganizationName;
                string ou = Request.Form.OrganizationalUnitName;
                var caConfiguration = new CaConfiguration();
                caConfiguration.CreateServerCertificate(name, passphrase, email, c, st, l, o, ou);
                return Response.AsRedirect("/ca");
            };

            Post["/services/ca/certificate/dc"] = x => {
                string name = Request.Form.Name;
                string passphrase = Request.Form.Passphrase;
                string dcGuid = Request.Form.Guid;
                string dcDns = Request.Form.Dns;
                string email = Request.Form.Email;
                string c = Request.Form.CountryName;
                string st = Request.Form.StateOrProvinceName;
                string l = Request.Form.LocalityName;
                string o = Request.Form.OrganizationName;
                string ou = Request.Form.OrganizationalUnitName;
                var caConfiguration = new CaConfiguration();
                caConfiguration.CreateDomainControllerCertificate(name, passphrase, dcGuid, dcDns, email, c, st, l, o, ou);
                return Response.AsRedirect("/ca");
            };

            Post["/services/ca/certificate/sc"] = x => {
                string name = Request.Form.Name;
                string passphrase = Request.Form.Passphrase;
                string upn = Request.Form.Upn;
                string email = Request.Form.Email;
                string c = Request.Form.CountryName;
                string st = Request.Form.StateOrProvinceName;
                string l = Request.Form.LocalityName;
                string o = Request.Form.OrganizationName;
                string ou = Request.Form.OrganizationalUnitName;
                var caConfiguration = new CaConfiguration();
                caConfiguration.CreateSmartCardCertificate(name, passphrase, upn, email, c, st, l, o, ou);
                return Response.AsRedirect("/ca");
            };
        }
    }
}