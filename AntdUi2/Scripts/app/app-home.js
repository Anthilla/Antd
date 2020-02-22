var app = angular.module("templateApp", [
    "ngSanitize",
    "pascalprecht.translate",
    "ui.router",
    "jlareau.pnotify",
    "vtortola.ng-terminal"
]);

//translate config
app.config(function ($translateProvider) {
    $translateProvider.useUrlLoader("/translate");
    $translateProvider.preferredLanguage("it");
    $translateProvider.useSanitizeValueStrategy('escape'); //sanitize, sanitizeParameters, escape, escapeParameters 
});

//http config
app.config(function ($provide) {
    $provide.decorator('$http', function $logDecorator($delegate, $q) {
        $delegate.with_abort = function (options) {
            let abort_defer = $q.defer();
            let new_options = angular.copy(options);
            new_options.timeout = abort_defer.promise;
            let do_throw_error = false;
            let http_promise = $delegate(new_options).then(
                response => response,
                error => {
                    if (do_throw_error) return $q.reject(error);
                    return $q(() => null); // prevent promise chain propagation
                });
            let real_then = http_promise.then;
            let then_function = function () {
                return mod_promise(real_then.apply(this, arguments));
            };
            function mod_promise(promise) {
                promise.then = then_function;
                promise.abort = (do_throw_error_param = false) => {
                    do_throw_error = do_throw_error_param;
                    abort_defer.resolve();
                };
                return promise;
            }
            return mod_promise(http_promise);
        };
        return $delegate;
    });
});

//route config
app.config(function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise("/host");

    $stateProvider
        .state("home", {
            cache: false, url: "^/", views: { 'content': { templateUrl: "pg/dashboard.min.html?v=" + new Date() } }
        })
        .state("host", {
            cache: false, url: "^/host", views: { 'content': { templateUrl: "pg/host.min.html?v=" + new Date() } }
        })
        .state("time", {
            cache: false, url: "^/time", views: { 'content': { templateUrl: "pg/time.min.html?v=" + new Date() } }
        })
        .state("user", {
            cache: false, url: "^/user", views: { 'content': { templateUrl: "pg/user.min.html?v=" + new Date() } }
        })

        .state("sysctl", {
            cache: false, url: "^/sysctl", views: { 'content': { templateUrl: "pg/sysctl.min.html?v=" + new Date() } }
        })
        .state("modules", {
            cache: false, url: "^/modules", views: { 'content': { templateUrl: "pg/modules.min.html?v=" + new Date() } }
        })
        .state("services", {
            cache: false, url: "^/services", views: { 'content': { templateUrl: "pg/services.min.html?v=" + new Date() } }
        })

        .state("dns_client", {
            cache: false, url: "^/dns/client", views: { 'content': { templateUrl: "pg/dns_client.min.html?v=" + new Date() } }
        })
        .state("interfaces", {
            cache: false, url: "^/interfaces", views: { 'content': { templateUrl: "pg/interfaces.min.html?v=" + new Date() } }
        })
        .state("routing_tables", {
            cache: false, url: "^/routing/tables", views: { 'content': { templateUrl: "pg/routing_tables.min.html?v=" + new Date() } }
        })
        .state("routing", {
            cache: false, url: "^/routing", views: { 'content': { templateUrl: "pg/routing.min.html?v=" + new Date() } }
        })

        .state("disks", {
            cache: false, url: "^/disks", views: { 'content': { templateUrl: "pg/disks.min.html?v=" + new Date() } }
        })
        .state("volumes", {
            cache: false, url: "^/volumes", views: { 'content': { templateUrl: "pg/volumes.min.html?v=" + new Date() } }
        })
        .state("webdav", {
            cache: false, url: "^/webdav", views: { 'content': { templateUrl: "pg/webdav.min.html?v=" + new Date() } }
        })
        .state("filemanager", {
            cache: false, url: "^/filemanager", views: { 'content': { templateUrl: "pg/filemanager.min.html?v=" + new Date() } }
        })

        .state("commands", {
            cache: false, url: "^/commands", views: { 'content': { templateUrl: "pg/commands.min.html?v=" + new Date() } }
        })
        .state("scheduler", {
            cache: false, url: "^/scheduler", views: { 'content': { templateUrl: "pg/scheduler.min.html?v=" + new Date() } }
        })

        .state("terminal", {
            cache: false, url: "^/terminal", views: { 'content': { templateUrl: "pg/terminal.min.html?v=" + new Date() } }
        })
        ;
});

app.directive("convertToNumber", function () {
    return {
        require: "ngModel",
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (val) {
                return val !== null ? parseInt(val, 10) : null;
            });
            ngModel.$formatters.push(function (val) {
                return val !== null ? "" + val : null;
            });
        }
    };
});

app.filter('bytes', function () {
    return function (bytes, precision) {
        if (isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
        if (typeof precision === 'undefined') precision = 1;
        var units = ['bytes', 'kB', 'MB', 'GB', 'TB', 'PB'],
            number = Math.floor(Math.log(bytes) / Math.log(1024));
        return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
    };
});

app.service('HttpService', ['$http', '$window', 'notificationService', function ($http, $window, notificationService) {
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

app.controller("LanguageSwitchController", ["$scope", "$rootScope", "$translate", LanguageSwitchController]);

function LanguageSwitchController($scope, $rootScope, $translate) {

    $scope.SelectedLanguage = "it";

    $scope.changeLanguage = function (langKey) {
        $translate.use(langKey);
        $scope.SelectedLanguage = langKey;
    };

    $rootScope.$on("$translateChangeSuccess", function (event, data) {
        var language = data.language;
        $rootScope.lang = language;
        var now = new Date();
        var exp = new Date(now.getFullYear() + 1, now.getMonth(), now.getDate());
    });
}

app.controller("NavbarController", ["$scope", "$http", "$window", "$interval", NavbarController]);

function NavbarController($scope, $http, $window, $interval) {

    $scope.User = {
        Name: "master"
    };

    $scope.AclHide = function (functionCode) {
        if (functionCode === "0000000a") { return false; }
    };
}

app.controller("SidebarController", ["$scope", "$http", SidebarController]);

function SidebarController($scope, $http) {

    var activeClass = "text-underline";

    $scope.Menu = [{
        Elements: [
            { Name: 'Host', Icon: 'fa-square fg-info', Destination: 'host' },
            { Name: 'Time and Date', Icon: 'fa-square fg-info', Destination: 'time' },
            { Name: 'Users', Icon: 'fa-square fg-info', Destination: 'user' },

            { Name: 'Sysctl', Icon: 'fa-square fg-success', Destination: 'sysctl' },
            { Name: 'Modules', Icon: 'fa-square fg-success', Destination: 'modules' },
            { Name: 'Services', Icon: 'fa-square fg-success', Destination: 'services' },

            //{ Name: 'DNS Client', Icon: 'fa-square fg-warning', Destination: 'dns_client' },
            { Name: 'Interfaces', Icon: 'fa-square fg-warning', Destination: 'interfaces' },
            { Name: 'Routing Tables', Icon: 'fa-square fg-warning', Destination: 'routing_tables' },
            { Name: 'Routing', Icon: 'fa-square fg-warning', Destination: 'routing' },

            { Name: 'Disks', Icon: 'fa-square fg-violet', Destination: 'disks' },
            { Name: 'Volumes', Icon: 'fa-square fg-violet', Destination: 'volumes' },
            { Name: 'Webdav', Icon: 'fa-square fg-violet', Destination: 'webdav' },
            { Name: 'File Manager', Icon: 'fa-square fg-violet', Destination: 'filemanager' },

            { Name: 'Commands', Icon: 'fa-square fg-danger', Destination: 'commands' },
            { Name: 'Scheduler', Icon: 'fa-square fg-danger', Destination: 'scheduler' },

            { Name: 'Terminal', Icon: 'fa-square fg-dark', Destination: 'terminal' },
        ]
    }];
}

