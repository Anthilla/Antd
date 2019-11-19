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

        .state("commands", {
            cache: false, url: "^/commands", views: { 'content': { templateUrl: "pg/commands.min.html?v=" + new Date() } }
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
            //{ Name: 'Home', Icon: 'fa-square fg-info', Destination: 'home', Active: activeClass },
            { Name: 'Host', Icon: 'fa-square fg-info', Destination: 'host', Active: activeClass },
            { Name: 'Time and Date', Icon: 'fa-square fg-info', Destination: 'time', Active: '' },

            { Name: 'Sysctl', Icon: 'fa-square fg-success', Destination: 'sysctl', Active: '' },
            { Name: 'Modules', Icon: 'fa-square fg-success', Destination: 'modules', Active: '' },
            { Name: 'Services', Icon: 'fa-square fg-success', Destination: 'services', Active: '' },

            //{ Name: 'DNS Client', Icon: 'fa-square fg-warning', Destination: 'dns_client', Active: '' },
            { Name: 'Interfaces', Icon: 'fa-square fg-warning', Destination: 'interfaces', Active: '' },
            { Name: 'Routing Tables', Icon: 'fa-square fg-warning', Destination: 'routing_tables', Active: '' },
            { Name: 'Routing', Icon: 'fa-square fg-warning', Destination: 'routing', Active: '' },

            { Name: 'Disks', Icon: 'fa-square fg-violet', Destination: 'disks', Active: '' },
            { Name: 'Volumes', Icon: 'fa-square fg-violet', Destination: 'volumes', Active: '' },

            { Name: 'Commands', Icon: 'fa-square fg-danger', Destination: 'commands', Active: '' },

            { Name: 'Terminal', Icon: 'fa-square fg-dark', Destination: 'terminal', Active: '' },

        ]
    }];

    $scope.MenuOLD = [
        {
            Name: 'Status',
            Elements: [
                {
                    Name: 'Home',
                    Icon: 'fa-tachometer',
                    Links: [
                        { Name: 'Dashboard', Destination: 'home_dashboard', ActiveClass: 'active' }
                    ]
                }
            ]
        },
        {
            Name: 'Machine Configuration',
            Elements: [
                {
                    Name: 'Host',
                    Icon: 'fa-laptop',
                    Links: [
                        { Name: 'Info', Destination: 'host_info', ActiveClass: '' },
                        { Name: 'Time and Date', Destination: 'host_timedate', ActiveClass: '' },
                        { Name: 'Webservice', Destination: 'host_webservice', ActiveClass: '' }
                    ]
                },
                {
                    Name: 'Boot',
                    Icon: 'fa-flash',
                    Links: [
                        { Name: 'Parameters (sysctl)', Destination: 'boot_parameters', ActiveClass: '' },
                        { Name: 'Modules', Destination: 'boot_modules', ActiveClass: '' },
                        { Name: 'Services', Destination: 'boot_services', ActiveClass: '' },
                        { Name: 'Commands', Destination: 'boot_commands', ActiveClass: '' }
                    ]
                },
                {
                    Name: 'Log',
                    Icon: 'fa-info',
                    Links: [
                        { Name: 'View', Destination: 'log_view', ActiveClass: '' }
                    ]
                },
                {
                    Name: 'Network',
                    Icon: 'fa-wifi',
                    Links: [
                        { Name: 'Internal Network', Destination: 'network_internalnetwork', ActiveClass: '' },
                        { Name: 'External Network', Destination: 'network_externalnetwork', ActiveClass: '' },
                        { Name: 'Interfaces', Destination: 'network_interfaces', ActiveClass: '' },
                        { Name: 'DNS Client', Destination: 'network_dnsclient', ActiveClass: '' },
                        { Name: 'Known Hosts', Destination: 'network_knownhosts', ActiveClass: '' },
                        { Name: 'Known Networks', Destination: 'network_knownnetworks', ActiveClass: '' },
                        { Name: 'Bridge', Destination: 'network_bridge', ActiveClass: '' },
                        { Name: 'Bond', Destination: 'network_bond', ActiveClass: '' },
                        { Name: 'Tun', Destination: 'network_tun', ActiveClass: '' },
                        { Name: 'Tap ', Destination: 'network_tap', ActiveClass: '' },
                        { Name: 'Gateways', Destination: 'network_gateways', ActiveClass: '' },
                        { Name: 'Routing', Destination: 'network_routing', ActiveClass: '' },
                        { Name: 'Routing Table', Destination: 'network_routingtable', ActiveClass: '' },
                        { Name: 'Wi-Fi', Destination: 'network_wifi', ActiveClass: '' }
                    ]
                },
                {
                    Name: 'SSH',
                    Icon: 'fa-sitemap',
                    Links: [
                        { Name: 'Public Key', Destination: 'ssh_publickey', ActiveClass: '' },
                        { Name: 'Authorized Keys', Destination: 'ssh_authorizedkeys', ActiveClass: '' }
                    ]
                },
                {
                    Name: 'User',
                    Icon: 'fa-users',
                    Links: [
                        { Name: 'Group', Destination: 'user_group', ActiveClass: '' },
                        { Name: 'System', Destination: 'user_system', ActiveClass: '' },
                        { Name: 'Applicative', Destination: 'user_applicative', ActiveClass: '' }
                    ]
                },
                {
                    Name: 'Bind',
                    Icon: 'fa-share-square-o',
                    Links: [
                        { Name: 'Configuration', Destination: 'bind_configuration', ActiveClass: '' },
                        { Name: 'Zones', Destination: 'bind_zones', ActiveClass: '' }
                    ]
                },
                {
                    Name: 'Virsh',
                    Icon: 'fa-tasks',
                    Links: [
                        { Name: 'Configuration', Destination: 'virsh_configuration', ActiveClass: '' }
                    ]
                },
                {
                    Name: 'Firewall',
                    Icon: 'fa-shield',
                    Links: [
                        { Name: 'Configuration', Destination: 'firewall_configuration', ActiveClass: '' }
                    ]
                },
                {
                    Name: 'Syslogng',
                    Icon: 'fa-list-ol',
                    Links: [
                        { Name: 'Configuration', Destination: 'virsh_configuration', ActiveClass: '' }
                    ]
                },
                {
                    Name: 'App',
                    Icon: 'fa-list-ol',
                    Links: [
                        { Name: 'Local', Destination: 'app_local', ActiveClass: '' }
                    ]
                }
            ]
        },
        {
            Name: 'Cluster',
            Elements: [
                {
                    Name: 'Status',
                    Icon: 'fa-bug',
                    Links: [
                        { Name: 'Dashboard', Destination: 'cluster_status', ActiveClass: '' }
                    ]
                },
                //{
                //    Name: 'Neighborhood',
                //    Icon: 'fa-windows',
                //    Links: [
                //        { Name: 'Find devices', Destination: 'cluster_neighborhood', ActiveClass: '' }
                //    ]
                //},
                {
                    Name: 'Cluster',
                    Icon: 'fa-sitemap',
                    Links: [
                        { Name: 'Configuration', Destination: 'cluster_configuration', ActiveClass: '' },
                        { Name: 'Shared Network', Destination: 'cluster_sharednetwork', ActiveClass: '' },
                        { Name: 'Shared Services', Destination: 'cluster_sharedservices', ActiveClass: '' },
                        { Name: 'Shared FS', Destination: 'cluster_sharedfs', ActiveClass: '' }
                    ]
                }
            ]
        }
    ];

    $scope.itemClicked = function (index, elements) {
        for (var i = 0; i < elements.length; i++) {
            elements[i].Active = '';
        }
        elements[index] = activeClass;
    };
}

