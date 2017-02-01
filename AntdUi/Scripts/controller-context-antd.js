"use strict";

app.controller("AntdOvermountStatusController", ["$scope", "$http", AntdOvermountStatusController]);

function AntdOvermountStatusController($scope, $http) {
    $http.get("/overmountstatus").success(function (data) {
        $scope.OvermountStatus = data;
    });
}

app.controller("AntdLosetupStatusController", ["$scope", "$http", AntdLosetupStatusController]);

function AntdLosetupStatusController($scope, $http) {
    $http.get("/losetupstatus").success(function (data) {
        $scope.LosetupStatus = data;
    });
}

app.controller("AntdSystemStatusController", ["$scope", "$http", AntdSystemStatusController]);

function AntdSystemStatusController($scope, $http) {
    $http.get("/systemstatus").success(function (data) {
        $scope.SystemStatus = data;
    });
}

app.controller("AntdServicesController", ["$scope", "$http", "$sce", AntdServicesController]);

function AntdServicesController($scope, $http, $sce) {
    $http.get("/services").success(function (data) {
        $scope.Units = data;
    });

    $scope.unitType = "service";
    $scope.FilterUnitType = function (type) {
        $scope.unitType = type;
    }

    $scope.HideIfNotType = function (type) {
        return type !== $scope.unitType;
    }

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
                alert("Ok!");
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
                alert("Ok!");
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
                alert("Ok!");
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
                alert("Ok!");
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
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    };
}

app.controller("AntdModulesStatusController", ["$scope", "$http", AntdModulesStatusController]);

function AntdModulesStatusController($scope, $http) {
    $http.get("/modulesstatus").success(function (data) {
        $scope.ModulesStatus = data;
    });
}

app.controller("AntdCpuStatusController", ["$scope", "$http", AntdCpuStatusController]);

function AntdCpuStatusController($scope, $http) {
    $http.get("/cpustatus").success(function (data) {
        $scope.CpuStatus = data;
    });
}

app.controller("AntdMemoryStatusController", ["$scope", "$http", AntdMemoryStatusController]);

function AntdMemoryStatusController($scope, $http) {
    $http.get("/memorystatus").success(function (data) {
        $scope.MemoryStatus = data;
    });
}

app.controller("AntdInfoController", ["$scope", "$http", AntdInfoController]);

function AntdInfoController($scope, $http) {
    $http.get("/info").success(function (data) {
        $scope.Info = data;
    });
}

app.controller("AntdHostParamController", ["$scope", "$http", AntdHostParamController]);

function AntdHostParamController($scope, $http) {

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
            },
        function (response) {
            console.log(response);
        });
    };

    $http.get("/config").success(function (data) {
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
