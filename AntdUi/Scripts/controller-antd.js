"use strict";

app.controller("MachineConfigController", ["$scope", "$http", "$interval", "$timeout", "$filter", MachineConfigController]);

function MachineConfigController($scope, $http, $interval, $timeout, $filter) {

    $scope.Host = null;

    $scope.loadHost = function () {
        console.log("loadHost");
        $scope.Host = null;
        $http.get("/host").success(function (data) {
            $scope.Host = data
        });
        $http.get("/host/running").success(function (data) {
            $scope.HostRunning = data
        });
    }

    $scope.saveHost = function () {
        console.log("saveHost");
        var data = $.param({
            Data: angular.toJson($scope.Host)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/save", data).then(function () {
            $scope.loadHost();
        }, function (r) { console.log(r); });
    }

    $scope.applyHost = function () {
        console.log("applyHost");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/apply").then(function () {
            $scope.loadHost();
        }, function (r) { console.log(r); });
    }

    $scope.TimeDate = null;

    $scope.loadTimeDate = function () {
        console.log("loadTimeDate");
        $scope.TimeDate = null;
        $http.get("/timedate").success(function (data) {
            $scope.TimeDate = data
        });
        $http.get("/timedate/running").success(function (data) {
            $scope.TimeDateRunning = data
        });
    }

    $scope.saveTimeDate = function () {
        console.log("saveTimeDate");
        var data = $.param({
            Data: angular.toJson($scope.TimeDate)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/timedate/save", data).then(function () {
            $scope.loadTimeDate();
        }, function (r) { console.log(r); });
    }

    $scope.applyTimeDate = function () {
        console.log("applyTimeDate");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/timedate/apply").then(function () {
            $scope.loadTimeDate();
        }, function (r) { console.log(r); });
    }

    $scope.BootParameters = null;

    $scope.loadBootParameters = function () {
        console.log("loadBootParameters");
        $scope.BootParameters = null;
        $http.get("/boot/parameters").success(function (data) {
            $scope.BootParameters = data
        });
    }

    $scope.saveBootParameters = function () {
        console.log("saveBootParameters");
        var data = $.param({
            Data: angular.toJson($scope.BootParameters)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/save/parameters", data).then(function () {
            $scope.loadBootParameters();
        }, function (r) { console.log(r); });
    }

    $scope.applyBootParameters = function () {
        console.log("applyBootParameters");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/apply/parameters").then(function () {
            $scope.loadBootParameters();
        }, function (r) { console.log(r); });
    }

    $scope.BootModules = null;

    $scope.NewBootModule = {
        Module: "",
        Active: false,
        Remove: false,
        Blacklist: false
    };

    $scope.loadBootModules = function () {
        console.log("loadBootModules");
        $scope.BootModules = null;
        $http.get("/boot/modules/list").success(function (data) {
            $scope.ModulesList = data
        });
        $http.get("/boot/modules").success(function (data) {
            $scope.BootModules = data
        });
    }

    $scope.saveBootModules = function () {
        console.log("saveBootModules");
        var data = $.param({
            Data: angular.toJson($scope.BootModules)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/save/modules", data).then(function () {
            $scope.loadBootModules();
        }, function (r) { console.log(r); });
    }

    $scope.applyBootModules = function () {
        console.log("applyBootModules");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/apply/modules").then(function () {
            $scope.loadBootModules();
        }, function (r) { console.log(r); });
    }

    $scope.BootServices = null;

    $scope.loadBootServices = function () {
        console.log("loadBootServices");
        $scope.BootServices = null;
        $http.get("/boot/services/list").success(function (data) {
            $scope.ServicesList = data
        });
        $http.get("/boot/services").success(function (data) {
            $scope.BootServices = data
        });
    }

    $scope.saveBootServices = function () {
        console.log("saveBootServices");
        var data = $.param({
            Data: angular.toJson($scope.BootServices)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/save/services", data).then(function () {
            $scope.loadBootServices();
        }, function (r) { console.log(r); });
    }

    $scope.applyBootServices = function () {
        console.log("applyBootServices");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/apply/services").then(function () {
            $scope.loadBootServices();
        }, function (r) { console.log(r); });
    }

    $scope.SetupCommands = null;

    $scope.NewSetupCommand = {
        BashCommand: ''
    };

    $scope.loadSetupCommands = function () {
        console.log("loadSetupCommands");
        $scope.SetupCommands = null;
        $http.get("/boot/commands").success(function (data) {
            $scope.SetupCommands = data
        });
    }

    $scope.saveSetupCommands = function () {
        console.log("saveSetupCommands");
        var data = $.param({
            Data: angular.toJson($scope.SetupCommands)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/save/commands", data).then(function () {
            $scope.loadSetupCommands();
        }, function (r) { console.log(r); });
    }

    $scope.applySetupCommands = function () {
        console.log("applySetupCommands");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/apply/commands").then(function () {
            $scope.loadSetupCommands();
        }, function (r) { console.log(r); });
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

    $scope.KnownDns = null;

    $scope.loadKnownDns = function () {
        console.log("loadKnownDns");
        $scope.KnownDns = null;
        $http.get("/network/knowndns").success(function (data) {
            $scope.KnownDns = data
        });
    }

    $scope.saveKnownDns = function () {
        console.log("saveKnownDns");
        var data = $.param({
            Data: angular.toJson($scope.KnownDns)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/knowndns", data).then(function () {
            $scope.loadKnownDns();
        }, function (r) { console.log(r); });
    }

    $scope.applyKnownDns = function () {
        console.log("applyKnownDns");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/knowndns").then(function () {
            $scope.loadKnownDns();
        }, function (r) { console.log(r); });
    }

    $scope.KnownHosts = null;

    $scope.NewKnownHost = {
        IpAddr: '',
        CommonNames: []
    };

    $scope.loadKnownHosts = function () {
        console.log("loadKnownHosts");
        $scope.KnownHosts = null;
        $http.get("/network/knownhosts").success(function (data) {
            $scope.KnownHosts = data
        });
        $http.get("/network/default/hosts").success(function (data) {
            $scope.DefaultKnownHosts = data
        });
    }

    $scope.saveKnownHosts = function () {
        console.log("saveKnownHosts");
        var data = $.param({
            Data: angular.toJson($scope.KnownHosts)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/knownhosts", data).then(function () {
            $scope.loadKnownHosts();
        }, function (r) { console.log(r); });
    }

    $scope.applyKnownHosts = function () {
        console.log("applyKnownHosts");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/knownhosts").then(function () {
            $scope.loadKnownHosts();
        }, function (r) { console.log(r); });
    }

    $scope.KnownNetworks = null;

    $scope.NewKnownNetwork = {
        Label: '',
        NetAddr: ''
    };

    $scope.loadKnownNetworks = function () {
        console.log("loadKnownNetworks");
        $scope.KnownNetworks = null;
        $http.get("/network/knownnetworks").success(function (data) {
            $scope.KnownNetworks = data
        });
        $http.get("/network/default/networks").success(function (data) {
            $scope.DefaultKnownNetworks = data
        });
    }

    $scope.saveKnownNetworks = function () {
        console.log("saveKnownNetworks");
        var data = $.param({
            Data: angular.toJson($scope.KnownNetworks)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/knownnetworks", data).then(function () {
            $scope.loadKnownNetworks();
        }, function (r) { console.log(r); });
    }

    $scope.applyKnownNetworks = function () {
        console.log("applyKnownNetworks");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/knownnetworks").then(function () {
            $scope.loadKnownNetworks();
        }, function (r) { console.log(r); });
    }

    $scope.Tuns = null;

    $scope.NewTun = {
        Id: ''
    };

    $scope.loadTuns = function () {
        console.log("loadTuns");
        $scope.Tuns = null;
        $http.get("/network/tuns").success(function (data) {
            $scope.Tuns = data
        });
    }

    $scope.saveTuns = function () {
        console.log("saveTuns");
        var data = $.param({
            Data: angular.toJson($scope.Tuns)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/tuns", data).then(function () {
            $scope.loadTuns();
        }, function (r) { console.log(r); });
    }

    $scope.applyTuns = function () {
        console.log("applyTuns");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/tuns").then(function () {
            $scope.loadTuns();
        }, function (r) { console.log(r); });
    }

    $scope.Taps = null;

    $scope.NewTap = {
        Id: ''
    };

    $scope.loadTaps = function () {
        console.log("loadTaps");
        $scope.Taps = null;
        $http.get("/network/taps").success(function (data) {
            $scope.Taps = data
        });
    }

    $scope.saveTaps = function () {
        console.log("saveTaps");
        var data = $.param({
            Data: angular.toJson($scope.Taps)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/taps", data).then(function () {
            $scope.loadTaps();
        }, function (r) { console.log(r); });
    }

    $scope.applyTaps = function () {
        console.log("applyTaps");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/taps").then(function () {
            $scope.loadTaps();
        }, function (r) { console.log(r); });
    }

    $scope.Bridges = null;

    $scope.NewBridge = {
        Id: '',
        Lower: []
    };

    $scope.loadBridges = function () {
        console.log("loadBridges");
        $scope.Bridges = null;
        $http.get("/network/devices").success(function (data) {
            $scope.NetworkDevices = data
        });
        $http.get("/brctl").success(function (data) {
            $scope.Bridges = data
        });
    }

    $scope.saveBridges = function () {
        console.log("saveBridges");
        var data = $.param({
            Data: angular.toJson($scope.Bridges)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/brctl/save", data).then(function () {
            $scope.loadBridges();
        }, function (r) { console.log(r); });
    }

    $scope.applyBridges = function () {
        console.log("applyBridges");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/brctl/apply").then(function () {
            $scope.loadBridges();
        }, function (r) { console.log(r); });
    }

    $scope.Bonds = null;

    $scope.NewBond = {
        Id: '',
        Lower: []
    };

    $scope.loadBonds = function () {
        console.log("loadBonds");
        $scope.Bonds = null;
        $http.get("/network/devices").success(function (data) {
            $scope.NetworkDevices = data
        });
        $http.get("/bond").success(function (data) {
            $scope.Bonds = data
        });
    }

    $scope.saveBonds = function () {
        console.log("saveBonds");
        var data = $.param({
            Data: angular.toJson($scope.Bonds)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bond/save", data).then(function () {
            $scope.loadBonds();
        }, function (r) { console.log(r); });
    }

    $scope.applyBonds = function () {
        console.log("applyBonds");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bond/apply").then(function () {
            $scope.loadBonds();
        }, function (r) { console.log(r); });
    }

    $scope.InternalNetwork = null;

    $scope.loadInternalNetwork = function () {
        console.log("loadInternalNetwork");
        $scope.InternalNetwork = null;
        $http.get("/network/devices").success(function (data) {
            $scope.NetworkDevices = data
        });
        $http.get("/network/internalnetwork").success(function (data) {
            $scope.InternalNetwork = data
        });
    }

    $scope.saveInternalNetwork = function () {
        console.log("saveInternalNetwork");
        var data = $.param({
            Data: angular.toJson($scope.InternalNetwork)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/internalnetwork", data).then(function () {
            $scope.loadInternalNetwork();
        }, function (r) { console.log(r); });
    }

    $scope.applyInternalNetwork = function () {
        console.log("applyInternalNetwork");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/internalnetwork").then(function () {
            $scope.loadInternalNetwork();
        }, function (r) { console.log(r); });
    }

    $scope.ExternalNetwork = null;

    $scope.loadExternalNetwork = function () {
        console.log("loadExternalNetwork");
        $scope.ExternalNetwork = null;
        $http.get("/network/devices").success(function (data) {
            $scope.NetworkDevices = data
        });
        $http.get("/network/externalnetwork").success(function (data) {
            $scope.ExternalNetwork = data
        });
    }

    $scope.saveExternalNetwork = function () {
        console.log("saveExternalNetwork");
        var data = $.param({
            Data: angular.toJson($scope.ExternalNetwork)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/externalnetwork", data).then(function () {
            $scope.loadExternalNetwork();
        }, function (r) { console.log(r); });
    }

    $scope.applyExternalNetwork = function () {
        console.log("applyExternalNetwork");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/externalnetwork").then(function () {
            $scope.loadExternalNetwork();
        }, function (r) { console.log(r); });
    }

    $scope.NetworkInterfaces = null;

    $scope.loadNetworkInterfaces = function () {
        console.log("loadNetworkInterfaces");
        $scope.NetworkInterfaces = null;
        $http.get("/network/networkinterfaces").success(function (data) {
            $scope.NetworkInterfaces = data
        });
    }

    $scope.saveNetworkInterfaces = function () {
        console.log("saveNetworkInterfaces");
        var data = $.param({
            Data: angular.toJson($scope.NetworkInterfaces)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/networkinterfaces", data).then(function () {
            $scope.loadNetworkInterfaces();
        }, function (r) { console.log(r); });
    }

    $scope.applyNetworkInterfaces = function () {
        console.log("applyNetworkInterfaces");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/networkinterfaces").then(function () {
            $scope.loadNetworkInterfaces();
        }, function (r) { console.log(r); });
    }

    $scope.Gateways = null;

    $scope.NewGateway = {
        Id: '',
        IpAddress: ''
    };

    $scope.loadGateways = function () {
        console.log("loadGateways");
        $scope.Gateways = null;
        $http.get("/gateway").success(function (data) {
            $scope.Gateways = data
        });
    }

    $scope.saveGateways = function () {
        console.log("saveGateways");
        var data = $.param({
            Data: angular.toJson($scope.Gateways)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/gateway/save", data).then(function () {
            $scope.loadGateways();
        }, function (r) { console.log(r); });
    }

    $scope.RoutingTables = null;

    $scope.NewRoutingTable = {
        Id: '',
        Alias: ''
    };

    $scope.loadRoutingTables = function () {
        console.log("loadRoutingTables");
        $scope.RoutingTables = null;
        $http.get("/network/routingtables").success(function (data) {
            $scope.RoutingTables = data
        });
    }

    $scope.saveRoutingTables = function () {
        console.log("saveRoutingTables");
        var data = $.param({
            Data: angular.toJson($scope.RoutingTables)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/routingtables", data).then(function () {
            $scope.loadRoutingTables();
        }, function (r) { console.log(r); });
    }

    $scope.applyRoutingTables = function () {
        console.log("applyRoutingTables");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/routingtables").then(function () {
            $scope.loadRoutingTables();
        }, function (r) { console.log(r); });
    }

    $scope.Routing = null;

    $scope.NewRoute = {
        Default: false,
        Destination: '',
        Gateway: '',
        Device: ''
    };

    $scope.loadRouting = function () {
        console.log("loadRouting");
        $scope.Routing = null;
        $http.get("/network/devices").success(function (data) {
            $scope.NetworkDevices = data
        });
        $http.get("/gateway").success(function (data) {
            $scope.Gateways = data
        });
        $http.get("/network/routing").success(function (data) {
            $scope.Routing = data
        });
    }

    $scope.saveRouting = function () {
        console.log("saveRouting");
        var data = $.param({
            Data: angular.toJson($scope.Routing)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/routing", data).then(function () {
            $scope.loadRouting();
        }, function (r) { console.log(r); });
    }

    $scope.applyRouting = function () {
        console.log("applyRouting");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/routing").then(function () {
            $scope.loadRouting();
        }, function (r) { console.log(r); });
    }

    $scope.NetworkInterfaces = null;

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

    $scope.loadNetworkInterfaces = function () {
        console.log("loadNetworkInterfaces");
        $scope.NetworkInterfaces = null;
        $http.get("/network/devices").success(function (data) {
            $scope.NetworkDevices = data
        });
        $http.get("/network/interfaces").success(function (data) {
            $scope.NetworkInterfaces = data
        });
    }

    $scope.saveNetworkInterfaces = function () {
        console.log("saveNetworkInterfaces");
        var data = $.param({
            Data: angular.toJson($scope.NetworkInterfaces)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/save/interfaces", data).then(function () {
            $scope.loadNetworkInterfaces();
        }, function (r) { console.log(r); });
    }

    $scope.applyNetworkInterfaces = function () {
        console.log("applyNetworkInterfaces");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/apply/interfaces").then(function () {
            $scope.loadNetworkInterfaces();
        }, function (r) { console.log(r); });
    }

    $scope.WiFi = null;

    $scope.loadWiFi = function () {
        console.log("loadWiFi");
        $scope.WiFi = null;
        $http.get("/network/devices").success(function (data) {
            $scope.NetworkDevices = data
        });
        $http.get("/wifi").success(function (data) {
            $scope.WiFi = data
        });
    }

    $scope.saveWiFi = function () {
        console.log("saveWiFi");
        var data = $.param({
            Data: angular.toJson($scope.WiFi)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/wifi/save", data).then(function () {
            $scope.loadWiFi();
        }, function (r) { console.log(r); });
    }

    $scope.applyWiFi = function () {
        console.log("applyWiFi");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/wifi/apply").then(function () {
            $scope.loadWiFi();
        }, function (r) { console.log(r); });
    }

    $scope.Webservice = null;

    $scope.loadWebservice = function () {
        console.log("loadWebservice");
        $scope.Webservice = null;
        $http.get("/webservice").success(function (data) {
            $scope.Webservice = data
        });
    }

    $scope.saveWebservice = function () {
        console.log("saveWebservice");
        var data = $.param({
            Data: angular.toJson($scope.Webservice)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/webservice/save", data).then(function () {
            $scope.loadWebservice();
        }, function (r) { console.log(r); });
    }

    $scope.applyWebservice = function () {
        console.log("applyWebservice");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/webservice/apply").then(function () {
            $scope.loadWebservice();
        }, function (r) { console.log(r); });
    }

    $scope.PublicKey = {
        Value: "",
        Copyicon: "mif-clipboard",
        Copycolor: "fg-yellow"
    };

    $scope.loadPublicKey = function () {
        console.log("loadWebservice");
        $scope.Webservice = null;
        $http.get("/ssh/publickey").success(function (data) {
            $scope.PublicKey.Value = data
        });
    }

    $scope.changeIcon = function () {
        $scope.PublicKey.Copyicon = "mif-checkmark";
        $scope.PublicKey.Copycolor = "fg-green";
    }

    $scope.AuthorizedKeys = null;

    $scope.NewAuthorizedKey = {
        User: "",
        Host: "",
        Key: ""
    };

    $scope.loadAuthorizedKeys = function () {
        console.log("loadAuthorizedKeys");
        $scope.AuthorizedKeys = null;
        $http.get("/ssh/authorizedkeys").success(function (data) {
            $scope.AuthorizedKeys = data
        });
    }

    $scope.saveAuthorizedKeys = function () {
        console.log("saveAuthorizedKeys");
        var data = $.param({
            Data: angular.toJson($scope.AuthorizedKeys)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ssh/save/authorizedkeys", data).then(function () {
            $scope.loadAuthorizedKeys();
        }, function (r) { console.log(r); });
    }

    $scope.applyAuthorizedKeys = function () {
        console.log("applyAuthorizedKeys");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/ssh/apply/authorizedkeys").then(function () {
            $scope.loadAuthorizedKeys();
        }, function (r) { console.log(r); });
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

    $scope.loadBind = function () {
        console.log("loadBind");
        $scope.Bind = null;
        $http.get("/bind").success(function (data) {
            $scope.Bind = data
        });
    }

    $scope.saveBind = function () {
        console.log("saveBind");
        var data = $.param({
            Data: angular.toJson($scope.Bind)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/save", data).then(function () {
            $scope.loadBind();
        }, function (r) { console.log(r); });
    }

    $scope.applyBind = function () {
        console.log("applyBind");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/apply").then(function () {
            $scope.loadBind();
        }, function (r) { console.log(r); });
    }

    $scope.Virsh = null;

    $scope.loadVirsh = function () {
        console.log("loadVirsh");
        $scope.Virsh = null;
        $http.get("/virsh").success(function (data) {
            $scope.Virsh = data
        });
    }

    $scope.saveVirsh = function () {
        console.log("saveVirsh");
        var data = $.param({
            Data: angular.toJson($scope.Virsh)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/save", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.applyVirsh = function () {
        console.log("applyVirsh");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/apply").then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.saveVirsh = function () {
        console.log("saveVirsh");
        var data = $.param({
            Data: angular.toJson($scope.Virsh)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/save", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.virshDestroy = function (domain) {
        console.log("virshDestroy");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/destroy", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.virshReboot = function (domain) {
        console.log("virshReboot");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/reboot", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.virshReset = function (domain) {
        console.log("virshReset");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/reset", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.virshRestore = function (domain) {
        console.log("virshRestore");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/restore", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.virshResume = function (domain) {
        console.log("virshResume");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/resume", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.virshShutdown = function (domain) {
        console.log("virshShutdown");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/shutdown", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.virshStart = function (domain) {
        console.log("virshStart");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/start", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.virshSuspend = function (domain) {
        console.log("virshSuspend");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/suspend", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.virshDompmsuspend = function (domain) {
        console.log("virshDompmsuspend");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/dompmsuspend", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
    }

    $scope.virshDompmwakeup = function (domain) {
        console.log("virshDompmwakeup");
        var data = $.param({
            Data: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/virsh/dompmwakeup", data).then(function () {
            $scope.loadVirsh();
        }, function (r) { console.log(r); });
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

    $scope.loadFirewall = function () {
        console.log("loadFirewall");
        $scope.Firewall = null;
        $http.get("/firewall").success(function (data) {
            $scope.Firewall = data
        });
    }

    $scope.saveFirewall = function () {
        console.log("saveFirewall");
        var data = $.param({
            Data: angular.toJson($scope.Firewall)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/save", data).then(function () {
            $scope.loadFirewall();
        }, function (r) { console.log(r); });
    }

    $scope.applyFirewall = function () {
        console.log("applyFirewall");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/apply").then(function () {
            $scope.loadFirewall();
        }, function (r) { console.log(r); });
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

















    $scope.iconSave = "mif-floppy-disk";
    $scope.iconApply = "mif-arrow-right";
    $scope.iconLoading = "mif-spinner2 mif-ani-spin";

    $scope.iconStyle = {
        "padding-right": "20px",
        color: "#A7BD39",
    };

    $scope.buttonOriginal = {
        width: "100%",
        color: "white",
        height: "32px",
        "background-color": "#3A3A3A",
        border: "1px solid #222222",
        cursor: "pointer"
    };

    $scope.buttonDisabled = {
        width: "100%",
        color: "white",
        height: "32px",
        "background-color": "#8C8C8C",
        border: "1px solid #222222",
        cursor: "wait"
    };

    $scope.buttonStyle = $scope.buttonOriginal;

    $scope.click = function (action) {
        $scope.buttonStyle = $scope.buttonDisabled;
        action();
        $timeout(function () {
            $scope.buttonStyle = $scope.buttonOriginal;
        }, 500);
    }

    $scope.addToList = function (element, list) {
        var newElement = angular.copy(element);
        list.push(newElement);
    }

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

app.controller("MachineStatusController", ["$scope", "$http", "$interval", "$timeout", "$filter", MachineStatusController]);

function MachineStatusController($scope, $http, $interval, $timeout, $filter) {
    $scope.loadMemory = function () {
        console.log("loadMemory");
        $http.get("/info/memory").success(function (data) {
            $scope.memory = data
        });
    }

    $scope.loadFree = function () {
        console.log("loadFree");
        $http.get("/info/free").success(function (data) {
            $scope.free = data
        });
    }

    $scope.loadCpu = function () {
        console.log("loadCpu");
        $http.get("/info/cpu").success(function (data) {
            $scope.cpu = data
        });
    }

    $scope.iconSave = "mif-floppy-disk";
    $scope.iconApply = "mif-arrow-right";
    $scope.iconLoading = "mif-spinner2 mif-ani-spin";

    $scope.iconStyle = {
        "padding-right": "20px",
        color: "#A7BD39",
    };

    $scope.buttonOriginal = {
        width: "100%",
        color: "white",
        height: "32px",
        "background-color": "#3A3A3A",
        border: "1px solid #222222",
        cursor: "pointer"
    };

    $scope.buttonDisabled = {
        width: "100%",
        color: "white",
        height: "32px",
        "background-color": "#8C8C8C",
        border: "1px solid #222222",
        cursor: "wait"
    };

    $scope.buttonStyle = $scope.buttonOriginal;

    $scope.click = function (action) {
        $scope.buttonStyle = $scope.buttonDisabled;
        action();
        $timeout(function () {
            $scope.buttonStyle = $scope.buttonOriginal;
        }, 500);
    }

    $scope.addToList = function (element, list) {
        var newElement = angular.copy(element);
        list.push(newElement);
    }

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

app.controller("AssetController", ["$scope", "$http", "$interval", "$timeout", "$filter", AssetController]);

function AssetController($scope, $http, $interval, $timeout, $filter) {

    $scope.Neighborhood = null;

    $scope.loadNeighborhood = function () {
        console.log("loadNeighborhood");
        $scope.Neighborhood = null;
        $http.get("/rssdp/discover").success(function (data) {
            $scope.Neighborhood = data
        });
    }

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
            $scope.loadCluster();
        }, function (r) {
            console.log(r);
        });
    }

    $scope.Cluster = null;

    $scope.loadCluster = function () {
        console.log("loadCluster");
        $scope.Cluster = null;
        $http.get("/network/devices").success(function (data) {
            $scope.NetworkDevices = data
        });
        $http.get("/cluster").success(function (data) {
            $scope.Cluster = data
        });
    }

    $scope.saveCluster = function () {
        console.log("saveCluster");
        var data = $.param({
            Data: angular.toJson($scope.Cluster)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/save", data).then(function () {
            $scope.loadCluster();
        }, function (r) { console.log(r); });
    }

    $scope.applyCluster = function () {
        console.log("applyCluster");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/apply").then(function () {
            $scope.loadCluster();
        }, function (r) { console.log(r); });
        $http.post("/cluster/deploy").then(function () {
            $scope.loadCluster();
        }, function (r) { console.log(r); });
    }

    $scope.NewPortMapping = {
        ServiceName: '',
        ServicePort: 0,
        VirtualPort: 0
    };

    $scope.loadClusterStatus = function () {
        console.log("loadClusterStatus");
        $scope.ClusterStatus = null;
        $http.get("/cluster/status").success(function (data) {
            $scope.ClusterStatus = data
        });
    }














    $scope.iconSave = "mif-floppy-disk";
    $scope.iconApply = "mif-arrow-right";
    $scope.iconLoading = "mif-spinner2 mif-ani-spin";

    $scope.iconStyle = {
        "padding-right": "20px",
        color: "#A7BD39",
    };

    $scope.buttonOriginal = {
        width: "100%",
        color: "white",
        height: "32px",
        "background-color": "#3A3A3A",
        border: "1px solid #222222",
        cursor: "pointer"
    };

    $scope.buttonDisabled = {
        width: "100%",
        color: "white",
        height: "32px",
        "background-color": "#8C8C8C",
        border: "1px solid #222222",
        cursor: "wait"
    };

    $scope.buttonStyle = $scope.buttonOriginal;

    $scope.click = function (action) {
        $scope.buttonStyle = $scope.buttonDisabled;
        action();
        $timeout(function () {
            $scope.buttonStyle = $scope.buttonOriginal;
        }, 500);
    }

    $scope.addToList = function (element, list) {
        var newElement = angular.copy(element);
        list.push(newElement);
    }

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