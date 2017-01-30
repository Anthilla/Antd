"use strict";

app.controller("ConfigControllerContent", ["$scope", "$http", ConfigControllerContent]);

function ConfigControllerContent($scope, $http) {
    $scope.Create = function () {
        var data = $.param({
            Port: $scope.Port
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/config", data).then(
            function (response) {
                console.log(response);
            },
        function (response) {
            console.log(response);
        });
    };

    $http.get("/config").success(function (data) {
        $scope.Port = data.Port;
        $scope.CurrentPort = data.Port;
    });

    $scope.Show = function (el) {
        if ($scope.Port !== $scope.CurrentPort) {
            $(el).show();
        } else {
            $(el).hide();
        }
    };
}
