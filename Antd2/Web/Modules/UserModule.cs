using Nancy;

namespace Antd2.Modules {
    public class UserModule : NancyModule {

        public UserModule() : base("/user") {

            Get("/get/system", x => ApiGetSystem());

            Get("/get/group", x => ApiGetGroup());

            Get("/get/group/running", x => ApiGetGroupRunning());

            Post("/apply/system", x => ApiPostApplySystem());

            Post("/apply/group", x => ApiPostApplyGroup());

            Post("/save/system", x => ApiPostSaveSystem());

            Post("/save/group", x => ApiPostSaveGroup());
        }

        private dynamic ApiGetSystem() {
            return Response.AsJson((object)Application.CurrentConfiguration.Users.SystemUsers);
        }

        private dynamic ApiGetGroup() {
            return Response.AsJson((object)Application.CurrentConfiguration.Users.SystemGroups);
        }

        private dynamic ApiGetGroupRunning() {
            return Response.AsJson((object)Application.RunningConfiguration.Users.SystemGroups);
        }
        private dynamic ApiPostApplySystem() {
            //Passwd.Set();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApplyGroup() {
            //Group.Set();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostSaveSystem() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<SystemUser[]>(data);
            //Application.CurrentConfiguration.Users.SystemUsers = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostSaveGroup() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<SystemGroup[]>(data);
            //Application.CurrentConfiguration.Users.SystemGroups = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }
    }
}