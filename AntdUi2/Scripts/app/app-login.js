var app = angular.module("templateApp", [
    "ngSanitize",
    "pascalprecht.translate"
]);

//translate config
app.config(function ($translateProvider) {
    $translateProvider.useUrlLoader("/translate");
    $translateProvider.preferredLanguage("it");
    $translateProvider.useSanitizeValueStrategy('escape'); //sanitize, sanitizeParameters, escape, escapeParameters 
});

app.service('HttpService', ['$http', '$window', function ($http, $window) {
    this.GET = function (url, data) {
        var config = {
            params: data,
            headers: { 'Accept': 'application/json' }
        };
        return $http.get(url, config);
    };

    this.POST = function (url, data) {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        return $http.post(url, data);
    };
}]);

app.controller("LoginController", ["$scope", "HttpService", LoginController]);

function LoginController($scope, HttpService) {

    $scope.Username = "";
    $scope.Password = "";

    $scope.UserExists = false;

    $scope.VerifyUsername = function () {
        $scope.UserExists = $scope.Username.length > 0;
    };

    $scope.submitForm = function () {
        if (!$scope.doSubmit) {
            return;
        }
        $scope.doSubmit = false;
        var data = $.param({
            Username: $scope.Username,
            Password: $scope.Password
        });
        HttpService.POST("/login", data).then(function () {
        }, function (r) {
        });
    };
}

