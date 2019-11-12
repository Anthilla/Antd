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

app.controller("LoginController", ["$scope", LoginController]);

function LoginController($scope) {

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
            console.log(0);
        }, function (r) {
            console.log(1);
        });
    };
}

