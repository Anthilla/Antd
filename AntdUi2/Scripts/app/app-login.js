var app = angular.module("antdLogin", []);

app.service('HttpService', ['$http', function ($H) {
    this.GET = function (url, data) {
        return $H.get(url, {
            params: data,
            headers: { 'Accept': 'application/json' }
        });
    };

    this.POST = function (url, data) {
        $H.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        return $H.post(url, data);
    };
}]);

app.controller("LoginController", ["$scope", "HttpService", $Login]);

function $Login($scope, $H) {
    var vm = this;

    vm.Username = "";
    vm.Password = "";
    vm.UserExists = false;

    vm.submitForm = function () {
        if (!vm.doSubmit) {
            return;
        }
        vm.doSubmit = false;
        $H.POST("/login", $.param({
            Username: vm.Username,
            Password: vm.Password
        }));
    };
}

