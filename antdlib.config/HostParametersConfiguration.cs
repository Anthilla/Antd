using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;

namespace antdlib.config {
    public class HostParametersConfiguration {

        public static HostParameters Conf => Parse();

        #region [    HostParameters conf   ]
        private static HostParameters Parse() {
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
        private static readonly string _modprobesFile = $"{Parameter.AntdCfgParameters}/modprobes.conf";
        private static readonly string _modprobesFileBackup = $"{Parameter.AntdCfgParameters}/modprobes.conf.bck";

        public static void SetModprobesList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(_modprobesFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] modprobes configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetModprobesList() {
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
        private static readonly string _rmmodFile = $"{Parameter.AntdCfgParameters}/rmmod.conf";
        private static readonly string _rmmodFileBackup = $"{Parameter.AntdCfgParameters}/rmmod.conf.bck";

        public static void SetRmmodList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(_rmmodFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] rmmod configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetRmmodList() {
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
        private static readonly string _modulesblacklistFile = $"{Parameter.AntdCfgParameters}/modulesblacklist.conf";
        private static readonly string _modulesblacklistFileBackup = $"{Parameter.AntdCfgParameters}/modulesblacklist.conf.bck";

        public static void SetModulesBlacklistList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(_modulesblacklistFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] modulesblacklist configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetModulesBlacklistList() {
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
        private static readonly string _osparametersFile = $"{Parameter.AntdCfgParameters}/osparameters.conf";
        private static readonly string _osparametersFileBackup = $"{Parameter.AntdCfgParameters}/osparameters.conf.bck";

        public static void SetOsParametersList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(_osparametersFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] osparameters configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetOsParametersList() {
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
        private static readonly string _servicesstartFile = $"{Parameter.AntdCfgParameters}/servicesstart.conf";
        private static readonly string _servicesstartFileBackup = $"{Parameter.AntdCfgParameters}/servicesstart.conf.bck";

        public static void SetServicesStartList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(_servicesstartFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] servicesstart configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetServicesStartList() {
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
        private static readonly string _servicesstopFile = $"{Parameter.AntdCfgParameters}/servicesstop.conf";
        private static readonly string _servicesstopFileBackup = $"{Parameter.AntdCfgParameters}/servicesstop.conf.bck";

        public static void SetServicesStopList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(_servicesstopFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] servicesstop configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetServicesStopList() {
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
        private static readonly string _startcommandsFile = $"{Parameter.AntdCfgParameters}/startcommands.conf";
        private static readonly string _startcommandsFileBackup = $"{Parameter.AntdCfgParameters}/startcommands.conf.bck";

        public static void SetStartCommandsList(List<Control> commands) {
            var text = JsonConvert.SerializeObject(commands, Formatting.Indented);
            try {
                FileWithAcl.WriteAllText(_startcommandsFile, text, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] startcommands configuration set error: {ex.Message}");
            }
        }

        private static List<Control> GetStartCommandsList() {
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
        private static readonly string _endcommandsFile = $"{Parameter.AntdCfgParameters}/endcommands.conf";
        private static readonly string _endcommandsFileBackup = $"{Parameter.AntdCfgParameters}/endcommands.conf.bck";

        public static void SetEndCommandsList(List<Control> commands) {
            var text = JsonConvert.SerializeObject(commands, Formatting.Indented);
            try {
                FileWithAcl.WriteAllText(_endcommandsFile, text, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] endcommands configuration set error: {ex.Message}");
            }
        }

        private static List<Control> GetEndCommandsList() {
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
