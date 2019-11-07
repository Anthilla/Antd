using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class UserModule : NancyModule {

        public UserModule() : base("/user") {

            Get["/get/system"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Users.SystemUsers);
            };

            Get["/get/group"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Users.SystemGroups);
            };

            Get["/get/group/running"] = x => {
                return JsonConvert.SerializeObject(Application.RunningConfiguration.Users.SystemGroups);
            };

            Get["/get/applicative"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Users.ApplicativeUsers);
            };

            Post["/apply/system"] = x => {
                Passwd.Set();
                return HttpStatusCode.OK;
            };

            Post["/apply/group"] = x => {
                Group.Set();
                return HttpStatusCode.OK;
            };

            Post["/save/system"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SystemUser[]>(data);
                Application.CurrentConfiguration.Users.SystemUsers = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/group"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SystemGroup[]>(data);
                Application.CurrentConfiguration.Users.SystemGroups = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/save/applicative"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<ApplicativeUser[]>(data);
                Application.CurrentConfiguration.Users.ApplicativeUsers = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Get["/get/password"] = x => {
                string data = Request.Form.Data;
                var pwd = Passwd.HashPasswd(data);
                return Response.AsText(pwd);
            };

            #region [    Authentication   ]
            Post["/authenticate"] = x => {
                string data = Request.Form.Data;
                var model = JsonConvert.DeserializeObject<AuthenticationDataModel>(data);
                return Login.Authenticate(model.Id, model.Claims);
            };

            Get["/get/applicative"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Users.ApplicativeUsers);
            };
            #endregion
        }
    }
}