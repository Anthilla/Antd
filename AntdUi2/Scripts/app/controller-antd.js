function jsonParam(o) {
    return $.param({
        Data: angular.toJson(o)
    });
}

app.controller("SharedController", ["HttpService", "$interval", "notificationService", $Shared]);

function $Shared($H, $I, $N) {

    var vm = this;

    var sync;

    function checkNewDisk() {
        $H.GET("/disks/new").then(function (r) {
            for (var i = 0; i < r.data.length; i++)
                $N.newDisk('New disk detected: ' + r.data[i]);

        });
    }

    function syncHandler() {
        checkNewDisk();
    }

    function startInterval() {
        if (angular.isDefined(sync)) return;
        sync = $I(syncHandler, 5000);
    }
    startInterval();

    function handleUnloadEvent(event) {
        event.returnValue = "Your warning text";
    }

    vm.addUnloadEvent = function () {
        if (window.addEventListener) {
            window.addEventListener("beforeunload", handleUnloadEvent);
        } else {
            window.attachEvent("onbeforeunload", handleUnloadEvent);
        }
    };

    vm.removeUnloadEvent = function () {
        if (window.removeEventListener) {
            window.removeEventListener("beforeunload", handleUnloadEvent);
        } else {
            window.detachEvent("onbeforeunload", handleUnloadEvent);
        }
    };

    //You could add the event when validating a form, for example
    //vm.validateForm = function () {
    //    if (vm.yourform.$invalid) {
    //        vm.addUnloadEvent();
    //        return;
    //    }
    //    else {
    //        vm.removeUnloadEvent();
    //    }
    //};

    vm.scrollConfigSidebar = {
        autoHideScrollbar: false,
        theme: 'light-3',
        mouseWheel: { preventDefault: true },
        axis: "y",
        scrollInertia: 0
    };

    vm.addToList = function (el, list) {
        list.push(angular.copy(el));
    };

    vm.removeFromList = function (i, list) {
        list.splice(i, 1);
    };
}

app.controller("HostController", ["$scope", "HttpService", "notificationService", $Host]);

function $Host($scope, $H, $N) {

    $scope.Host = null;

    $scope.load = function () {
        $scope.Host = null;
        $H.GET("/host/config").then(function (r) {
            $scope.Host = r.data;
        });
    };
    $scope.load();

    $scope.save = function () {
        $H.POST("/host/config/save", jsonParam($scope.Host)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    $scope.apply = function () {
        $H.POST("/host/config/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        });
    };
}

app.controller("TimedateController", ["$scope", "HttpService", "notificationService", $Timedate]);

function $Timedate($scope, $H, $N) {
    $scope.TimeDate = null;

    $scope.load = function () {
        $scope.TimeDate = null;
        $H.GET("/timedate/config").then(function (r) {
            $scope.TimeDate = r.data;
        });
    };
    $scope.load();

    $scope.save = function () {
        $H.POST("/timedate/config/save", jsonParam($scope.TimeDate)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    $scope.apply = function () {
        $H.POST("/timedate/config/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        });
    };
}

app.controller("UsersController", ["$scope", "HttpService", "notificationService", $Users]);

function $Users($scope, $H, $N) {
    $scope.Users = [];

    $scope.NewUser = {
        Name: '',
        Password: ''
    };

    $scope.load = function () {
        $H.GET("/user/config").then(function (r) {
            $scope.Users = r.data;
        });
    };
    $scope.load();

    $scope.save = function () {
        $H.POST("/user/config/save", jsonParam($scope.Users)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    $scope.apply = function () {
        $H.POST("/user/config/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        });
    };
}

app.controller("SysctlController", ["$scope", "HttpService", "notificationService", $Sysctl]);

function $Sysctl($scope, $H, $N) {
    $scope.BootSysctl = null;

    $scope.load = function () {
        $H.GET("/boot/config/sysctl").then(function (r) {
            $scope.BootSysctl = r.data;
        });
    };
    $scope.load();

    $scope.save = function () {
        $H.POST("/boot/config/sysctl/save", jsonParam($scope.BootSysctl.SysctlTxt)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    $scope.apply = function () {
        $H.POST("/boot/config/sysctl/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        });
    };
}

app.controller("ModulesController", ["$scope", "HttpService", "notificationService", $Modules]);

function $Modules($scope, $H, $N) {
    $scope.BootModules = null;

    $scope.load = function () {
        $H.GET("/boot/config/modules").then(function (r) {
            $scope.BootModules = r.data;
        });
    };
    $scope.load();

    $scope.save = function () {
        $H.POST("/boot/config/modules/save", jsonParam($scope.BootModules)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    $scope.apply = function () {
        $H.POST("/boot/config/modules/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        });
    };
}

app.controller("ServicesController", ["$scope", "HttpService", "notificationService", $Services]);

function $Services($scope, $H, $N) {
    $scope.BootServices = [];

    $scope.load = function () {
        $H.GET("/boot/config/services").then(function (r) {
            $scope.BootServices = r.data;
        });
    };
    $scope.load();

    $scope.save = function () {
        $H.POST("/boot/config/services/save", jsonParam($scope.BootServices)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    $scope.apply = function () {
        $H.POST("/boot/config/services/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        });
    };
}

app.controller("CommandsController", ["$scope", "HttpService", "$interval", "$timeout", "$filter", "notificationService", $Commands]);

function $Commands($scope, $H, $interval, $timeout, $filter, $N) {
    $scope.SetupCommandsTxt = null;

    $scope.load = function () {
        $H.GET("/setupcmd/config").then(function (r) {
            $scope.SetupCommandsTxt = r.data;
        });
    };
    $scope.load();

    $scope.save = function () {
        $H.POST("/setupcmd/config/save", jsonParam($scope.SetupCommandsTxt)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    $scope.apply = function () {
        $H.POST("/setupcmd/config/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        });
    };
}

app.controller("SchedulerController", ["$scope", "HttpService", "notificationService", $Scheduler]);

function $Scheduler($scope, $H, $N) {
    $scope.Scheduler = [];

    $scope.NewScheduler = {
        Id: '',
        Name: '',
        RulesTxt: ''
    };

    $scope.load = function () {
        $H.GET("/scheduler").then(function (r) {
            $scope.Scheduler = r.data;
        });
    };
    $scope.load();

    $scope.save = function () {
        $H.POST("/scheduler/save", jsonParam($scope.Scheduler)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    $scope.apply = function () {
        $H.POST("/scheduler/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        });
    };
}

app.controller("RoutingTablesController", ["$scope", "HttpService", "notificationService", $RoutingTables]);

function $RoutingTables($scope, $H, $N) {
    $scope.RoutingTables = [];

    $scope.NewRoutingTable = {
        Id: '',
        Name: '',
        RulesTxt: ''
    };

    $scope.load = function () {
        $H.GET("/network/config/routingtables").then(function (r) {
            $scope.RoutingTables = r.data;
        });
    };
    $scope.load();

    $scope.save = function () {
        $H.POST("/network/config/routingtables/save", jsonParam($scope.RoutingTables)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    $scope.apply = function () {
        $H.POST("/network/config/routingtables/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        });
    };
}

app.controller("RoutingController", ["$scope", "HttpService", "notificationService", $Routing]);

function $Routing($scope, $H, $N) {
    $scope.Routes = [];

    $scope.NewRoute = {
        Destination: '',
        Gateway: '',
        Device: ''
    };

    $scope.load = function () {
        $H.GET("/network/config/routing").then(function (r) {
            $scope.Routes = r.data;
        });
    };
    $scope.load();

    $scope.save = function () {
        $H.POST("/network/config/routing/save", jsonParam($scope.Routes)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    $scope.apply = function () {
        $H.POST("/network/config/routing/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        });
    };
}

app.controller("InterfacesController", ["$scope", "HttpService", "notificationService", $Interfaces]);

function $Interfaces($scope, $H, $N) {
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
        $H.GET("/network/config/interfaces").then(function (r) {
            $scope.Interfaces = r.data;
        });
    };
    $scope.load();

    $scope.save = function () {
        $H.POST("/network/config/interfaces/save", jsonParam($scope.Interfaces)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    $scope.apply = function () {
        $H.POST("/network/config/interfaces/apply").then(function () {
            $scope.load();
            $N.success('Data applied');
        });
    };
}

app.controller("DisksController", ["$scope", "HttpService", "notificationService", $Disks]);

function $Disks($scope, $H, $N) {

    $scope.Disks = [];
    $scope.Mode = 0;

    $scope.load = function () {
        $H.GET("/disks").then(function (r) {
            $scope.Disks = r.data;
        });
    };
    $scope.load();

    function p1(d, l) {
        return $.param({
            Device: d,
            Label: l
        });
    }

    $scope.createPartitionTable = function (d, l) {
        $H.POST("/disks/create/partition/table", p1(d, l)).then(function () {
            $scope.load();
            $N.success('Partition Table Created');
        });
    };

    $scope.createPartition = function (device, partType, partName, fsType, start, end) {
        var data = $.param({
            Device: device,
            PartName: partName,
            FsType: fsType,
            Start: start,
            End: end
        });
        $H.POST("/disks/create/partition", data).then(function () {
            $scope.load();
            $N.success('Partition Created');
        });
    };

    $scope.createFsExt4 = function (d, l) {
        $H.POST("/disks/create/fs/ext4", p1(d, l)).then(function () {
            $scope.load();
            $N.success('Ext4 Created');
        });
    };

    $scope.createFsZfs = function (device, pool, label) {
        var data = $.param({
            Device: device,
            Pool: pool,
            Label: label
        });
        $H.POST("/disks/create/fs/zfs", data).then(function () {
            $scope.load();
            $N.success('Zfs Created');
        });
    };

    $scope.checkFs = function (partition) {
        $H.GET("/disks/check/fs/" + partition.path).then(function (r) {
            partition.Check = r.data;
        });
    };

}

app.controller("VolumesController", ["$scope", "HttpService", "notificationService", $Volumes]);

function $Volumes($scope, $H, $N) {

    $scope.Volumes = [];
    $scope.Mode = 0;

    $scope.load = function () {
        $H.GET("/volumes").then(function (r) {
            $scope.Volumes = r.data;
        });
    };
    $scope.load();

    function p1(p, l) {
        return $.param({
            Partition: p,
            Label: l
        });
    }

    function p2(m) {
        return $.param({
            Mountpoint: m
        });
    }

    $scope.mountVolume = function (p, l) {
        $H.POST("/volumes/mount", p1(p, l)).then(function () {
            $scope.load();
            $N.success('Volume mounted');
        });
    };

    $scope.umountVolume = function (m) {
        $H.POST("/volumes/umount", p2(m)).then(function () {
            $scope.load();
            $N.success('Volume unmounted');
        });
    };
}

app.controller("WebdavController", ["$scope", "HttpService", "notificationService", $Webdav]);

function $Webdav($scope, $H, $N) {

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
        $H.GET("/webdav").then(function (r) {
            $scope.Webdav = r.data.Webdav;
            $scope.Targets = r.data.Targets;
            $scope.Users = r.data.Users;
            $scope.IpAddressList = r.data.IpAddressList;

            $scope.NewWebdav.MappedUsers = angular.copy($scope.Users);

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
        $H.POST("/webdav/save", jsonParam($scope.Webdav)).then(function () {
            $scope.load();
            $N.success('Data saved');
        });
    };

    function p(v) {
        return $.param({
            Mountpoint: v
        });
    }

    $scope.webdavStart = function (v) {
        $H.POST("/webdav/start", p(v)).then(function () {
            $scope.load();
            $N.success('Webdav Started');
        });
    };

    $scope.webdavStop = function (v) {
        $H.POST("/webdav/stop", p(v)).then(function () {
            $scope.load();
            $N.success('Webdav Stopped');
        });
    };
}

app.controller("TerminalController", ["$scope", "HttpService", "notificationService", $Terminal]);

function $Terminal($scope, $H, $N) {

    $scope.$on('terminal-input', function (e, ci) {
        $H.POST("/terminal", $.param({
            Command: ci[0]
        })).then(function (r) {
            $scope.$broadcast('terminal-output', {
                output: true,
                text: r.data,
                breakLine: true
            });
        });
    });
}

app.controller("FileManagerController", ["$scope", "HttpService", "notificationService", $FileManager]);

function $FileManager($scope, $H, $N) {

    $scope.Volumes = [];
    $scope.SrcDirectory = null;
    $scope.DstDirectory = null;

    $scope.load = function () {
        $H.GET("/volumes/mounted").then(function (r) {
            $scope.Volumes = r.data;
        });
    };
    $scope.load();

    $scope.loadSrcDirectory = function (path) {
        $H.GET("/fm/" + path).then(function (r) {
            $scope.SrcDirectory = r.data;
        });
    };

    $scope.loadDstDirectory = function (path) {
        $H.GET("/fm/" + path).then(function (r) {
            $scope.DstDirectory = r.data;
        });
    };

    function p(s, d) {
        return $.param({
            Source: s,
            Destination: d
        });
    }

    $scope.syncFolder = function (src, dst) {
        $H.POST("/fm/folder/sync", p(src, dst)).then(function () {
            $scope.loadDstDirectory($scope.DstDirectory.Path);
            $N.success('Sync ' + src + ' to ' + dst);
        });
    };

    $scope.syncFile = function (src, dst) {
        $H.POST("/fm/file/sync", p(src, dst)).then(function () {
            $scope.loadDstDirectory($scope.DstDirectory.Path);
            $N.success('Sync ' + src + ' to ' + dst);
        });
    };
}

app.controller("FinderController", ["HttpService", $Finder]);

function $Finder($H) {

    var vm = this;

    vm.Files = [];
    vm.Query = '';

    vm.search = function () {
        $H.GET("/finder?p=" + vm.Query).then(function (r) {
            vm.Files = r.data;
        });
    };
}
