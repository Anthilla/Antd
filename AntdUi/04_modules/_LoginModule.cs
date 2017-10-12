using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Cookies;
using Nancy.Extensions;
using System;
using System.Dynamic;
using System.Linq;
using anthilla.core;
using AntdUi.Auth;

namespace AntdUi {
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
                return this.Login(GuidExtensions.ToGuid(validationGuid), expiration).WithCookie(sessionCookie);
                //return this.LoginAndRedirect(GuidExtensions.ToGuid(validationGuid), expiration, returnUrl).WithCookie(sessionCookie);
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