"use strict";

app.controller("AssetClusterController", ["$scope", "$http", "$interval", AssetClusterController]);

function AssetClusterController($scope, $http, $interval) {

    $scope.removeService = function () {
        if (index > -1) {
            $scope.Info.PortMapping.splice(index, 1);
        }
    }

    $scope.addService = function () {
        var node = {
            ServiceName: "",
            VirtualPort: "",
            ServicePort: ""
        };
        $scope.Info.PortMapping.push(node);
    }

    $scope.removeHostFromCluster = function (index) {
        $scope.ClusterNodes.splice(index, 1);
    }

    $scope.save = function ($event) {
        var config = angular.toJson($scope.ClusterNodes, true);
        var config2 = angular.toJson($scope.Info, true);
        var data = $.param({
            Config: config,
            Ip: config2
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/save", data).then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.apply = function ($event) {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/apply").then(function () { $scope.AppendMessageToButton($event.currentTarget, "ok"); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/restart").then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/stop").then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/enable").then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/disable").then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/set").then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.addPortMapping = function () {
        var portMap = {
            Hostname: "",
            IpAddress: ""
        };
        $scope.Info.PortMapping.push(portMap);
    }

    $scope.removePortMapping = function (i) {
        $scope.Info.PortMapping.splice(i, 1);
    }

    $scope.addNode = function () {
        var node = {
            VirtualPort: "",
            ServicePort: ""
        };
        $scope.ClusterNodes.push(node);
    }

    $scope.removeNode = function (i) {
        $scope.Info.ClusterNodes.splice(i, 1);
    }

    //$scope.PublicIp = "";
    $scope.ClusterNodes = [];
    $scope.Get = function () {
        $http.get("/cluster").success(function (data) {
            $scope.Info = data.Info;
            $scope.ClusterNodes = data.ClusterNodes;
            $scope.NetworkAdapters = data.NetworkAdapters;
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

    $scope.AppendMessageToButton = function (el, message) {
        var originalText = el.innerHTML;
        var newText = originalText + " - " + message;
        el.innerHTML = newText;
        $interval(function () {
            el.innerHTML = originalText;
        }, 1666);
    }
}

app.controller("AssetGlusterController", ["$scope", "$http", "$interval", "$sce", AssetGlusterController]);

function AssetGlusterController($scope, $http, $interval, $sce) {

    $scope.Gluster = {
        VolumesLabels: [],
        Nodes: []
    };

    $scope.save = function () {
        var config = angular.toJson($scope.Gluster, true);
        var data = $.param({
            Config: config
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/gluster/save", data).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.removeNode = function (index) {
        if (index > -1) {
            $scope.Gluster.Nodes.splice(index, 1);
        }
    }

    $scope.addNode = function () {
        var node = {
            Hostname: "",
            Volumes: []
        };
        $scope.Gluster.Nodes.push(node);
    }

    $scope.removeVolume = function (index, node) {
        if (index > -1) {
            node.Volumes.splice(index, 1);
        }
    }

    $scope.addVolume = function (node) {
        var volume = {
            Label: "",
            Brick: "",
            MountPoint: ""
        };
        node.Volumes.push(volume);
    }

    $interval(function () {
        $scope.Gluster.VolumesLabels = [];
        angular.forEach($scope.Gluster.Nodes, function (node) {
            angular.forEach(node.Volumes, function (volume) {
                var label = volume.Label;
                if ($scope.Gluster.VolumesLabels.indexOf(label) < 0) {
                    $scope.Gluster.VolumesLabels.push(label);
                }
            });
        });
    }, 500);

    $scope.Get = function () {
        $http.get("/gluster").success(function (data) {
            $scope.Gluster = data.Gluster;
            $scope.GlusterPeerStatus = $sce.trustAsHtml(data.GlusterPeerStatus);
            $scope.GlusterVolumeStatus = $sce.trustAsHtml(data.GlusterVolumeStatus);
            if ($scope.Gluster.Nodes.length < 1) {
                $http.get("/cluster").success(function (data) {
                    $scope.ClusterNodes = data.ClusterNodes;
                    angular.forEach(data.ClusterNodes, function (node) {
                        $scope.Gluster.Nodes.push({
                            Hostname: node.Hostname,
                            Volumes: []
                        });
                    });
                });
            }
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

app.controller("AssetDiscoveryController", ["$scope", "$http", AssetDiscoveryController]);

function AssetDiscoveryController($scope, $http) {
    $scope.addToCluster = function (el) {
        var json = angular.toJson(el);
        var data = $.param({
            HostJson: json
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/cluster/device/add", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.Get = function () {
        $http.get("/discovery").success(function (data) {
            $scope.Discovery = data.List;
        });
    }
    $scope.Get();

    $scope.reload = function () {
        $scope.Get();
    }
}

app.controller("AssetScanController", ["$scope", "$http", AssetScanController]);

function AssetScanController($scope, $http) {
    $scope.scanPort = function (machine) {
        $http.get("/scan/" + machine.Ip).success(function (data) {
            machine.Ports = data;
        });
    }

    $scope.wol = function (machine) {
        var data = $.param({
            MacAddress: machine.MacAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/asset/wol", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.shareKey = function (machine) {
        var data = $.param({
            Host: machine.Ip,
            Port: machine.Port
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/asset/handshake/start", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.scan = function () {
        if ($scope.NetScan.length > 0) {
            $http.get("/scan/" + $scope.NetScan)
                .success(function (data) {
                    $scope.AssetScan = data;
                });
        } else {
            alert("Value not valid!");
        }
    }

    $http.get("/scan").success(function (data) {
        $scope.ScanSettings = data.Subnets;
    });
}

app.controller("AssetSettingController", ["$scope", "$http", AssetSettingController]);

function AssetSettingController($scope, $http) {
    $scope.saveLabels = function () {
        angular.forEach($scope.ScanSettings, function (el) {
            if (el.IsChanged && el.Label.length > 0) {
                var data = $.param({
                    Letter: el.Letter,
                    Number: el.Number,
                    Label: el.Label
                });
                $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
                $http.post("/netscan/setlabel", data).then(function () { }, function (r) { console.log(r); });
            }
        });
    }

    $scope.checkElement = function (el) {
        if (el.Label !== el.OriginLabel) {
            el.IsChanged = true;
        } else {
            el.IsChanged = false;
        }
    }

    $scope.saveSubnet = function () {
        var data = $.param({
            Subnet: $scope.Subnet,
            Label: $scope.Label
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/netscan/setsubnet", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/assetsetting").success(function (data) {
        $scope.Subnet = data.SettingsSubnet;
        $scope.Label = data.SettingsSubnetLabel;
        $scope.ScanSettings = data.Settings;
    });
}

app.controller("AssetSyncMachineController", ["$scope", "$http", AssetSyncMachineController]);

function AssetSyncMachineController($scope, $http) {
    $scope.deleteMachine = function (guid) {
        var data = $.param({
            Guid: guid
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/machine/del", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.addMachine = function (el) {
        var data = $.param({
            MachineAddress: el.MachineAddress
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/machine", data).then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.restart = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/restart").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.stop = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/stop").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.enable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/enable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.disable = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/disable").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $scope.set = function () {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/syncmachine/set").then(function () { console.log(1); }, function (r) { console.log(r); });
    }

    $http.get("/syncmachine").success(function (data) {
        $scope.isActive = data.IsActive;
        $scope.SyncedMachines = data.SyncedMachines;
    });
}