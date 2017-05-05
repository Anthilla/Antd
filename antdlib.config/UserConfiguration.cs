using Antd.Users;
using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.config {
    public static class UserConfiguration {

        private static UserConfigurationModel _serviceModel => Load();

        private static readonly string _cfgFile = $"{Parameter.AntdCfgServices}/users.conf";
        private static readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/users.conf.bck";
        private const string MainFilePath = "/etc/shadow";

        public static UserConfigurationModel Load() {
            if(!File.Exists(_cfgFile)) {
                return new UserConfigurationModel();
            }
            try {
                var text = File.ReadAllText(_cfgFile);
                var obj = JsonConvert.DeserializeObject<UserConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new UserConfigurationModel();
            }
        }

        public static void Save(UserConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
        }

        public static void Import() {
            if(!File.Exists(MainFilePath)) { return; }
            var lines = File.ReadAllLines(MainFilePath);
            var users = new List<User>();
            foreach(var line in lines) {
                var info = line.Split(':');
                if(_serviceModel.Users.Any(_ => _.Name == info[0])) {
                    continue;
                }
                var mo = new User { Name = info[0], Password = info[1] };
                users.Add(mo);
            }
            _serviceModel.Users = users;
            Save(_serviceModel);
        }

        public static void Set() {
            if(!File.Exists(MainFilePath)) { return; }
            var lines = File.ReadAllLines(MainFilePath);
            var sysUser = new SystemUser();
            foreach(var user in _serviceModel.Users) {
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
            return _serviceModel == null ? new List<User>() : _serviceModel.Users;
        }

        public static void AddUser(User user) {
            var users = _serviceModel.Users;
            if(users.Any(_ => _.Name == user.Name)) {
                RemoveUser(user.Name);
            }
            users.Add(user);
            _serviceModel.Users = users;
            Save(_serviceModel);
            var lines = File.ReadAllLines(MainFilePath);
            var sysUser = new SystemUser();
            var line = lines.FirstOrDefault(_ => _.StartsWith(user.Name));
            if(line == null) {
                sysUser.Create(user.Name);
            }
            sysUser.SetPassword(user.Name, user.Password);
        }

        public static void RemoveUser(string name) {
            var users = _serviceModel.Users;
            var model = users.First(_ => _.Name == name);
            if(model == null) {
                return;
            }
            users.Remove(model);
            _serviceModel.Users = users;
            Save(_serviceModel);
        }
    }
}
