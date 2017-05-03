using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.models;
using anthilla.commands;

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
                ServicesStart = GetServicesStartList(),
                ServicesStop = GetServicesStopList(),

            };
            return hostParameters;
        }
        #endregion


        #region [    modprobes    ]
        private readonly string _modprobesFile = $"{Parameter.AntdCfgParameters}/modprobes.conf";
        private readonly string _modprobesFileBackup = $"{Parameter.AntdCfgParameters}/modprobes.conf.bck";

        public void SetModprobesList(List<string> modules) {
            var lines = modules;
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

        public void SetRmmodList(List<string> modules) {
            var lines = modules;
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

        public void SetModulesBlacklistList(List<string> modules) {
            var lines = modules;
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

        #region [    servicesstart    ]
        private readonly string _servicesstartFile = $"{Parameter.AntdCfgParameters}/servicesstart.conf";
        private readonly string _servicesstartFileBackup = $"{Parameter.AntdCfgParameters}/servicesstart.conf.bck";

        public void SetServicesStartList(List<string> modules) {
            var lines = modules;
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

        public void SetServicesStopList(List<string> modules) {
            var lines = modules;
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
    }
}
