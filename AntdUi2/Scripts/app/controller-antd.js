app.controller("SharedController", ["$scope", SharedController]);

function SharedController($scope) {

    $scope.addUnloadEvent = function() {
        if (window.addEventListener) {
            window.addEventListener("beforeunload", handleUnloadEvent);
        } else {
            window.attachEvent("onbeforeunload", handleUnloadEvent);
        }
    };

    function handleUnloadEvent(event) {
        event.returnValue = "Your warning text";
    }

    //Call this when you want to remove the event, example, if users fills necessary info
    $scope.removeUnloadEvent = function() {
        if (window.removeEventListener) {
            window.removeEventListener("beforeunload", handleUnloadEvent);
        } else {
            window.detachEvent("onbeforeunload", handleUnloadEvent);
        }
    };

    //You could add the event when validating a form, for example
    $scope.validateForm = function() {
        if ($scope.yourform.$invalid) {
            $scope.addUnloadEvent();
            return;
        }
        else {
            $scope.removeUnloadEvent();
        }
    };

    $scope.scrollConfigSidebar = {
        autoHideScrollbar: false,
        theme: 'light-3',
        mouseWheel: { preventDefault: true },
        axis: "y",
        scrollInertia: 0
    };

    $scope.addToList = function(element, list) {
        var newElement = angular.copy(element);
        list.push(newElement);
    };

    $scope.removeFromList = function(index, list) {
        list.splice(index, 1);
    };

    $scope.selectizeSingleConfig = function() {
        return {
            valueField: "value",
            labelField: "value",
            searchField: ["value"],
            persist: true,
            create: function(input) {
                return {
                    value: input,
                    text: input
                };
            },
            maxItems: 1
        };
    };

    $scope.selectizeConfig = function(list) {
        return {
            valueField: "value",
            labelField: "value",
            searchField: ["value"],
            persist: true,
            create: function(input) {
                return {
                    value: input,
                    text: input
                };
            },
            onChange: function(values) {
                list = [];
                angular.forEach(values, function(value) {
                    list.push(value);
                });
            },
            delimiter: ","
        };
    };

    $scope.selectizeOptions = function(list) {
        var options = [];
        angular.forEach(list, function(el) {
            options.push({ value: el });
        });
        return options;
    };
}

app.controller("HostController", ["$scope", "$http", "notificationService", HostController]);

function HostController($scope, $http, notificationService) {

    $scope.Host = null;

    $scope.load = function() {
        console.log("loadHost");
        $scope.Host = null;
        $http.get("/host/config").then(function(r) {
            $scope.Host = r.data;
        }).catch(function(r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function() {
        var data = $.param({
            Data: angular.toJson($scope.Host)
        });
        console.log("saveHost");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/config/save", data).then(function() {
            notificationService.success('Data saved');
        }, function(r) {
            console.log(r);
        });
    };

    $scope.apply = function() {
        console.log("applyHost");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/config/apply").then(function() {
            notificationService.success('Data applied');
        }, function(r) {
            console.log(r);
        });
    };
}

app.controller("TimedateController", ["$scope", "$http", "notificationService", TimedateController]);

function TimedateController($scope, $http, notificationService) {
    $scope.TimeDate = null;

    $scope.load = function() {
        console.log("loadTimeDate");
        $scope.TimeDate = null;
        $http.get("/timedate/config").then(function(r) {
            $scope.TimeDate = r.data;
        }).catch(function(r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function() {
        var data = $.param({
            Data: angular.toJson($scope.TimeDate)
        });
        console.log("saveTimeDate");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/timedate/config/save", data).then(function() {
            notificationService.success('Data saved');
        }, function(r) { console.log(r); });
    };

    $scope.apply = function() {
        console.log("applyTimeDate");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/timedate/config/apply").then(function() {
            notificationService.success('Data applied');
        }, function(r) { console.log(r); });
    };
}

app.controller("SysctlController", ["$scope", "$http", "notificationService", SysctlController]);

function SysctlController($scope, $http, notificationService) {
    $scope.SysctlTxt = null;

    $scope.load = function() {
        console.log("loadBootParameters");
        $http.get("/boot/config/sysctl").then(function(r) {
            $scope.SysctlTxt = r.data;
        }).catch(function(r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function() {
        var data = $.param({
            Data: angular.toJson($scope.SysctlTxt)
        });
        console.log("saveBootParameters");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/sysctl/save", data).then(function() {
            notificationService.success('Data saved');
        }, function(r) { console.log(r); });
    };

    $scope.apply = function() {
        console.log("applyBootParameters");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/sysctl/apply").then(function() {
            notificationService.success('Data applied');
        }, function(r) { console.log(r); });
    };
}

app.controller("ModulesController", ["$scope", "$http", "notificationService", ModulesController]);

function ModulesController($scope, $http, notificationService) {
    $scope.BootModules = null;

    $scope.load = function() {
        console.log("loadBootModules");
        $http.get("/boot/config/modules").then(function(r) {
            $scope.BootModules = r.data;
        }).catch(function(r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function() {
        var data = $.param({
            Data: angular.toJson($scope.BootModules)
        });
        console.log("saveBootModules");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/modules/save", data).then(function() {
            notificationService.success('Data saved');
        }, function(r) { console.log(r); });
    };

    $scope.apply = function() {
        console.log("applyBootModules");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/modules/apply").then(function() {
            notificationService.success('Data applied');
        }, function(r) { console.log(r); });
    };
}

app.controller("ServicesController", ["$scope", "$http", "notificationService", ServicesController]);

function ServicesController($scope, $http, notificationService) {
    $scope.BootServices = [];

    $scope.load = function() {
        console.log("loadBootServices");
        $http.get("/boot/config/services").then(function(r) {
            $scope.BootServices = r.data;
        }).catch(function(r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function() {
        var data = $.param({
            Data: angular.toJson($scope.BootServices)
        });
        console.log("saveBootServices");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/services/save", data).then(function() {
            notificationService.success('Data saved');
        }, function(r) { console.log(r); });
    };

    $scope.apply = function() {
        console.log("applyBootServices");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/services/apply").then(function() {
            notificationService.success('Data applied');
        }, function(r) { console.log(r); });
    };
}

app.controller("CommandsController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", CommandsController]);

function CommandsController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.SetupCommands = [];

    $scope.NewSetupCommand = {
        BashCommand: ''
    };

    $scope.load = function() {
        console.log("loadSetupCommands");
        $http.get("/setupcmd/config").then(function(r) {
            $scope.SetupCommands = r.data;
        }).catch(function(r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function() {
        console.log("saveSetupCommands");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/setupcmd/config/save", $.param($scope.SetupCommands)).then(function() {
            $scope.load();
            notificationService.success('Data saved');
        }, function(r) { console.log(r); });
    };

    $scope.apply = function() {
        console.log("applySetupCommands");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/setupcmd/config/apply").then(function() {
            $scope.load();
            notificationService.success('Data applied');
        }, function(r) { console.log(r); });
    };

    $scope.moveUp = function(index, list) { //-1
        var from = index;
        var to = index - 1;
        var target = list[from];
        var increment = to < from ? -1 : 1;
        for (var k = from; k !== to; k += increment) {
            list[k] = list[k + increment];
        }
        list[to] = target;
    };

    $scope.moveDown = function(index, list) { //+1
        var from = index;
        var to = index + 1;
        var target = list[from];
        var increment = to < from ? -1 : 1;
        for (var k = from; k !== to; k += increment) {
            list[k] = list[k + increment];
        }
        list[to] = target;
    };
}

app.controller("RoutingTablesController", ["$scope", "$http", "notificationService", RoutingTablesController]);

function RoutingTablesController($scope, $http, notificationService) {
    $scope.RoutingTables = [];

    $scope.NewRoutingTable = {
        Id: '',
        Alias: ''
    };

    $scope.load = function() {
        console.log("loadRoutingTables");
        $http.get("/network/routingtables").then(function(r) {
            $scope.RoutingTables = r.data;
        }).catch(function(r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function() {
        console.log("saveRoutingTables");
        var data = $.param({
            Data: angular.toJson($scope.RoutingTables)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/routingtables", data).then(function() {
            $scope.load();
            notificationService.success('Data saved');
        }, function(r) { console.log(r); });
    };

    $scope.apply = function() {
        console.log("applyRoutingTables");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/routingtables").then(function() {
            $scope.load();
            notificationService.success('Data applied');
        }, function(r) { console.log(r); });
    };
}

app.controller("RoutingController", ["$scope", "$http", "notificationService", RoutingController]);

function RoutingController($scope, $http, notificationService) {
    $scope.Routes = [];
    $scope.NetworkDevices = [];
    $scope.Gateways = [];

    $scope.NewRoute = {
        Default: false,
        Destination: '',
        Gateway: '',
        Device: ''
    };

    $scope.load = function() {
        console.log("loadRouting");
        $http.get("/network/devices").then(function(r) {
            $scope.NetworkDevices = r.data;
        }).catch(function(r) {
            console.log(r);
        });
        $http.get("/gateway").then(function(r) {
            $scope.Gateways = r.data;
        }).catch(function(r) {
            console.log(r);
        });
        $http.get("/network/routing").then(function(r) {
            $scope.Routes = r.data;
        }).catch(function(r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function() {
        console.log("saveRouting");
        var data = $.param({
            Data: angular.toJson($scope.Routing)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/routing", data).then(function() {
            $scope.load();
            notificationService.success('Data saved');
        }, function(r) { console.log(r); });
    };

    $scope.apply = function() {
        console.log("applyRouting");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/routing").then(function() {
            $scope.load();
            notificationService.success('Data applied');
        }, function(r) { console.log(r); });
    };
}

app.controller("InterfacesController", ["$scope", "$http", "notificationService", InterfacesController]);

function InterfacesController($scope, $http, notificationService) {
    $scope.Interfaces = [];

    $scope.NewInterface = {
        AutoBool: true,
        Iface: '',
        Address: false,
        PreUpTxt: '',
        PostUpTxt: '',
        PreDownTxt: '',
        PostDownTxt: ''
    };

    $scope.load = function() {
        console.log("loadNetworkInterfaces");
        $http.get("/network/config/interfaces").then(function(r) {
            $scope.Interfaces = r.data;
        }).catch(function(r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function() {
        var data = $.param({
            Data: angular.toJson($scope.Interfaces)
        });
        console.log("saveNetworkInterfaces");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/config/interfaces/save", data).then(function() {
            notificationService.success('Data saved');
        }, function(r) {
            console.log(r);
        });
    };

    $scope.apply = function() {
        console.log("applyNetworkInterfaces");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/config/interfaces/apply").then(function() {
            notificationService.success('Data applied');
        }, function(r) {
            console.log(r);
        });
    };
}

app.controller("DisksController", ["$scope", "$http", "notificationService", DisksController]);

function DisksController($scope, $http, notificationService) {

    $scope.Disks = [];

    $scope.load = function() {
        $http.get("/disks").then(function(r) {
            $scope.Disks = r.data;
        }).catch(function(r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function() {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/sysctl/save", $.param($scope.BootParameters)).then(function() {
            $scope.load();
            notificationService.success('Data saved');
        }, function(r) { console.log(r); });
    };

    $scope.apply = function() {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/sysctl/apply").then(function() {
            $scope.load();
            notificationService.success('Data applied');
        }, function(r) { console.log(r); });
    };
}