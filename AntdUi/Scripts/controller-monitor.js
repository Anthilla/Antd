"use strict";

app.controller("MonitorController", ["$scope", "$http", "$interval", MonitorController]);

function MonitorController($scope, $http, $interval) {
    $scope.GetResources = $http.get("/monitor/resources").success(function (data) {
        console.log(data);
        $scope.Resources = data;
        $scope.digest();
    });
    $interval(function () { console.log(1); }, 3000);
}
