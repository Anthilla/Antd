"use strict";

app.controller("SharedController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", SharedController]);

function SharedController($scope, $http, $interval, $timeout, $filter, notificationService) {

    $scope.addUnloadEvent = function () {
        if (window.addEventListener) {
            window.addEventListener("beforeunload", handleUnloadEvent);
        } else {
            //For IE browsers
            window.attachEvent("onbeforeunload", handleUnloadEvent);
        }
    };

    function handleUnloadEvent(event) {
        event.returnValue = "Your warning text";
    }

    //Call this when you want to remove the event, example, if users fills necessary info
    $scope.removeUnloadEvent = function () {
        if (window.removeEventListener) {
            window.removeEventListener("beforeunload", handleUnloadEvent);
        } else {
            window.detachEvent("onbeforeunload", handleUnloadEvent);
        }
    };

    //You could add the event when validating a form, for example
    $scope.validateForm = function () {
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

    $scope.addToList = function (element, list) {
        var newElement = angular.copy(element);
        list.push(newElement);
    };

    $scope.removeFromList = function (index, list) {
        list.splice(index, 1);
    }

    $scope.selectizeSingleConfig = function () {
        return {
            valueField: "value",
            labelField: "value",
            searchField: ["value"],
            persist: true,
            create: function (input) {
                return {
                    value: input,
                    text: input
                }
            },
            maxItems: 1
        };
    }

    $scope.selectizeConfig = function (list) {
        return {
            valueField: "value",
            labelField: "value",
            searchField: ["value"],
            persist: true,
            create: function (input) {
                return {
                    value: input,
                    text: input
                }
            },
            onChange: function (values) {
                list = [];
                angular.forEach(values, function (value) {
                    list.push(value);
                });
            },
            delimiter: ","
        };
    }

    $scope.selectizeOptions = function (list) {
        var options = [];
        angular.forEach(list, function (el) {
            options.push({ value: el });
        });
        return options;
    }
}

app.controller("DashboardController", ["$rootScope", "$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", DashboardController]);

function DashboardController($rootScope, $scope, $http, $interval, $timeout, $filter, notificationService) {

    $scope.Dashboard = {
        LastUpdated: moment().toDate()
    };

    $scope.memoryLabels = ["Used", "Free", "Shared", "Buff/Cache", "Available"];
    $scope.memoryData = [0, 0, 0, 0, 0];

    $scope.load = function () {
        console.log("loadInfo");
        $http.get("/info").then(function (r) {
            $scope.Dashboard.LastUpdated = moment().toDate();
            $scope.info = r.data;

            $scope.memoryData = [
                parseInt($scope.info.Free[0].Used),
                parseInt($scope.info.Free[0].Free),
                parseInt($scope.info.Free[0].Shared),
                parseInt($scope.info.Free[0].BuffCache),
                parseInt($scope.info.Free[0].Available)
            ];

        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    };
    $scope.load();

    $scope.loadInterval = $interval(function () {
        $scope.load();
    }, 3000);

    var dereg = $rootScope.$on('$locationChangeSuccess', function () {
        $interval.cancel($scope.loadInterval);
        dereg();
    });
}

app.controller("HostInfoController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", HostInfoController]);

function HostInfoController($scope, $http, $interval, $timeout, $filter, notificationService) {

    $scope.Host = null;

    $scope.load = function () {
        console.log("loadHost");
        $scope.Host = null;
        $http.get("/host").then(function (r) {
            $scope.Host = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/host/running").then(function (r) {
            $scope.HostRunning = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveHost");
        var data = $.param({
            Data: angular.toJson($scope.Host)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) {
            notificationService.error('Error! ' + r);
        });
    }

    $scope.apply = function () {
        console.log("applyHost");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/apply").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) {
            notificationService.error('Error! ' + r);
        });
    }
}

app.controller("HostTimedateController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", HostTimedateController]);

function HostTimedateController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.TimeDate = null;

    $scope.load = function () {
        console.log("loadTimeDate");
        $scope.TimeDate = null;
        $http.get("/timedate").then(function (r) {
            $scope.TimeDate = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/timedate/running").then(function (r) {
            $scope.TimeDateRunning = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveTimeDate");
        var data = $.param({
            Data: angular.toJson($scope.TimeDate)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/timedate/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyTimeDate");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/timedate/apply").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("HostWebserviceController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", HostWebserviceController]);

function HostWebserviceController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.Webservice = null;

    $scope.load = function () {
        console.log("loadWebservice");
        $scope.Webservice = null;
        $http.get("/webservice").then(function (r) {
            $scope.Webservice = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveWebservice");
        var data = $.param({
            Data: angular.toJson($scope.Webservice)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/webservice/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyWebservice");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/webservice/apply").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("BootParametersController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", BootParametersController]);

function BootParametersController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.BootParameters = null;

    $scope.load = function () {
        console.log("loadBootParameters");
        $scope.BootParameters = null;
        $http.get("/boot/parameters").then(function (r) {
            $scope.BootParameters = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveBootParameters");
        var data = $.param({
            Data: angular.toJson($scope.BootParameters)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/save/parameters", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyBootParameters");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/apply/parameters").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("BootModulesController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", BootModulesController]);

function BootModulesController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.BootModules = [];

    $scope.NewBootModule = {
        Module: "",
        Active: false,
        Remove: false,
        Blacklist: false
    };

    $scope.load = function () {
        console.log("loadBootModules");
        $http.get("/boot/modules").then(function (r) {
            $scope.BootModules = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveBootModules");
        var data = $.param({
            Data: angular.toJson($scope.BootModules)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/save/modules", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyBootModules");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/apply/modules").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("BootServicesController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", BootServicesController]);

function BootServicesController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.BootServices = [];

    $scope.load = function () {
        console.log("loadBootServices");
        $http.get("/boot/services").then(function (r) {
            $scope.BootServices = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveBootServices");
        var data = $.param({
            Data: angular.toJson($scope.BootServices)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/save/services", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyBootServices");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/apply/services").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("SetupCommandsController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", SetupCommandsController]);

function SetupCommandsController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.SetupCommands = [];

    $scope.NewSetupCommand = {
        BashCommand: ''
    };

    $scope.load = function () {
        console.log("loadSetupCommands");
        $http.get("/boot/commands").then(function (r) {
            $scope.SetupCommands = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveSetupCommands");
        var data = $.param({
            Data: angular.toJson($scope.SetupCommands)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/save/commands", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applySetupCommands");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/apply/commands").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.moveUp = function (cmd, index, list) { //-1
        var from = index;
        var to = index - 1;
        var target = list[from];
        var increment = to < from ? -1 : 1;
        for (var k = from; k !== to; k += increment) {
            list[k] = list[k + increment];
        }
        list[to] = target;
    }

    $scope.moveDown = function (cmd, index, list) { //+1
        var from = index;
        var to = index + 1;
        var target = list[from];
        var increment = to < from ? -1 : 1;
        for (var k = from; k !== to; k += increment) {
            list[k] = list[k + increment];
        }
        list[to] = target;
    }
}

app.controller("KnownDnsController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", KnownDnsController]);

function KnownDnsController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.selectizeConfig = function (list) {
        return {
            valueField: "value",
            labelField: "value",
            searchField: ["value"],
            persist: true,
            create: function (input) {
                return {
                    value: input,
                    text: input
                }
            },
            onChange: function (values) {
                list = [];
                angular.forEach(values, function (value) {
                    list.push(value);
                });
            },
            delimiter: ","
        };
    }

    $scope.selectizeOptions = function (list) {
        var options = [];
        angular.forEach(list, function (el) {
            options.push({ value: el });
        });
        return options;
    }

    $scope.KnownDns = [];

    $scope.load = function () {
        console.log("loadKnownDns");
        $http.get("/network/knowndns").then(function (r) {
            $scope.KnownDns = r.data;
            $scope.KnownDns.NameserverConfig = $scope.selectizeConfig($scope.KnownDns.Nameserver);
            $scope.KnownDns.NameserverOptions = $scope.selectizeOptions($scope.KnownDns.Nameserver);

        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveKnownDns");
        var data = $.param({
            Data: angular.toJson($scope.KnownDns)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/knowndns", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyKnownDns");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/knowndns").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("KnownHostsController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", KnownHostsController]);

function KnownHostsController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.selectizeConfig = function (list) {
        return {
            valueField: "value",
            labelField: "value",
            searchField: ["value"],
            persist: true,
            create: function (input) {
                return {
                    value: input,
                    text: input
                }
            },
            onChange: function (values) {
                list = [];
                angular.forEach(values, function (value) {
                    list.push(value);
                });
            },
            delimiter: ","
        };
    }

    $scope.selectizeOptions = function (list) {
        var options = [];
        angular.forEach(list, function (el) {
            options.push({ value: el });
        });
        return options;
    }

    $scope.KnownHosts = [];

    $scope.NewKnownHost = {
        IpAddr: '',
        CommonNames: []
    };

    $scope.load = function () {
        console.log("loadKnownHosts");
        $http.get("/network/knownhosts").then(function (r) {
            $scope.NewKnownHost.CommonNamesConfig = $scope.selectizeConfig($scope.NewKnownHost.CommonNames);
            $scope.NewKnownHost.CommonNamesOptions = $scope.selectizeOptions($scope.NewKnownHost.CommonNames);

            $scope.KnownHosts = r.data;
            for (var i = 0; i < $scope.KnownHosts.length; i++) {
                $scope.KnownHosts[i].CommonNamesConfig = $scope.selectizeConfig($scope.KnownHosts[i].CommonNames);
                $scope.KnownHosts[i].CommonNamesOptions = $scope.selectizeOptions($scope.KnownHosts[i].CommonNames);
            }
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/network/default/hosts").then(function (r) {
            $scope.DefaultKnownHosts = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveKnownHosts");
        var data = $.param({
            Data: angular.toJson($scope.KnownHosts)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/knownhosts", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyKnownHosts");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/knownhosts").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("KnownNetworksController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", KnownNetworksController]);

function KnownNetworksController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.KnownNetworks = [];

    $scope.NewKnownNetwork = {
        Label: '',
        NetAddr: ''
    };

    $scope.load = function () {
        console.log("loadKnownNetworks");
        $http.get("/network/knownnetworks").then(function (r) {
            $scope.KnownNetworks = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/network/default/networks").then(function (r) {
            $scope.DefaultKnownNetworks = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveKnownNetworks");
        var data = $.param({
            Data: angular.toJson($scope.KnownNetworks)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/knownnetworks", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyKnownNetworks");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/knownnetworks").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("TunController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", TunController]);

function TunController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.Tuns = [];

    $scope.NewTun = {
        Id: ''
    };

    $scope.load = function () {
        console.log("loadTuns");
        $http.get("/network/tuns").then(function (r) {
            $scope.Tuns = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveTuns");
        var data = $.param({
            Data: angular.toJson($scope.Tuns)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/tuns", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyTuns");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/tuns").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("TapController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", TapController]);

function TapController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.Taps = [];

    $scope.NewTap = {
        Id: ''
    };

    $scope.load = function () {
        console.log("loadTaps");
        $http.get("/network/taps").then(function (r) {
            $scope.Taps = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveTaps");
        var data = $.param({
            Data: angular.toJson($scope.Taps)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/taps", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyTaps");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/taps").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("BridgeController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", BridgeController]);

function BridgeController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.selectizeConfig = function (list) {
        return {
            valueField: "value",
            labelField: "value",
            searchField: ["value"],
            persist: true,
            create: function (input) {
                return {
                    value: input,
                    text: input
                }
            },
            onChange: function (values) {
                list = [];
                angular.forEach(values, function (value) {
                    list.push(value);
                });
            },
            delimiter: ","
        };
    }

    $scope.selectizeOptions = function (list) {
        var options = [];
        angular.forEach(list, function (el) {
            options.push({ value: el });
        });
        return options;
    }

    $scope.Bridges = [];
    $scope.NetworkDevices = [];

    $scope.NewBridge = {
        Id: '',
        Lower: []
    };

    $scope.load = function () {
        console.log("loadBridges");
        $http.get("/network/devices").then(function (r) {
            $scope.NetworkDevices = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/brctl").then(function (r) {
            $scope.NewBridge.LowerConfig = $scope.selectizeConfig($scope.NewBridge.Lower);
            $scope.NewBridge.LowerOptions = $scope.selectizeOptions($scope.NewBridge.Lower);

            $scope.Bridges = r.data;
            for (var i = 0; i < $scope.Bridges.length; i++) {
                $scope.Bridges[i].LowerConfig = $scope.selectizeConfig($scope.Bridges[i].Lower);
                $scope.Bridges[i].LowerOptions = $scope.selectizeOptions($scope.Bridges[i].Lower);
            }
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveBridges");
        var data = $.param({
            Data: angular.toJson($scope.Bridges)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/brctl/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyBridges");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/brctl/apply").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("BondController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", BondController]);

function BondController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.selectizeConfig = function (list) {
        return {
            valueField: "value",
            labelField: "value",
            searchField: ["value"],
            persist: true,
            create: function (input) {
                return {
                    value: input,
                    text: input
                }
            },
            onChange: function (values) {
                list = [];
                angular.forEach(values, function (value) {
                    list.push(value);
                });
            },
            delimiter: ","
        };
    }

    $scope.selectizeOptions = function (list) {
        var options = [];
        angular.forEach(list, function (el) {
            options.push({ value: el });
        });
        return options;
    }

    $scope.Bonds = [];
    $scope.NetworkDevices = [];

    $scope.NewBond = {
        Id: '',
        Lower: []
    };

    $scope.load = function () {
        console.log("loadBonds");
        $http.get("/network/devices").then(function (r) {
            $scope.NetworkDevices = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/bond").then(function (r) {
            $scope.NewBond.LowerConfig = $scope.selectizeConfig($scope.NewBond.Lower);
            $scope.NewBond.LowerOptions = $scope.selectizeOptions($scope.NewBond.Lower);

            $scope.Bonds = r.data;
            for (var i = 0; i < $scope.Bonds.length; i++) {
                $scope.Bonds[i].LowerConfig = $scope.selectizeConfig($scope.Bonds[i].Lower);
                $scope.Bonds[i].LowerOptions = $scope.selectizeOptions($scope.Bonds[i].Lower);
            }
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveBonds");
        var data = $.param({
            Data: angular.toJson($scope.Bonds)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bond/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyBonds");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bond/apply").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("InternalNetworkController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", InternalNetworkController]);

function InternalNetworkController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.InternalNetwork = null;
    $scope.NetworkDevices = [];

    $scope.load = function () {
        console.log("loadInternalNetwork");
        $http.get("/network/devices").then(function (r) {
            $scope.NetworkDevices = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/network/internalnetwork").then(function (r) {
            $scope.InternalNetwork = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveInternalNetwork");
        var data = $.param({
            Data: angular.toJson($scope.InternalNetwork)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/internalnetwork", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyInternalNetwork");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/internalnetwork").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("ExternalNetworkController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", ExternalNetworkController]);

function ExternalNetworkController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.ExternalNetwork = null;
    $scope.NetworkDevices = [];

    $scope.load = function () {
        console.log("loadExternalNetwork");
        $http.get("/network/devices").then(function (r) {
            $scope.NetworkDevices = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/network/externalnetwork").then(function (r) {
            $scope.ExternalNetwork = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveExternalNetwork");
        var data = $.param({
            Data: angular.toJson($scope.ExternalNetwork)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/externalnetwork", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyExternalNetwork");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/externalnetwork").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("GatewaysController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", GatewaysController]);

function GatewaysController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.Gateways = [];

    $scope.NewGateway = {
        Id: '',
        IpAddress: ''
    };

    $scope.load = function () {
        console.log("loadGateways");
        $http.get("/gateway").then(function (r) {
            $scope.Gateways = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveGateways");
        var data = $.param({
            Data: angular.toJson($scope.Gateways)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/gateway/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("RoutingTablesController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", RoutingTablesController]);

function RoutingTablesController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.RoutingTables = [];

    $scope.NewRoutingTable = {
        Id: '',
        Alias: ''
    };

    $scope.load = function () {
        console.log("loadRoutingTables");
        $http.get("/network/routingtables").then(function (r) {
            $scope.RoutingTables = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveRoutingTables");
        var data = $.param({
            Data: angular.toJson($scope.RoutingTables)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/routingtables", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyRoutingTables");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/routingtables").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("RoutingController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", RoutingController]);

function RoutingController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.Routes = [];
    $scope.NetworkDevices = [];
    $scope.Gateways = [];

    $scope.NewRoute = {
        Default: false,
        Destination: '',
        Gateway: '',
        Device: ''
    };

    $scope.load = function () {
        console.log("loadRouting");
        $http.get("/network/devices").then(function (r) {
            $scope.NetworkDevices = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/gateway").then(function (r) {
            $scope.Gateways = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/network/routing").then(function (r) {
            $scope.Routes = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveRouting");
        var data = $.param({
            Data: angular.toJson($scope.Routing)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/routing", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyRouting");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/routing").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("NetworkInterfacesController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", NetworkInterfacesController]);

function NetworkInterfacesController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.NetworkInterfaces = [];
    $scope.NetworkDevices = [];

    $scope.NewNetworkInterface = {
        Active: true,
        Id: "",
        Name: false,
        PrimaryAddressConfiguration: {
            StaticAddress: true,
            IpAddr: "",
            NetworkRange: 16
        },
        HardwareConfiguration: {
            Mtu: 6000,
            Txqueuelen: 10000
        }
    };

    $scope.load = function () {
        console.log("loadNetworkInterfaces");
        $http.get("/network/devices").then(function (r) {
            $scope.NetworkDevices = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/network/interfaces").then(function (r) {
            $scope.NetworkInterfaces = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveNetworkInterfaces");
        var data = $.param({
            Data: angular.toJson($scope.NetworkInterfaces)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/interfaces", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyNetworkInterfaces");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/interfaces").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("WifiController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", WifiController]);

function WifiController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.WiFi = null;
    $scope.NetworkDevices = [];

    $scope.load = function () {
        console.log("loadWiFi");
        $http.get("/network/devices").then(function (r) {
            $scope.NetworkDevices = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/wifi").then(function (r) {
            $scope.WiFi = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveWiFi");
        var data = $.param({
            Data: angular.toJson($scope.WiFi)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/wifi/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyWiFi");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/wifi/apply").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("PublicKeyController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", PublicKeyController]);

function PublicKeyController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.PublicKey = {
        Value: "",
        Copyicon: "fa-clipboard",
        Copycolor: ""
    };

    $scope.load = function () {
        console.log("loadWebservice");
        $http.get("/ssh/publickey").then(function (r) {
            $scope.PublicKey.Value = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.changeIcon = function () {
        $scope.PublicKey.Copyicon = "fa-check";
        $scope.PublicKey.Copycolor = "fg-success";
    }
}

app.controller("AuthorizedKeyController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", AuthorizedKeyController]);

function AuthorizedKeyController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.AuthorizedKeys = [];

    $scope.NewAuthorizedKey = {
        User: "",
        Host: "",
        Key: ""
    };

    $scope.load = function () {
        console.log("loadAuthorizedKeys");
        $http.get("/ssh/authorizedkeys").then(function (r) {
            $scope.AuthorizedKeys = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveAuthorizedKeys");
        var data = $.param({
            Data: angular.toJson($scope.AuthorizedKeys)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ssh/save/authorizedkeys", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyAuthorizedKeys");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ssh/apply/authorizedkeys").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("BindController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", BindController]);

function BindController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.selectizeConfig = function (list) {
        return {
            valueField: "value",
            labelField: "value",
            searchField: ["value"],
            persist: true,
            create: function (input) {
                return {
                    value: input,
                    text: input
                }
            },
            onChange: function (values) {
                list = [];
                angular.forEach(values, function (value) {
                    list.push(value);
                });
            },
            delimiter: ","
        };
    }

    $scope.selectizeOptions = function (list) {
        var options = [];
        angular.forEach(list, function (el) {
            options.push({ value: el });
        });
        return options;
    }

    $scope.Bind = null;

    $scope.NewBindZoneRecord = {
        Name: "",
        Ttl: "14400",
        Type: "",
        Value: "",
        CaaFlag: "",
        CaaTag: "",
        Priority: "",
        Weight: "",
        Port: ""
    };

    $scope.NewBindZone = {
        Name: "",
        Records: [],
        NewZoneRecord: $scope.NewBindZoneRecord
    };

    $scope.load = function () {
        console.log("loadBind");
        $http.get("/bind").then(function (r) {
            $scope.Bind = r.data;
            $scope.Bind.ForwardersConfig = $scope.selectizeConfig($scope.Bind.Forwarders);
            $scope.Bind.ForwardersOptions = $scope.selectizeOptions($scope.Bind.Forwarders)
            $scope.Bind.AllowNotifyConfig = $scope.selectizeConfig($scope.Bind.AllowNotify);
            $scope.Bind.AllowNotifyOptions = $scope.selectizeOptions($scope.Bind.AllowNotify)
            $scope.Bind.AllowTransferConfig = $scope.selectizeConfig($scope.Bind.AllowTransfer);
            $scope.Bind.AllowTransferOptions = $scope.selectizeOptions($scope.Bind.AllowTransfer)
            $scope.Bind.AllowQueryConfig = $scope.selectizeConfig($scope.Bind.AllowQuery);
            $scope.Bind.AllowQueryOptions = $scope.selectizeOptions($scope.Bind.AllowQuery)
            $scope.Bind.AllowRecursionConfig = $scope.selectizeConfig($scope.Bind.AllowRecursion);
            $scope.Bind.AllowRecursionOptions = $scope.selectizeOptions($scope.Bind.AllowRecursion)
            $scope.Bind.ListenOnV6Config = $scope.selectizeConfig($scope.Bind.ListenOnV6);
            $scope.Bind.ListenOnV6Options = $scope.selectizeOptions($scope.Bind.ListenOnV6)
            $scope.Bind.ListenOnPort53Config = $scope.selectizeConfig($scope.Bind.ListenOnPort53);
            $scope.Bind.ListenOnPort53Options = $scope.selectizeOptions($scope.Bind.ListenOnPort53)
            $scope.Bind.ControlAllowConfig = $scope.selectizeConfig($scope.Bind.ControlAllow);
            $scope.Bind.ControlAllowOptions = $scope.selectizeOptions($scope.Bind.ControlAllow)
            $scope.Bind.ControlKeysConfig = $scope.selectizeConfig($scope.Bind.ControlKeys);
            $scope.Bind.ControlKeysOptions = $scope.selectizeOptions($scope.Bind.ControlKeys)
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveBind");
        var data = $.param({
            Data: angular.toJson($scope.Bind)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyBind");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/apply").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("VirshController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", VirshController]);

function VirshController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.Virsh = null;

    $scope.load = function () {
        console.log("loadVirsh");
        $http.get("/virsh").then(function (r) {
            $scope.Virsh = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveVirsh");
        var data = $.param({
            Data: angular.toJson($scope.Virsh)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyVirsh");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/apply").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.save = function () {
        console.log("saveVirsh");
        var data = $.param({
            Data: angular.toJson($scope.Virsh)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.virshDestroy = function (domain) {
        console.log("virshDestroy");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/destroy", data).then(function () {
            $scope.load();
            notificationService.success('OK');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.virshReboot = function (domain) {
        console.log("virshReboot");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/reboot", data).then(function () {
            $scope.load();
            notificationService.success('OK');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.virshReset = function (domain) {
        console.log("virshReset");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/reset", data).then(function () {
            $scope.load();
            notificationService.success('OK');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.virshRestore = function (domain) {
        console.log("virshRestore");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/restore", data).then(function () {
            $scope.load();
            notificationService.success('OK');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.virshResume = function (domain) {
        console.log("virshResume");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/resume", data).then(function () {
            $scope.load();
            notificationService.success('OK');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.virshShutdown = function (domain) {
        console.log("virshShutdown");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/shutdown", data).then(function () {
            $scope.load();
            notificationService.success('OK');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.virshStart = function (domain) {
        console.log("virshStart");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/start", data).then(function () {
            $scope.load();
            notificationService.success('OK');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.virshSuspend = function (domain) {
        console.log("virshSuspend");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/suspend", data).then(function () {
            $scope.load();
            notificationService.success('OK');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.virshDompmsuspend = function (domain) {
        console.log("virshDompmsuspend");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/dompmsuspend", data).then(function () {
            $scope.load();
            notificationService.success('OK');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.virshDompmwakeup = function (domain) {
        console.log("virshDompmwakeup");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/dompmwakeup", data).then(function () {
            $scope.load();
            notificationService.success('OK');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("FirewallController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", FirewallController]);

function FirewallController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.selectizeConfig = function (list) {
        return {
            valueField: "value",
            labelField: "value",
            searchField: ["value"],
            persist: true,
            create: function (input) {
                return {
                    value: input,
                    text: input
                }
            },
            onChange: function (values) {
                list = [];
                angular.forEach(values, function (value) {
                    list.push(value);
                });
            },
            delimiter: ","
        };
    }

    $scope.selectizeOptions = function (list) {
        var options = [];
        angular.forEach(list, function (el) {
            options.push({ value: el });
        });
        return options;
    }

    $scope.Firewall = null;

    $scope.NewFirewallTable = {
        Family: "",
        Name: "",
        Sets: [],
        Chains: [],
        NewSet: $scope.NewSet,
        NewChain: $scope.NewChain
    };

    $scope.NewSet = {
        Name: "",
        Type: "",
        Flags: "",
        Elements: []
    };

    $scope.NewChain = {
        Name: "",
        Type: "",
        Hook: "",
        Rules: [],
        NewRule: $scope.NewRule
    };

    $scope.NewRule = {
        Match: "",
        MatchArgument: "",
        Object: "",
        Jump: "accept"
    };

    $scope.load = function () {
        console.log("loadFirewall");
        $http.get("/firewall").then(function (r) {
            $scope.NewSet.ElementsConfig = $scope.selectizeConfig($scope.NewSet.Elements);
            $scope.NewSet.ElementsOptions = $scope.selectizeOptions($scope.NewSet.Elements);

            $scope.Firewall = r.data;

            for (var i = 0; i < $scope.Firewall.Sets; i++) {
                $scope.Firewall.Sets[i].ElementsConfig = $scope.selectizeConfig($scope.Firewall.Sets[i].Elements);
                $scope.Firewall.Sets[i].ElementsOptions = $scope.selectizeOptions($scope.Firewall.Sets[i].Elements);
            }

            for (var i = 0; i < $scope.Firewall.Tables; i++) {
                $scope.Firewall.Tables[i].SetsConfig = selectizeFirewallTableSetsConfig($scope.Firewall.Sets, $scope.Firewall.Tables[i]);
                $scope.Firewall.Tables[i].TmpSets = createTmpSets($scope.Firewall.Tables[i].Sets);
            }


        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveFirewall");
        var data = $.param({
            Data: angular.toJson($scope.Firewall)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyFirewall");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/apply").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.suggestSetName = function (set) {
        if (set.Name.length < 1) {
            set.Name = set.Type + "000";
        }
    }

    $scope.selectizeFirewallTableSetsConfig = function (options, table) {
        return {
            valueField: "Name",
            labelField: "Name",
            searchField: ["Name", "name"],
            persist: true,
            onChange: function (values) {
                var list = [];
                angular.forEach(values, function (value) {
                    var foundSet = $filter('filter')(options, { Name: value })[0];
                    list.push(foundSet);
                });
                table.Sets = list;
            },
            delimiter: ","
        };
    }

    $scope.createTmpSets = function (list) {
        var result = [];
        angular.forEach(list, function (el) {
            result.push(el.Name);
        });
        return result;
    }
}

app.controller("SyslogNgController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", SyslogNgController]);

function SyslogNgController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.SyslogNg = null;

    $scope.load = function () {
        console.log("loadSyslogNg");
        $http.get("/syslogng").then(function (r) {
            $scope.SyslogNg = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveSyslogNg");
        var data = $.param({
            Data: angular.toJson($scope.SyslogNg)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syslogng/save", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applySyslogNg");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syslogng/apply").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("GroupSysController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", GroupSysController]);

function GroupSysController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.Groups = [];

    $scope.NewGroup = {
        Active: true,
        Alias: ''
    };

    $scope.load = function () {
        console.log("loadGroup");
        $http.get("/user/get/group").then(function (r) {
            $scope.Groups = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveGroup");
        var data = $.param({
            Data: angular.toJson($scope.Groups)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/user/save/group", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyGroup");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/user/apply/group").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("UserSysController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", UserSysController]);

function UserSysController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.UserSystem = [];
    $scope.Groups = [];

    $scope.NewUserSystem = {
        Active: true,
        Alias: '',
        Password: '',
        Group: ''
    };

    $scope.load = function () {
        console.log("loadUserSystem");
        $http.get("/user/get/system").then(function (r) {
            $scope.UserSystem = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/user/get/group/running").then(function (r) {
            $scope.Groups = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveUserSystem");
        var data = $.param({
            Data: angular.toJson($scope.UserSystem)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/user/save/system", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyUserSystem");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/user/apply/system").then(function () {
            $scope.load();
            notificationService.success('Data applied');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("UserAppController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", UserAppController]);

function UserAppController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.UserApplicative = [];

    $scope.NewUserApplicative = {
        Active: true,
        Alias: '',
        Password: ''
    };

    $scope.load = function () {
        console.log("loadUserApplicative");
        $http.get("/user/get/applicative").then(function (r) {
            $scope.UserApplicative = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.save = function () {
        console.log("saveUserApplicative");
        var data = $.param({
            Data: angular.toJson($scope.UserApplicative)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/user/save/applicative", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("AppLocalController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", AppLocalController]);

function AppLocalController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.Apps = [];

    $scope.load = function () {
        console.log("loadApps");
        $http.get("/app").then(function (r) {
            $scope.Apps = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.restart = function (app) {
        console.log("restartApp");
        var data = $.param({
            Data: app
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/app/restart", data).then(function () {
            $scope.load();
            notificationService.success('Data saved');
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("LogController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", LogController]);

function LogController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.load = function () {
        console.log("loadLog");
        $http.get("/journalctl").then(function (r) {
            $scope.Journalctl = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();
}

app.controller("NeighborhoodController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", NeighborhoodController]);

function NeighborhoodController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.Neighborhood = [];

    $scope.load = function () {
        console.log("loadNeighborhood");
        $http.get("/rssdp/discover").then(function (r) {
            $scope.Neighborhood = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.createCluster = function () {
        console.log("createCluster");
        var nodes = [];
        angular.forEach($scope.Neighborhood, function (node) {
            if (node.IsSelected) {
                nodes.push(node);
            }
        });
        console.log(nodes);
        var data = $.param({
            Data: angular.toJson(nodes)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/handshake/begin", data).then(function () {
            $scope.load();
        }, function (r) {
            notificationService.error('Error! ' + r);
        });
    }
}

app.controller("ClusterController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", ClusterController]);

function ClusterController($scope, $http, $interval, $timeout, $filter, notificationService) {

    $scope.NewPortMapping = {
        ServiceName: '',
        ServicePort: 0,
        VirtualPort: 0
    };

    $scope.NewGlusterVolume = {
        Label: '',
        Brick: '',
        Mountpoint: ''
    };

    $scope.refreshNodeConfiguration = function (labels, nodes) {
        angular.forEach(nodes, function (node) {
            var volumes = [];
            angular.forEach(labels, function (label) {
                volumes.push({
                    Label: label,
                    Brick: '',
                    Mountpoint: ''
                });
            });
            node.Volumes = volumes;
        });
    }

    $scope.Cluster = null;
    $scope.NetworkDevices = [];

    $scope.load = function () {
        console.log("loadCluster");
        $http.get("/network/devices").then(function (r) {
            $scope.NetworkDevices = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
        $http.get("/cluster").then(function (r) {
            $scope.Cluster = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();

    $scope.addVolumeToNodes = function (label, nodes) {
        for (var i = 0; i < nodes.length; i++) {
            var newGlusterVolume = {
                Label: label,
                Brick: '',
                Mountpoint: ''
            };
            nodes[i].Volumes.push(newGlusterVolume);
        }
    }

    $scope.save = function () {
        console.log("saveCluster");
        var data = $.param({
            Data: angular.toJson($scope.Cluster)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/save", data).then(function () {
            $scope.load();
        }, function (r) { notificationService.error('Error! ' + r); });
    }

    $scope.apply = function () {
        console.log("applyCluster");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/apply").then(function () {
            $scope.load();
        }, function (r) { notificationService.error('Error! ' + r); });
        $http.post("/cluster/deploy").then(function () {
            $scope.load();
        }, function (r) { notificationService.error('Error! ' + r); });
    }
}

app.controller("ClusterStatusController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", ClusterStatusController]);

function ClusterStatusController($scope, $http, $interval, $timeout, $filter, notificationService) {
    $scope.load = function () {
        console.log("loadClusterStatus");
        $http.get("/cluster/status").then(function (r) {
            $scope.ClusterStatus = r.data;
        }).catch(function (r) {
            notificationService.error('Error! ' + r);
        });
    }
    $scope.load();
}

//app.controller("HostInfoController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", HostInfoController]);

//function HostInfoController($scope, $http, $interval, $timeout, $filter, notificationService) {
//}
