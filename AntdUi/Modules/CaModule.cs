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

using antdlib.common;
using antdlib.models;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AntdUi.Modules {
    public class CaModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public CaModule() {
            Get["/ca"] = x => {
                var model = _api.Get<PageCaModel>($"http://127.0.0.1:{Application.ServerPort}/ca");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/ca/set"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/ca/set");
            };

            Post["/ca/enable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/ca/enable");
            };

            Post["/ca/disable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/ca/disable");
            };

            Post["/ca/options"] = x => {
                string keyPassout = Request.Form.KeyPassout;
                string rootCountryName = Request.Form.RootCountryName;
                string rootStateOrProvinceName = Request.Form.RootStateOrProvinceName;
                string rootLocalityName = Request.Form.RootLocalityName;
                string rootOrganizationName = Request.Form.RootOrganizationName;
                string rootOrganizationalUnitName = Request.Form.RootOrganizationalUnitName;
                string rootCommonName = Request.Form.RootCommonName;
                string rootEmailAddress = Request.Form.RootEmailAddress;
                var dict = new Dictionary<string, string> {
                    { "KeyPassout", keyPassout },
                    { "RootCountryName", rootCountryName },
                    { "RootStateOrProvinceName", rootStateOrProvinceName },
                    { "RootLocalityName", rootLocalityName },
                    { "RootOrganizationName", rootOrganizationName },
                    { "RootOrganizationalUnitName", rootOrganizationalUnitName },
                    { "RootCommonName", rootCommonName },
                    { "RootEmailAddress", rootEmailAddress },
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/ca/options", dict);
            };

            Get["/ca/crl"] = x => {
                //todo controlla qui
                var crl = $"{Parameter.AntdCfg}/ca/intermediate/crl/intermediate.crl.pem";
                if(!File.Exists(crl)) {
                    return HttpStatusCode.ExpectationFailed;
                }
                var file = new FileStream(crl, FileMode.Open);
                const string fileName = "intermediate.crl.pem";
                var response = new StreamResponse(() => file, MimeTypes.GetMimeType(fileName));
                return response.AsAttachment(fileName);
            };

            Post["/ca/certificate/user"] = x => {
                string name = Request.Form.Name;
                string passphrase = Request.Form.Passphrase;
                string email = Request.Form.Email;
                string c = Request.Form.CountryName;
                string st = Request.Form.StateOrProvinceName;
                string l = Request.Form.LocalityName;
                string o = Request.Form.OrganizationName;
                string ou = Request.Form.OrganizationalUnitName;
                var dict = new Dictionary<string, string> {
                        { "Name", name },
                        { "Passphrase", passphrase },
                        { "Email", email },
                        { "CountryName", c },
                        { "StateOrProvinceName", st },
                        { "LocalityName", l },
                        { "OrganizationName", o },
                        { "OrganizationalUnitName", ou }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/ca/certificate/user", dict);
            };

            Post["/ca/certificate/server"] = x => {
                string name = Request.Form.Name;
                string passphrase = Request.Form.Passphrase;
                string email = Request.Form.Email;
                string c = Request.Form.CountryName;
                string st = Request.Form.StateOrProvinceName;
                string l = Request.Form.LocalityName;
                string o = Request.Form.OrganizationName;
                string ou = Request.Form.OrganizationalUnitName;
                var dict = new Dictionary<string, string> {
                    { "Name", name },
                    { "Passphrase", passphrase },
                    { "Email", email },
                    { "CountryName", c },
                    { "StateOrProvinceName", st },
                    { "LocalityName", l },
                    { "OrganizationName", o },
                    { "OrganizationalUnitName", ou }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/ca/certificate/server", dict);
            };

            Post["/ca/certificate/dc"] = x => {
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
                var dict = new Dictionary<string, string> {
                    { "Name", name },
                    { "Passphrase", passphrase },
                    { "Guid", dcGuid },
                    { "Dns", dcDns },
                    { "Email", email },
                    { "CountryName", c },
                    { "StateOrProvinceName", st },
                    { "LocalityName", l },
                    { "OrganizationName", o },
                    { "OrganizationalUnitName", ou }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/ca/certificate/dc", dict);
            };

            Post["/ca/certificate/sc"] = x => {
                string name = Request.Form.Name;
                string passphrase = Request.Form.Passphrase;
                string upn = Request.Form.Upn;
                string email = Request.Form.Email;
                string c = Request.Form.CountryName;
                string st = Request.Form.StateOrProvinceName;
                string l = Request.Form.LocalityName;
                string o = Request.Form.OrganizationName;
                string ou = Request.Form.OrganizationalUnitName;
                var dict = new Dictionary<string, string> {
                    { "Name", name },
                    { "Passphrase", passphrase },
                    { "Upn", upn },
                    { "Email", email },
                    { "CountryName", c },
                    { "StateOrProvinceName", st },
                    { "LocalityName", l },
                    { "OrganizationName", o },
                    { "OrganizationalUnitName", ou }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/ca/certificate/sc", dict);
            };
        }
    }
}