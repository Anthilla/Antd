"use strict";

app.controller("BootCommandsController", ["$scope", "$http", "$interval", BootCommandsController]);

function BootCommandsController($scope, $http, $interval) {
    $scope.moveUp = function (cmd, index) { //-1
        var from = index;
        var to = index - 1;
        var target = $scope.Commands[from];
        var increment = to < from ? -1 : 1;

        for (var k = from; k !== to; k += increment) {
            $scope.Commands[k] = $scope.Commands[k + increment];
        }
        $scope.Commands[to] = target;
        $scope.refreshIndex();
    }

    $scope.moveDown = function (cmd, index) { //+1
        var from = index;
        var to = index + 1;
        var target = $scope.Commands[from];
        var increment = to < from ? -1 : 1;

        for (var k = from; k !== to; k += increment) {
            $scope.Commands[k] = $scope.Commands[k + increment];
        }
        $scope.Commands[to] = target;
        $scope.refreshIndex();
    }

    $scope.remove = function (cmd) {
        var index = $scope.Commands.indexOf(cmd);
        if (index > -1) {
            $scope.Commands.splice(index, 1);
        }
        angular.forEach($scope.Commands, function (v, i) {
            v.Index = i;
        });
    }

    $scope.refreshIndex = function () {
        angular.forEach($scope.Commands, function (v, i) {
            v.Index = i;
        });
    }

    $scope.add = function () {
        var newIndex = $scope.Commands.length;
        $scope.Commands.push({ Index: newIndex, FirstCommand: "", ControlCommand: "", Check: "" });
    }

    $scope.save = function () {
        var commands = "";
        angular.forEach($scope.Commands, function (v) {
            commands += v.Index + "," + v.FirstCommand + ";";
        });
        var data = $.param({
            Data: commands
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/commands", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.Commands = [];

    $scope.Get = function () {
        $http.get("/boot/commands").success(function (data) {
            $scope.Commands = data.Controls;
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

app.controller("BootModulesController", ["$scope", "$http", "$interval", BootModulesController]);

function BootModulesController($scope, $http, $interval) {

    $scope.FormatList = function (list) {
        var d = "";
        angular.forEach(list, function (v) {
            d += v + "\n";
        });
        return d;
    }

    $scope.saveModblacklist = function () {
        var data = $.param({
            Data: $scope.ModulesBlacklistStr.replace(/\n/g, ";")
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/hostparam/set/modulesblacklistlist", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.saveRmmodules = function () {
        var data = $.param({
            Data: $scope.RmmodStr.replace(/\n/g, ";")
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/hostparam/set/rmmodlist", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.saveModules = function () {
        var data = $.param({
            Data: $scope.ModprobesStr.replace(/\n/g, ";")
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/hostparam/set/modprobeslist", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.Get = function () {
        $http.get("/hostparam").success(function (data) {
            $scope.Modprobes = data.Modprobes;
            $scope.ModprobesStr = $scope.FormatList($scope.Modprobes);
            $scope.Rmmod = data.Rmmod;
            $scope.RmmodStr = $scope.FormatList($scope.Rmmod);
            $scope.ModulesBlacklist = data.ModulesBlacklist;
            $scope.ModulesBlacklistStr = $scope.FormatList($scope.ModulesBlacklist);
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

app.controller("BootOsParametersController", ["$scope", "$http", "$interval", BootOsParametersController]);

function BootOsParametersController($scope, $http, $interval) {
    $scope.FormatList = function (list) {
        var d = "";
        angular.forEach(list, function (v) {
            d += v + "\n";
        });
        return d;
    }

    $scope.save = function () {
        var data = $.param({
            Data: $scope.OsParametersStr.replace(/\n/g, ";")
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/hostparam/set/osparameters", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.Get = function () {
        $http.get("/hostparam").success(function (data) {
            $scope.OsParameters = data.OsParameters;
            $scope.OsParametersStr = $scope.FormatList($scope.OsParameters);
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

app.controller("BootServicesController", ["$scope", "$http", "$interval", BootServicesController]);

function BootServicesController($scope, $http, $interval) {
    $scope.FormatList = function (list) {
        var d = "";
        angular.forEach(list, function (v) {
            d += v + "\n";
        });
        return d;
    }

    $scope.saveStop = function () {
        var data = $.param({
            Data: $scope.ServicesStopStr.replace(/\n/g, ";")
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/hostparam/set/servicesstoplist", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.saveStart = function () {
        var data = $.param({
            Data: $scope.ServicesStartStr.replace(/\n/g, ";")
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/hostparam/set/servicesstartlist", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.Get = function () {
        $http.get("/hostparam").success(function (data) {
            $scope.ServicesStart = data.ServicesStart;
            $scope.ServicesStartStr = $scope.FormatList($scope.ServicesStart);
            $scope.ServicesStop = data.ServicesStop;
            $scope.ServicesStopStr = $scope.FormatList($scope.ServicesStop);
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