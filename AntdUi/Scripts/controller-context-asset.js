"use strict";

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
        $http.post("/asset/wol", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.shareKey = function (machine) {
        var data = $.param({
            Host: machine.Ip,
            Port: machine.Port
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/asset/handshake/start", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }
 
    $scope.sync = function (machine) {
        var data = $.param({
            MachineAddress: machine.Ip + ":" + machine.Port
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/machine", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
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
    $scope.Scan = function (subnet) {
        $http.get("/scan/" + subnet).success(function (data) {
            return data;
        });
    }

    $http.get("/scan").success(function (data) {
        $scope.AssetScan = data;
    });
}

app.controller("AssetSettingController", ["$scope", "$http", AssetSettingController]);

function AssetSettingController($scope, $http) {
    $scope.setLabel = function (el) {
        var data = $.param({
            Letter: el.Letter,
            Subnet: el.Subnet,
            Label: el.Label
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/netscan/setlabel", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.setSubnet = function (el) {
        var data = $.param({
            Subnet: el.Subnet,
            Label: el.Label
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/netscan/setsubnet", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/assetsetting").success(function (data) {
        $scope.AssetSetting = data;
    });
}

app.controller("AssetSyncMachineController", ["$scope", "$http", AssetSyncMachineController]);

function AssetSyncMachineController($scope, $http) {
    $scope.deleteMachine = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/machine/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addMachine = function (el) {
        var data = $.param({
            MachineAddress: el.MachineAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/machine", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/syncmachine").success(function (data) {
        $scope.SyncMachine = data;
    });
}