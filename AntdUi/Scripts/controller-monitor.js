"use strict";

app.controller("MonitorController", ["$scope", "$http", "$interval", MonitorController]);

function MonitorController($scope, $http, $interval) {
    $scope.GetResources = function () {
        $http.get("/monitor/resources").success(function (data) {
            $scope.Resources = data;
        });
    }
    $scope.GetResources();
    $interval(function () { $scope.GetResources(); }, 10 * 1000);
}
