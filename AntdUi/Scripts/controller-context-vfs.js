"use strict";

app.controller("VfsController", ["$scope", "$http", VfsController]);

function VfsController($scope, $http) {
    $scope.saveUserMasters = function () {
        var json = angular.toKson($scope.UserMasters);
        var data = $.param({
            Config: json
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vfs/save/users", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.saveApiKeyPermissions = function () {
        var json = angular.toKson($scope.ApiKeyPermissions);
        var data = $.param({
            Config: json
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vfs/save/apikeypermissions", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.saveApiKeys = function () {
        var json = angular.toKson($scope.ApiKeys);
        var data = $.param({
            Config: json
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vfs/save/apikeys", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.saveTopology = function () {
        var json = angular.toKson($scope.Topology);
        var data = $.param({
            Config: json
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vfs/save/topology", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.saveSettings = function () {
        var json = angular.toKson($scope.Settings);
        var data = $.param({
            Config: json
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vfs/save/system", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.Get = $http.get("/vfs").success(function (data) {
        $scope.Settings = data.Settings;
        $scope.Topology = data.Topology;
        $scope.ApiKeys = data.ApiKeys;
        $scope.ApiKeyPermissions = data.ApiKeyPermissions;
        $scope.UserMasters = data.UserMasters;
    });
    $scope.Get();

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

