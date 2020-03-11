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

app.controller("HostController", ["HttpService", "notificationService", $Host]);

function $Host($H, $N) {
    var vm = this;

    vm.Host = null;

    vm.load = function () {
        $H.GET("/host/config").then(function (r) {
            vm.Host = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/host/config/save", jsonParam(vm.Host)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/host/config/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("TimedateController", ["HttpService", "notificationService", $Timedate]);

function $Timedate($H, $N) {
    var vm = this;

    vm.TimeDate = null;

    vm.load = function () {
        $H.GET("/timedate/config").then(function (r) {
            vm.TimeDate = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/timedate/config/save", jsonParam(vm.TimeDate)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/timedate/config/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("UsersController", ["HttpService", "notificationService", $Users]);

function $Users($H, $N) {
    var vm = this;

    vm.Users = [];

    vm.NewUser = {
        Name: '',
        Password: ''
    };

    vm.load = function () {
        $H.GET("/user/config").then(function (r) {
            vm.Users = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/user/config/save", jsonParam(vm.Users)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/user/config/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("SysctlController", ["HttpService", "notificationService", $Sysctl]);

function $Sysctl($H, $N) {

    var vm = this;

    vm.BootSysctl = null;

    vm.load = function () {
        $H.GET("/boot/config/sysctl").then(function (r) {
            vm.BootSysctl = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/boot/config/sysctl/save", jsonParam(vm.BootSysctl.SysctlTxt)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/boot/config/sysctl/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("ModulesController", ["HttpService", "notificationService", $Modules]);

function $Modules($H, $N) {

    var vm = this;

    vm.BootModules = null;

    vm.load = function () {
        $H.GET("/boot/config/modules").then(function (r) {
            vm.BootModules = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/boot/config/modules/save", jsonParam(vm.BootModules)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/boot/config/modules/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("ServicesController", ["HttpService", "notificationService", $Services]);

function $Services($H, $N) {

    var vm = this;

    vm.BootServices = [];

    vm.load = function () {
        $H.GET("/boot/config/services").then(function (r) {
            vm.BootServices = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/boot/config/services/save", jsonParam(vm.BootServices)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/boot/config/services/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("CommandsController", ["HttpService", "notificationService", $Commands]);

function $Commands($H, $N) {

    var vm = this;

    vm.SetupCommandsTxt = null;

    vm.load = function () {
        $H.GET("/setupcmd/config").then(function (r) {
            vm.SetupCommandsTxt = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/setupcmd/config/save", jsonParam(vm.SetupCommandsTxt)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/setupcmd/config/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("SchedulerController", ["HttpService", "notificationService", $Scheduler]);

function $Scheduler($H, $N) {

    var vm = this;

    vm.Scheduler = [];

    vm.NewScheduler = {
        Id: '',
        Name: '',
        RulesTxt: ''
    };

    vm.load = function () {
        $H.GET("/scheduler").then(function (r) {
            vm.Scheduler = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/scheduler/save", jsonParam(vm.Scheduler)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/scheduler/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("RoutingTablesController", ["HttpService", "notificationService", $RoutingTables]);

function $RoutingTables($H, $N) {

    var vm = this;

    vm.RoutingTables = [];

    vm.NewRoutingTable = {
        Id: '',
        Name: '',
        RulesTxt: ''
    };

    vm.load = function () {
        $H.GET("/network/config/routingtables").then(function (r) {
            vm.RoutingTables = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/network/config/routingtables/save", jsonParam(vm.RoutingTables)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/network/config/routingtables/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("RoutingController", ["HttpService", "notificationService", $Routing]);

function $Routing($H, $N) {

    var vm = this;

    vm.Routes = [];

    vm.NewRoute = {
        Destination: '',
        Gateway: '',
        Device: ''
    };

    vm.load = function () {
        $H.GET("/network/config/routing").then(function (r) {
            vm.Routes = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/network/config/routing/save", jsonParam(vm.Routes)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/network/config/routing/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("BridgeController", ["HttpService", "notificationService", $Bridge]);

function $Bridge($H, $N) {

    var vm = this;

    vm.Bridges = [];

    vm.NewBridge = {
        Name: '',
        LowerTxt: ''
    };

    vm.load = function () {
        $H.GET("/network/config/bridge").then(function (r) {
            vm.Bridges = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/network/config/bridge/save", jsonParam(vm.Bridges)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/network/config/bridge/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("BondController", ["HttpService", "notificationService", $Bond]);

function $Bond($H, $N) {

    var vm = this;

    vm.Bonds = [];

    vm.NewBond = {
        Name: '',
        LowerTxt: ''
    };

    vm.load = function () {
        $H.GET("/network/config/bond").then(function (r) {
            vm.Bonds = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/network/config/bond/save", jsonParam(vm.Bonds)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/network/config/bond/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("TunController", ["HttpService", "notificationService", $Tun]);

function $Tun($H, $N) {

    var vm = this;

    vm.Tuns = [];

    vm.NewTun = {
        Name: ''
    };

    vm.load = function () {
        $H.GET("/network/config/tun").then(function (r) {
            vm.Tuns = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/network/config/tun/save", jsonParam(vm.Tuns)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/network/config/tun/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("TapController", ["HttpService", "notificationService", $Tap]);

function $Tap($H, $N) {

    var vm = this;

    vm.Taps = [];

    vm.NewTap = {
        Name: ''
    };

    vm.load = function () {
        $H.GET("/network/config/tap").then(function (r) {
            vm.Taps = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/network/config/tap/save", jsonParam(vm.Taps)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/network/config/tap/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("InterfacesController", ["HttpService", "notificationService", $Interfaces]);

function $Interfaces($H, $N) {

    var vm = this;

    vm.Interfaces = [];

    vm.NewInterface = {
        AutoBool: true,
        Iface: '',
        Address: '',
        PreUpTxt: '',
        PostUpTxt: '',
        PreDownTxt: '',
        PostDownTxt: ''
    };

    vm.load = function () {
        $H.GET("/network/config/interfaces").then(function (r) {
            vm.Interfaces = r.data;
        });
    };
    vm.load();

    vm.save = function () {
        $H.POST("/network/config/interfaces/save", jsonParam(vm.Interfaces)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    vm.apply = function () {
        $H.POST("/network/config/interfaces/apply").then(function () {
            vm.load();
            $N.success('Data applied');
        });
    };
}

app.controller("DisksController", ["HttpService", "notificationService", $Disks]);

function $Disks($H, $N) {

    var vm = this;

    vm.Disks = [];
    vm.Mode = 0;

    vm.load = function () {
        $H.GET("/disks").then(function (r) {
            vm.Disks = r.data;
        });
    };
    vm.load();

    function p1(d, l) {
        return $.param({
            Device: d,
            Label: l
        });
    }

    vm.createPartitionTable = function (d, l) {
        $H.POST("/disks/create/partition/table", p1(d, l)).then(function () {
            vm.load();
            $N.success('Partition Table Created');
        });
    };

    vm.createPartition = function (device, partType, partName, fsType, start, end) {
        var data = $.param({
            Device: device,
            PartName: partName,
            FsType: fsType,
            Start: start,
            End: end
        });
        $H.POST("/disks/create/partition", data).then(function () {
            vm.load();
            $N.success('Partition Created');
        });
    };

    vm.createFsExt4 = function (d, l) {
        $H.POST("/disks/create/fs/ext4", p1(d, l)).then(function () {
            vm.load();
            $N.success('Ext4 Created');
        });
    };

    vm.createFsZfs = function (device, pool, label) {
        var data = $.param({
            Device: device,
            Pool: pool,
            Label: label
        });
        $H.POST("/disks/create/fs/zfs", data).then(function () {
            vm.load();
            $N.success('Zfs Created');
        });
    };

    vm.checkFs = function (partition) {
        $H.GET("/disks/check/fs/" + partition.path).then(function (r) {
            partition.Check = r.data;
        });
    };

}

app.controller("VolumesController", ["HttpService", "notificationService", $Volumes]);

function $Volumes($H, $N) {
    var vm = this;

    vm.Volumes = [];
    vm.Mode = 0;

    vm.load = function () {
        $H.GET("/volumes").then(function (r) {
            vm.Volumes = r.data;
        });
    };
    vm.load();

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

    vm.mountVolume = function (p, l) {
        $H.POST("/volumes/mount", p1(p, l)).then(function () {
            vm.load();
            $N.success('Volume mounted');
        });
    };

    vm.umountVolume = function (m) {
        $H.POST("/volumes/umount", p2(m)).then(function () {
            vm.load();
            $N.success('Volume unmounted');
        });
    };
}

app.controller("WebdavController", ["HttpService", "notificationService", $Webdav]);

function $Webdav($H, $N) {
    var vm = this;

    vm.Webdav = [];
    vm.Targets = [];
    vm.Users = [];
    vm.IpAddressList = [];

    vm.NewWebdav = {
        Target: '',
        Address: '',
        Port: 0,
        Users: [],
        MappedUsers: []
    };

    vm.load = function () {
        $H.GET("/webdav").then(function (r) {
            vm.Webdav = r.data.Webdav;
            vm.Targets = r.data.Targets;
            vm.Users = r.data.Users;
            vm.IpAddressList = r.data.IpAddressList;

            vm.NewWebdav.MappedUsers = angular.copy(vm.Users);

        });
    };
    vm.load();

    vm.save = function () {
        for (var i = 0; i < vm.Webdav.length; i++) {
            vm.Webdav[i].Users = [];
            for (var u = 0; u < vm.Webdav[i].MappedUsers.length; u++) {
                if (vm.Webdav[i].MappedUsers[u].IsSelected) {
                    vm.Webdav[i].Users.push(vm.Webdav[i].MappedUsers[u].Name);
                }
            }
        }
        $H.POST("/webdav/save", jsonParam(vm.Webdav)).then(function () {
            vm.load();
            $N.success('Data saved');
        });
    };

    function p(v) {
        return $.param({
            Mountpoint: v
        });
    }

    vm.webdavStart = function (v) {
        $H.POST("/webdav/start", p(v)).then(function () {
            vm.load();
            $N.success('Webdav Started');
        });
    };

    vm.webdavStop = function (v) {
        $H.POST("/webdav/stop", p(v)).then(function () {
            vm.load();
            $N.success('Webdav Stopped');
        });
    };
}

app.controller("TerminalController", ["HttpService", "notificationService", $Terminal]);

function $Terminal($H, $N) {
    var vm = this;

    vm.$on('terminal-input', function (e, ci) {
        $H.POST("/terminal", $.param({
            Command: ci[0]
        })).then(function (r) {
            vm.$broadcast('terminal-output', {
                output: true,
                text: r.data,
                breakLine: true
            });
        });
    });
}

app.controller("FileManagerController", ["HttpService", "notificationService", $FileManager]);

function $FileManager($H, $N) {
    var vm = this;

    vm.Volumes = [];
    vm.SrcDirectory = null;
    vm.DstDirectory = null;

    vm.load = function () {
        $H.GET("/volumes/mounted").then(function (r) {
            vm.Volumes = r.data;
        });
    };
    vm.load();

    vm.loadSrcDirectory = function (path) {
        $H.GET("/fm/" + path).then(function (r) {
            vm.SrcDirectory = r.data;
        });
    };

    vm.loadDstDirectory = function (path) {
        $H.GET("/fm/" + path).then(function (r) {
            vm.DstDirectory = r.data;
        });
    };

    function p(s, d) {
        return $.param({
            Source: s,
            Destination: d
        });
    }

    vm.syncFolder = function (src, dst) {
        $H.POST("/fm/folder/sync", p(src, dst)).then(function () {
            vm.loadDstDirectory(vm.DstDirectory.Path);
            $N.success('Sync ' + src + ' to ' + dst);
        });
    };

    vm.syncFile = function (src, dst) {
        $H.POST("/fm/file/sync", p(src, dst)).then(function () {
            vm.loadDstDirectory(vm.DstDirectory.Path);
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
