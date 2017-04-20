"use strict";

app.controller("BootCommandsController", ["$scope", "$http", BootCommandsController]);

function BootCommandsController($scope, $http) {
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
        $http.post("/boot/commands", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.Commands = [];

    $scope.Get = function () {
        $http.get("/boot/commands").success(function (data) {
            $scope.Commands = data.Controls;
        });
    }
    $scope.Get();
}

app.controller("BootModulesController", ["$scope", "$http", BootModulesController]);

function BootModulesController($scope, $http) {
    $scope.saveModblacklist = function () {
        var data = $.param({
            Config: $scope.Blacklist
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/modblacklist", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveRmmodules = function () {
        var data = $.param({
            Config: $scope.RmModules
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/rmmodules", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveModules = function () {
        var data = $.param({
            Config: $scope.Modules
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/modules", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/boot/modules").success(function (data) {
        $scope.Modules = data.Modules;
        $scope.RmModules = data.RmModules;
        $scope.Blacklist = data.Blacklist;
    });
}

app.controller("BootOsParametersController", ["$scope", "$http", BootOsParametersController]);

function BootOsParametersController($scope, $http) {
    $scope.save = function () {
        var data = $.param({
            Config: $scope.OsParameters
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/osparameter", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/boot/osparameter").success(function (data) {
        $scope.OsParameters = data.OsParameters;
    });
}

app.controller("BootServicesController", ["$scope", "$http", BootServicesController]);

function BootServicesController($scope, $http) {
    $scope.save = function () {
        var data = $.param({
            Config: $scope.Services
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/services", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/boot/services").success(function (data) {
        $scope.Services = data.Services;
    });
}