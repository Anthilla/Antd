using Antd2.cmds;
using Antd2.Configuration;
using Antd2.Storage;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Antd2.Modules {
    public class WebdavModule : NancyModule {

        public class WebdavView {
            public List<WebdavParameters> Webdav { get; set; }
            public List<string> Targets { get; set; }
            public List<Configuration.SystemUser> Users { get; set; }
            public List<string> IpAddressList { get; set; }
        }

        public WebdavModule() : base("/webdav") {

            Get("/", x => ApiGet());

            Post("/save", x => ApiPostSave());

            Post("/start", x => ApiPostWebdavStart());

            Post("/stop", x => ApiPostWebdavStop());

        }

        private dynamic ApiGet() {
            var disks = Lsblk.Get();

            var targets = new List<string>();
            targets.Add("/Data");
            foreach (var disk in disks) {
                foreach (var partition in disk.Children) {
                    if (!string.IsNullOrEmpty(partition.Mountpoint))
                        targets.Add(partition.Mountpoint);
                }
            }

            var view = new WebdavView() {
                Targets = targets,
                Users = ConfigManager.Config.Saved.Users.ToList(),
                IpAddressList = Ip.GetIpAddressList(),
                Webdav = ConfigManager.Config.Saved.Webdav.ToList()
            };

            foreach (var wd in view.Webdav) {
                if (!StartCommand.WebdavStatus.ContainsKey(wd.Target))
                    wd.IsActive = false;
                else
                    wd.IsActive = StartCommand.WebdavStatus[wd.Target];

                wd.MappedUsers = new List<SystemUser>();
                foreach (var user in view.Users) {
                    user.IsSelected = wd.Users.Contains(user.Name);
                    wd.MappedUsers.Add(user);
                }
                wd.MappedUsers.OrderBy(_ => _.Name);
            }

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(view);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostSave() {
            string json = Request.Form.Data;
            var model = JsonConvert.DeserializeObject<WebdavParameters[]>(json);
            ConfigManager.Config.Saved.Webdav = model;
            ConfigManager.Config.Dump();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostWebdavStart() {
            string root = Request.Form.Mountpoint;

            var webdavConf = ConfigManager.Config.Saved.Webdav.FirstOrDefault(_ => _.Target == root);

            if (webdavConf.Enable == false)
                return HttpStatusCode.OK;

            var activeUsers = ConfigManager.Config.Saved.Users
                .Where(_ => webdavConf.Users.Contains(_.Name))
                .ToList();

            if (!activeUsers.Any())
                return HttpStatusCode.OK;

            webdavConf.MappedUsers = activeUsers;

            System.Threading.Tasks.Task.Factory.StartNew(() => {
                var wd = new WebDavServer(webdavConf);
                StartCommand.WebdavStatus[webdavConf.Target] = true;
                StartCommand.WebdavInstances[webdavConf.Target] = wd;
                wd.Start();
            });

            return HttpStatusCode.OK;
        }

        private dynamic ApiPostWebdavStop() {
            string root = Request.Form.Mountpoint;
            if (StartCommand.WebdavInstances.ContainsKey(root)) {
                if (StartCommand.WebdavInstances[root] != null) {
                    StartCommand.WebdavInstances[root].Stop();
                    StartCommand.WebdavInstances[root] = null;
                }
            }
            if (StartCommand.WebdavStatus.ContainsKey(root)) {
                StartCommand.WebdavStatus[root] = false;
            }
            return HttpStatusCode.OK;
        }
    }
}