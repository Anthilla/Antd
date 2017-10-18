"use strict";

var app = angular.module("templateApp", [
    "pascalprecht.translate",
    "ui.router",
    "selectize",
    "trumbowyg-ng",
    "ngFileUpload",
    "ngclipboard"
]);

app.config(function ($translateProvider, $stateProvider, $urlRouterProvider) {
    $translateProvider.useUrlLoader("/translate");
    $translateProvider.preferredLanguage("it");
    $urlRouterProvider.otherwise("/");

    $stateProvider
    .state("config", {
        cache: false,
        url: "^/",
        views: {
            'content': { templateUrl: "pages/config.html?v=" + new Date() }
        }
    })
    .state("status", {
        cache: false,
        url: "^/status",
        views: {
            'content': { templateUrl: "pages/status.html?v=" + new Date() }
        }
    })
    .state("cluster", {
        cache: false,
        url: "^/cluster",
        views: {
            'content': { templateUrl: "pages/cluster.html?v=" + new Date() }
        }
    })
    .state("log", {
        cache: false,
        url: "^/log",
        views: {
            'content': { templateUrl: "pages/log.html?v=" + new Date() }
        }
    })
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

app.service("authenticationService", function () {
    var authenticatedUser = null;
    return {
        set: function (user) {
            authenticatedUser = user;
            return;
        },
        get: function () {
            return authenticatedUser;
        }
    }
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

app.controller("NavbarController", ["$scope", "$http", "$window", "$interval", NavbarController]);

function NavbarController($scope, $http, $window, $interval) {
    function param(el) {
        return $.param({
            Message: el.Message
        });
    }

    $scope.save = function (el) {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/repo/ticket/send", param(el)).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    }

    $scope.date = new Date();
    $interval(function () {
        $scope.date = new Date();
    }, 36000);

    $scope.getMonitor = function () {
        $http.get("/monitor").success(function (data) { $scope.monitor = data; });
    }
    $scope.getMonitor();
    $interval(function () { $scope.getMonitor(); }, 1000 * 10);
}

app.controller("AuthenticationController", ["$rootScope", "$scope", "$http", "$window", AuthenticationController]);

function AuthenticationController($rootScope, $scope, $http, $window) {

    //$scope.UserName = "user";

    //$http.get("/auth/user").success(function (data) {
    //    if (data == null) {
    //        $window.location.href = "/login";
    //        return;
    //    }
    //    $scope.UserName = data.FirstName + " " + data.LastName;
    //});

    $scope.AclHide = function (functionCode) {
        if (functionCode == "0000000a") { return false; }
        //var hasAcl = false;
        //while ($scope.AuthenticatedUser != null) {
        //    hasAcl = $scope.AuthenticatedUser.FunctionCodes.indexOf(functionCode) > 0;
        //    break;
        //}
        //return !hasAcl;
    }
}

app.controller("ToolbarController", ["$scope", "$http", ToolbarController]);

function ToolbarController($scope, $http) {
    $scope.separator = "";

    $scope.Menu = [
        { AclId: "0000000a", Href: "config", Label: "MACHINE_CONFIG", Icon: "cog", IsActive: "active", SubMenu: [] },
        { AclId: "0000000a", Href: "cluster", Label: "CLUSTER", Icon: "equalizer", IsActive: "", SubMenu: [] },
        { AclId: "0000000a", Href: "status", Label: "MACHINE_STATUS", Icon: "stack3", IsActive: "", SubMenu: [] },
        { AclId: "0000000a", Href: "log", Label: "LOG", Icon: "list2", IsActive: "", SubMenu: [] },
    ];

    $scope.SubMenu = $scope.Menu[0].SubMenu;
    $scope.SubMenuShow = $scope.SubMenu.length > 0;
    $scope.ContextTitle = $scope.Menu[0].Label;
    $scope.SubContextTitle = "";

    $scope.activateThis = function (button) {
        $scope.ContextTitle = button.Label;
        angular.forEach($scope.Menu, function (btn) {
            btn.IsActive = "";
        });
        button.IsActive = "active";
        $scope.SubMenu = button.SubMenu;
        $scope.SubMenuShow = $scope.SubMenu.length > 0;
        if ($scope.SubMenu.length > 0) {
            $scope.separator = ": ";
            $scope.activateThisSub($scope.SubMenu[0]);
        }
    }

    $scope.activateThisSub = function (button) {
        $scope.SubContextTitle = button.Label;
        angular.forEach($scope.SubMenu, function (btn) {
            btn.IsActive = "";
        });
        button.IsActive = "active";
    }
}

