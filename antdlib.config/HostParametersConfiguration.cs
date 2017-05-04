using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;

namespace antdlib.config {
    public class HostParametersConfiguration {

        public HostParameters Conf;

        private readonly string _dir = Parameter.AntdCfgParameters;

        public HostParametersConfiguration() {
            Directory.CreateDirectory(_dir);
            Conf = Parse();
        }

        #region [    HostParameters conf   ]
        private HostParameters Parse() {
            var hostParameters = new HostParameters {
                Modprobes = GetModprobesList(),
                Rmmod = GetRmmodList(),
                ModulesBlacklist = GetModulesBlacklistList(),
                OsParameters = GetOsParametersList(),
                ServicesStart = GetServicesStartList(),
                ServicesStop = GetServicesStopList(),
                StartCommands = GetStartCommandsList(),
                EndCommands = GetEndCommandsList()
            };
            return hostParameters;
        }
        #endregion

        #region [    modprobes    ]
        private readonly string _modprobesFile = $"{Parameter.AntdCfgParameters}/modprobes.conf";
        private readonly string _modprobesFileBackup = $"{Parameter.AntdCfgParameters}/modprobes.conf.bck";

        public void SetModprobesList(List<string> objects) {
            var lines = objects;
            if(File.Exists(_modprobesFile)) {
                File.Copy(_modprobesFile, _modprobesFileBackup, true);
            }
            try {
                File.WriteAllLines(_modprobesFile, lines);
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] modprobes configuration set error: {ex.Message}");
            }
        }

        private List<string> GetModprobesList() {
            if(!File.Exists(_modprobesFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(_modprobesFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] modprobes configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion

        #region [    rmmod    ]
        private readonly string _rmmodFile = $"{Parameter.AntdCfgParameters}/rmmod.conf";
        private readonly string _rmmodFileBackup = $"{Parameter.AntdCfgParameters}/rmmod.conf.bck";

        public void SetRmmodList(List<string> objects) {
            var lines = objects;
            if(File.Exists(_rmmodFile)) {
                File.Copy(_rmmodFile, _rmmodFileBackup, true);
            }
            try {
                File.WriteAllLines(_rmmodFile, lines);
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] rmmod configuration set error: {ex.Message}");
            }
        }

        private List<string> GetRmmodList() {
            if(!File.Exists(_rmmodFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(_rmmodFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] rmmod configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion

        #region [    modulesblacklist    ]
        private readonly string _modulesblacklistFile = $"{Parameter.AntdCfgParameters}/modulesblacklist.conf";
        private readonly string _modulesblacklistFileBackup = $"{Parameter.AntdCfgParameters}/modulesblacklist.conf.bck";

        public void SetModulesBlacklistList(List<string> objects) {
            var lines = objects;
            if(File.Exists(_modulesblacklistFile)) {
                File.Copy(_modulesblacklistFile, _modulesblacklistFileBackup, true);
            }
            try {
                File.WriteAllLines(_modulesblacklistFile, lines);
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] modulesblacklist configuration set error: {ex.Message}");
            }
        }

        private List<string> GetModulesBlacklistList() {
            if(!File.Exists(_modulesblacklistFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(_modulesblacklistFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] modulesblacklist configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion}

        #region [    osparameters    ]
        private readonly string _osparametersFile = $"{Parameter.AntdCfgParameters}/osparameters.conf";
        private readonly string _osparametersFileBackup = $"{Parameter.AntdCfgParameters}/osparameters.conf.bck";

        public void SetOsParametersList(List<string> objects) {
            var lines = objects;
            if(File.Exists(_osparametersFile)) {
                File.Copy(_osparametersFile, _osparametersFileBackup, true);
            }
            try {
                File.WriteAllLines(_osparametersFile, lines);
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] osparameters configuration set error: {ex.Message}");
            }
        }

        private List<string> GetOsParametersList() {
            if(!File.Exists(_osparametersFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(_osparametersFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] osparameters configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion}

        #region [    servicesstart    ]
        private readonly string _servicesstartFile = $"{Parameter.AntdCfgParameters}/servicesstart.conf";
        private readonly string _servicesstartFileBackup = $"{Parameter.AntdCfgParameters}/servicesstart.conf.bck";

        public void SetServicesStartList(List<string> objects) {
            var lines = objects;
            if(File.Exists(_servicesstartFile)) {
                File.Copy(_servicesstartFile, _servicesstartFileBackup, true);
            }
            try {
                File.WriteAllLines(_servicesstartFile, lines);
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] servicesstart configuration set error: {ex.Message}");
            }
        }

        private List<string> GetServicesStartList() {
            if(!File.Exists(_servicesstartFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(_servicesstartFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] servicesstart configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion

        #region [    servicesstop    ]
        private readonly string _servicesstopFile = $"{Parameter.AntdCfgParameters}/servicesstop.conf";
        private readonly string _servicesstopFileBackup = $"{Parameter.AntdCfgParameters}/servicesstop.conf.bck";

        public void SetServicesStopList(List<string> objects) {
            var lines = objects;
            if(File.Exists(_servicesstopFile)) {
                File.Copy(_servicesstopFile, _servicesstopFileBackup, true);
            }
            try {
                File.WriteAllLines(_servicesstopFile, lines);
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] servicesstop configuration set error: {ex.Message}");
            }
        }

        private List<string> GetServicesStopList() {
            if(!File.Exists(_servicesstopFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(_servicesstopFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] servicesstop configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion

        #region [    startcommands    ]
        private readonly string _startcommandsFile = $"{Parameter.AntdCfgParameters}/startcommands.conf";
        private readonly string _startcommandsFileBackup = $"{Parameter.AntdCfgParameters}/startcommands.conf.bck";

        public void SetStartCommandsList(List<Control> commands) {
            var text = JsonConvert.SerializeObject(commands, Formatting.Indented);
            if(File.Exists(_startcommandsFile)) {
                File.Copy(_startcommandsFile, _startcommandsFileBackup, true);
            }
            try {
                File.WriteAllText(_startcommandsFile, text);
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] startcommands configuration set error: {ex.Message}");
            }
        }

        private List<Control> GetStartCommandsList() {
            if(!File.Exists(_startcommandsFile)) {
                return new List<Control>();
            }
            try {
                var text = File.ReadAllText(_startcommandsFile);
                var objects = JsonConvert.DeserializeObject<List<Control>>(text);
                return objects;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] startcommands configuration get error: {ex.Message}");
                return new List<Control>();
            }
        }
        #endregion

        #region [    endcommands    ]
        private readonly string _endcommandsFile = $"{Parameter.AntdCfgParameters}/endcommands.conf";
        private readonly string _endcommandsFileBackup = $"{Parameter.AntdCfgParameters}/endcommands.conf.bck";

        public void SetEndCommandsList(List<Control> commands) {
            var text = JsonConvert.SerializeObject(commands, Formatting.Indented);
            if(File.Exists(_endcommandsFile)) {
                File.Copy(_endcommandsFile, _endcommandsFileBackup, true);
            }
            try {
                File.WriteAllText(_endcommandsFile, text);
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] endcommands configuration set error: {ex.Message}");
            }
        }

        private List<Control> GetEndCommandsList() {
            if(!File.Exists(_endcommandsFile)) {
                return new List<Control>();
            }
            try {
                var text = File.ReadAllText(_endcommandsFile);
                var objects = JsonConvert.DeserializeObject<List<Control>>(text);
                return objects;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] endcommands configuration get error: {ex.Message}");
                return new List<Control>();
            }
        }
        #endregion
    }
}
