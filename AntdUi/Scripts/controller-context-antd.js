"use strict";

app.controller("AntdHostParam", ["$rootScope", "$scope", "$http", AntdHostParam]);

function AntdHostParam($rootScope, $scope, $http) {

    $scope.Save = function () {
        var data = $.param({
            AntdPort: $scope.AntdPort,
            AntdUiPort: $scope.AntdUiPort,
            DatabasePath: $scope.DatabasePath
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/config", data).then(
            function () {
                alert("Ok!");
                $scope.GetConfig();
            },
        function (response) {
            console.log(response);
        });
    };

    $scope.GetConfig();
    $scope.GetConfig = $http.get("/config").success(function (data) {
        $scope.AntdPort = data.AntdPort;
        $scope.AntdUiPort = data.AntdUiPort;
        $scope.DatabasePath = data.DatabasePath;
    });

    $scope.Show = function (el) {
        if ($scope.Port !== $scope.CurrentPort) {
            $(el).show();
        } else {
            $(el).hide();
        }
    };
}
