"use strict";

var app = angular.module("templateApp", ["pascalprecht.translate", "ui.router", "selectize", "trumbowyg-ng", "ngMaterial"]);

app.config(function ($translateProvider, $stateProvider, $urlRouterProvider) {
    $translateProvider.useUrlLoader("/translate");
    $translateProvider.preferredLanguage("it");
    $urlRouterProvider.otherwise("/");

    $stateProvider
    .state("antd", {
        cache: false,
        url: "^/",
        views: {
            'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() },
            'content': { templateUrl: "pages/antd/info.html?v=" + new Date() }
        }
    })
    .state("info", { cache: false, url: "^/info", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/info.html?v=" + new Date() } } })
    .state("memory", { cache: false, url: "^/memory", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/memory.html?v=" + new Date() } } })
    .state("cpu", { cache: false, url: "^/cpu", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/cpu.html?v=" + new Date() } } })
    .state("svcs", { cache: false, url: "^/svcs", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/svcs.html?v=" + new Date() } } })
    .state("mod", { cache: false, url: "^/mod", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/mod.html?v=" + new Date() } } })
    .state("system", { cache: false, url: "^/system", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/system.html?v=" + new Date() } } })
    .state("losetup", { cache: false, url: "^/losetup", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/losetup.html?v=" + new Date() } } })
    .state("update", { cache: false, url: "^/update", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/update.html?v=" + new Date() } } })
    .state("host", { cache: false, url: "^/host", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/host2.html?v=" + new Date() } } })
    .state("hostparam", { cache: false, url: "^/app", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/hostparam.html?v=" + new Date() } } })
    .state("time", { cache: false, url: "^/time", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/time.html?v=" + new Date() } } })
    .state("ns", { cache: false, url: "^/ns", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/ns.html?v=" + new Date() } } })
    .state("bind", { cache: false, url: "^/bind", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/bind.html?v=" + new Date() } } })
    .state("sshd", { cache: false, url: "^/sshd", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/sshd.html?v=" + new Date() } } })
    .state("network", { cache: false, url: "^/network", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/network2.html?v=" + new Date() } } })
    .state("routes", { cache: false, url: "^/routes", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/network2-routes.html?v=" + new Date() } } })
    .state("lag", { cache: false, url: "^/lag", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/network2-lag.html?v=" + new Date() } } })
    .state("dnsclient", { cache: false, url: "^/dnsclient", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/network2-dnsclient.html?v=" + new Date() } } })
    .state("dhcpd", { cache: false, url: "^/dhcpd", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/dhcpd.html?v=" + new Date() } } })
    .state("leases", { cache: false, url: "^/leases", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/leases.html?v=" + new Date() } } })
    .state("vpn", { cache: false, url: "^/vpn", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/vpn.html?v=" + new Date() } } })
    .state("tor", { cache: false, url: "^/tor", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/tor.html?v=" + new Date() } } })
    .state("fw", { cache: false, url: "^/fw", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/fw.html?v=" + new Date() } } })
    .state("cron", { cache: false, url: "^/cron", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/cron.html?v=" + new Date() } } })
    .state("storage", { cache: false, url: "^/storage", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/storage.html?v=" + new Date() } } })
    .state("zfs", { cache: false, url: "^/zfs", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/zfs.html?v=" + new Date() } } })
    .state("disk", { cache: false, url: "^/disk", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/disk.html?v=" + new Date() } } })
    .state("om", { cache: false, url: "^/om", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/om.html?v=" + new Date() } } })
    .state("overlay", { cache: false, url: "^/overlay", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/overlay.html?v=" + new Date() } } })
    .state("acl", { cache: false, url: "^/acl", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/acl.html?v=" + new Date() } } })
    .state("rsync", { cache: false, url: "^/rsync", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/rsync.html?v=" + new Date() } } })
    .state("vm", { cache: false, url: "^/vm", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/vm.html?v=" + new Date() } } })
    .state("samba", { cache: false, url: "^/samba", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/samba.html?v=" + new Date() } } })
    .state("users", { cache: false, url: "^/users", views: { 'menu': { templateUrl: "pages/menus/antd.html?v=" + new Date() }, 'content': { templateUrl: "pages/antd/users.html?v=" + new Date() } } })
    .state("apps", {
        cache: false,
        url: "^/apps",
        views: {
            'menu': { templateUrl: "pages/menus/apps.html?v=" + new Date() },
            'content': { templateUrl: "pages/apps/appsmgmt.html?v=" + new Date() }
        }
    })
    .state("appsmgmt", { cache: false, url: "^/appsmgmt", views: { 'menu': { templateUrl: "pages/menus/apps.html?v=" + new Date() }, 'content': { templateUrl: "pages/apps/appsmgmt.html?v=" + new Date() } } })
    .state("appsdetect", { cache: false, url: "^/appsdetect", views: { 'menu': { templateUrl: "pages/menus/apps.html?v=" + new Date() }, 'content': { templateUrl: "pages/apps/appsdetect.html?v=" + new Date() } } })
    .state("log", {
        cache: false,
        url: "^/log",
        views: {
            'menu': { templateUrl: "pages/menus/log.html?v=" + new Date() },
            'content': { templateUrl: "pages/log/log.html?v=" + new Date() }
        }
    })
    .state("journal", { cache: false, url: "^/journal", views: { 'menu': { templateUrl: "pages/menus/log.html?v=" + new Date() }, 'content': { templateUrl: "pages/log/journal.html?v=" + new Date() } } })
    .state("logconfig", { cache: false, url: "^/logconfig", views: { 'menu': { templateUrl: "pages/menus/log.html?v=" + new Date() }, 'content': { templateUrl: "pages/log/logconfig.html?v=" + new Date() } } })
    .state("report", { cache: false, url: "^/report", views: { 'menu': { templateUrl: "pages/menus/log.html?v=" + new Date() }, 'content': { templateUrl: "pages/log/report.html?v=" + new Date() } } })
    .state("syslogng", { cache: false, url: "^/syslogng", views: { 'menu': { templateUrl: "pages/menus/log.html?v=" + new Date() }, 'content': { templateUrl: "pages/log/syslogng.html?v=" + new Date() } } })
    .state("vnc", {
        cache: false,
        url: "^/vnc",
        views: {
            'menu': { templateUrl: "pages/menus/vnc.html?v=" + new Date() },
            'content': { templateUrl: "pages/vnc/content.html?v=" + new Date() }
        }
    })
    .state("asset", {
        cache: false,
        url: "^/asset",
        views: {
            'menu': { templateUrl: "pages/menus/asset.html?v=" + new Date() },
            'content': { templateUrl: "pages/asset/discovery.html?v=" + new Date() }
        }
    })
    .state("cluster", { cache: false, url: "^/cluster", views: { 'menu': { templateUrl: "pages/menus/asset.html?v=" + new Date() }, 'content': { templateUrl: "pages/asset/cluster.html?v=" + new Date() } } })
    .state("gluster", { cache: false, url: "^/gluster", views: { 'menu': { templateUrl: "pages/menus/asset.html?v=" + new Date() }, 'content': { templateUrl: "pages/asset/gluster.html?v=" + new Date() } } })
    .state("discovery", { cache: false, url: "^/discovery", views: { 'menu': { templateUrl: "pages/menus/asset.html?v=" + new Date() }, 'content': { templateUrl: "pages/asset/discovery.html?v=" + new Date() } } })
    .state("scan", { cache: false, url: "^/scan", views: { 'menu': { templateUrl: "pages/menus/asset.html?v=" + new Date() }, 'content': { templateUrl: "pages/asset/scan.html?v=" + new Date() } } })
    .state("assetsync", { cache: false, url: "^/assetsync", views: { 'menu': { templateUrl: "pages/menus/asset.html?v=" + new Date() }, 'content': { templateUrl: "pages/asset/assetsync.html?v=" + new Date() } } })
    .state("assetconfig", { cache: false, url: "^/assetconfig", views: { 'menu': { templateUrl: "pages/menus/asset.html?v=" + new Date() }, 'content': { templateUrl: "pages/asset/assetconfig.html?v=" + new Date() } } })
    .state("ca", {
        cache: false,
        url: "^/ca",
        views: {
            'menu': { templateUrl: "pages/menus/ca.html?v=" + new Date() },
            'content': { templateUrl: "pages/ca/ca.html?v=" + new Date() }
        }
    })
    .state("cert", { cache: false, url: "^/cert", views: { 'menu': { templateUrl: "pages/menus/ca.html?v=" + new Date() }, 'content': { templateUrl: "pages/ca/cert.html?v=" + new Date() } } })
    .state("dc", { cache: false, url: "^/dc", views: { 'menu': { templateUrl: "pages/menus/ca.html?v=" + new Date() }, 'content': { templateUrl: "pages/ca/dc.html?v=" + new Date() } } })
    .state("dcusers", { cache: false, url: "^/dcusers", views: { 'menu': { templateUrl: "pages/menus/ca.html?v=" + new Date() }, 'content': { templateUrl: "pages/ca/dcusers.html?v=" + new Date() } } })
    .state("boot", {
        cache: false,
        url: "^/boot",
        views: {
            'menu': { templateUrl: "pages/menus/boot.html?v=" + new Date() },
            'content': { templateUrl: "pages/boot/bootcmd.html?v=" + new Date() }
        }
    })
    .state("bootcmd", { cache: false, url: "^/bootcmd", views: { 'menu': { templateUrl: "pages/menus/boot.html?v=" + new Date() }, 'content': { templateUrl: "pages/boot/bootcmd.html?v=" + new Date() } } })
    .state("bootmod", { cache: false, url: "^/bootmod", views: { 'menu': { templateUrl: "pages/menus/boot.html?v=" + new Date() }, 'content': { templateUrl: "pages/boot/bootmod.html?v=" + new Date() } } })
    .state("bootsvc", { cache: false, url: "^/bootsvc", views: { 'menu': { templateUrl: "pages/menus/boot.html?v=" + new Date() }, 'content': { templateUrl: "pages/boot/bootsvc.html?v=" + new Date() } } })
    .state("bootparam", { cache: false, url: "^/bootparam", views: { 'menu': { templateUrl: "pages/menus/boot.html?v=" + new Date() }, 'content': { templateUrl: "pages/boot/bootparam.html?v=" + new Date() } } })
    ;
});

app.directive("convertToNumber", function () {
    return {
        require: "ngModel",
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (val) {
                return val != null ? parseInt(val, 10) : null;
            });
            ngModel.$formatters.push(function (val) {
                return val != null ? "" + val : null;
            });
        }
    };
});

app.filter("orderByOnce", function (item) {
    return function (input, uppercase) {
        input = input || "";
        var out = "";
        for (var i = 0; i < input.length; i++) {
            out = input.charAt(i) + out;
        }
        // conditional based on optional argument
        if (uppercase) {
            out = out.toUpperCase();
        }
        return out;
    };
});

app.controller("LanguageSwitchController", ["$scope", "$rootScope", "$translate",
  function ($scope, $rootScope, $translate) {
      $scope.changeLanguage = function (langKey) {
          $translate.use(langKey);
      };

      $rootScope.$on("$translateChangeSuccess", function (event, data) {
          var language = data.language;
          $rootScope.lang = language;
      });
  }]);

app.controller("CurrentDateTimeController", ["$scope", function ($scope) { $scope.date = new Date(); }]);

app.controller("AuthenticationController", ["$rootScope", "$scope", "$http", "$window", AuthenticationController]);

function AuthenticationController($rootScope, $scope, $http, $window) {

    $scope.UserName = "user";

    $http.get("/auth/user").success(function (data) {
        if (data == null) {
            $window.location.href = "/login";
            return;
        }
        $scope.UserName = data.FirstName + " " + data.LastName;
    });

}