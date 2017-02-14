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

app.controller("WizardController", ["$rootScope", "$scope", "$http", WizardController]);

function WizardController($rootScope, $scope, $http) {

    $scope.checkPassword = function () {
        console.log($scope.Password);
        console.log($scope.PasswordCheck);
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
        $http.post("/wizard", data).then(function () { alert("Ok!"); }, function (r) { console.log(r); });
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
        $scope.NetworkInterfaceList = data.NetworkInterfaceList;
    });
}