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

using antdlib.Models;
using library.u2f;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DeNSo;

namespace antdlib.Auth {

    public class TokenAuthentication {
        public static IEnumerable<GlobalUserModel> Show() {
            return Session.New.Get<GlobalUserModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="values">
        ///     TKey è il tipo di identità (facoltativo)
        ///     TValue è il valore dell'identità
        /// </param>
        public static void RegisterUser(string guid, HashSet<KeyValuePair<string, string>> values) {
            var i = Session.New.Get<GlobalUserModel>(_ => _.GlobalUID == guid).FirstOrDefault() ??
                    new GlobalUserModel() { _Id = Guid.NewGuid().ToString(), GlobalUID = Guid.NewGuid().ToString() };
            i.Identities = i.Identities.Union(values);
            Session.New.Set(i);
        }

        public static void AssignOtpToken(string guid, string tokenId) {
            var otpKvp = new KeyValuePair<string, string>("otptoken", tokenId.Substring(0, 12));
            RegisterUser(guid, new HashSet<KeyValuePair<string, string>> { otpKvp });
        }

        public static void DeleteRelation(string guid) {
            var i = Session.New.Get<GlobalUserModel>(_ => _.GlobalUID == guid).FirstOrDefault();
            Session.New.Delete(i);
        }

        /// <summary>
        /// controlla se esiste un'identità nel DB e ne restituisce il GlobalUID
        /// return null se non esiste niente
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string CheckIdentity(string identity) {
            var i = Session.New.Get<GlobalUserModel>().First(g => g.Identities.Select(_ => _.Value).ToList().Contains(identity));
            return i?.GlobalUID;
        }

        private static bool ValidateUserIdentity(string username, string password) {
            var request = (HttpWebRequest)WebRequest.Create("http://127.0.0.1/wsapi/u2fval/");
            request.Method = "GET";
            request.UseDefaultCredentials = false;
            request.PreAuthenticate = true;
            request.Credentials = new NetworkCredential(username, password);
            var response = (HttpWebResponse)request.GetResponse();
            return response.StatusCode == HttpStatusCode.OK;
        }

        private static bool ConfirmTokenValidity(string otp) {
            try {
                if (otp.Length < 12) {
                    return false;
                }
                var answer = new U2FRequest("25311", "5hQfQbHQGLIauepG9Sa5LQAMGYk=").Validate(otp);
                return answer.IsSignatureValid || answer.IsValid;
            }
            catch (Exception) {
                return false;
            }
        }

        public static bool Validate(string username, string password, string otp) {
            var validateUserIdentity = ValidateUserIdentity(username, password);
            var confirmTokenValidity = ConfirmTokenValidity(otp);
            var validateToken = CheckIdentity(otp);
            return validateUserIdentity && confirmTokenValidity && !string.IsNullOrEmpty(validateToken);
        }
    }
}
