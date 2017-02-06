"use strict";

app.controller("BootCommandsController", ["$scope", "$http", BootCommandsController]);

function BootCommandsController($scope, $http) {
    $scope.save = function (guid) {
        //todo
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/commands", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/boot/commands").success(function (data) {
        $scope.Commands = data;
    });
}

app.controller("BootModulesController", ["$scope", "$http", BootModulesController]);

function BootModulesController($scope, $http) {
    $scope.saveModblacklist = function (config) {
        var data = $.param({
            Config: config
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/modblacklist", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.saveRmmodules = function (config) {
        var data = $.param({
            Config: config
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/rmmodules", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.saveModules = function (config) {
        var data = $.param({
            Config: config
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/modules", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/boot/modules").success(function (data) {
        $scope.Modules = data;
    });
}

app.controller("BootOsParametersController", ["$scope", "$http", BootOsParametersController]);

function BootOsParametersController($scope, $http) {
    $scope.saveModules = function (config) {
        var data = $.param({
            Config: config
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/osparameter", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/boot/osparameter").success(function (data) {
        $scope.OsParameters = data;
    });
}

app.controller("BootServicesController", ["$scope", "$http", BootServicesController]);

function BootServicesController($scope, $http) {
    $scope.saveModules = function (config) {
        var data = $.param({
            Config: config
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/services", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/boot/services").success(function (data) {
        $scope.Services = data;
    });
}