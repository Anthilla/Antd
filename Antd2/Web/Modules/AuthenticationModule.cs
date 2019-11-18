using Antd2.Configuration;
using anthilla.core;
using Nancy;
using Newtonsoft.Json;
using System;

namespace Antd2.Modules {

    public class AuthenticationModule : NancyModule {

        public AuthenticationModule() : base("/phlegyas") {

            Post("/authenticate", x => ApiPost_authenticate());

            //Get("/authenticate/user/{session}", x => ApiGet_authenticate_user_session(x));

            //Get("/user/by-session/{session}", x => ApiGet_user_bysession_session(x));

            //Get("/userguid/by-session/{session}", x => ApiGet_userguid_bysession_session(x));

            //Get("/user/reference", x => ApiPost_user_reference());

            //Post("/authenticate/basic", x => ApiPost_authenticate_basic());

            //Post("/checkuser", x => ApiPost_checkuser());
        }

        private dynamic ApiPost_authenticate() {
            string username = Request.Form.Username;
            string password = Request.Form.Password;

            Console.WriteLine($"[pha] login attempt from {username}");

            if (username != ConfigManager.Config.Saved.App.DefaultUser)
                return JsonConvert.SerializeObject(new Tuple<HttpStatusCode, Guid>(HttpStatusCode.InternalServerError, Guid.Empty));

            if (username != ConfigManager.Config.Saved.App.DefaultPassword)
                return JsonConvert.SerializeObject(new Tuple<HttpStatusCode, Guid>(HttpStatusCode.InternalServerError, Guid.Empty));

            return JsonConvert.SerializeObject(new Tuple<HttpStatusCode, Guid>(HttpStatusCode.OK, Guid.NewGuid()));
        }

        //private dynamic ApiGet_authenticate_user_session(dynamic x) {
        //    string session = x.session;
        //    if (!Guid.TryParse(session, out var g))
        //        return HttpStatusCode.InternalServerError;
        //    var token = DatabaseActionV5.Instance.First<PhlegyasTokenAuthClientIdModel>(_ => _.SessionGuid == g);
        //    if (token == null)
        //        return HttpStatusCode.InternalServerError;
        //    var users = DatabaseActionV5.Instance.Get<UserModel>().ToList();
        //    var userGroups = DatabaseActionV5.Instance.Get<UserGroupModel>();
        //    var functionGroups = DatabaseActionV5.Instance.Get<FunctionGroupModel>();

        //    var manageMaster = new ManageMaster();
        //    users.Add(new UserModel {
        //        Guid = Guid.Parse("4B640B85-1DCD-4C70-8981-2F3FD03E3013"),
        //        Alias = manageMaster.Name,
        //        Email = manageMaster.Name,
        //        FirstName = manageMaster.Name,
        //        LastName = manageMaster.Name,
        //        Password = manageMaster.Password,
        //        UsergroupGuids = userGroups.Select(_ => _.Guid.ToString()).ToArray()
        //    });

        //    var user = users.FirstOrDefault(_ => _.Guid == token.UserGuid);
        //    if (user == null)
        //        return HttpStatusCode.InternalServerError;

        //    var ugs = user.UsergroupGuids.Select(_ => userGroups.FirstOrDefault(__ => __.Guid == _.ToGuid())).ToList();
        //    var fgss = ugs.Select(_ => _.FunctionGroupGuids).Merge();
        //    var fgs = new List<FunctionGroupModel>();
        //    foreach (var fgg in fgss) {
        //        var fg = functionGroups.FirstOrDefault(_ => _.Guid == fgg.ToGuid());
        //        if (fg != null) {
        //            fgs.Add(fg);
        //        }
        //    }
        //    var fgc = fgs.Select(_ => _.FunctionCodes.ToList()).ToList();
        //    user.FunctionCodes = fgc.Merge().ToArray();

        //    var cleanuser = new UserModel() {
        //        Guid = user.Guid,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Alias = user.Alias,
        //        Email = user.Email,
        //        CompanyGuid = user.CompanyGuid,
        //        ProjectGuids = user.ProjectGuids,
        //        UsergroupGuids = user.UsergroupGuids,
        //        UserGuids = user.UserGuids,
        //        MailAccountGuids = user.MailAccountGuids,
        //        FunctionCodes = user.FunctionCodes
        //    };
        //    var company = DatabaseActionV5.Instance.First<CompanyModel>(cleanuser.CompanyGuid.ToGuid());
        //    if (company == null) {
        //        user.IsInternal = false;
        //    }
        //    else {
        //        user.IsInternal = company.IsInternal;
        //    }
        //    return JsonConvert.SerializeObject(cleanuser, Formatting.Indented);
        //}

        //private dynamic ApiGet_user_bysession_session(dynamic x) {
        //    string session = x.session;
        //    if (!Guid.TryParse(session, out var g))
        //        return HttpStatusCode.InternalServerError;
        //    var token = DatabaseActionV5.Instance.First<PhlegyasTokenAuthClientIdModel>(_ => _.SessionGuid == g);
        //    if (token == null)
        //        return HttpStatusCode.InternalServerError;
        //    var users = DatabaseActionV5.Instance.Get<UserModel>().ToList();
        //    var userGroups = DatabaseActionV5.Instance.Get<UserGroupModel>();
        //    var functionGroups = DatabaseActionV5.Instance.Get<FunctionGroupModel>();

        //    var manageMaster = new ManageMaster();
        //    users.Add(new UserModel {
        //        Guid = Guid.Parse("4B640B85-1DCD-4C70-8981-2F3FD03E3013"),
        //        Alias = manageMaster.Name,
        //        Email = manageMaster.Name,
        //        FirstName = manageMaster.Name,
        //        LastName = manageMaster.Name,
        //        Password = manageMaster.Password,
        //        UsergroupGuids = userGroups.Select(_ => _.Guid.ToString()).ToArray()
        //    });

        //    var user = users.FirstOrDefault(_ => _.Guid == token.UserGuid);
        //    if (user == null)
        //        return HttpStatusCode.InternalServerError;

        //    var ugs = user.UsergroupGuids.Select(_ => userGroups.FirstOrDefault(__ => __.Guid == _.ToGuid())).ToList();
        //    var fgss = ugs.Where(_ => _ != null).Select(_ => _.FunctionGroupGuids).Merge();
        //    var fgs = new List<FunctionGroupModel>();
        //    foreach (var fgg in fgss) {
        //        var fg = functionGroups.FirstOrDefault(_ => _.Guid == fgg.ToGuid());
        //        if (fg != null) {
        //            fgs.Add(fg);
        //        }
        //    }
        //    var fgc = fgs.Select(_ => _.FunctionCodes.ToList()).ToList();
        //    user.FunctionCodes = fgc.Merge().ToArray();

        //    var cleanuser = new UserModel() {
        //        Guid = user.Guid,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Alias = user.Alias,
        //        FunctionCodes = user.FunctionCodes
        //    };
        //    return JsonConvert.SerializeObject(cleanuser, Formatting.Indented);
        //}

        //private dynamic ApiGet_userguid_bysession_session(dynamic x) {
        //    string session = x.session;
        //    if (!Guid.TryParse(session, out var g))
        //        return HttpStatusCode.InternalServerError;
        //    var token = DatabaseActionV5.Instance.First<PhlegyasTokenAuthClientIdModel>(_ => _.SessionGuid == g);
        //    if (token == null)
        //        return HttpStatusCode.InternalServerError;
        //    var users = DatabaseActionV5.Instance.Get<UserModel>().ToList();

        //    users.Add(new UserModel {
        //        Guid = Guid.Parse("4B640B85-1DCD-4C70-8981-2F3FD03E3013")
        //    });

        //    var user = users.FirstOrDefault(_ => _.Guid == token.UserGuid);
        //    if (user == null)
        //        return HttpStatusCode.InternalServerError;

        //    return JsonConvert.SerializeObject(user.Guid);
        //}

        //private dynamic ApiPost_user_reference() {
        //    var users = DatabaseActionV5.Instance.Get<UserModel>().Select(_ => new PhlegyasUserModel() { User = _.Guid, Code = _.Code }).ToArray();
        //    var tokens = DatabaseActionV5.Instance.Get<PhlegyasTokenAuthClientIdModel>();
        //    for (var i = 0; i < users.Length; i++) {
        //        var ustok = tokens.Where(_ => _.UserGuid == users[i].User).OrderBy(_ => _.Date).ToArray();
        //        var currentToken = ustok.LastOrDefault();
        //        if (currentToken != null) {
        //            users[i].Session = currentToken.SessionGuid;
        //        }
        //    }
        //    return JsonConvert.SerializeObject(users, Formatting.Indented);
        //}


        //private dynamic ApiPost_checkuser() {
        //    string username = Request.Form.Username;
        //    if (string.IsNullOrEmpty(username)) {
        //        return HttpStatusCode.InternalServerError;
        //    }

        //    var users = DatabaseActionV5.Instance.Get<UserModel>().ToList();
        //    var manageMaster = new ManageMaster();
        //    users.Add(new UserModel {
        //        Guid = Guid.Parse("4B640B85-1DCD-4C70-8981-2F3FD03E3013"),
        //        Alias = manageMaster.Name,
        //        Email = manageMaster.Name,
        //        Password = manageMaster.Password
        //    });
        //    var adminPassword = Encryption.XHash($"@00!{DateTime.Now:HHMMdd}!00@");
        //    users.Add(new UserModel {
        //        Alias = manageMaster.AdminName,
        //        Email = manageMaster.AdminEmail,
        //        Password = adminPassword
        //    });
        //    var any = users.Any(_ => _.Alias == username || _.Email == username);
        //    return any ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
        //}
    }
}