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

using AntdUi.Auth;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Cookies;
using Nancy.Extensions;
using System;
using System.Dynamic;
using System.Linq;
using anthilla.core;

namespace AntdUi.Modules {
    public class LoginModule : NancyModule {
        public LoginModule() {

            Get["/login"] = x => {
                dynamic model = new ExpandoObject();
                var returnUrl = (string)Request.Query.returnUrl;
                model.ReturnUrl = returnUrl;
                model.Copyright = @"© 2013 - " + DateTime.Now.ToString("yyyy") + " Anthilla S.r.l.";
                return View["login", model];
            };

            Post["/login"] = x => {
                var username = (string)Request.Form.Username;
                var password = (string)Request.Form.Password;
                if(string.IsNullOrEmpty(username + password)) {
                    return Context.GetRedirect("/login");
                }
                ConsoleLogger.Log($"login attempt from {username}");
                var expiration = DateTime.Now.AddHours(4);
                var cookies = Request.Cookies;
                while(cookies.Any()) {
                    cookies.Clear();
                }
                var validationGuid = UserDatabase.ValidateUser(username, password);
                if(validationGuid == null) {
                    return Context.GetRedirect("/login");
                }
                var sessionCookie = new NancyCookie("antd-session", GuidExtensions.ToGuid(validationGuid).ToString(), expiration);
                ConsoleLogger.Log($"{username} logged in successfully");
                var returnUrl = (string)Request.Form.Return;
                return this.LoginAndRedirect(GuidExtensions.ToGuid(validationGuid), expiration, returnUrl).WithCookie(sessionCookie);
            };

            Get["/logout"] = x => {
                var cookies = Request.Cookies;
                while(cookies.Any()) {
                    cookies.Clear();
                }
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

            Post["/login/verify"] = x => {
                var username = (string)Request.Form.Username;
                var password = (string)Request.Form.Password;
                var validationGuid = UserDatabase.ValidateUser(username, password);
                var response = validationGuid != null;
                return Response.AsJson(response);
            };
        }
    }
}