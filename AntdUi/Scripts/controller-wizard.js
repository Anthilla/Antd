"use strict";

var app = angular.module("templateApp", ["pascalprecht.translate", "ui.router", "selectize", "trumbowyg-ng", "ngMaterial"]);

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

app.controller("WizardController", ["$rootScope", "$scope", "$http", "$window", WizardController]);

function WizardController($rootScope, $scope, $http, $window) {

    $scope.UserName = "user";

    $http.get("/auth/user").success(function (data) {
        if (data == null) {
            $window.location.href = "/login";
            return;
        }
        $scope.UserName = data.FirstName + " " + data.LastName;
    });

}