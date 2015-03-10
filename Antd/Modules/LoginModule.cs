///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using Antd.Auth;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Cookies;
using Nancy.Extensions;
using Newtonsoft.Json;
using System;
using System.Dynamic;

namespace Antd {

    public static class Extension {

        public static Guid ToGuid(this Guid? source) {
            return source ?? Guid.Empty;
        }
    }

    public class LoginModule : NancyModule {

        public LoginModule() {
            Get["/login"] = x => {
                dynamic model = new ExpandoObject();
                model.Errored = this.Request.Query.error.HasValue;
                return View["login", model];
            };

            Post["/login"] = x => {
                string username = (string)this.Request.Form.Username;
                string password = (string)this.Request.Form.Password;

                DateTime? expiry = DateTime.Now.AddHours(12);
                if (this.Request.Form.RememberMe.HasValue) {
                    expiry = DateTime.Now.AddDays(3);
                }

                Guid? validationGuid = UserDatabase.ValidateUser(username, password);

                if (validationGuid == null) {
                    return this.Context.GetRedirect("~/login?error=true&Username=" + (string)this.Request.Form.Username);
                }
                else {
                    NancyCookie cookie = new NancyCookie("session", validationGuid.ToGuid().ToString());
                    return this.LoginAndRedirect(validationGuid.ToGuid(), expiry).WithCookie(cookie);
                }
            };

            Get["/logout"] = x => {
                var request = this.Request;
                var cookies = request.Cookies;
                cookies.Clear();
                return this.LogoutAndRedirect("~/");
            };

            Get["/get/user/{usr}"] = x => {
                var u = (string)x.usr;
                var i = MapSystemUser.GetRootPwd(u);
                return JsonConvert.SerializeObject(i);
            };
        }
    }
}