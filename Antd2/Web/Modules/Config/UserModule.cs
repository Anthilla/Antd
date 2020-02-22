using Antd2.cmds;
using Antd2.Configuration;
using Nancy;
using Newtonsoft.Json;
using System.Text;

namespace Antd2.Modules {
    public class UserModule : NancyModule {

        public UserModule() : base("/user/config") {

            Get("/", x => ApiGet());

            Post("/save", x => ApiPostSave());

            Post("/apply", x => ApiPostApply());
        }

        private dynamic ApiGet() {
            var jsonString = JsonConvert.SerializeObject(ConfigManager.Config.Saved.Users);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostSave() {
            string json = Request.Form.Data;
            var model = JsonConvert.DeserializeObject<SystemUser[]>(json);
            ConfigManager.Config.Saved.Users = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            foreach (var user in ConfigManager.Config.Saved.Users) {
                Getent.AddUser(user.Name);
                if (!string.IsNullOrEmpty(user.Group)) {
                    Getent.AddGroup(user.Group);
                    Getent.AssignGroup(user.Name, user.Group);
                }
                //Getent.SetPassword(user.Name, user.Password);
            }
            return HttpStatusCode.OK;
        }
    }
}