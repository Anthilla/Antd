using AntdUi2.Modules.Auth;
using anthilla.core;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Cookies;
using Nancy.Extensions;
using System;
using System.Dynamic;
using System.Linq;

namespace AntdUi2.Modules {
    public class LoginModule : NancyModule {

        public LoginModule() {

            Get("/login", x => ApiGet());

            Post("/login", x => ApiPost());

            Get("/logout", x => ApiGetLogout());

            Get("/login/auth/{username}/{password}", x => ApiGetLoginAuth(x));

            Get("/login/token/{session}", x => ApiGetLoginToken(x));

            Post("/login/verify", x => ApiPostLoginVerify());
        }

        private dynamic ApiGet() {
            dynamic model = new ExpandoObject();
            var returnUrl = (string)Request.Query.returnUrl;
            model.ReturnUrl = returnUrl;
            model.Copyright = @"© 2013 - " + DateTime.Now.ToString("yyyy") + " Anthilla S.r.l.";
            return View["login.min.html", model];
        }

        private dynamic ApiPost() {
            var username = (string)Request.Form.Username;
            var password = (string)Request.Form.Password;
            if (string.IsNullOrEmpty(username + password)) {
                return Context.GetRedirect("/login");
            }
            var expiration = DateTime.Now.AddHours(4);
            var cookies = Request.Cookies;
            while (cookies.Any()) {
                cookies.Clear();
            }
            var validationGuid = UserDatabase.ValidateUser(Application.ServerUrl, username, password);
            if (validationGuid == null) {
                return Context.GetRedirect("/login");
            }
            var sessionCookie = new NancyCookie("antd-session", GuidExtensions.ToGuid(validationGuid).ToString(), expiration);
            return this.Login(GuidExtensions.ToGuid(validationGuid), expiration).WithCookie(sessionCookie);
        }

        private dynamic ApiGetLogout() {
            var cookies = Request.Cookies;
            while (cookies.Any()) {
                cookies.Clear();
            }
            return this.LogoutAndRedirect("/");
        }

        private dynamic ApiGetLoginAuth(dynamic x) {
            dynamic model = new ExpandoObject();
            model.WantsEmail = true;
            model.HasToken = false;
            model.Username = x.username;
            model.Password = x.password;
            model.Email = "";
            return View["login", model];
        }

        private dynamic ApiGetLoginToken(dynamic x) {
            dynamic model = new ExpandoObject();
            model.Session = x.session;
            return View["login-token", model];
        }

        private dynamic ApiPostLoginVerify() {
            var username = (string)Request.Form.Username;
            var password = (string)Request.Form.Password;
            var validationGuid = UserDatabase.ValidateUser(Application.ServerUrl, username, password);
            var response = validationGuid != null;
            return Response.AsJson(response);
        }
    }
}