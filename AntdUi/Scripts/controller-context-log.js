"use strict";

app.controller("LogJournaldController", ["$scope", "$http", LogJournaldController]);

function LogJournaldController($scope, $http) {
    $scope.saveOptions = function (journald) {
        var data = $.param({
            Storage: journald.Storage,
            Compress: journald.Compress,
            Seal: journald.Seal,
            SplitMode: journald.SplitMode,
            SyncIntervalSec: journald.SyncIntervalSec,
            RateLimitInterval: journald.RateLimitInterval,
            RateLimitBurst: journald.RateLimitBurst,
            SystemMaxUse: journald.SystemMaxUse,
            SystemKeepFree: journald.SystemKeepFree,
            SystemMaxFileSize: journald.SystemMaxFileSize,
            RuntimeMaxUse: journald.RuntimeMaxUse,
            RuntimeKeepFree: journald.RuntimeKeepFree,
            RuntimeMaxFileSize: journald.RuntimeMaxFileSize,
            MaxRetentionSec: journald.MaxRetentionSec,
            MaxFileSec: journald.MaxFileSec,
            ForwardToSyslog: journald.ForwardToSyslog,
            ForwardToKMsg: journald.ForwardToKMsg,
            ForwardToConsole: journald.ForwardToConsole,
            ForwardToWall: journald.ForwardToWall,
            TTYPath: journald.TTYPath,
            MaxLevelStore: journald.MaxLevelStore,
            MaxLevelSyslog: journald.MaxLevelSyslog,
            MaxLevelKMsg: journald.MaxLevelKMsg,
            MaxLevelConsole: journald.MaxLevelConsole,
            MaxLevelWall: journald.MaxLevelWall
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/journald/options", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/journald/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/journald/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/journald/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/journald/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/journald/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/journald").success(function (data) {
        $scope.isActive = data.JournaldIsActive;
        $scope.Journald = data.JournaldOptions;
    });
}

app.controller("LogJournalController", ["$scope", "$http", LogJournalController]);

function LogJournalController($scope, $http) {
    $http.get("/journal").success(function (data) {
        $scope.Logs = data.Logs;
    });
}

app.controller("LogController", ["$scope", "$http", LogController]);

function LogController($scope, $http) {
    $http.get("/log").success(function (data) {
        $scope.Logs = data.Logs;
        console.log(data.Logs.length);
        console.log($scope.Logs.length);
    });
}

app.controller("LogReportController", ["$scope", "$http", LogReportController]);

function LogReportController($scope, $http) {
    $scope.create = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/report").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/report").success(function (data) {
        $scope.Report = data.LogReports;
    });
}

app.controller("LogSyslogNgController", ["$scope", "$http", LogSyslogNgController]);

function LogSyslogNgController($scope, $http) {
    $scope.saveOptions = function (syslogng) {
        var data = $.param({
            Threaded: syslogng.Threaded,
            ChainHostname: syslogng.ChainHostname,
            StatsFrequency: syslogng.StatsFrequency,
            MarkFrequency: syslogng.MarkFrequency,
            CheckHostname: syslogng.CheckHostname,
            CreateDirectories: syslogng.CreateDirectories,
            DnsCache: syslogng.DnsCache,
            KeepHostname: syslogng.KeepHostname,
            DirAcl: syslogng.DirAcl,
            Acl: syslogng.Acl,
            UseDns: syslogng.UseDns,
            UseFqdn: syslogng.UseFqdn,
            RootPath: syslogng.RootPath,
            PortLevelApplication: syslogng.PortLevelApplication,
            PortLevelSecurity: syslogng.PortLevelSecurity,
            PortLevelSystem: syslogng.PortLevelSystem
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syslogng/options", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syslogng/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syslogng/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syslogng/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syslogng/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syslogng/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/syslogng").success(function (data) {
        $scope.isActive = data.SyslogNgIsActive;
        $scope.SyslogNg = data.SyslogNgOptions;
    });
}