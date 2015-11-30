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
using antdlib.Boot;
using antdlib.Certificate;
using Antd.Helpers;
using Nancy;
using Nancy.Responses;
using Nancy.Security;

namespace Antd.Modules {

    public class CertificateAuthorityModule : CoreModule {
        public CertificateAuthorityModule() {
            this.RequiresAuthentication();

            Get["/ca/ssl/status"] = x => Response.AsJson(CoreParametersConfig.GetSsl());

            Post["/ca/ssl/toggle"] = x => {
                if (CoreParametersConfig.GetSsl() == "yes") {
                    CoreParametersConfig.DisableSsl();
                    return Response.AsJson(true);
                }
                CoreParametersConfig.EnableSsl();
                return Response.AsJson(true);
            };

            Post["/ca/ssl/enable"] = x => {
                CoreParametersConfig.EnableSsl();
                return Response.AsJson(true);
            };

            Post["/ca/ssl/disable"] = x => {
                CoreParametersConfig.DisableSsl();
                return Response.AsJson(true);
            };

            Post["/ca/setup"] = x => {
                var caDirectory = (string)Request.Form.CaDirectory;
                var caCountry = (string)Request.Form.CaCountry;
                var caProvince = (string)Request.Form.CaProvince;
                var caLocality = (string)Request.Form.CaLocality;
                var caOrganization = (string)Request.Form.CaOrganization;
                var caOrganizationalUnit = (string)Request.Form.CaOrganizationalUnit;
                var caCommonName = (string)Request.Form.CaCommonName;
                var caEmail = (string)Request.Form.CaEmail;
                var caPassphrase = (string)Request.Form.CaPassphrase;
                CertificateAuthority.Setup(caDirectory, caPassphrase, caCountry, caProvince, caLocality, caOrganization, caOrganizationalUnit, caCommonName, caEmail);
                return Response.AsJson(true);
            };

            Post["/ca/certificate/new"] = x => {
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
                var assignment = ((string)Request.Form.Assignment.Value).Length < 1 ? CertificateAssignment.User : CertificateAssignmentType.Detect((string)Request.Form.Assignment.Value);
                var userGuid = ((string)Request.Form.UserGuid).Length < 1 ? "" : (string)Request.Form.UserGuid;
                var serviceGuid = ((string)Request.Form.ServiceGuid).Length < 1 ? "" : (string)Request.Form.ServiceGuid;
                var serviceAlias = ((string)Request.Form.ServiceAlias).Length < 1 ? "" : (string)Request.Form.ServiceAlias;
                CertificateAuthority.Certificate.Create(countryName, stateProvinceName, localityName, organizationName, organizationalUnitName, commonName, emailAddress, password, assignment, bytesLength, userGuid, serviceGuid, serviceAlias);
                return Response.AsRedirect("/");
            };

            Get["/ca/certificate/download/{format}/{guid}"] = x => {
                var guid = (string)x.guid;
                var certificate = CertificateRepository.GetByGuid(guid);
                if (certificate == null)
                    return HttpStatusCode.InternalServerError;
                string path;
                var format = (string)x.format;
                switch (format) {
                    case "der":
                        path = certificate.CertificateDerPath;
                        break;
                    case "pfx":
                        path = certificate.CertificatePfxPath;
                        break;
                    default:
                        path = certificate.CertificatePath;
                        break;
                }
                var file = new FileStream(path, FileMode.Open);
                var fileName = Path.GetFileName(certificate.CertificatePath);
                var response = new StreamResponse(() => file, MimeTypes.GetMimeType(fileName));
                return response.AsAttachment(fileName);
            };
        }
    }
}