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
using System.Dynamic;
using antdlib.Auth;
using antdlib.Auth.T2FA;
using antdlib.Boot;
using antdlib.Common;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Cookies;
using Nancy.Extensions;

namespace Antd.Modules {

    public class LoginModule : NancyModule {

        public LoginModule() {
            Get["/login/{message?}"] = x => {
                dynamic model = new ExpandoObject();
                model.WantsEmail = false;
                model.HasToken = false;
                model.Username = "";
                model.Password = "";
                model.Email = "";
                model.Message = "";
                switch ((string)x.message) {
                    case "fail":
                        model.Message = "Login has failed, chech your username or password!";
                        break;
                    case "expired":
                        model.Message = "Your session is expired, you need to authenticate again!";
                        break;
                    case "tkn":
                        model.Message = "Your token is not valid!";
                        break;
                }
                model.Title = "Welcome to Anthilla";
                model.Copyright = @"© 2013 - " + DateTime.Now.ToString("yyyy") + " Anthilla S.r.l.";
                return View["login", model];
            };

            Post["/login"] = x => {
                var username = (string)Request.Form.Username;
                var password = (string)Request.Form.Password;
                var validationGuid = UserDatabase.ValidateUser(username, password);
                if (validationGuid == null) {
                    return Context.GetRedirect("/login/fail");
                }
                var cookies = Request.Cookies;
                cookies.Clear();
                cookies.Remove("antd-session");
                NancyCookie cookie;
                if (CoreParametersConfig.GetT2Fa() == false) {
                    cookie = new NancyCookie("antd-session", validationGuid.ToGuid().ToString());
                    return this.LoginAndRedirect(validationGuid.ToGuid(), DateTime.Now.AddHours(100)).WithCookie(cookie);
                }
                var validationEmail = UserDatabase.GetUserEmail(validationGuid.ToGuid());
                var requestEmail = (string)Request.Form.Email;
                if (validationEmail == null && requestEmail == "") {
                    return Response.AsRedirect("/login/auth/" + username + "/" + password);
                }
                var email = validationEmail ?? requestEmail;
                if (email != null) {
                    Authentication.SendNotification(validationGuid.ToGuid().ToString(), username, email);
                }
                cookie = new NancyCookie("antd-session", validationGuid.ToGuid().ToString());
                return Response.AsRedirect("/login/token/" + validationGuid.ToGuid().ToString()).WithCookie(cookie);
            };

            Get["/logout"] = x => {
                var request = Request;
                var cookies = request.Cookies;
                cookies.Clear();
                return this.LogoutAndRedirect("/");
            };

            Get["/login/auth/{username}/{password}"] = x => {
                dynamic model = new ExpandoObject();
                model.WantsEmail = true;
                model.HasToken = false;
                model.Username = x.username;
                model.Password = x.password;
                model.Email = "";
                return View["login", model];
            };

            Get["/login/token/{session}"] = x => {
                dynamic model = new ExpandoObject();
                model.Session = x.session;
                return View["login-token", model];
            };

            Post["/token"] = x => {
                var token = (string)Request.Form.Token;
                var session = (string)Request.Form.Session;
                var validation = Authentication.Confirm(session, token);
                var username = (string)Request.Form.Username;
                var password = (string)Request.Form.Password;
                var validationGuid = UserDatabase.ValidateUser(username, password);
                DateTime? expiry = DateTime.Now.AddHours(100);
                if (Request.Form.RememberMe.HasValue) {
                    expiry = DateTime.Now.AddHours(8);
                }

                if (validation == false) {
                    return Context.GetRedirect("/login");
                }
                if (validationGuid == null) {
                    return Context.GetRedirect("/login");
                }
                var cookie = new NancyCookie("antd-session", session);
                return this.LoginAndRedirect(Guid.Parse(session), expiry).WithCookie(cookie);
            };

            Post["/login/verify"] = x => {
                var username = (string)Request.Form.Username;
                var password = (string)Request.Form.Password;
                var validationGuid = UserDatabase.ValidateUser(username, password);
                var response = validationGuid != null;
                return Response.AsJson(response);
            };

            Post["/antd/authentication"] = x => {
                //var username = (string)Request.Form.Username;
                //var password = (string)Request.Form.Password;
                //var validationGuid = UserDatabase.ValidateUser(username, password);
                //if (validationGuid == null) {
                //    return Context.GetRedirect("~/login/fail");
                //}
                //var cookies = Request.Cookies;
                //cookies.Clear();
                //cookies.Remove("antd-session");
                //NancyCookie cookie;
                //if (Config.IsEnabled == false) {
                //    cookie = new NancyCookie("antd-session", validationGuid.ToGuid().ToString());
                //    return this.LoginAndRedirect(validationGuid.ToGuid(), DateTime.Now.AddHours(100)).WithCookie(cookie);
                //}
                //var validationEmail = UserDatabase.GetUserEmail(validationGuid.ToGuid());
                //var requestEmail = (string)Request.Form.Email;
                //if (validationEmail == null && requestEmail == "") {
                //    return Response.AsRedirect("/login/auth/" + username + "/" + password);
                //}
                //var email = validationEmail ?? requestEmail;
                //if (email != null) {
                //    Authentication.SendNotification(validationGuid.ToGuid().ToString(), username, email);
                //}
                //cookie = new NancyCookie("antd-session", validationGuid.ToGuid().ToString());
                //return Response.AsRedirect("/login/token/" + validationGuid.ToGuid().ToString()).WithCookie(cookie);
                return HttpStatusCode.OK;
            };
        }
    }
}