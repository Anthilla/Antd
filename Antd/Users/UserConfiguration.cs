using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using Newtonsoft.Json;
using IoDir = System.IO.Directory;

namespace Antd.Users {
    public class UserConfiguration {

        private readonly UserConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/users.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/users.conf.bck";
        private const string MainFilePath = "/etc/shadow";

        public UserConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new UserConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<UserConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new UserConfigurationModel();
                }
            }
        }

        public void Save(UserConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
        }

        public void Import() {
            var lines = File.ReadAllLines(MainFilePath);
            var users = new List<User>();
            foreach(var line in lines) {
                if(line.StartsWith("root"))
                    continue;
                var arr = line.Split(':');
                var name = arr[0];
                if(_serviceModel.Users.Any(_ => _.Name == name))
                    continue;
                var pwd = arr[1];
                var mo = new User { Name = name, Password = pwd };
                users.Add(mo);
                _serviceModel.Users = users;
            }
            Save(_serviceModel);
        }

        public void Set() {
            var lines = File.ReadAllLines(MainFilePath);
            var newLines = new List<string>();
            var missingUsers = new List<User>();
            foreach(var user in _serviceModel.Users) {
                var name = user.Name;
                var line = lines.FirstOrDefault(_ => _.Contains(name));
                if(line == null) {
                    //utente mancante dal sistema quindi lo gestisco dopo
                    missingUsers.Add(user);
                    continue;
                }
                var oldPassw = line.Split(':')[1];
                var pwd = user.Password;
                var replaced = line.Replace(oldPassw, pwd);
                newLines.Add(replaced);
            }
            File.WriteAllLines(MainFilePath, newLines);
            var sysUser = new SystemUser();
            foreach(var user in missingUsers) {
                sysUser.Create(user.Name);
                sysUser.SetPassword(user.Name, user.Password);
            }
        }

        public List<User> Get() {
            return _serviceModel == null ? new List<User>() : _serviceModel.Users;
        }

        public void AddUser(User model) {
            var resources = _serviceModel.Users;
            if(resources.Any(_ => _.Name == model.Name)) {
                RemoveUser(model.Name);
            }
            resources.Add(model);
            _serviceModel.Users = resources;
            Save(_serviceModel);
            Set();
        }

        public void RemoveUser(string name) {
            var resources = _serviceModel.Users;
            var model = resources.First(_ => _.Name == name);
            if(model == null) {
                return;
            }
            resources.Remove(model);
            _serviceModel.Users = resources;
            Save(_serviceModel);
            Set();
        }
    }
}
