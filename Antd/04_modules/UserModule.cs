using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class UserModule : NancyModule {

        public UserModule() : base("/user") {

            Post["/apply"] = x => {
                Passwd.Set();
                return HttpStatusCode.OK;
            };

            Post["/set/users/applicative"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<ApplicativeUser[]>(data);
                Application.CurrentConfiguration.Users.ApplicativeUsers = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/set/users/system"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<SystemUser[]>(data);
                Application.CurrentConfiguration.Users.SystemUsers = objects;
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