"use strict";

var app = angular.module("templateApp", [
    "ngSanitize",
    "pascalprecht.translate",
    "ui.router",
    "selectize",
    "jlareau.pnotify",
    "ui.calendar",
    "trumbowyg-ng",
    "ngclipboard",
    "chart.js",
    "NgSwitchery"
]);

app.config(function ($translateProvider, $stateProvider, $urlRouterProvider) {
    $translateProvider.useUrlLoader("/translate");
    $translateProvider.preferredLanguage("it");
    $translateProvider.useSanitizeValueStrategy('sanitize');
    $urlRouterProvider.otherwise("/");

    $stateProvider
        .state("home_dashboard", {
            cache: false,
            url: "^/",
            views: {
                'content': { templateUrl: "pages/home/dashboard.html?v=" + new Date() }
            }
        })

        .state("host_info", {
            cache: false,
            url: "^/host/info",
            views: {
                'content': { templateUrl: "pages/host/info.html?v=" + new Date() }
            }
        })
        .state("host_timedate", {
            cache: false,
            url: "^/host/timedate",
            views: {
                'content': { templateUrl: "pages/host/timedate.html?v=" + new Date() }
            }
        })
        .state("host_webservice", {
            cache: false,
            url: "^/host/webservice",
            views: {
                'content': { templateUrl: "pages/host/webservice.html?v=" + new Date() }
            }
        })

        .state("boot_parameters", {
            cache: false,
            url: "^/boot/parameters",
            views: {
                'content': { templateUrl: "pages/boot/parameters.html?v=" + new Date() }
            }
        })
        .state("boot_modules", {
            cache: false,
            url: "^/boot/modules",
            views: {
                'content': { templateUrl: "pages/boot/modules.html?v=" + new Date() }
            }
        })
        .state("boot_services", {
            cache: false,
            url: "^/boot/services",
            views: {
                'content': { templateUrl: "pages/boot/services.html?v=" + new Date() }
            }
        })
        .state("boot_commands", {
            cache: false,
            url: "^/boot/commands",
            views: {
                'content': { templateUrl: "pages/boot/commands.html?v=" + new Date() }
            }
        })

        .state("log_view", {
            cache: false,
            url: "^/log/view",
            views: {
                'content': { templateUrl: "pages/log/view.html?v=" + new Date() }
            }
        })

        .state("network_internalnetwork", {
            cache: false, url: "^/network/internalnetwork",
            views: {
                'content': { templateUrl: "pages/network/internalnetwork.html?v=" + new Date() }
            }
        })
        .state("network_externalnetwork", {
            cache: false, url: "^/network/externalnetwork",
            views: {
                'content': { templateUrl: "pages/network/externalnetwork.html?v=" + new Date() }
            }
        })
        .state("network_interfaces", {
            cache: false, url: "^/network/interfaces",
            views: {
                'content': { templateUrl: "pages/network/interfaces.html?v=" + new Date() }
            }
        })
        .state("network_dnsclient", {
            cache: false, url: "^/network/dnsclient",
            views: {
                'content': { templateUrl: "pages/network/dnsclient.html?v=" + new Date() }
            }
        })
        .state("network_knownhosts", {
            cache: false, url: "^/network/knownhosts",
            views: {
                'content': { templateUrl: "pages/network/knownhosts.html?v=" + new Date() }
            }
        })
        .state("network_knownnetworks", {
            cache: false, url: "^/network/knownnetworks",
            views: {
                'content': { templateUrl: "pages/network/knownnetworks.html?v=" + new Date() }
            }
        })
        .state("network_bridge", {
            cache: false, url: "^/network/bridge",
            views: {
                'content': { templateUrl: "pages/network/bridge.html?v=" + new Date() }
            }
        })
        .state("network_bond", {
            cache: false, url: "^/network/bond",
            views: {
                'content': { templateUrl: "pages/network/bond.html?v=" + new Date() }
            }
        })
        .state("network_tun", {
            cache: false, url: "^/network/tun",
            views: {
                'content': { templateUrl: "pages/network/tun.html?v=" + new Date() }
            }
        })
        .state("network_tap", {
            cache: false, url: "^/network/tap",
            views: {
                'content': { templateUrl: "pages/network/tap.html?v=" + new Date() }
            }
        })
        .state("network_gateways", {
            cache: false, url: "^/network/gateways",
            views: {
                'content': { templateUrl: "pages/network/gateways.html?v=" + new Date() }
            }
        })
        .state("network_routing", {
            cache: false, url: "^/network/routing",
            views: {
                'content': { templateUrl: "pages/network/routing.html?v=" + new Date() }
            }
        })
        .state("network_routingtable", {
            cache: false, url: "^/network/routingtable",
            views: {
                'content': { templateUrl: "pages/network/routingtable.html?v=" + new Date() }
            }
        })
        .state("network_wifi", {
            cache: false, url: "^/network/wifi",
            views: {
                'content': { templateUrl: "pages/network/wifi.html?v=" + new Date() }
            }
        })

        .state("ssh_publickey", {
            cache: false, url: "^/ssh/publickey",
            views: {
                'content': { templateUrl: "pages/ssh/publickey.html?v=" + new Date() }
            }
        })
        .state("ssh_authorizedkeys", {
            cache: false, url: "^/ssh/authorizedkeys",
            views: {
                'content': { templateUrl: "pages/ssh/authorizedkeys.html?v=" + new Date() }
            }
        })

        .state("user_group", {
            cache: false, url: "^/user/group",
            views: {
                'content': { templateUrl: "pages/user/group.html?v=" + new Date() }
            }
        })
        .state("user_system", {
            cache: false, url: "^/user/system",
            views: {
                'content': { templateUrl: "pages/user/system.html?v=" + new Date() }
            }
        })
        .state("user_applicative", {
            cache: false, url: "^/user/applicative",
            views: {
                'content': { templateUrl: "pages/user/applicative.html?v=" + new Date() }
            }
        })

        .state("bind_configuration", {
            cache: false, url: "^/bind/configuration",
            views: {
                'content': { templateUrl: "pages/bind/configuration.html?v=" + new Date() }
            }
        })
        .state("bind_zones", {
            cache: false, url: "^/bind/zones",
            views: {
                'content': { templateUrl: "pages/bind/zones.html?v=" + new Date() }
            }
        })

        .state("virsh_configuration", {
            cache: false, url: "^/virsh/configuration",
            views: {
                'content': { templateUrl: "pages/virsh/configuration.html?v=" + new Date() }
            }
        })

        .state("firewall_configuration", {
            cache: false, url: "^/firewall/configuration",
            views: {
                'content': { templateUrl: "pages/firewall/configuration.html?v=" + new Date() }
            }
        })

        .state("syslogng_configuration", {
            cache: false, url: "^/syslogng/configuration",
            views: {
                'content': { templateUrl: "pages/syslogng/configuration.html?v=" + new Date() }
            }
        })

        .state("app_local", {
            cache: false, url: "^/app/local",
            views: {
                'content': { templateUrl: "pages/app/local.html?v=" + new Date() }
            }
        })

        .state("cluster_status", {
            cache: false, url: "^/cluster/status",
            views: {
                'content': { templateUrl: "pages/cluster/status.html?v=" + new Date() }
            }
        })
        .state("cluster_neighborhood", {
            cache: false, url: "^/cluster/neighborhood",
            views: {
                'content': { templateUrl: "pages/cluster/neighborhood.html?v=" + new Date() }
            }
        })
        .state("cluster_configuration", {
            cache: false, url: "^/cluster/configuration",
            views: {
                'content': { templateUrl: "pages/cluster/configuration.html?v=" + new Date() }
            }
        })
        .state("cluster_sharednetwork", {
            cache: false, url: "^/cluster/sharednetwork",
            views: {
                'content': { templateUrl: "pages/cluster/sharednetwork.html?v=" + new Date() }
            }
        })
        .state("cluster_sharedservices", {
            cache: false, url: "^/cluster/sharedservices",
            views: {
                'content': { templateUrl: "pages/cluster/sharedservices.html?v=" + new Date() }
            }
        })
        .state("cluster_sharedfs", {
            cache: false, url: "^/cluster/sharedfs",
            views: {
                'content': { templateUrl: "pages/cluster/sharedfs.html?v=" + new Date() }
            }
        })
        ;
});

app.directive("convertToNumber", function () {
    return {
        require: "ngModel",
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (val) {
                return val != null ? parseInt(val, 10) : null;
            });
            ngModel.$formatters.push(function (val) {
                return val != null ? "" + val : null;
            });
        }
    };
});

app.directive('iCheck', function ($timeout, $parse) {
    return {
        require: "ngModel",
        link: function ($scope, element, $attrs) {
            var value = $attrs['value'],
                ngModelGetter = $parse($attrs['ngModel']);

            return $timeout(function () {

                $scope.$watch($attrs.ngModel, function (newValue) {
                    $(element).iCheck('update');
                });

                $(element).iCheck({
                    checkboxClass: 'icheckbox_flat-green',
                    radioClass: 'iradio_flat-green'
                }).on('ifChanged', function (event) {

                    var elemType = $(element).attr('type');

                    if (elemType === 'checkbox' && $attrs.ngModel) {
                        $scope.$apply(function () {
                            return ngModelGetter.assign($scope, event.target.checked);
                        });
                    }
                    else if (elemType === 'radio' && $attrs.ngModel) {
                        return $scope.$apply(function () {
                            return ngModelGetter.assign($scope, value);
                        });
                    }
                });

            });
        }
    };
})

app.service("authenticationService", function () {
    var authenticatedUser = null;
    return {
        set: function (user) {
            authenticatedUser = user;
            return;
        },
        get: function () {
            return authenticatedUser;
        }
    }
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

app.controller("NavbarController", ["$scope", "$http", "$window", "$interval", NavbarController]);

function NavbarController($scope, $http, $window, $interval) {
    //function param(el) {
    //    return $.param({
    //        Message: el.Message
    //    });
    //}

    //$scope.save = function (el) {
    //    $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
    //    $http.post("/repo/ticket/send", param(el)).then(function () { $scope.ShowResponseMessage("ok"); }, function (r) { console.log(r); });
    //}

    //$scope.date = new Date();
    //$interval(function () {
    //    $scope.date = new Date();
    //}, 36000);

    //$scope.getMonitor = function () {
    //    $http.get("/monitor").success(function (data) { $scope.monitor = data; });
    //}
    //$scope.getMonitor();
    //$interval(function () { $scope.getMonitor(); }, 1000 * 10);

    $scope.User = {
        Name: "master"
    }

    //$http.get("/auth/user").success(function (data) {
    //    if (data == null) {
    //        $window.location.href = "/login";
    //        return;
    //    }
    //    $scope.UserName = data.FirstName + " " + data.LastName;
    //});

    $scope.AclHide = function (functionCode) {
        if (functionCode == "0000000a") { return false; }
        //var hasAcl = false;
        //while ($scope.AuthenticatedUser != null) {
        //    hasAcl = $scope.AuthenticatedUser.FunctionCodes.indexOf(functionCode) > 0;
        //    break;
        //}
        //return !hasAcl;
    }
}

app.controller("SidebarController", ["$scope", "$http", SidebarController]);

function SidebarController($scope, $http) {

    $scope.Menu = [
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
                {
                    Name: 'Neighborhood',
                    Icon: 'fa-windows',
                    Links: [
                        { Name: 'Find devices', Destination: 'cluster_neighborhood', ActiveClass: '' }
                    ]
                },
                {
                    Name: 'Cluster',
                    Icon: 'fa-sitemap',
                    Links: [
                        { Name: 'Configuration', Destination: 'cluster_configuration', ActiveClass: '' },
                        { Name: 'Shared Network', Destination: 'cluster_sharednetwork', ActiveClass: '' },
                        { Name: 'Shared Services', Destination: 'cluster_sharedservices', ActiveClass: '' },
                        { Name: 'Shared FS (glusterfs)', Destination: 'cluster_sharedfs', ActiveClass: '' }
                    ]
                }
            ]
        }
    ];

    $scope.itemClicked = function (index, elements) {
        for (var i = 0; i < elements.length; i++) {
            elements[i].ActiveClass = '';
        }
        elements[index] = "active";
    }
}

