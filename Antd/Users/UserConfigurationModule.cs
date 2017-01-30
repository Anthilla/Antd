using antdlib.models;
using Nancy;
using Nancy.Security;
using Newtonsoft.Json;

namespace Antd.Users {

    public class UserConfigurationModule : NancyModule {

        private readonly UserConfiguration _userConfiguration = new UserConfiguration();

        public UserConfigurationModule() {

            Get["/users"] = _ => {
                var list = _userConfiguration.Get();
                var manageMaster = new ManageMaster();
                list.Add(new User {
                    Name = manageMaster.Name,
                    Password = manageMaster.Password
                });
                return JsonConvert.SerializeObject(list);
            };

            Get["/users/system"] = x => {
                this.RequiresAuthentication();
                var systemUser = new SystemUser();
                return JsonConvert.SerializeObject(systemUser.GetAll());
            };

            Post["/users/system"] = x => {
                this.RequiresAuthentication();
                var user = (string)Request.Form.User;
                var password = (string)Request.Form.Password;
                if(string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password)) {
                    return HttpStatusCode.BadRequest;
                }
                var systemUser = new SystemUser();
                var hpwd = systemUser.HashPasswd(password);
                var mo = new User {
                    Name = user,
                    Password = hpwd
                };
                var userConfiguration = new UserConfiguration();
                userConfiguration.AddUser(mo);
                return Response.AsRedirect("/");
            };

            Post["/master/change/password"] = x => {
                this.RequiresAuthentication();
                var password = (string)Request.Form.Password;
                if(string.IsNullOrEmpty(password)) {
                    return HttpStatusCode.BadRequest;
                }
                var masterManager = new ManageMaster();
                masterManager.ChangePassword(password);
                return HttpStatusCode.OK;
            };
        }
    }
}