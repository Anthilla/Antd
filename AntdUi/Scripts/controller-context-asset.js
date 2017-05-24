"use strict";

app.controller("AssetClusterController", ["$scope", "$http", "$interval", AssetClusterController]);

function AssetClusterController($scope, $http, $interval) {
    $scope.save = function () {
        var config = angular.toJson($scope.ClusterNodes, true);
        var config2 = angular.toJson($scope.Info, true);
        var data = $.param({
            Config: config,
            Ip: config2
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/save", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/restart").then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/stop").then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/enable").then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/disable").then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/set").then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.addNode = function () {
        var node = {
            Hostname: "",
            IpAddress: ""
        };
        $scope.ClusterNodes.push(node);
    }

    $scope.PublicIp = "";
    $scope.ClusterNodes =[];
    $scope.Get = function () {
        $http.get("/cluster").success(function (data) {
            $scope.Info = data.Info;
            $scope.ClusterNodes = data.ClusterNodes;
        });
    }
    $scope.Get();

    //$scope.ShowResponseMessage("ok");
    $scope.ResponseMessage = "";
    $scope.ResponseMessageHide = true;
    $scope.ShowResponseMessage = function (message) {
        $scope.ResponseMessageHide = false;
        $scope.ResponseMessage = message;
        $interval(function () {
            $scope.ResponseMessageHide = true;
        }, 1666);
        $scope.Get();
    }

    $scope.ResponseMessageHideSelf = function () {
        $scope.ResponseMessage = "";
        $scope.ResponseMessageHide = true;
    }
}

app.controller("AssetDiscoveryController", ["$scope", "$http", AssetDiscoveryController]);

function AssetDiscoveryController($scope, $http) {
    $scope.scanPort = function (machine) {
        $http.get("/scan/" + machine.Ip).success(function (data) {
            machine.Ports = data;
        });
    }

    $scope.wol = function (machine) {
        var data = $.param({
            MacAddress: machine.MacAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/asset/wol", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.shareKey = function (machine) {
        var data = $.param({
            Host: machine.Ip,
            Port: machine.Port
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/asset/handshake/start", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.sync = function (machine) {
        var data = $.param({
            MachineAddress: machine.Ip + ":" + machine.Port
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/machine", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.reload = function () {
        $http.get("/discovery").success(function (data) {
            $scope.Discovery = data;
        });
    }

    $http.get("/discovery").success(function (data) {
        $scope.Discovery = data;
    });
}

app.controller("AssetScanController", ["$scope", "$http", AssetScanController]);

function AssetScanController($scope, $http) {
    $scope.scanPort = function (machine) {
        $http.get("/scan/" + machine.Ip).success(function (data) {
            machine.Ports = data;
        });
    }

    $scope.wol = function (machine) {
        var data = $.param({
            MacAddress: machine.MacAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/asset/wol", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.shareKey = function (machine) {
        var data = $.param({
            Host: machine.Ip,
            Port: machine.Port
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/asset/handshake/start", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.scan = function () {
        if ($scope.NetScan.length > 0) {
            $http.get("/scan/" + $scope.NetScan)
                .success(function (data) {
                    $scope.AssetScan = data;
                });
        } else {
            alert("Value not valid!");
        }
    }

    $http.get("/scan").success(function (data) {
        $scope.ScanSettings = data.Subnets;
    });
}

app.controller("AssetSettingController", ["$scope", "$http", AssetSettingController]);

function AssetSettingController($scope, $http) {
    $scope.saveLabels = function () {
        angular.forEach($scope.ScanSettings, function (el) {
            if (el.IsChanged && el.Label.length > 0) {
                var data = $.param({
                    Letter: el.Letter,
                    Number: el.Number,
                    Label: el.Label
                });
                $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
                $http.post("/netscan/setlabel", data).then(function () { }, function (r) { console.log(r); });
            }
        });
    }

    $scope.checkElement = function (el) {
        if (el.Label !== el.OriginLabel) {
            el.IsChanged = true;
        } else {
            el.IsChanged = false;
        }
    }

    $scope.saveSubnet = function () {
        var data = $.param({
            Subnet: $scope.Subnet,
            Label: $scope.Label
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/netscan/setsubnet", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/assetsetting").success(function (data) {
        $scope.Subnet = data.SettingsSubnet;
        $scope.Label = data.SettingsSubnetLabel;
        $scope.ScanSettings = data.Settings;
    });
}

app.controller("AssetSyncMachineController", ["$scope", "$http", AssetSyncMachineController]);

function AssetSyncMachineController($scope, $http) {
    $scope.deleteMachine = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/machine/del", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.addMachine = function (el) {
        var data = $.param({
            MachineAddress: el.MachineAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/machine", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/syncmachine").success(function (data) {
        $scope.isActive = data.IsActive;
        $scope.SyncedMachines = data.SyncedMachines;
    });
}