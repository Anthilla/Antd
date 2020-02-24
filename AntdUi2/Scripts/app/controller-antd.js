app.controller("SharedController", ["$scope", "$http", "$interval", "notificationService", $Shared]);

function $Shared($scope, $http, $interval, $N) {

    var sync;

    function checkNewDisk() {
        $http.get("/disks/new").then(function (r) {
            for (var i = 0; i < r.data.length; i++)
                $N.newDisk('New disk detected: ' + r.data[i]);

        }).catch(function (r) {
            console.log(r);
        });
    }

    function syncHandler() {
        checkNewDisk();
    }

    function startInterval() {
        if (angular.isDefined(sync)) return;
        sync = $interval(syncHandler, 5000);
    }
    startInterval();

    $scope.addUnloadEvent = function () {
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
    };
}

app.controller("HostController", ["$scope", "$http", "notificationService", $Host]);

function $Host($scope, $http, $N) {

    $scope.Host = null;

    $scope.load = function () {
        console.log("loadHost");
        $scope.Host = null;
        $http.get("/host/config").then(function (r) {
            $scope.Host = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        var data = $.param({
            Data: angular.toJson($scope.Host)
        });
        console.log("saveHost");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/config/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) {
            console.log(r);
        });
    };

    $scope.apply = function () {
        console.log("applyHost");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/host/config/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        }, function (r) {
            console.log(r);
        });
    };
}

app.controller("TimedateController", ["$scope", "$http", "notificationService", $Timedate]);

function $Timedate($scope, $http, $N) {
    $scope.TimeDate = null;

    $scope.load = function () {
        console.log("loadTimeDate");
        $scope.TimeDate = null;
        $http.get("/timedate/config").then(function (r) {
            $scope.TimeDate = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        var data = $.param({
            Data: angular.toJson($scope.TimeDate)
        });
        console.log("saveTimeDate");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/timedate/config/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) { console.log(r); });
    };

    $scope.apply = function () {
        console.log("applyTimeDate");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/timedate/config/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        }, function (r) { console.log(r); });
    };
}

app.controller("UsersController", ["$scope", "$http", "notificationService", $Users]);

function $Users($scope, $http, $N) {
    $scope.Users = [];

    $scope.NewUser = {
        Name: '',
        Password: ''
    };

    $scope.load = function () {
        console.log("loadUsers");
        $http.get("/user/config").then(function (r) {
            $scope.Users = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        var data = $.param({
            Data: angular.toJson($scope.Users)
        });
        console.log("saveUsers");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/user/config/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) {
            console.log(r);
        });
    };

    $scope.apply = function () {
        console.log("applyUsers");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/user/config/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        }, function (r) {
            console.log(r);
        });
    };
}

app.controller("SysctlController", ["$scope", "$http", "notificationService", $Sysctl]);

function $Sysctl($scope, $http, $N) {
    $scope.BootSysctl = null;

    $scope.load = function () {
        console.log("loadBootParameters");
        $http.get("/boot/config/sysctl").then(function (r) {
            $scope.BootSysctl = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        var data = $.param({
            Data: angular.toJson($scope.BootSysctl.SysctlTxt)
        });
        console.log("saveBootParameters");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/sysctl/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) { console.log(r); });
    };

    $scope.apply = function () {
        console.log("applyBootParameters");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/sysctl/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        }, function (r) { console.log(r); });
    };
}

app.controller("ModulesController", ["$scope", "$http", "notificationService", $Modules]);

function $Modules($scope, $http, $N) {
    $scope.BootModules = null;

    $scope.load = function () {
        console.log("loadBootModules");
        $http.get("/boot/config/modules").then(function (r) {
            $scope.BootModules = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        var data = $.param({
            Data: angular.toJson($scope.BootModules)
        });
        console.log("saveBootModules");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/modules/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) { console.log(r); });
    };

    $scope.apply = function () {
        console.log("applyBootModules");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/modules/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        }, function (r) { console.log(r); });
    };
}

app.controller("ServicesController", ["$scope", "$http", "notificationService", $Services]);

function $Services($scope, $http, $N) {
    $scope.BootServices = [];

    $scope.load = function () {
        console.log("loadBootServices");
        $http.get("/boot/config/services").then(function (r) {
            $scope.BootServices = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        var data = $.param({
            Data: angular.toJson($scope.BootServices)
        });
        console.log("saveBootServices");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/services/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) { console.log(r); });
    };

    $scope.apply = function () {
        console.log("applyBootServices");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/boot/config/services/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        }, function (r) { console.log(r); });
    };
}

app.controller("CommandsController", ["$scope", "$http", "$interval", "$timeout", "$filter", "notificationService", $Commands]);

function $Commands($scope, $http, $interval, $timeout, $filter, $N) {
    $scope.SetupCommandsTxt = null;

    $scope.load = function () {
        console.log("loadSetupCommands");
        $http.get("/setupcmd/config").then(function (r) {
            $scope.SetupCommandsTxt = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        var data = $.param({
            Data: angular.toJson($scope.SetupCommandsTxt)
        });
        console.log("saveSetupCommands");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/setupcmd/config/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) { console.log(r); });
    };

    $scope.apply = function () {
        console.log("applySetupCommands");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/setupcmd/config/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        }, function (r) { console.log(r); });
    };
}

app.controller("SchedulerController", ["$scope", "$http", "notificationService", $Scheduler]);

function $Scheduler($scope, $http, $N) {
    $scope.Scheduler = [];

    $scope.NewScheduler = {
        Id: '',
        Name: '',
        RulesTxt: ''
    };

    $scope.load = function () {
        console.log("loadScheduler");
        $http.get("/scheduler").then(function (r) {
            $scope.Scheduler = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        console.log("saveScheduler");
        var data = $.param({
            Data: angular.toJson($scope.Scheduler)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/scheduler/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) { console.log(r); });
    };

    $scope.apply = function () {
        console.log("applyScheduler");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/scheduler/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        }, function (r) { console.log(r); });
    };
}


app.controller("RoutingTablesController", ["$scope", "$http", "notificationService", $RoutingTables]);

function $RoutingTables($scope, $http, $N) {
    $scope.RoutingTables = [];

    $scope.NewRoutingTable = {
        Id: '',
        Name: '',
        RulesTxt: ''
    };

    $scope.load = function () {
        console.log("loadRoutingTables");
        $http.get("/network/config/routingtables").then(function (r) {
            $scope.RoutingTables = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        console.log("saveRoutingTables");
        var data = $.param({
            Data: angular.toJson($scope.RoutingTables)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/config/routingtables/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) { console.log(r); });
    };

    $scope.apply = function () {
        console.log("applyRoutingTables");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/config/routingtables/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        }, function (r) { console.log(r); });
    };
}

app.controller("RoutingController", ["$scope", "$http", "notificationService", $Routing]);

function $Routing($scope, $http, $N) {
    $scope.Routes = [];

    $scope.NewRoute = {
        Destination: '',
        Gateway: '',
        Device: ''
    };

    $scope.load = function () {
        console.log("loadRouting");
        $http.get("/network/config/routing").then(function (r) {
            $scope.Routes = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        console.log("saveRouting");
        var data = $.param({
            Data: angular.toJson($scope.Routes)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/config/routing/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) { console.log(r); });
    };

    $scope.apply = function () {
        console.log("applyRouting");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/config/routing/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        }, function (r) { console.log(r); });
    };
}

app.controller("InterfacesController", ["$scope", "$http", "notificationService", $Interfaces]);

function $Interfaces($scope, $http, $N) {
    $scope.Interfaces = [];

    $scope.NewInterface = {
        AutoBool: true,
        Iface: '',
        Address: '',
        PreUpTxt: '',
        PostUpTxt: '',
        PreDownTxt: '',
        PostDownTxt: ''
    };

    $scope.load = function () {
        console.log("loadNetworkInterfaces");
        $http.get("/network/config/interfaces").then(function (r) {
            $scope.Interfaces = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        var data = $.param({
            Data: angular.toJson($scope.Interfaces)
        });
        console.log("saveNetworkInterfaces");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/config/interfaces/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) {
            console.log(r);
        });
    };

    $scope.apply = function () {
        console.log("applyNetworkInterfaces");
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/network/config/interfaces/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        }, function (r) {
            console.log(r);
        });
    };
}

app.controller("DisksController", ["$scope", "$http", "notificationService", $Disks]);

function $Disks($scope, $http, $N) {

    $scope.Disks = [];
    $scope.Mode = 0;

    $scope.load = function () {
        $http.get("/disks").then(function (r) {
            $scope.Disks = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.createPartitionTable = function (device, label) {
        var data = $.param({
            Device: device,
            Label: label
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/disks/create/partition/table", data).then(function () {
            $scope.load();
            $N.success('Partition Table Created');
        }, function (r) { console.log(r); });
    };

    $scope.createPartition = function (device, partType, partName, fsType, start, end) {
        var data = $.param({
            Device: device,
            //PartType: partType,
            PartName: partName,
            FsType: fsType,
            Start: start,
            End: end
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/disks/create/partition", data).then(function () {
            $scope.load();
            $N.success('Partition Created');
        }, function (r) { console.log(r); });
    };

    $scope.createFsExt4 = function (device, label) {
        var data = $.param({
            Device: device,
            Label: label
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/disks/create/fs/ext4", data).then(function () {
            $scope.load();
            $N.success('Ext4 Created');
        }, function (r) { console.log(r); });
    };

    $scope.createFsZfs = function (device, pool, label) {
        var data = $.param({
            Device: device,
            Pool: pool,
            Label: label
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/disks/create/fs/zfs", data).then(function () {
            $scope.load();
            $N.success('Zfs Created');
        }, function (r) { console.log(r); });
    };

    $scope.checkFs = function (partition) {
        $http.get("/disks/check/fs/" + partition.path).then(function (r) {
            partition.Check = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };



}

app.controller("VolumesController", ["$scope", "$http", "notificationService", $Volumes]);

function $Volumes($scope, $http, $N) {

    $scope.Volumes = [];
    $scope.Mode = 0;

    $scope.load = function () {
        $http.get("/volumes").then(function (r) {
            $scope.Volumes = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.mountVolume = function (partition, label) {
        var data = $.param({
            Partition: partition,
            Label: label
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/volumes/mount", data).then(function () {
            $scope.load();
            $N.success('Volume mounted');
        }, function (r) { console.log(r); });
    };

    $scope.umountVolume = function (mountpoint) {
        var data = $.param({
            Mountpoint: mountpoint
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/volumes/umount", data).then(function () {
            $scope.load();
            $N.success('Volume unmounted');
        }, function (r) { console.log(r); });
    };
}

app.controller("WebdavController", ["$scope", "$http", "notificationService", $Webdav]);

function $Webdav($scope, $http, $N) {

    $scope.Webdav = [];
    $scope.Targets = [];
    $scope.Users = [];
    $scope.IpAddressList = [];

    $scope.NewWebdav = {
        Target: '',
        Address: '',
        Port: 0,
        Users: [],
        MappedUsers: []
    };

    $scope.load = function () {
        $http.get("/webdav").then(function (r) {
            $scope.Webdav = r.data.Webdav;
            $scope.Targets = r.data.Targets;
            $scope.Users = r.data.Users;
            $scope.IpAddressList = r.data.IpAddressList;

            $scope.NewWebdav.MappedUsers = angular.copy($scope.Users);

        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.save = function () {
        for (var i = 0; i < $scope.Webdav.length; i++) {
            $scope.Webdav[i].Users = [];
            for (var u = 0; u < $scope.Webdav[i].MappedUsers.length; u++) {
                if ($scope.Webdav[i].MappedUsers[u].IsSelected) {
                    $scope.Webdav[i].Users.push($scope.Webdav[i].MappedUsers[u].Name);
                }
            }
        }
        var data = $.param({
            Data: angular.toJson($scope.Webdav)
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/webdav/save", data).then(function () {
            $scope.load();
            $N.success('Data saved');
        }, function (r) { console.log(r); });
    };

    $scope.webdavStart = function (mountpoint) {
        var data = $.param({
            Mountpoint: mountpoint
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/webdav/start", data).then(function () {
            $scope.load();
            $N.success('Webdav Started');
        }, function (r) { console.log(r); });
    };

    $scope.webdavStop = function (mountpoint) {
        var data = $.param({
            Mountpoint: mountpoint
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/webdav/stop", data).then(function () {
            $scope.load();
            $N.success('Webdav Stopped');
        }, function (r) { console.log(r); });
    };
}

app.controller("TerminalController", ["$scope", "$http", "notificationService", $Terminal]);

function $Terminal($scope, $http, $N) {

    $scope.$on('terminal-input', function (e, consoleInput) {
        var cmd = consoleInput[0];
        var data = $.param({
            Command: cmd
        });
        console.log(data);
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/terminal", data).then(function (r) {
            $scope.$broadcast('terminal-output', {
                output: true,
                text: r.data,
                breakLine: true
            });
        }, function (r) { console.log(r); });
    });
}

app.controller("FileManagerController", ["$scope", "$http", "notificationService", $FileManager]);

function $FileManager($scope, $http, $N) {

    $scope.Volumes = [];
    $scope.SrcDirectory = null;
    $scope.DstDirectory = null;

    $scope.load = function () {
        $http.get("/volumes/mounted").then(function (r) {
            $scope.Volumes = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
    $scope.load();

    $scope.loadSrcDirectory = function (path) {
        $http.get("/fm/" + path).then(function (r) {
            $scope.SrcDirectory = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };

    $scope.loadDstDirectory = function (path) {
        $http.get("/fm/" + path).then(function (r) {
            $scope.DstDirectory = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };

    $scope.syncFolder = function (src, dst) {
        var data = $.param({
            Source: src,
            Destination: dst
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/fm/folder/sync", data).then(function () {
            $scope.loadDstDirectory($scope.DstDirectory.Path);
            $N.success('Sync ' + src + ' to ' + dst);
        }, function (r) { console.log(r); });
    };

    $scope.syncFile = function (src, dst) {
        var data = $.param({
            Source: src,
            Destination: dst
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/fm/file/sync", data).then(function () {
            $scope.loadDstDirectory($scope.DstDirectory.Path);
            $N.success('Sync ' + src + ' to ' + dst);
        }, function (r) { console.log(r); });
    };
}

app.controller("FinderController", ["$http", $Finder]);

function $Finder($H) {

    var vm = this;

    vm.Files = [];
    vm.Query = '';

    vm.search = function () {
        $H.get("/finder?p=" + vm.Query).then(function (r) {
            vm.Files = r.data;
        }).catch(function (r) {
            console.log(r);
        });
    };
}

