"use strict";

var app = angular.module("templateApp", ["pascalprecht.translate", "ui.router", "selectize", "trumbowyg-ng", "ngMaterial"]);

app.config(function ($translateProvider, $stateProvider, $urlRouterProvider) {
    $translateProvider.useUrlLoader("/translate");
    $translateProvider.preferredLanguage("it");
    $urlRouterProvider.otherwise("/");

    $stateProvider
    .state("home", {
        cache: false,
        url: "^/",
        views: {
            'dashboard': { templateUrl: "anthilladoc/pages/home/dashboard.html?v=" + new Date() },
            'content': { templateUrl: "anthilladoc/pages/home/content.html?v=" + new Date() }
        }
    })
    .state("config", {
        cache: false,
        url: "^/config",
        views: {
            'dashboard': { templateUrl: "anthilladoc/pages/config/dashboard.html?v=" + new Date() },
            'content': { templateUrl: "anthilladoc/pages/config/content.html?v=" + new Date() }
        }
    })
    .state("requestmedicine", {
        cache: false,
        url: "^/requestmedicine",
        views: {
            'dashboard': { templateUrl: "anthilladoc/pages/request_medicine/dashboard.html?v=" + new Date() },
            'content': { templateUrl: "anthilladoc/pages/request_medicine/content.html?v=" + new Date() }
        }
    })
    .state("requestmaterialgeneric", {
        cache: false,
        url: "^/requestmaterialgeneric",
        views: {
            'dashboard': { templateUrl: "anthilladoc/pages/request_material_generic/dashboard.html?v=" + new Date() },
            'content': { templateUrl: "anthilladoc/pages/request_material_generic/content.html?v=" + new Date() }
        }
    })
    .state("requestmaterialcleaning", {
        cache: false,
        url: "^/requestmaterialcleaning",
        views: {
            'dashboard': { templateUrl: "anthilladoc/pages/request_material_cleaning/dashboard.html?v=" + new Date() },
            'content': { templateUrl: "anthilladoc/pages/request_material_cleaning/content.html?v=" + new Date() }
        }
    })
    .state("requestmaterialsanitary", {
        cache: false,
        url: "^/requestmaterialsanitary",
        views: {
            'dashboard': { templateUrl: "anthilladoc/pages/request_material_sanitary/dashboard.html?v=" + new Date() },
            'content': { templateUrl: "anthilladoc/pages/request_material_sanitary/content.html?v=" + new Date() }
        }
    })
    .state("textfile", {
        cache: false,
        url: "^/textfile",
        views: {
            'dashboard': { templateUrl: "anthilladoc/pages/text_file/dashboard.html?v=" + new Date() },
            'content': { templateUrl: "anthilladoc/pages/text_file/content.html?v=" + new Date() }
        }
    })
    .state("mairecap", {
        cache: false,
        url: "^/mairecap",
        views: {
            'dashboard': { templateUrl: "anthilladoc/pages/mai_recap/dashboard.html?v=" + new Date() },
            'content': { templateUrl: "anthilladoc/pages/mai_recap/content.html?v=" + new Date() }
        }
    })
    ;
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