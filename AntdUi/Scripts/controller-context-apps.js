"use strict";

app.controller("AppsDetectController", ["$scope", "$http", AppsDetectController]);

function AppsDetectController($scope, $http) {
    $http.get("/apps/detect").success(function (data) {
        $scope.AppsDetect = data;
    });
}

app.controller("AppsManagementController", ["$scope", "$http", AppsManagementController]);

function AppsManagementController($scope, $http) {
    $scope.stop = function (appName) {
        var data = $.param({
            Name: appName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/apps/stop", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function (appName) {
        var data = $.param({
            Name: appName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/apps/restart", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.AppIsActive = function (appName) {
        $http.get("/apps/active/" + appName).success(function (data) {
            return data;
        });
    }

    $scope.AppStatus = function (appName) {
        $http.get("/apps/status/" + appName).success(function (data) {
            return data;
        });
    }

    $scope.setup = function (appName) {
        var data = $.param({
            AppName: appName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/apps/setup", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/apps/management").success(function (data) {
        $scope.Apps = data;
    });
}