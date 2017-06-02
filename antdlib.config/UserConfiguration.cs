using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public static class UserConfiguration {

        private static UserConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/users.conf";
        private const string MainFilePath = "/etc/shadow";

        public static UserConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new UserConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<UserConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new UserConfigurationModel();
            }
        }

        public static void Save(UserConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
        }

        public static void Import() {
            if(!File.Exists(MainFilePath)) { return; }
            var lines = File.ReadAllLines(MainFilePath);
            var users = new List<User>();
            foreach(var line in lines) {
                var info = line.Split(':');
                if(ServiceModel.Users.Any(_ => _.Name == info[0])) {
                    continue;
                }
                var mo = new User { Name = info[0], Password = info[1] };
                users.Add(mo);
            }
            ServiceModel.Users = users;
            Save(ServiceModel);
        }

        public static void Set() {
            if(!File.Exists(MainFilePath)) { return; }
            var lines = File.ReadAllLines(MainFilePath);
            var sysUser = new SystemUser();
            foreach(var user in ServiceModel.Users) {
                if(user.Password.Length < 2) {
                    continue;
                }
                var line = lines.FirstOrDefault(_ => _.StartsWith(user.Name));
                if(line == null) {
                    sysUser.Create(user.Name);
                    sysUser.SetPassword(user.Name, user.Password);
                    continue;
                }
                if(!line.Contains(user.Password)) {
                    sysUser.SetPassword(user.Name, user.Password);
                }
            }
        }

        public static List<User> Get() {
            return ServiceModel == null ? new List<User>() : ServiceModel.Users;
        }

        public static void AddUser(User user) {
            var users = ServiceModel.Users;
            if(users.Any(_ => _.Name == user.Name)) {
                RemoveUser(user.Name);
            }
            users.Add(user);
            ServiceModel.Users = users;
            Save(ServiceModel);
            var lines = File.ReadAllLines(MainFilePath);
            var sysUser = new SystemUser();
            var line = lines.FirstOrDefault(_ => _.StartsWith(user.Name));
            if(line == null) {
                sysUser.Create(user.Name);
            }
            sysUser.SetPassword(user.Name, user.Password);
        }

        public static void RemoveUser(string name) {
            var users = ServiceModel.Users;
            var model = users.First(_ => _.Name == name);
            if(model == null) {
                return;
            }
            users.Remove(model);
            ServiceModel.Users = users;
            Save(ServiceModel);
        }
    }
}
