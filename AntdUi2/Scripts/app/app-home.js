var app = angular.module("antdApp", [
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
        .state("interfaces_virtual", {
            cache: false, url: "^/interfaces/virtual", views: { 'content': { templateUrl: "pg/interfaces_virtual.min.html?v=" + new Date() } }
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
        .state("finder", {
            cache: false, url: "^/finder", views: { 'content': { templateUrl: "pg/finder.min.html?v=" + new Date() } }
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

app.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });
                event.preventDefault();
            }
        });
    };
});

app.controller("LanguageSwitchController", ["$rootScope", "$translate", $LanguageSwitch]);

function $LanguageSwitch($R, $T) {
    var vm = this;

    vm.SelectedLanguage = "it";

    vm.changeLanguage = function (langKey) {
        $T.use(langKey);
        vm.SelectedLanguage = langKey;
    };

    $R.$on("$translateChangeSuccess", function (event, data) {
        $R.lang = data.language;
    });
}

app.controller("NavbarController", [$Navbar]);

function $Navbar() {
    var vm = this;

    vm.User = {
        Name: "master"
    };
}

app.controller("SidebarController", [$Sidebar]);

function $Sidebar() {
    var vm = this;

    vm.Menu = [{
        List: [
            { Lbl: 'Host', Ico: 'fa-square fg-info', Dst: 'host' },
            { Lbl: 'Time and Date', Ico: 'fa-square fg-info', Dst: 'time' },
            { Lbl: 'Users', Ico: 'fa-square fg-info', Dst: 'user' },

            { Lbl: 'Sysctl', Ico: 'fa-square fg-success', Dst: 'sysctl' },
            { Lbl: 'Modules', Ico: 'fa-square fg-success', Dst: 'modules' },
            { Lbl: 'Services', Ico: 'fa-square fg-success', Dst: 'services' },

            //{ Lbl: 'DNS Client', Ico: 'fa-square fg-warning', Dst: 'dns_client' },
            { Lbl: 'Virtual IF', Ico: 'fa-square fg-warning', Dst: 'interfaces_virtual' },
            { Lbl: 'Interfaces', Ico: 'fa-square fg-warning', Dst: 'interfaces' },
            { Lbl: 'Routing Tables', Ico: 'fa-square fg-warning', Dst: 'routing_tables' },
            { Lbl: 'Routing', Ico: 'fa-square fg-warning', Dst: 'routing' },

            { Lbl: 'Disks', Ico: 'fa-square fg-violet', Dst: 'disks' },
            { Lbl: 'Volumes', Ico: 'fa-square fg-violet', Dst: 'volumes' },
            { Lbl: 'Webdav', Ico: 'fa-square fg-violet', Dst: 'webdav' },
            { Lbl: 'File Manager', Ico: 'fa-square fg-violet', Dst: 'filemanager' },
            { Lbl: 'Finder', Ico: 'fa-square fg-violet', Dst: 'finder' },

            { Lbl: 'Commands', Ico: 'fa-square fg-danger', Dst: 'commands' },
            { Lbl: 'Scheduler', Ico: 'fa-square fg-danger', Dst: 'scheduler' },

            { Lbl: 'Terminal', Ico: 'fa-square fg-dark', Dst: 'terminal' }
        ]
    }];
}

