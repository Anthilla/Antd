"use strict";

var app = angular.module("wizardApp", ["pascalprecht.translate", "ui.router", "selectize", "trumbowyg-ng", "ngMaterial"]);

app.config(function ($translateProvider) {
    $translateProvider.useUrlLoader("/translate");
    $translateProvider.preferredLanguage("it");
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

app.controller("WizardController", ["$rootScope", "$scope", "$http", "$window", "$interval", WizardController]);

function WizardController($rootScope, $scope, $http, $window, $interval) {

    $scope.hideSaveButton = true;
    $scope.hideFakeSaveButton = false;
    $interval(function () {
        var confirmed = [];
        $("[ng-required]").each(function () {
            if ($(this).is("input")) {
                var tv1 = $(this).val();
                if (tv1 !== null && tv1 !== undefined && tv1.length > 0) {
                    confirmed.push(0);
                }
            }
            if ($(this).is("select")) {
                var tv2 = $(this).find("option:selected").val();
                if (tv2 !== null && tv2 !== undefined && tv2.length > 0) {
                    confirmed.push(0);
                }
            }
            if ($(this).is("textarea")) {
                var tv3 = $(this).val();
                if (tv3 !== null && tv3 !== undefined && tv3.length > 0) {
                    confirmed.push(0);
                }
            }
        });
        var elements = $("[ng-required]").length;
        $scope.test = confirmed.length + "/" + elements;
        if (confirmed.length === elements) {
            $scope.hideSaveButton = false;
            $scope.hideFakeSaveButton = true;
        } else {
            $scope.hideSaveButton = true;
            $scope.hideFakeSaveButton = false;
        }
        var perc = (confirmed.length * 100) / elements;
        $("#ProgressBar").find("div").css({ "width": perc + "%" });
    }, 500);

    $scope.SelectInterfaceConfig = {
        persist: false,
        create: false,
        delimiter: ",",
        maxItems: 1
    };

    $scope.checkPassword = function () {
        var el = $('[ng-role="passwordCheck"]');
        if ($scope.Password !== $scope.PasswordCheck) {
            el.addClass("fg-anthilla-orange");
        } else {
            el.removeClass("fg-anthilla-orange");
        }
    }

    $scope.saveConfiguration = function () {
        var data = $.param({
            Password: $scope.Password,
            Hostname: $scope.Hostname,
            Location: $scope.Location,
            Chassis: $scope.Chassis,
            Deployment: $scope.Deployment,
            Timezone: $scope.Timezone,
            NtpServer: $scope.NtpServer,
            DomainInt: $scope.DomainInt,
            DomainExt: $scope.DomainExt,
            Hosts: $scope.Hosts,
            Networks: $scope.Networks,
            Resolv: $scope.Resolv,
            Nsswitch: $scope.Nsswitch,
            Interface: $scope.Interface,
            Txqueuelen: $scope.Txqueuelen,
            Mtu: $scope.Mtu,
            Mode: $scope.Mode,
            StaticAddress: $scope.StaticAddress,
            StaticRange: $scope.StaticRange

        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/wizard", data).then(function () {
            $window.location.href = "/logout";
        }, function (r) { console.log(r); });
    }

    $scope.NtpServer = "0.it.pool.ntp.org";
    $scope.Txqueuelen = "10000";
    $scope.Mtu = "6000";

    $http.get("/wizard/data").success(function (data) {
        $scope.DomainInt = data.DomainInt;
        $scope.DomainExt = data.DomainExt;
        $scope.Hosts = data.Hosts;
        $scope.Networks = data.Networks;
        $scope.Resolv = data.Resolv;
        $scope.Nsswitch = data.Nsswitch;
        $scope.Timezones = data.Timezones;
        $scope.NetworkIfList = data.NetworkInterfaceList;
        $scope.Hostname = data.StaticHostname;
        $scope.Location = data.Location;
        $scope.Chassis = data.Chassis;
        $scope.Deployment = data.Deployment;
    });
}