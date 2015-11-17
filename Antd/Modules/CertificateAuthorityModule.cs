using antdlib.Boot;
using antdlib.Certificate;
using Nancy;
using Nancy.Routing.Constraints;
using Nancy.Security;

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

namespace Antd.Modules {

    public class CertificateAuthorityModule : NancyModule {
        public CertificateAuthorityModule()
            : base("/ca") {
            this.RequiresAuthentication();

            Get["/"] = x => View["login-authentication"];

            Get["/ssl/status"] = x => Response.AsJson(CoreParametersConfig.GetSsl());

            Post["/ssl/toggle"] = x => {
                if (CoreParametersConfig.GetSsl() == "yes") {
                    CoreParametersConfig.DisableSsl();
                    return Response.AsJson(true);
                }
                CoreParametersConfig.EnableSsl();
                return Response.AsJson(true);
            };

            Post["/ssl/enable"] = x => {
                CoreParametersConfig.EnableSsl();
                return Response.AsJson(true);
            };

            Post["/ssl/disable"] = x => {
                CoreParametersConfig.DisableSsl();
                return Response.AsJson(true);
            };

            Get["/cert/get"] = x => Response.AsJson(CoreParametersConfig.GetCertificatePath());

            Post["/cert/set"] = x => {
                CoreParametersConfig.SetCertificatePath((string)Request.Form.CertificatePath);
                return Response.AsJson(true);
            };

            Post["/setup"] = x => {
                CertificateAuthority.Setup();
                return Response.AsJson(true);
            };

            Post["/certificate/new"] = x => {
                var countryName = ((string)Request.Form.CountryName).Length < 1 ? "." : (string)Request.Form.CountryName;
                var stateProvinceName = ((string)Request.Form.StateProvinceName).Length < 1 ? "." : (string)Request.Form.StateProvinceName;
                var localityName = ((string)Request.Form.LocalityName).Length < 1 ? "." : (string)Request.Form.LocalityName;
                var organizationName = ((string)Request.Form.OrganizationName).Length < 1 ? "." : (string)Request.Form.OrganizationName;
                var organizationalUnitName = ((string)Request.Form.OrganizationalUnitName).Length < 1 ? "." : (string)Request.Form.OrganizationalUnitName;
                var commonName = ((string)Request.Form.CommonName).Length < 1 ? "*" : (string)Request.Form.CommonName;
                var emailAddress = ((string)Request.Form.EmailAddress).Length < 1 ? "." : (string)Request.Form.EmailAddress;
                var password = ((string)Request.Form.Password).Length < 1 ? "" : (string)Request.Form.CommoPasswordnName;
                var usePasswordForPrivateKey = ((string)Request.Form.Password).Length > 0;
                CertificateAuthority.Certificate.Create(countryName, stateProvinceName, localityName, organizationName, organizationalUnitName, commonName, emailAddress, password, usePasswordForPrivateKey);
                return Response.AsRedirect("/system");
            };
        }
    }
}