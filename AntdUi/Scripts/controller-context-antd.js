"use strict";

app.controller("AntdTimeController", ["$scope", "$http", AntdTimeController]);

function AntdTimeController($scope, $http) {
    $scope.SaveTimeNtp = function (ntp) {
        var data = $.param({
            Ntp: ntp
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/ntpd", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveTimeNtpdate = function (ntpdate) {
        var data = $.param({
            Ntpdate: ntpdate
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/ntpdate", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveTimeSync = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/synctime").then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveTimeTimezone = function (timezone) {
        var data = $.param({
            Timezone: timezone
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/timezone", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/time").success(function (data) {
        $scope.Time = data;
    });
}

app.controller("AntdHostController", ["$scope", "$http", AntdHostController]);

function AntdHostController($scope, $http) {
    $scope.SaveHostLocation = function (location) {
        var data = $.param({
            Location: location
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/location", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveHostDeployment = function (deployment) {
        var data = $.param({
            Deployment: deployment
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/deployment", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveHostChassis = function (chassis) {
        var data = $.param({
            Chassis: chassis
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/chassis", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveHostName = function (staticHostname) {
        var data = $.param({
            Name: staticHostname
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/name", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/host").success(function (data) {
        $scope.Host = data;
    });
}

app.controller("AntdUpdateController", ["$scope", "$http", AntdUpdateController]);

function AntdUpdateController($scope, $http) {
    $scope.ApplyUpdate = function (context) {
        var data = $.param({
            Context: context
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/update", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/update").success(function (data) {
        $scope.Update = data;
    });
}

app.controller("AntdUsersController", ["$scope", "$http", AntdUsersController]);

function AntdUsersController($scope, $http) {
    $scope.masterPassword = "";
    $scope.EditMaster = function () {
        var data = $.param({
            Password: $scope.masterPassword
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/users/master/password", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.AddUser = function (user) {
        var data = $.param({
            User: user.Name,
            Password: user.Password
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/users", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/users").success(function (data) {
        $scope.Users = data;
    });
}

app.controller("AntdOverlayController", ["$scope", "$http", AntdOverlayController]);

function AntdOverlayController($scope, $http) {
    $scope.SetDirectory = function(dir) {
        var data = $.param({
            Directory: dir.Key
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/overlay/setdirectory", data).then(
            function () {
                alert("Ok!");
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/overlay").success(function (data) {
        $scope.Overlay = data;
    });
}

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
