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
        private static readonly string ModprobesFile = $"{Parameter.AntdCfgParameters}/modprobes.conf";

        public static void SetModprobesList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(ModprobesFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] modprobes configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetModprobesList() {
            if(!File.Exists(ModprobesFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(ModprobesFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] modprobes configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion

        #region [    rmmod    ]
        private static readonly string RmmodFile = $"{Parameter.AntdCfgParameters}/rmmod.conf";

        public static void SetRmmodList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(RmmodFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] rmmod configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetRmmodList() {
            if(!File.Exists(RmmodFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(RmmodFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] rmmod configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion

        #region [    modulesblacklist    ]
        private static readonly string ModulesblacklistFile = $"{Parameter.AntdCfgParameters}/modulesblacklist.conf";

        public static void SetModulesBlacklistList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(ModulesblacklistFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] modulesblacklist configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetModulesBlacklistList() {
            if(!File.Exists(ModulesblacklistFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(ModulesblacklistFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] modulesblacklist configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion}

        #region [    osparameters    ]
        private static readonly string OsparametersFile = $"{Parameter.AntdCfgParameters}/osparameters.conf";

        public static void SetOsParametersList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(OsparametersFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] osparameters configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetOsParametersList() {
            if(!File.Exists(OsparametersFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(OsparametersFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] osparameters configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion}

        #region [    servicesstart    ]
        private static readonly string ServicesstartFile = $"{Parameter.AntdCfgParameters}/servicesstart.conf";

        public static void SetServicesStartList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(ServicesstartFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] servicesstart configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetServicesStartList() {
            if(!File.Exists(ServicesstartFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(ServicesstartFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] servicesstart configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion

        #region [    servicesstop    ]
        private static readonly string ServicesstopFile = $"{Parameter.AntdCfgParameters}/servicesstop.conf";

        public static void SetServicesStopList(List<string> objects) {
            var lines = objects;
            try {
                FileWithAcl.WriteAllLines(ServicesstopFile, lines, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] servicesstop configuration set error: {ex.Message}");
            }
        }

        private static List<string> GetServicesStopList() {
            if(!File.Exists(ServicesstopFile)) {
                return new List<string>();
            }
            try {
                var lines = File.ReadAllLines(ServicesstopFile).ToList();
                return lines;
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] servicesstop configuration get error: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion

        #region [    startcommands    ]
        private static readonly string StartcommandsFile = $"{Parameter.AntdCfgParameters}/startcommands.conf";

        public static void SetStartCommandsList(List<Control> commands) {
            var text = JsonConvert.SerializeObject(commands, Formatting.Indented);
            try {
                FileWithAcl.WriteAllText(StartcommandsFile, text, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] startcommands configuration set error: {ex.Message}");
            }
        }

        private static List<Control> GetStartCommandsList() {
            if(!File.Exists(StartcommandsFile)) {
                return new List<Control>();
            }
            try {
                var text = File.ReadAllText(StartcommandsFile);
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
        private static readonly string EndcommandsFile = $"{Parameter.AntdCfgParameters}/endcommands.conf";

        public static void SetEndCommandsList(List<Control> commands) {
            var text = JsonConvert.SerializeObject(commands, Formatting.Indented);
            try {
                FileWithAcl.WriteAllText(EndcommandsFile, text, "644", "root", "wheel");
            }
            catch(Exception ex) {
                ConsoleLogger.Error($"[host parameters] endcommands configuration set error: {ex.Message}");
            }
        }

        private static List<Control> GetEndCommandsList() {
            if(!File.Exists(EndcommandsFile)) {
                return new List<Control>();
            }
            try {
                var text = File.ReadAllText(EndcommandsFile);
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
