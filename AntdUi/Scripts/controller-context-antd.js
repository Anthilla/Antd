"use strict";

app.controller("AntdAclController", ["$scope", "$http", AntdAclController]);

function AntdAclController($scope, $http) {
    $scope.GetDetails = function (guid) {
        $http.get("/acl/get/" + guid).success(function (data) {
            return data;
        });
    }

    $scope.AclApplyScript = function (user) {
        var data = $.param({
            User: user
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/apply/script", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.AclDelete = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/delete", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.AclCreate = function (guid, acl) {
        var data = $.param({
            Guid: guid,
            Acl: acl
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/create", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.AclAdd = function (path) {
        var data = $.param({
            Path: path
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/add", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/acl/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/acl").success(function (data) {
        $scope.isActive = data.IsActive;
        $scope.Acl = data.Settings;
    });
}

app.controller("AntdBindController", ["$scope", "$http", AntdBindController]);

function AntdBindController($scope, $http) {
    $scope.deleteZone = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/zone/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
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
        $http.post("/bind/zone", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
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
        $http.post("/bind/options", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/bind/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/bind").success(function (data) {
        $scope.isActive = data.BindIsActive;
        $scope.Bind = data.BindOptions;
        $scope.BindZones = data.BindZones;
    });
}

app.controller("AntdDhcpdController", ["$scope", "$http", AntdDhcpdController]);

function AntdDhcpdController($scope, $http) {
    $scope.deleteReservation = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/reservation/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addReservation = function (el) {
        var data = $.param({
            HostName: el.HostName,
            MacAddress: el.MacAddress,
            IpAddress: el.IpAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/reservation", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.deletePool = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/pool/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addPool = function (el) {
        var data = $.param({
            Option: el.Option
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/pool", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.deleteClass = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/class/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addClass = function (el) {
        var data = $.param({
            Name: el.Name,
            MacVendor: el.MacVendor
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/class", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
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
        $http.post("/dhcpd/options", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/dhcpd/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
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
        $scope.DhcpLeases = data;
    });
}

app.controller("AntdDiskUsageController", ["$scope", "$http", AntdDiskUsageController]);

function AntdDiskUsageController($scope, $http) {
    $http.get("/diskusage").success(function (data) {
        $scope.DiskUsage = data;
    });
}

app.controller("AntdFirewallController", ["$scope", "$http", AntdFirewallController]);

function AntdFirewallController($scope, $http) {
    $scope.ipv6NatChain = function (el) {
        var data = $.param({
            Chain: el.Chain,
            Elements: el.Elements
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/ipv6/nat/chain", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.ipv6NatSet = function (el) {
        var data = $.param({
            Set: el.Set,
            Type: el.Type,
            Elements: el.Elements
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/ipv6/nat/set", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.ipv6FilterChain = function (el) {
        var data = $.param({
            Chain: el.Chain,
            Elements: el.Elements
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/ipv6/filter/chain", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.ipv6FilterSet = function (el) {
        var data = $.param({
            Set: el.Set,
            Type: el.Type,
            Elements: el.Elements
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/ipv6/filter/set", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.ipv4NatChain = function (el) {
        var data = $.param({
            Chain: el.Chain,
            Elements: el.Elements
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/ipv4/nat/chain", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.ipv4NatSet = function (el) {
        var data = $.param({
            Set: el.Set,
            Type: el.Type,
            Elements: el.Elements
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/ipv4/nat/set", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.ipv4FilterChain = function (el) {
        var data = $.param({
            Chain: el.Chain,
            Elements: el.Elements
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/ipv4/filter/chain", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.ipv4FilterSet = function (el) {
        var data = $.param({
            Set: el.Set,
            Type: el.Type,
            Elements: el.Elements
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/ipv4/filter/set", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/firewall/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/firewall").success(function (data) {
        $scope.isActive = data.FirewallIsActive;
        $scope.Firewall = data.FirewallOptions;
        $scope.FwIp4Filter = data.FwIp4Filter;
        $scope.FwIp4Nat = data.FwIp4Nat;
        $scope.FwIp6Filter = data.FwIp6Filter;
        $scope.FwIp6Nat = data.FwIp6Nat;
    });
}

app.controller("AntdGlusterController", ["$scope", "$http", AntdGlusterController]);

function AntdGlusterController($scope, $http) {
    $scope.deleteNode = function (el) {
        var data = $.param({
            Node: el.Node
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/gluster/node/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addNode = function (el) {
        var data = $.param({
            Node: el.Node
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/gluster/node", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.saveOptions = function (gluster) {
        var data = $.param({
            GlusterNode: gluster.GlusterNode,
            GlusterVolumeName: gluster.GlusterVolumeName,
            GlusterVolumeBrick: gluster.GlusterVolumeBrick,
            GlusterVolumeMountPoint: gluster.GlusterVolumeMountPoint
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/gluster/options", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/gluster/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/gluster/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/gluster/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/gluster/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/gluster/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/gluster").success(function (data) {
        $scope.isActive = data.GlusterIsActive;
        $scope.GlusterNodes = data.Nodes;
        $scope.GlusterVolumes = data.Volumes;
    });
}

app.controller("AntdNameServiceController", ["$scope", "$http", AntdNameServiceController]);

function AntdNameServiceController($scope, $http) {
    $scope.saveExtDomain = function (domain) {
        var data = $.param({
            Domain: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/ext/domain", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.saveIntDomain = function (domain) {
        var data = $.param({
            Domain: domain
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/int/domain", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.saveSwitch = function (switches) {
        var data = $.param({
            Switch: switches
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/nameservice/switch", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.saveResolv = function (resolv) {
        var data = $.param({
            Resolv: resolv
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/nameservice/resolv", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.saveNetworks = function (networks) {
        var data = $.param({
            Networks: networks
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/nameservice/networks", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.saveHosts = function (hosts) {
        var data = $.param({
            Hosts: hosts
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/nameservice/hosts", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/nameservice").success(function (data) {
        $scope.NameService = data;
    });
}

app.controller("AntdNetworkController", ["$scope", "$http", AntdNetworkController]);

function AntdNetworkController($scope, $http) {
    $scope.deleteInterface = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/interface/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addInterface = function (el) {
        var data = $.param({
            Interface: el.Interface,
            Mode: el.Mode,
            Status: el.Status,
            StaticAddres: el.StaticAddres,
            StaticRange: el.StaticRange,
            Txqueuelen: el.Txqueuelen,
            Mtu: el.Mtu
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/interface", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/network").success(function (data) {
        $scope.Network = data;
    });
}

app.controller("AntdRsyncController", ["$scope", "$http", AntdRsyncController]);

function AntdRsyncController($scope, $http) {
    $scope.deleteDirectory = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/directory/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addDirectory = function (el) {
        var data = $.param({
            Source: el.Source,
            Destination: el.Destination,
            Type: el.Type
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/directory", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/rsync/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/rsync").success(function (data) {
        $scope.isActive = data.RsyncIsActive;
        $scope.Rsync = data.RsyncDirectories;
    });
}

app.controller("AntdSambaController", ["$scope", "$http", AntdSambaController]);

function AntdSambaController($scope, $http) {
    $scope.deleteReservation = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/reservation/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addReservation = function (el) {
        var data = $.param({
            HostName: el.HostName,
            MacAddress: el.MacAddress,
            IpAddress: el.IpAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/reservation", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.deletePool = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/pool/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addPool = function (el) {
        var data = $.param({
            Option: el.Option
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/pool", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.deleteResource = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/resource/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addResource = function (el) {
        var data = $.param({
            Name: el.Name,
            Path: el.Path,
            Comment: el.Comment
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/resource", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
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
        $http.post("/samba/options", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/samba/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
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
        $http.post("/scheduler/timer/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addTimer = function (el) {
        var data = $.param({
            Alias: el.Alias,
            Time: el.Time,
            Command: el.Command
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/scheduler/timer", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/scheduler/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/scheduler/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/scheduler/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/scheduler").success(function (data) {
        $scope.Scheduler = data.Jobs;
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
        $http.post("/sshd/options", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/sshd/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/sshd/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/sshd/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/sshd/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/sshd/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/sshd").success(function (data) {
        $scope.isActive = data.SshdIsActive;
        $scope.Sshd = data.SshdOptions;
    });
}

app.controller("AntdStorageController", ["$scope", "$http", AntdStorageController]);

function AntdStorageController($scope, $http) {
    $scope.print = function (el) {
        var data = $.param({
            Disk: el.Disk
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/storage/print", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.mklabel = function (el) {
        var data = $.param({
            Disk: el.Disk,
            Type: el.Type,
            Confirm: el.Confirm
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/storage/mklabel", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/storage").success(function (data) {
        $scope.Storage = data;
    });
}

app.controller("AntdVmController", ["$scope", "$http", AntdVmController]);

function AntdVmController($scope, $http) {
    $http.get("/vm").success(function (data) {
        $scope.Vm = data;
    });
}

app.controller("AntdVpnController", ["$scope", "$http", AntdVpnController]);

function AntdVpnController($scope, $http) {
    $scope.addReservation = function (el) {
        var data = $.param({
            HostName: el.HostName,
            MacAddress: el.MacAddress,
            IpAddress: el.IpAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/reservation", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.deletePool = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/pool/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addPool = function (el) {
        var data = $.param({
            Option: el.Option
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/pool", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.deleteClass = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/class/del", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.addClass = function (el) {
        var data = $.param({
            Name: el.Name,
            MacVendor: el.MacVendor
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/class", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.saveOptions = function (el) {
        var data = $.param({
            RemoteHost: el.RemoteHost,
            RemoteAddress: el.RemoteAddress,
            RemoteRange: el.RemoteRange,
            LocalAddress: el.LocalAddress,
            LocalRange: el.LocalRange
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/options", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/restart").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/stop").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/enable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/disable").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/vpn/set").then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/vpn").success(function (data) {
        $scope.isActive = data.VpnIsActive;
        $scope.VpnLocalPoint = data.VpnLocalPoint;
        $scope.VpnRemoteHost = data.VpnRemoteHost;
        $scope.VpnRemotePoint = data.VpnRemotePoint;
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
        $http.post("/zfs/create", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.zpoolCreate = function (el) {
        var data = $.param({
            Altroot: el.Altroot,
            Name: el.Name,
            Type: el.Type,
            Id: el.Id
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/zpool/create", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.snapDisable = function (el) {
        var data = $.param({
            Guid: el.Guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/zfs/snap/disable", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $scope.snap = function (zfs) {
        var data = $.param({
            Pool: zfs.Pool,
            Interval
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/zfs/snap", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
    }

    $http.get("/zfs").success(function (data) {
        $scope.Zfs = data;
    });
}

app.controller("AntdTimeController", ["$scope", "$http", AntdTimeController]);

function AntdTimeController($scope, $http) {
    $scope.SaveTimeNtp = function (ntp) {
        var data = $.param({
            Ntp: ntp
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/time/ntpd", data).then(function () { alert("Ok!"); }, function (response) { console.log(response); });
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
    $scope.SetDirectory = function (dir) {
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
