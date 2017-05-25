"use strict";

app.controller("AntdAclController", ["$scope", "$http", "$sce", AntdAclController]);

function AntdAclController($scope, $http, $sce) {
    $scope.GetDetails = function (acl) {
        $http.get("/acl/get/" + acl.Guid).success(function (data) {
            acl.HtmlData = $sce.trustAsHtml(data);
            acl.Hide = false;
        });
    }

    $scope.AclApplyScript = function (user) {
        var data = $.param({
            User: user
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/apply/script", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.AclDelete = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/delete", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.AclCreate = function (guid, acl) {
        var data = $.param({
            Guid: guid,
            Acl: acl.HtmlData
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/create", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.AclAdd = function (path) {
        var data = $.param({
            Path: path
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/add", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/acl").success(function (data) {
        $scope.isActive = data.IsActive;
        $scope.Acl = data.Acl;
    });
}

app.controller("AntdBindController", ["$scope", "$http", AntdBindController]);

function AntdBindController($scope, $http) {
    $scope.toggle = function (el) {
        $(el).toggle();
    }

    $scope.deleteZone = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/zone/del", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.addZone = function (el) {
        var data = $.param({
            Name: el.Name,
            Type: el.Type,
            File: el.File,
            NameSerialUpdateMethod: el.NameSerialUpdateMethod,
            AllowUpdate: el.AllowUpdate,
            AllowQuery: el.AllowQuery,
            AllowTransfer: el.AllowTransfer
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/zone", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveOptions = function (bind) {
        var data = $.param({
            Notify: bind.Notify,
            MaxCacheSize: bind.MaxCacheSize,
            MaxCacheTtl: bind.MaxCacheTtl,
            MaxNcacheTtl: bind.MaxNcacheTtl,
            Forwarders: bind.Forwarders,
            AllowNotify: bind.AllowNotify,
            AllowTransfer: bind.AllowTransfer,
            Recursion: bind.Recursion,
            TransferFormat: bind.TransferFormat,
            QuerySourceAddress: bind.QuerySourceAddress,
            QuerySourcePort: bind.QuerySourcePort,
            Version: bind.Version,
            AllowQuery: bind.AllowQuery,
            AllowRecursion: bind.AllowRecursion,
            IxfrFromDifferences: bind.IxfrFromDifferences,
            ListenOnV6: bind.ListenOnV6,
            ListenOnPort53: bind.ListenOnPort53,
            DnssecEnabled: bind.DnssecEnabled,
            DnssecValidation: bind.DnssecValidation,
            DnssecLookaside: bind.DnssecLookaside,
            AuthNxdomain: bind.AuthNxdomain,
            KeyName: bind.KeyName,
            KeySecret: bind.KeySecret,
            ControlAcl: bind.ControlAcl,
            ControlIp: bind.ControlIp,
            ControlPort: bind.ControlPort,
            ControlAllow: bind.ControlAllow,
            LoggingChannel: bind.LoggingChannel,
            LoggingDaemon: bind.LoggingDaemon,
            LoggingSeverity: bind.LoggingSeverity,
            LoggingPrintCategory: bind.LoggingPrintCategory,
            LoggingPrintSeverity: bind.LoggingPrintSeverity,
            LoggingPrintTime: bind.LoggingPrintTime,
            TrustedKey: bind.TrustedKey,
            AclLocalInterfaces: bind.AclLocalInterfaces,
            AclInternalInterfaces: bind.AclInternalInterfaces,
            AclExternalInterfaces: bind.AclExternalInterfaces,
            AclLocalNetworks: bind.AclLocalNetworks,
            AclInternalNetworks: bind.AclInternalNetworks,
            AclExternalNetworks: bind.AclExternalNetworks
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/options", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/bind").success(function (data) {
        $scope.isActive = data.BindIsActive;
        $scope.Bind = data.BindOptions;
        $scope.BindZones = data.BindZones;
    });
}

app.controller("AntdDhcpdController", ["$scope", "$http", AntdDhcpdController]);

function AntdDhcpdController($scope, $http) {
    $scope.toggle = function (el) {
        $(el).toggle();
    }

    $scope.deleteReservation = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/reservation/del", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.addReservation = function (el) {
        var data = $.param({
            HostName: el.HostName,
            MacAddress: el.MacAddress,
            IpAddress: el.IpAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/reservation", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.deletePool = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/pool/del", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.addPool = function (el) {
        var data = $.param({
            Option: el.Option
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/pool", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.deleteClass = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/class/del", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.addClass = function (el) {
        var data = $.param({
            Name: el.Name,
            MacVendor: el.MacVendor
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/class", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveOptions = function (dhcpd) {
        var data = $.param({
            Allow: dhcpd.Allow,
            UpdateStaticLeases: dhcpd.UpdateStaticLeases,
            UpdateConflictDetection: dhcpd.UpdateConflictDetection,
            UseHostDeclNames: dhcpd.UseHostDeclNames,
            DoForwardUpdates: dhcpd.DoForwardUpdates,
            DoReverseUpdates: dhcpd.DoReverseUpdates,
            LogFacility: dhcpd.LogFacility,
            Option: dhcpd.Option,
            ZoneName: dhcpd.ZoneName,
            ZonePrimaryAddress: dhcpd.ZonePrimaryAddress,
            DdnsUpdateStyle: dhcpd.DdnsUpdateStyle,
            DdnsUpdates: dhcpd.DdnsUpdates,
            DdnsDomainName: dhcpd.DdnsDomainName,
            DdnsRevDomainName: dhcpd.DdnsRevDomainName,
            DefaultLeaseTime: dhcpd.DefaultLeaseTime,
            MaxLeaseTime: dhcpd.MaxLeaseTime,
            KeyName: dhcpd.KeyName,
            KeySecret: dhcpd.KeySecret,
            IpFamily: dhcpd.IpFamily,
            IpMask: dhcpd.IpMask,
            OptionRouters: dhcpd.OptionRouters,
            NtpServers: dhcpd.NtpServers,
            DoForTimeServerswardUpdates: dhcpd.DoForTimeServerswardUpdates,
            DomainNameServers: dhcpd.DomainNameServers,
            BroadcastAddress: dhcpd.BroadcastAddress,
            SubnetMask: dhcpd.SubnetMask
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/options", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/dhcpd").success(function (data) {
        $scope.isActive = data.DhcpdIsActive;
        $scope.Dhcpd = data.DhcpdOptions;
        $scope.DhcpdClass = data.DhcpdClass;
        $scope.DhcpdPools = data.DhcpdPools;
        $scope.DhcpdReservation = data.DhcpdReservation;
    });
}

app.controller("AntdDhcpLeasesController", ["$scope", "$http", AntdDhcpLeasesController]);

function AntdDhcpLeasesController($scope, $http) {
    $http.get("/dhcpleases").success(function (data) {
        $scope.DhcpLeases = data.DhcpdLeases;
    });
}

app.controller("AntdDiskUsageController", ["$scope", "$http", AntdDiskUsageController]);

function AntdDiskUsageController($scope, $http) {
    //todo fare remove lease

    $http.get("/diskusage").success(function (data) {
        $scope.DiskUsage = data.DisksUsage;
    });
}

app.controller("AntdFirewallController", ["$scope", "$http", AntdFirewallController]);

function AntdFirewallController($scope, $http) {
    $scope.saveFwIp6Nat = function (el) {
        angular.forEach(el.Sets, function (s) {
            var set = $.param({
                Set: s.Set,
                Type: s.Type,
                Elements: s.Elements
            });
            $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
            $http.post("/firewall/ipv6/nat/set", set).then(function () { console.log(1); }, function (r) { console.log(r); });
        });
        angular.forEach(el.Chains, function (c) {
            var chain = $.param({
                Chain: c.Chain,
                Elements: c.Elements
            });
            $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
            $http.post("/firewall/ipv6/nat/chain", chain).then(function () { console.log(1); }, function (r) { console.log(r); });
        });
    }

    $scope.saveFwIp6Filter = function (el) {
        angular.forEach(el.Sets, function (s) {
            var set = $.param({
                Set: s.Set,
                Type: s.Type,
                Elements: s.Elements
            });
            $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
            $http.post("/firewall/ipv6/filter/set", set).then(function () { console.log(1); }, function (r) { console.log(r); });
        });
        angular.forEach(el.Chains, function (c) {
            var chain = $.param({
                Chain: c.Chain,
                Elements: c.Elements
            });
            $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
            $http.post("/firewall/ipv6/filter/chain", chain).then(function () { console.log(1); }, function (r) { console.log(r); });
        });
    }

    $scope.saveFwIp4Nat = function (el) {
        angular.forEach(el.Sets, function (s) {
            var set = $.param({
                Set: s.Set,
                Type: s.Type,
                Elements: s.Elements
            });
            $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
            $http.post("/firewall/ipv4/nat/set", set).then(function () { console.log(1); }, function (r) { console.log(r); });
        });
        angular.forEach(el.Chains, function (c) {
            var chain = $.param({
                Chain: c.Chain,
                Elements: c.Elements
            });
            $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
            $http.post("/firewall/ipv4/nat/chain", chain).then(function () { console.log(1); }, function (r) { console.log(r); });
        });
    }

    $scope.saveFwIp4Filter = function (el) {
        angular.forEach(el.Sets, function (s) {
            var set = $.param({
                Set: s.Set,
                Type: s.Type,
                Elements: s.Elements
            });
            $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
            $http.post("/firewall/ipv4/filter/set", set).then(function () { console.log(1); }, function (r) { console.log(r); });
        });
        angular.forEach(el.Chains, function (c) {
            var chain = $.param({
                Chain: c.Chain,
                Elements: c.Elements
            });
            $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
            $http.post("/firewall/ipv4/filter/chain", chain).then(function () { console.log(1); }, function (r) { console.log(r); });
        });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/firewall").success(function (data) {
        $scope.isActive = data.FirewallIsActive;
        $scope.FwIp4Filter = data.FwIp4Filter;
        $scope.FwIp4Nat = data.FwIp4Nat;
        $scope.FwIp6Filter = data.FwIp6Filter;
        $scope.FwIp6Nat = data.FwIp6Nat;
    });
}

app.controller("AntdNameServiceController", ["$scope", "$http", AntdNameServiceController]);

function AntdNameServiceController($scope, $http) {
    $scope.saveExtDomain = function (domain) {
        var data = $.param({
            Domain: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/ext/domain", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveIntDomain = function (domain) {
        var data = $.param({
            Domain: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/int/domain", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveSwitch = function (switches) {
        var data = $.param({
            Switch: switches
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/nameservice/switch", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveResolv = function (resolv) {
        var data = $.param({
            Resolv: resolv
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/nameservice/resolv", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveNetworks = function (networks) {
        var data = $.param({
            Networks: networks
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/nameservice/networks", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveHosts = function (hosts) {
        var data = $.param({
            Hosts: hosts
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/nameservice/hosts", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/nameservice").success(function (data) {
        $scope.Hostname = data.Hostname;
        $scope.DomainInt = data.DomainInt;
        $scope.DomainExt = data.DomainExt;
        $scope.Hosts = data.Hosts;
        $scope.HostsEdit = data.HostsEdit;
        $scope.Networks = data.Networks;
        $scope.NetworksEdit = data.NetworksEdit;
        $scope.Resolv = data.Resolv;
        $scope.ResolvEdit = data.ResolvEdit;
        $scope.Nsswitch = data.Nsswitch;
        $scope.NsswitchEdit = data.NsswitchEdit;
    });
}

app.controller("AntdNetworkController", ["$scope", "$http", AntdNetworkController]);

function AntdNetworkController($scope, $http) {

    $scope.SelectInterfaceConfig = {
        valueField: "text",
        labelField: "text",
        searchField: ["text"],
        persist: false,
        create: false,
        //onChange: function (value) {
        //    $scope.RecipientsData = "";
        //    angular.forEach(value, function (v) {
        //        $scope.RecipientsData += v + ",";
        //    });
        //},
        delimiter: ",",
        maxItems: 5
    };

    $scope.saveBond = function (el) {
        var data = $.param({
            Interface: el.Interface,
            Mode: el.Mode,
            Status: el.Status,
            StaticAddres: el.StaticAddres,
            StaticRange: el.StaticRange,
            Txqueuelen: el.Txqueuelen,
            Mtu: el.Mtu,
            InterfaceList: el.InterfaceList,
            Route: el.Route,
            Gateway: el.Gateway
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/interface/bond", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveBridge = function (el) {
        var data = $.param({
            Interface: el.Interface,
            Mode: el.Mode,
            Status: el.Status,
            StaticAddres: el.StaticAddres,
            StaticRange: el.StaticRange,
            Txqueuelen: el.Txqueuelen,
            Mtu: el.Mtu,
            InterfaceList: el.InterfaceList,
            Route: el.Route,
            Gateway: el.Gateway
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/interface/bridge", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.deleteInterface = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/interface/del", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveInterface = function (el) {
        var data = $.param({
            Interface: el.Interface,
            Mode: el.Mode,
            Status: el.Status,
            StaticAddres: el.StaticAddres,
            StaticRange: el.StaticRange,
            Txqueuelen: el.Txqueuelen,
            Mtu: el.Mtu,
            Route: el.Route,
            Gateway: el.Gateway
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/interface", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/network").success(function (data) {
        $scope.NetworkIfList = data.NetworkIfList;
        $scope.NetworkPhysicalIf = data.NetworkPhysicalIf;
        $scope.NetworkBridgeIf = data.NetworkBridgeIf;
        $scope.NetworkBondIf = data.NetworkBondIf;
        $scope.NetworkVirtualIf = data.NetworkVirtualIf;
    });
}

app.controller("AntdNetwork2Controller", ["$scope", "$http", "$interval", AntdNetwork2Controller]);

function AntdNetwork2Controller($scope, $http, $interval) {

    $scope.saveDefaultValues = function ($event) {
        var data = $.param({
            HostName: $scope.Variables.HostName,
            HostChassis: $scope.Variables.HostChassis,
            HostDeployment: $scope.Variables.HostDeployment,
            HostLocation: $scope.Variables.HostLocation,
            HostAliasPrimary: $scope.Variables.HostAliasPrimary,
            InternalDomainPrimary: $scope.Variables.InternalDomainPrimary,
            ExternalDomainPrimary: $scope.Variables.ExternalDomainPrimary,
            InternalHostIpPrimary: $scope.Variables.InternalHostIpPrimary,
            ExternalHostIpPrimary: $scope.Variables.ExternalHostIpPrimary,
            InternalNetPrimaryBits: $scope.Variables.InternalNetPrimaryBits,
            ExternalNetPrimaryBits: $scope.Variables.ExternalNetPrimaryBits,
            ResolvNameserver: $scope.Variables.ResolvNameserver,
            ResolvDomain: $scope.Variables.ResolvDomain,
            Timezone: $scope.Variables.Timezone,
            NtpdateServer: $scope.Variables.NtpdateServer,
            Cloud: $scope.Variables.Cloud
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host2/info", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (response) { console.log(response); });
    }

    $scope.IpModeOnChange = function (mode) {
        if (mode === "2") {
            $scope.HideIfDynamic = true;
        } else {
            $scope.HideIfDynamic = false;
        }
    }

    $scope.HideIfDynamic = true;

    $scope.createBond = function ($event, el) {
        var data = $.param({
            Name: el.Name
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/add/bond", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.createBridge = function ($event, el) {
        var data = $.param({
            Name: el.Name
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/add/bridge", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.NewLagConfiguration = {
        Id: "",
        Parent: "",
        Children: []
    };

    $scope.deleteLagConfiguration = function ($event, el) {
        var data = $.param({
            Guid: el.Id
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/lagconfiguration/del", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.saveLagConfiguration = function ($event, el) {
        var data = $.param({
            Id: el.Id,
            Parent: el.Parent,
            Children: el.Children
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/lagconfiguration", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.NewDnsConfiguration = {
        Id: "",
        Type: "",
        Mode: "",
        Destination: "",
        Domain: "",
        Ip: ""
    };

    $scope.saveActiveDnsConfiguration = function ($event) {
        var data = $.param({
            Guid: $scope.ActiveDnsConfiguration
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/dnsconfiguration/active", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.deleteDnsConfiguration = function ($event, el) {
        var data = $.param({
            Guid: el.Id
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/dnsconfiguration/del", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.saveDnsConfiguration = function ($event, el) {
        var data = $.param({
            Id: el.Id,
            Type: el.Type,
            Domain: el.Domain,
            Ip: el.Ip
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/dnsconfiguration", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    //$scope.AddIpConfiguration = function ($event, el) {
    //    console.log(el.AdditionalConfigurations);
    //    el.AdditionalConfigurations.push(el.AdditionalConfigurations.length);
    //    console.log(el.AdditionalConfigurations);
    //}

    //$scope.AddThisIpConfigurationToList = function (index, el, conf) {
    //    el.AdditionalConfigurations.push(conf);
    //}

    //$scope.DelIpConfiguration = function ($event, el, list) {
    //    var index = list.indexOf(el);
    //    if (index > -1) {
    //        list.splice(index, 1);
    //    }
    //}

    $scope.NewInterface = {
        Device: "",
        Status: "1",
        Configuration: "",
        AdditionalConfigurations: [],
        GatewayConfiguration: "",
        HardwareConfiguration: "",
        ManagedInterfaces: []
    };

    $scope.delete = function (index) {
        console.log($scope.Configuration.length);
        $scope.Configuration.splice(index, 1);
        console.log($scope.Configuration.length);
    }

    $scope.addAndSave = function ($event, el) {
        $scope.Configuration.push(el);
        $scope.save();
    }

    $scope.save = function ($event) {
        var config = angular.toJson($scope.Configuration, true);
        var data = $.param({
            Config: config
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/interface2", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.NewRouteConfiguration = {
        Id: "",
        DestinationIp: "",
        DestinationRange: "",
        Gateway: ""
    };

    $scope.deleteRouteConfiguration = function ($event, el) {
        var data = $.param({
            Guid: el.Id
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/routeconfiguration/del", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.saveRouteConfiguration = function ($event, el) {
        var data = $.param({
            Id: el.Id,
            DestinationIp: el.DestinationIp,
            DestinationRange: el.DestinationRange,
            Gateway: el.Gateway
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/routeconfiguration", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.AppendSlash = function (s) {
        return s === "default" ? s : s + "/";
    }

    $scope.IsGwDefaultStr = function (s) {
        return s === true ? "(default)" : "";
    }

    $scope.NewGatewayConfiguration = {
        Id: "",
        GatewayAddress: "",
        Description: "",
        IsDefault: true
    };

    $scope.deleteGatewayConfiguration = function ($event, el) {
        var data = $.param({
            Guid: el.Id
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/gatewayconfiguration/del", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.saveGatewayConfiguration = function ($event, el) {
        var data = $.param({
            Id: el.Id,
            Description: el.Description,
            Default: el.IsDefault,
            GatewayAddress: el.GatewayAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/gatewayconfiguration", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.NewHwConfiguration = {
        Id: "",
        Txqueuelen: "",
        Mtu: "",
        MacAddress: ""
    };

    $scope.deleteHwConfiguration = function ($event, el) {
        var data = $.param({
            Guid: el.Id
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/hardwareconfiguration/del", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.saveHwConfiguration = function ($event, el) {
        var data = $.param({
            Id: el.Id,
            Txqueuelen: el.Txqueuelen,
            Mtu: el.Mtu,
            MacAddress: el.MacAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/hardwareconfiguration", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.NewInterfaceConfiguration = {
        Id: "",
        Type: "",
        Description: "",
        Verb: "",
        Mode: "2",
        Ip: "",
        Range: "",
        Adapter: "",
        Ifs: "",
        PostIfs: ""
    };

    $scope.deleteInterfaceConfiguration = function ($event, el) {
        var data = $.param({
            Guid: el.Id
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/interfaceconfiguration/del", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.saveInterfaceConfiguration = function ($event, el) {
        var data = $.param({
            Id: el.Id,
            Type: el.Type,
            Description: el.Description,
            Mode: el.Mode,
            Ip: el.Ip,
            Range: el.Range
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/interfaceconfiguration", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.restart = function ($event) {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network2/restart").then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.AppendMessageToButton = function (el, message) {
        var originalText = el.innerHTML;
        var newText = originalText + " - " + message;
        el.innerHTML = newText;
        $interval(function () {
            el.innerHTML = originalText;
        }, 1666);
        $scope.Get();
    }

    $scope.Get = function () {
        $http.get("/network2").success(function (data) {
            $scope.ActiveDnsConfiguration = data.ActiveDnsConfiguration;
            $scope.DnsConfigurationList = data.DnsConfigurationList;
            $scope.InterfaceConfigurationList = data.InterfaceConfigurationList;
            $scope.GatewayConfigurationList = data.GatewayConfigurationList;
            $scope.RouteConfigurationList = data.RouteConfigurationList;
            $scope.NetworkHardwareConfigurationList = data.NetworkHardwareConfigurationList;
            $scope.LagConfigurationList = data.LagConfigurationList;
            $scope.Configuration = data.Configuration;
            $scope.AllIfs = data.AllIfs;
            $scope.PhysicalIf = data.PhysicalIf;
            $scope.BridgeIf = data.BridgeIf;
            $scope.BondIf = data.BondIf;
            $scope.VirtualIf = data.VirtualIf;
            $scope.Variables = data.Variables;
        });
    }
    $scope.Get();

    //$scope.ShowResponseMessage("ok");
    $scope.ResponseMessage = "";
    $scope.ResponseMessageHide = true;
    $scope.ShowResponseMessage = function (message) {
        $scope.ResponseMessageHide = false;
        $scope.ResponseMessage = message;
        $interval(function () {
            $scope.ResponseMessageHide = true;
        }, 1666);
        $scope.Get();
    }

    $scope.ResponseMessageHideSelf = function () {
        $scope.ResponseMessage = "";
        $scope.ResponseMessageHide = true;
    }
}

app.controller("AntdRsyncController", ["$scope", "$http", AntdRsyncController]);

function AntdRsyncController($scope, $http) {
    $scope.toggle = function (el) {
        $(el).toggle();
    }

    $scope.deleteDirectory = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/directory/del", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.addDirectory = function (el) {
        var data = $.param({
            Source: el.Source,
            Destination: el.Destination,
            Type: el.Type
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/directory", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/rsync").success(function (data) {
        $scope.isActive = data.RsyncIsActive;
        $scope.Rsync = data.RsyncDirectories;
    });
}

app.controller("AntdSambaController", ["$scope", "$http", AntdSambaController]);

function AntdSambaController($scope, $http) {
    $scope.toggle = function (el) {
        $(el).toggle();
    }

    $scope.deleteResource = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/resource/del", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.addResource = function (el) {
        var data = $.param({
            Name: el.Name,
            Path: el.Path,
            Comment: el.Comment
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/resource", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.saveOptions = function (el) {
        var data = $.param({
            DosCharset: el.DosCharset,
            Workgroup: el.Workgroup,
            ServerString: el.ServerString,
            MapToGuest: el.MapToGuest,
            ObeyPamRestrictions: el.ObeyPamRestrictions,
            GuestAccount: el.GuestAccount,
            PamPasswordChange: el.PamPasswordChange,
            PasswdProgram: el.PasswdProgram,
            UnixPasswordSync: el.UnixPasswordSync,
            ResetOnZeroVc: el.ResetOnZeroVc,
            HostnameLookups: el.HostnameLookups,
            LoadPrinters: el.LoadPrinters,
            PrintcapName: el.PrintcapName,
            DisableSpoolss: el.DisableSpoolss,
            TemplateShell: el.TemplateShell,
            WinbindEnumUsers: el.WinbindEnumUsers,
            WinbindEnumGroups: el.WinbindEnumGroups,
            WinbindUseDefaultDomain: el.WinbindUseDefaultDomain,
            WinbindNssInfo: el.WinbindNssInfo,
            WinbindRefreshTickets: el.WinbindRefreshTickets,
            WinbindNormalizeNames: el.WinbindNormalizeNames,
            RecycleTouch: el.RecycleTouch,
            RecycleKeeptree: el.RecycleKeeptree,
            RecycleRepository: el.RecycleRepository,
            Nfs4Chown: el.Nfs4Chown,
            Nfs4Acedup: el.Nfs4Acedup,
            Nfs4Mode: el.Nfs4Mode,
            ShadowFormat: el.ShadowFormat,
            ShadowLocaltime: el.ShadowLocaltime,
            ShadowSort: el.ShadowSort,
            ShadowSnapdir: el.ShadowSnapdir,
            RpcServerDefault: el.RpcServerDefault,
            RpcServerSvcctl: el.RpcServerSvcctl,
            RpcServerSrvsvc: el.RpcServerSrvsvc,
            RpcServerEventlog: el.RpcServerEventlog,
            RpcServerNtsvcs: el.RpcServerNtsvcs,
            RpcServerWinreg: el.RpcServerWinreg,
            RpcServerSpoolss: el.RpcServerSpoolss,
            RpcDaemonSpoolssd: el.RpcDaemonSpoolssd,
            RpcServerTcpip: el.RpcServerTcpip,
            IdmapConfigBackend: el.IdmapConfigBackend,
            ReadOnly: el.ReadOnly,
            GuestOk: el.GuestOk,
            AioReadSize: el.AioReadSize,
            AioWriteSize: el.AioWriteSize,
            EaSupport: el.EaSupport,
            DirectoryNameCacheSize: el.DirectoryNameCacheSize,
            CaseSensitive: el.CaseSensitive,
            MapReadonly: el.MapReadonly,
            StoreDosAttributes: el.StoreDosAttributes,
            WideLinks: el.WideLinks,
            DosFiletimeResolution: el.DosFiletimeResolution,
            VfsObjects: el.VfsObjects
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/options", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/samba").success(function (data) {
        $scope.isActive = data.SambaIsActive;
        $scope.Samba = data.SambaOptions;
        $scope.SambaResources = data.SambaResources;
    });
}

app.controller("AntdSchedulerController", ["$scope", "$http", AntdSchedulerController]);

function AntdSchedulerController($scope, $http) {
    $scope.deleteTimer = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/scheduler/timer/del", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.addTimer = function (el) {
        var data = $.param({
            Alias: el.Alias,
            Time: el.Time,
            Command: el.Command
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/scheduler/timer", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/scheduler/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/scheduler/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/scheduler/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/scheduler").success(function (data) {
        $scope.Jobs = data.Jobs;
    });
}

app.controller("AntdSshdController", ["$scope", "$http", AntdSshdController]);

function AntdSshdController($scope, $http) {
    $scope.saveOptions = function (el) {
        var data = $.param({
            Port: el.Port,
            PermitRootLogin: el.PermitRootLogin,
            PermitTunnel: el.PermitTunnel,
            MaxAuthTries: el.MaxAuthTries,
            MaxSessions: el.MaxSessions,
            RsaAuthentication: el.RsaAuthentication,
            PubkeyAuthentication: el.PubkeyAuthentication,
            UsePam: el.UsePam
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/sshd/options", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/sshd/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/sshd/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/sshd/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/sshd/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/sshd/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/sshd").success(function (data) {
        $scope.isActive = data.SshdIsActive;
        $scope.Sshd = data.SshdOptions;
    });
}

app.controller("AntdStorageController", ["$scope", "$http", AntdStorageController]);

function AntdStorageController($scope, $http) {
    $scope.createPartition = function (el) {
        var data = $.param({
            Disk: "/dev/" + el.Name,
            PartitionNumber: el.PartitionNumber,
            PartitionFirstSector: el.PartitionFirstSector,
            PartitionSize: el.PartitionSize,
            PartitionName: el.PartitionName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/zpool/create", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.createPool = function (el) {
        var data = $.param({
            Altroot: el.Altroot,
            Disk: "/dev/" + el.Name,
            Type: el.PoolType,
            Name: el.PoolName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/zpool/create", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.print = function (el) {
        var data = $.param({
            Disk: el.Disk
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/storage/print", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.mklabel = function (el) {
        var data = $.param({
            Disk: "/dev/" + el.Name,
            Type: el.PartitionTable,
            Confirm: ""
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/storage/mklabel", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/storage").success(function (data) {
        $scope.Disks = data.DisksList;
    });
}

app.controller("AntdVmController", ["$scope", "$http", AntdVmController]);

function AntdVmController($scope, $http) {
    $http.get("/vm").success(function (data) {
        $scope.Vm = data.VmList;
    });
}

app.controller("AntdVpnController", ["$scope", "$http", AntdVpnController]);

function AntdVpnController($scope, $http) {
    $scope.saveOptions = function (el) {
        var data = $.param({
            RemoteHost: el.RemoteHost,
            RemoteAddress: el.RemoteAddress,
            RemoteRange: el.RemoteRange,
            LocalAddress: el.LocalAddress,
            LocalRange: el.LocalRange
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/options", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/vpn").success(function (data) {
        $scope.isActive = data.VpnIsActive;
        $scope.Vpn = {
            RemoteHost: data.VpnRemoteHost,
            RemoteAddress: data.VpnRemotePoint.Address,
            RemoteRange: data.VpnRemotePoint.Range,
            LocalAddress: data.VpnLocalPoint.Address,
            LocalRange: data.VpnLocalPoint.Range
        };
    });
}

app.controller("AntdTorController", ["$scope", "$http", "$interval", AntdTorController]);

function AntdTorController($scope, $http, $interval) {

    $scope.remove = function (el, list) {
        var index = list.indexOf(el);
        if (index > -1) {
            list.splice(index, 1);
        }
    }

    $scope.add = function () {
        var ns = { Name: "", IpAddress: "", TorPort: "" };
        $scope.Services.push(ns);
    }

    $scope.save = function ($event) {
        var config = angular.toJson($scope.Services);
        var data = $.param({
            Config: config
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/tor/save", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/tor/restart").then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/tor/stop").then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/tor/enable").then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/tor/disable").then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/tor/set").then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.AppendMessageToButton = function (el, message) {
        var originalText = el.innerHTML;
        var newText = originalText + " - " + message;
        el.innerHTML = newText;
        $interval(function () {
            el.innerHTML = originalText;
        }, 1666);
    }

    $scope.Services = [];

    $http.get("/tor").success(function (data) {
        $scope.isActive = data.TorIsActive;
        $scope.Services = data.Services;
    });
}

app.controller("AntdZfsController", ["$scope", "$http", AntdZfsController]);

function AntdZfsController($scope, $http) {
    $scope.zfsCreate = function (el) {
        var data = $.param({
            Altroot: el.Altroot,
            Name: el.Name,
            Dataset: el.Dataset
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/zfs/create", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.zpoolCreate = function (el) {
        var data = $.param({
            Altroot: el.Altroot,
            Disk: el.Disk,
            Type: el.PoolType,
            Name: el.PoolName
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/zpool/create", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.snapDisable = function (el) {
        var data = $.param({
            Guid: el.Guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/zfs/snap/disable", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.snap = function (el) {
        var data = $.param({
            Pool: el.Name,
            Interval: el.Timer
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/zfs/snap", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/zfs").success(function (data) {
        $scope.ZpoolList = data.ZpoolList;
        $scope.ZfsList = data.ZfsList;
        $scope.ZfsSnap = data.ZfsSnap;
        $scope.ZpoolHistory = data.ZpoolHistory;
        $scope.Disks = data.DisksList;
    });
}

app.controller("AntdTimeController", ["$scope", "$http", AntdTimeController]);

function AntdTimeController($scope, $http) {
    $scope.SaveTimeNtp = function (ntp) {
        var data = $.param({
            Ntp: ntp
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/ntpd", data).then(function () { console.log(1); }, function (response) { console.log(response); });
    }

    $scope.SaveTimeNtpdate = function (ntpdate) {
        var data = $.param({
            Ntpdate: ntpdate
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/ntpdate", data).then(
            function () {
                console.log(1);
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.SaveTimeSync = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/synctime").then(
            function () {
                console.log(1);
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
                console.log(1);
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/time").success(function (data) {
        $scope.Time = data;
        $scope.Timezones = data;
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
                console.log(1);
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
                console.log(1);
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
                console.log(1);
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
                console.log(1);
            },
        function (response) {
            console.log(response);
        });
    }

    $http.get("/host").success(function (data) {
        $scope.Host = data;
    });
}

app.controller("AntdHost2Controller", ["$scope", "$http", "$interval", AntdHost2Controller]);

function AntdHost2Controller($scope, $http, $interval) {
    $scope.Save = function () {
        var data = $.param({
            HostName: $scope.Host.HostName,
            HostChassis: $scope.Host.HostChassis,
            HostDeployment: $scope.Host.HostDeployment,
            HostLocation: $scope.Host.HostLocation,
            HostAliasPrimary: $scope.Host.HostAliasPrimary,
            InternalDomainPrimary: $scope.Host.InternalDomainPrimary,
            ExternalDomainPrimary: $scope.Host.ExternalDomainPrimary,
            InternalHostIpPrimary: $scope.Host.InternalHostIpPrimary,
            ExternalHostIpPrimary: $scope.Host.ExternalHostIpPrimary,
            InternalNetPrimaryBits: $scope.Host.InternalNetPrimaryBits,
            ExternalNetPrimaryBits: $scope.Host.ExternalNetPrimaryBits,
            ResolvNameserver: $scope.Host.ResolvNameserver,
            ResolvDomain: $scope.Host.ResolvDomain,
            Timezone: $scope.Host.Timezone,
            NtpdateServer: $scope.Host.NtpdateServer,
            Cloud: $scope.Host.Cloud
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host2/info", data).then(
            function () {
                $scope.ShowResponseMessage("ok");
            },
        function (response) {
            console.log(response);
        });
    }

    $scope.Get = function () {
        $http.get("/host2").success(function (data) {
            $scope.Host = data.Host;
            $scope.IconName = data.IconName;
            $scope.MachineId = data.MachineId;
            $scope.BootId = data.BootId;
            $scope.Virtualization = data.Virtualization;
            $scope.Os = data.Os;
            $scope.Kernel = data.Kernel;
            $scope.Architecture = data.Architecture;
        });
    }
    $scope.Get();

    //$scope.ShowResponseMessage("ok");
    $scope.ResponseMessage = "";
    $scope.ResponseMessageHide = true;
    $scope.ShowResponseMessage = function (message) {
        $scope.ResponseMessageHide = false;
        $scope.ResponseMessage = message;
        $interval(function () {
            $scope.ResponseMessageHide = true;
        }, 1666);
        $scope.Get();
    }

    $scope.ResponseMessageHideSelf = function () {
        $scope.ResponseMessage = "";
        $scope.ResponseMessageHide = true;
    }
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
                console.log(1);
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
                console.log(1);
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
                console.log(1);
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
    $scope.SetDirectory = function (dir) {
        var data = $.param({
            Directory: dir.Key
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/overlay/setdirectory", data).then(
            function () {
                console.log(1);
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
            DatabasePath: $scope.DatabasePath,
            CloudAddress: $scope.CloudAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/config", data).then(
            function () {
                console.log(1);
            },
        function (response) {
            console.log(response);
        });
    };

    $http.get("/config").success(function (data) {
        $scope.AntdPort = data.AntdPort;
        $scope.AntdUiPort = data.AntdUiPort;
        $scope.DatabasePath = data.DatabasePath;
        $scope.CloudAddress = data.CloudAddress;
    });

    $scope.Show = function (el) {
        if ($scope.Port !== $scope.CurrentPort) {
            $(el).show();
        } else {
            $(el).hide();
        }
    };
}
