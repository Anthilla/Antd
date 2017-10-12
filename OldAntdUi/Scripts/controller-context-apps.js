"use strict";

app.controller("AppsDetectController", ["$scope", "$http", AppsDetectController]);

function AppsDetectController($scope, $http) {
    $scope.setup = function (appName) {
        var data = $.param({
            AppName: appName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/apps/setup", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/apps/detect").success(function (data) {
        $scope.AppsDetect = data;
    });
}

app.controller("AppsManagementController", ["$scope", "$http", AppsManagementController]);

function AppsManagementController($scope, $http) {
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
                console.log(1);
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
                console.log(1);
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
                console.log(1);
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
                console.log(1);
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
                console.log(1);
            },
        function (response) {
            console.log(response);
        });
    };

    $http.get("/apps/management").success(function (data) {
        $scope.Apps = data;
    });
}