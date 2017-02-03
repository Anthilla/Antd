"use strict";

app.controller("AntdAclController", ["$scope", "$http", AntdAclController]);

function AntdAclController($scope, $http) {
    $scope.GetDetails = function (guid) {
        $http.get("/acl/get/" + guid).success(function (data) {
            return data;
        });
    }

    $scope.AclApplyScript = function (user) {
        var data = $.param({
            User: user
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/apply/script", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.AclDelete = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/delete", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.AclCreate = function (guid, acl) {
        var data = $.param({
            Guid: guid,
            Acl: acl
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/create", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.AclAdd = function (path) {
        var data = $.param({
            Path: path
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/add", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/acl").success(function (data) {
        $scope.Acl = data;
    });
}

app.controller("AntdBindController", ["$scope", "$http", AntdBindController]);

function AntdBindController($scope, $http) {
    $scope.deleteZone = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/zone/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addZone = function (el) {
        var data = $.param({
            Name: el.Name,
            Type: el.Type,
            File: el.File,
            NameSerialUpdateMethod: el.NameSerialUpdateMethod,
            AllowUpdate: el.AllowUpdate,
            AllowQuery: el.AllowQuery,
            AllowTransfer: el.AllowTransfer
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/zone", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.saveOptions = function (bind) {
        var data = $.param({
            Notify: bind.Notify,
            MaxCacheSize: bind.MaxCacheSize,
            MaxCacheTtl: bind.MaxCacheTtl,
            MaxNcacheTtl: bind.MaxNcacheTtl,
            Forwarders: bind.Forwarders,
            AllowNotify: bind.AllowNotify,
            AllowTransfer: bind.AllowTransfer,
            Recursion: bind.Recursion,
            TransferFormat: bind.TransferFormat,
            QuerySourceAddress: bind.QuerySourceAddress,
            QuerySourcePort: bind.QuerySourcePort,
            Version: bind.Version,
            AllowQuery: bind.AllowQuery,
            AllowRecursion: bind.AllowRecursion,
            IxfrFromDifferences: bind.IxfrFromDifferences,
            ListenOnV6: bind.ListenOnV6,
            ListenOnPort53: bind.ListenOnPort53,
            DnssecEnabled: bind.DnssecEnabled,
            DnssecValidation: bind.DnssecValidation,
            DnssecLookaside: bind.DnssecLookaside,
            AuthNxdomain: bind.AuthNxdomain,
            KeyName: bind.KeyName,
            KeySecret: bind.KeySecret,
            ControlAcl: bind.ControlAcl,
            ControlIp: bind.ControlIp,
            ControlPort: bind.ControlPort,
            ControlAllow: bind.ControlAllow,
            LoggingChannel: bind.LoggingChannel,
            LoggingDaemon: bind.LoggingDaemon,
            LoggingSeverity: bind.LoggingSeverity,
            LoggingPrintCategory: bind.LoggingPrintCategory,
            LoggingPrintSeverity: bind.LoggingPrintSeverity,
            LoggingPrintTime: bind.LoggingPrintTime,
            TrustedKey: bind.TrustedKey,
            AclLocalInterfaces: bind.AclLocalInterfaces,
            AclInternalInterfaces: bind.AclInternalInterfaces,
            AclExternalInterfaces: bind.AclExternalInterfaces,
            AclLocalNetworks: bind.AclLocalNetworks,
            AclInternalNetworks: bind.AclInternalNetworks,
            AclExternalNetworks: bind.AclExternalNetworks
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/options", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/bind").success(function (data) {
        $scope.Bind = data;
    });
}

app.controller("AntdDhcpdController", ["$scope", "$http", AntdDhcpdController]);

function AntdDhcpdController($scope, $http) {
    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/dhcpd").success(function (data) {
        $scope.Dhcpd = data;
    });
}

app.controller("AntdDhcpLeasesController", ["$scope", "$http", AntdDhcpLeasesController]);

function AntdDhcpLeasesController($scope, $http) {

}

app.controller("AntdDiskUsageController", ["$scope", "$http", AntdDiskUsageController]);

function AntdDiskUsageController($scope, $http) {

}

app.controller("AntdFirewallController", ["$scope", "$http", AntdFirewallController]);

function AntdFirewallController($scope, $http) {

}

app.controller("AntdGlusterController", ["$scope", "$http", AntdGlusterController]);

function AntdGlusterController($scope, $http) {

}

app.controller("AntdNameServiceController", ["$scope", "$http", AntdNameServiceController]);

function AntdNameServiceController($scope, $http) {

}

app.controller("AntdNetworkController", ["$scope", "$http", AntdNetworkController]);

function AntdNetworkController($scope, $http) {

}

app.controller("AntdRsyncController", ["$scope", "$http", AntdRsyncController]);

function AntdRsyncController($scope, $http) {

}

app.controller("AntdSambaController", ["$scope", "$http", AntdSambaController]);

function AntdSambaController($scope, $http) {

}

app.controller("AntdSchedulerController", ["$scope", "$http", AntdSchedulerController]);

function AntdSchedulerController($scope, $http) {

}

app.controller("AntdSshdController", ["$scope", "$http", AntdSshdController]);

function AntdSshdController($scope, $http) {

}

app.controller("AntdStorageController", ["$scope", "$http", AntdStorageController]);

function AntdStorageController($scope, $http) {

}

app.controller("AntdVmController", ["$scope", "$http", AntdVmController]);

function AntdVmController($scope, $http) {

}

app.controller("AntdVpnController", ["$scope", "$http", AntdVpnController]);

function AntdVpnController($scope, $http) {

}

app.controller("AntdZfsController", ["$scope", "$http", AntdZfsController]);

function AntdZfsController($scope, $http) {

}

app.controller("AppsDetectController", ["$scope", "$http", AppsDetectController]);

function AppsDetectController($scope, $http) {

}

app.controller("AppsManagementController", ["$scope", "$http", AppsManagementController]);

function AppsManagementController($scope, $http) {

}

app.controller("AssetDiscoveryController", ["$scope", "$http", AssetDiscoveryController]);

function AssetDiscoveryController($scope, $http) {

}

app.controller("AssetScanController", ["$scope", "$http", AssetScanController]);

function AssetScanController($scope, $http) {

}

app.controller("AssetSettingController", ["$scope", "$http", AssetSettingController]);

function AssetSettingController($scope, $http) {

}

app.controller("AssetSyncMachineController", ["$scope", "$http", AssetSyncMachineController]);

function AssetSyncMachineController($scope, $http) {

}

app.controller("BootCommandsController", ["$scope", "$http", BootCommandsController]);

function BootCommandsController($scope, $http) {

}

app.controller("BootModulesController", ["$scope", "$http", BootModulesController]);

function BootModulesController($scope, $http) {

}

app.controller("BootOsParametersController", ["$scope", "$http", BootOsParametersController]);

function BootOsParametersController($scope, $http) {

}

app.controller("BootServicesController", ["$scope", "$http", BootServicesController]);

function BootServicesController($scope, $http) {

}

app.controller("CaController", ["$scope", "$http", CaController]);

function CaController($scope, $http) {

}

app.controller("LogJournaldController", ["$scope", "$http", LogJournaldController]);

function LogJournaldController($scope, $http) {

}

app.controller("LogJournalController", ["$scope", "$http", LogJournalController]);

function LogJournalController($scope, $http) {

}

app.controller("LogController", ["$scope", "$http", LogController]);

function LogController($scope, $http) {

}

app.controller("LogReportController", ["$scope", "$http", LogReportController]);

function LogReportController($scope, $http) {

}

app.controller("LogSyslogNgController", ["$scope", "$http", LogSyslogNgController]);

function LogSyslogNgController($scope, $http) {

}

app.controller("AntdTimeController", ["$scope", "$http", AntdTimeController]);

function AntdTimeController($scope, $http) {
    $scope.SaveTimeNtp = function (ntp) {
        var data = $.param({
            Ntp: ntp
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/ntpd", data).then(function () { alert("Ok!"); }, function (response) { console.log(response); });
    }

    $scope.SaveTimeNtpdate = function (ntpdate) {
        var data = $.param({
            Ntpdate: ntpdate
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/ntpdate", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveTimeSync = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/synctime").then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveTimeTimezone = function (timezone) {
        var data = $.param({
            Timezone: timezone
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/timezone", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/time").success(function (data) {
        $scope.Time = data;
    });
}

app.controller("AntdHostController", ["$scope", "$http", AntdHostController]);

function AntdHostController($scope, $http) {
    $scope.SaveHostLocation = function (location) {
        var data = $.param({
            Location: location
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/location", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveHostDeployment = function (deployment) {
        var data = $.param({
            Deployment: deployment
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/deployment", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveHostChassis = function (chassis) {
        var data = $.param({
            Chassis: chassis
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/chassis", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveHostName = function (staticHostname) {
        var data = $.param({
            Name: staticHostname
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/name", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/host").success(function (data) {
        $scope.Host = data;
    });
}

app.controller("AntdUpdateController", ["$scope", "$http", AntdUpdateController]);

function AntdUpdateController($scope, $http) {
    $scope.ApplyUpdate = function (context) {
        var data = $.param({
            Context: context
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/update", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/update").success(function (data) {
        $scope.Update = data;
    });
}

app.controller("AntdUsersController", ["$scope", "$http", AntdUsersController]);

function AntdUsersController($scope, $http) {
    $scope.masterPassword = "";
    $scope.EditMaster = function () {
        var data = $.param({
            Password: $scope.masterPassword
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/users/master/password", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.AddUser = function (user) {
        var data = $.param({
            User: user.Name,
            Password: user.Password
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/users", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/users").success(function (data) {
        $scope.Users = data;
    });
}

app.controller("AntdOverlayController", ["$scope", "$http", AntdOverlayController]);

function AntdOverlayController($scope, $http) {
    $scope.SetDirectory = function (dir) {
        var data = $.param({
            Directory: dir.Key
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/overlay/setdirectory", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/overlay").success(function (data) {
        $scope.Overlay = data;
    });
}

app.controller("AntdOvermountStatusController", ["$scope", "$http", AntdOvermountStatusController]);

function AntdOvermountStatusController($scope, $http) {
    $http.get("/overmountstatus").success(function (data) {
        $scope.OvermountStatus = data;
    });
}

app.controller("AntdLosetupStatusController", ["$scope", "$http", AntdLosetupStatusController]);

function AntdLosetupStatusController($scope, $http) {
    $http.get("/losetupstatus").success(function (data) {
        $scope.LosetupStatus = data;
    });
}

app.controller("AntdSystemStatusController", ["$scope", "$http", AntdSystemStatusController]);

function AntdSystemStatusController($scope, $http) {
    $http.get("/systemstatus").success(function (data) {
        $scope.SystemStatus = data;
    });
}

app.controller("AntdServicesController", ["$scope", "$http", "$sce", AntdServicesController]);

function AntdServicesController($scope, $http, $sce) {
    $http.get("/services").success(function (data) {
        $scope.Units = data;
    });

    $scope.unitType = "service";
    $scope.FilterUnitType = function (type) {
        $scope.unitType = type;
    }

    $scope.HideIfNotType = function (type) {
        return type !== $scope.unitType;
    }

    $scope.ShowLog = function (unit) {
        $http.get("/services/log?unit=" + unit.Name).success(function (data) {
            unit.LogHtml = $sce.trustAsHtml(data);
            unit.Hide = false;
        });
    }

    $scope.Start = function (unit) {
        var data = $.param({
            Unit: unit.Name
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/services/start", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    };

    $scope.Restart = function (unit) {
        var data = $.param({
            Unit: unit.Name
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/services/restart", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    };

    $scope.Stop = function (unit) {
        var data = $.param({
            Unit: unit.Name
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/services/stop", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    };

    $scope.Enable = function (unit) {
        var data = $.param({
            Unit: unit.Name
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/services/enable", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    };

    $scope.Disable = function (unit) {
        var data = $.param({
            Unit: unit.Name
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/services/disable", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    };
}

app.controller("AntdModulesStatusController", ["$scope", "$http", AntdModulesStatusController]);

function AntdModulesStatusController($scope, $http) {
    $http.get("/modulesstatus").success(function (data) {
        $scope.ModulesStatus = data;
    });
}

app.controller("AntdCpuStatusController", ["$scope", "$http", AntdCpuStatusController]);

function AntdCpuStatusController($scope, $http) {
    $http.get("/cpustatus").success(function (data) {
        $scope.CpuStatus = data;
    });
}

app.controller("AntdMemoryStatusController", ["$scope", "$http", AntdMemoryStatusController]);

function AntdMemoryStatusController($scope, $http) {
    $http.get("/memorystatus").success(function (data) {
        $scope.MemoryStatus = data;
    });
}

app.controller("AntdInfoController", ["$scope", "$http", AntdInfoController]);

function AntdInfoController($scope, $http) {
    $http.get("/info").success(function (data) {
        $scope.Info = data;
    });
}

app.controller("AntdHostParamController", ["$scope", "$http", AntdHostParamController]);

function AntdHostParamController($scope, $http) {

    $scope.Save = function () {
        var data = $.param({
            AntdPort: $scope.AntdPort,
            AntdUiPort: $scope.AntdUiPort,
            DatabasePath: $scope.DatabasePath
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/config", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    };

    $http.get("/config").success(function (data) {
        $scope.AntdPort = data.AntdPort;
        $scope.AntdUiPort = data.AntdUiPort;
        $scope.DatabasePath = data.DatabasePath;
    });

    $scope.Show = function (el) {
        if ($scope.Port !== $scope.CurrentPort) {
            $(el).show();
        } else {
            $(el).hide();
        }
    };
}
