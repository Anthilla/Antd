"use strict";

var app = angular.module("templateApp", [
    "ngSanitize",
    "pascalprecht.translate",
    "ui.router",
    "angularjs-datetime-picker",
    "selectize",
    "jlareau.pnotify",
    "ui.calendar",
    "trumbowyg-ng",
    "ngclipboard",
    "ngFileUpload",
    "chart.js",
    "ngCookies",
    "rzModule",
    "NgSwitchery",
    "dbaq.emoji",
    "720kb.tooltips",
    "g1b.calendar-heatmap",
    "com.2fdevs.videogular",
    "com.2fdevs.videogular.plugins.controls",
    "com.2fdevs.videogular.plugins.overlayplay",
    "com.2fdevs.videogular.plugins.poster",
    "com.2fdevs.videogular.plugins.dash"
]);

app.config(function ($translateProvider, $stateProvider, $urlRouterProvider) {
    $translateProvider.useUrlLoader("/translate");
    $translateProvider.preferredLanguage("it");
    $translateProvider.useSanitizeValueStrategy('escape'); //sanitize, sanitizeParameters, escape, escapeParameters 
    $urlRouterProvider.otherwise("/");

    $stateProvider
        //.state("home", {
        //    cache: false,
        //    url: "^/",
        //    views: {
        //        'content': { templateUrl: "pages/dashboard.html?v=" + new Date() }
        //    }
        //})
       .state("home", {
            cache: false,
            url: "^/",
            views: {
                'content': { templateUrl: "pages/event.html?v=" + new Date() }
            }
        })

       .state("testmqtt", {
            cache: false,
            url: "^/",
            views: {
                'content': { templateUrl: "pages/testmqtt.html?v=" + new Date() }
            }
        })

        .state("dashboard", {
            cache: false,
            url: "^/dashboard",
            views: {
                'content': { templateUrl: "pages/dashboard.html?v=" + new Date() }
            }
        })

        .state("event", {
            cache: false,
            url: "^/event",
            views: {
                'content': { templateUrl: "pages/event.html?v=" + new Date() }
            }
        })
        .state("video", {
            cache: false,
            url: "^/video",
            views: {
                'content': { templateUrl: "pages/video.html?v=" + new Date() }
            }
        })
        .state("guest", {
            cache: false,
            url: "^/guest",
            views: {
                'content': { templateUrl: "pages/guest.html?v=" + new Date() }
            }
        })
        .state("live", {
            cache: false,
            url: "^/live",
            views: {
                'content': { templateUrl: "pages/live.html?v=" + new Date() }
            }
        })
        .state("command", {
            cache: false,
            url: "^/command",
            views: {
                'content': { templateUrl: "pages/command.html?v=" + new Date() }
            }
        })
        .state("warning", {
            cache: false,
            url: "^/warning",
            views: {
                'content': { templateUrl: "pages/warning.html?v=" + new Date() }
            }
        })

        .state("zone", {
            cache: false,
            url: "^/zone",
            views: {
                'content': { templateUrl: "pages/d/zone.html?v=" + new Date() }
            }
        })
        .state("camera", {
            cache: false,
            url: "^/camera",
            views: {
                'content': { templateUrl: "pages/d/camera.html?v=" + new Date() }
            }
        })
        .state("relay", {
            cache: false,
            url: "^/relay",
            views: {
                'content': { templateUrl: "pages/d/relay.html?v=" + new Date() }
            }
        })
        .state("access", {
            cache: false,
            url: "^/access",
            views: {
                'content': { templateUrl: "pages/d/access.html?v=" + new Date() }
            }
        })
        .state("sensor", {
            cache: false,
            url: "^/sensor",
            views: {
                'content': { templateUrl: "pages/d/sensor.html?v=" + new Date() }
            }
        })

        .state("volumeconfig", {
            cache: false,
            url: "^/volumeconfig",
            views: {
                'content': { templateUrl: "pages/c/volumeconfig.html?v=" + new Date() }
            }
        })
        .state("volume", {
            cache: false,
            url: "^/volume",
            views: {
                'content': { templateUrl: "pages/c/volume.html?v=" + new Date() }
            }
        })
        .state("contact", {
            cache: false,
            url: "^/contact",
            views: {
                'content': { templateUrl: "pages/c/contact.html?v=" + new Date() }
            }
        })
        .state("account", {
            cache: false,
            url: "^/account",
            views: {
                'content': { templateUrl: "pages/c/account.html?v=" + new Date() }
            }
        })
        .state("restapi", {
            cache: false,
            url: "^/restapi",
            views: {
                'content': { templateUrl: "pages/c/restapi.html?v=" + new Date() }
            }
        })
        .state("sms", {
            cache: false,
            url: "^/sms",
            views: {
                'content': { templateUrl: "pages/c/sms.html?v=" + new Date() }
            }
        })
        .state("mqtt", {
            cache: false,
            url: "^/mqtt",
            views: {
                'content': { templateUrl: "pages/c/mqtt.html?v=" + new Date() }
            }
        })

        .state("company", {
            cache: false,
            url: "^/company",
            views: {
                'content': { templateUrl: "pages/r/company.html?v=" + new Date() }
            }
        })
        .state("functiongroup", {
            cache: false,
            url: "^/functiongroup",
            views: {
                'content': { templateUrl: "pages/r/functiongroup.html?v=" + new Date() }
            }
        })
        .state("license", {
            cache: false,
            url: "^/license",
            views: {
                'content': { templateUrl: "pages/r/license.html?v=" + new Date() }
            }
        })
        .state("project", {
            cache: false,
            url: "^/project",
            views: {
                'content': { templateUrl: "pages/r/project.html?v=" + new Date() }
            }
        })
        .state("user", {
            cache: false,
            url: "^/user",
            views: {
                'content': { templateUrl: "pages/r/user.html?v=" + new Date() }
            }
        })
        .state("usergroup", {
            cache: false,
            url: "^/usergroup",
            views: {
                'content': { templateUrl: "pages/r/usergroup.html?v=" + new Date() }
            }
        })
        .state("log", {
            cache: false,
            url: "^/log",
            views: {
                'content': { templateUrl: "pages/r/log.html?v=" + new Date() }
            }
        })
        .state("app", {
            cache: false,
            url: "^/app",
            views: {
                'content': { templateUrl: "pages/r/app.html?v=" + new Date() }
            }
        })
        .state("profile", {
            cache: false,
            url: "^/profile/:guid",
            views: {
                'content': { templateUrl: "pages/u/profile.html?v=" + new Date() }
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

app.directive('scrollToBottom', function ($timeout, $window) {
    return {
        scope: {
            scrollToBottom: "="
        },
        restrict: 'A',
        link: function (scope, element, attr) {
            scope.$watchCollection('scrollToBottom', function (newVal) {
                if (newVal) {
                    $timeout(function () {
                        element[0].scrollTop = element[0].scrollHeight;
                    }, 0);
                }
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

app.controller("CookieController", ["$scope", "$window", "$interval", "$cookies", CookieController]);

function CookieController($scope, $window, $interval, $cookies) {
    $scope.showCookieMessage = false;

    $scope.manageCookie = function () {
        var cookieInfoCookie = $cookies.get('anthillasp_cookie_notice_accepted');
        if (cookieInfoCookie != "true") {
            $scope.showCookieMessage = true;
        }
        else {
            $scope.showCookieMessage = false;
        }
    }
    $scope.manageCookie();

    $scope.acceptCookie = function () {
        var expiration = moment().add(30, 'days').toDate();
        $cookies.put('anthillasp_cookie_notice_accepted', true, { 'expires': expiration });
        $scope.manageCookie();
    }

    $scope.cookiePolicyInfo = function () {
        $window.location.href = "/policy/cookie";
    }
}

app.controller("LoginController", ["$scope", "$http", "notificationService", LoginController]);

function LoginController($scope, $http, notificationService) {
    $scope.Username = "";

    $scope.UserExists = false;

    $scope.VerifyUsername = function () {
        $scope.UserExists = $scope.Username.length > 0;
    }

    $scope.slider = {
        minValue: 0,
        maxValue: 9,
        options: {
            floor: 0,
            ceil: 9,
            step: 1,
            showTicks: false,
            labelOverlapSeparator: '',
            translate: function (value) {
                return '';
            }
        }
    }

    $scope.CaptchaData = [];
    $scope.CaptchaReady = false;
    $scope.HintUrl = '';

    $scope.load = function () {
        $scope.CaptchaData = [];
        $scope.CaptchaReady = false;
        $http.get("/captcha/session_request").then(function (r) {
            var data = r.data;
            $scope.HintUrl = "/captcha/session_hint";
            $scope.CaptchaData = data.data;
            $scope.CaptchaReady = true;
        }).catch(function (r) {
            notificationService.error('Error! ' + angular.toJson(r.Message));
        });
    }
    //$scope.load();

    $scope.SubmitCaptcha = function () {
        var less = $scope.slider.maxValue > $scope.slider.minValue ? $scope.slider.minValue : $scope.slider.maxValue;
        var most = $scope.slider.maxValue > $scope.slider.minValue ? $scope.slider.maxValue : $scope.slider.minValue;
        var code = '';
        for (var i = less; i <= most; i++) {
            code += $scope.CaptchaData[i].code;
        }
        var data = $.param({
            CAPTCHA_CODE: code
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/captcha/session_verify", data).then(function () {
            notificationService.success('Ok');
        }, function (r) {
            notificationService.error('Error! ' + angular.toJson(r.Message));
        });
    }
}

app.controller("AuthenticationController", ["$rootScope", "$scope", "$http", "$window", "authenticationService", "$interval", AuthenticationController]);

function AuthenticationController($rootScope, $scope, $http, $window, authenticationService, $interval) {

    $scope.UserName = "";
    $scope.UserGuid = "";

    $scope.Login = function () {
        if ($scope.UserName.length > 0) {
            return;
        }
        $http({
            method: "GET",
            url: "/login/user"
        }).then(function successCallback(r) {
            var data = r.data;
            if (data == null) {
                return;
            }
            $scope.AuthenticatedUser = data;
            authenticationService.set(data);
            if (data.FirstName === "null" && data.LastName === "null") {
                $scope.UserName = data.Alias;
            } else {
                $scope.UserName = data.FirstName + " " + data.LastName;
            }
            $scope.UserGuid = data.Guid;
        }, function errorCallback(r) {
            console.log("session expired: " + r);
            $window.location.href = "/logout";
        });
    }
    $scope.Login();

    $scope.AclHide = function (functionCode) {
        if (functionCode == "FFFFFFFF") { return true; }
        var hasAcl = false;
        while ($scope.AuthenticatedUser != null) {
            hasAcl = $scope.AuthenticatedUser.FunctionCodes.indexOf(functionCode) > -1;
            break;
        }
        return hasAcl;
    }

    $scope.LockScreenHidden = true;

    $scope.SupportLink = null;

    $scope.GetSupportLink = function () {
        $http.get("/login/supportlink").success(function (data) {
            $scope.SupportLink = data;
        });
    }
    $scope.GetSupportLink();

    $scope.OpenSupport = function () {
        alert(data);
        //$scope.GetSupportLink();
        $window.open($scope.SupportLink, '_blank');
    }
}

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
    $scope.date = new Date();

    $interval(function () {
        $scope.date = new Date();
    }, 35900);
}

app.controller("SidebarController", ["$scope", "$http", SidebarController]);

function SidebarController($scope, $http) {

    $scope.CompanyName = "owner";

    $scope.GetOwner = function () {
        $http.get("/repo/company/owner").success(function (data) {
            $scope.Company = data;
        });
    }
    $scope.GetOwner();

    $scope.Menu = [
        {
            Name: '',
            IconColor: 'fg-info',
            Elements: [
                //{
                //    Id: 'FFFFFFFF',
                //    Name: 'DASHBOARD',
                //    Icon: 'fa-tachometer',
                //    Destination: 'dashboard',
                //    Links: []
                //},
                {
                    Id: '0501B301',
                    Name: 'EVENTS',
                    Icon: 'fa-calendar',
                    Destination: 'event',
                    Links: []
                },
                {
                    Id: '1601B301',
                    Name: 'VIDEO',
                    Icon: 'fa-film',
                    Destination: 'video',
                    Links: []
                },
                {
                    Id: '0702B301',
                    Name: 'GUESTS',
                    Icon: 'fa-users',
                    Destination: 'guest',
                    Links: []
                },
                {
                    Id: '0C03B301',
                    Name: 'LIVE',
                    Icon: 'fa-video-camera',
                    Destination: 'live',
                    Links: []
                },
                {
                    Id: '0304B301',
                    Name: 'COMMANDS',
                    Icon: 'fa-rocket',
                    Destination: 'command',
                    Links: []
                },
                {
                    Id: '1703B301',
                    Name: 'WARNINGS',
                    Icon: 'fa-bullhorn',
                    Destination: 'warning',
                    Links: []
                }
            ]
        },
        {
            Name: '',
            IconColor: 'fg-warning',
            Elements: [
                {
                    Id: '1A01B301',
                    Name: 'ZONES',
                    Icon: 'fa-building-o',
                    Destination: 'zone',
                    Links: []
                },
                {
                    Id: '1305B301',
                    Name: 'DEVICES',
                    Icon: 'fa-crosshairs',
                    Destination: 'sensor',
                    Links: []
                },

                {
                    Id: '0306B301',
                    Name: 'CAMERAS',
                    Icon: 'fa-video-camera',
                    Destination: 'camera',
                    Links: []
                },
                {
                    Id: '1205B301',
                    Name: 'RELAYS',
                    Icon: 'fa-toggle-on',
                    Destination: 'relay',
                    Links: []
                },
                {
                    Id: '0103B301',
                    Name: 'ACESSES',
                    Icon: 'fa-gears',
                    Destination: 'access',
                    Links: []
                },
                {
                    Id: 'DDDDDDDD',
                    Name: 'MQTT Test',
                    Icon: 'fa-bomb',
                    Destination: 'testmqtt',
                    Links: []
                }
            ]
        },
        {
            Name: '',
            IconColor: 'fg-success',
            Elements: [
                {
                    Id: '1602B301',
                    Name: 'CONF_VOLUMES',
                    Icon: 'fa-database',
                    Destination: 'volumeconfig',
                    Links: []
                },
                {
                    Id: '1603B301',
                    Name: 'VOLUMES',
                    Icon: 'fa-database',
                    Destination: 'volume',
                    Links: []
                },
                {
                    Id: '0102B301',
                    Name: 'ACCOUNTS',
                    Icon: 'fa-user',
                    Destination: 'account',
                    Links: []
                },
                {
                    Id: '0305B301',
                    Name: 'CONTACTS',
                    Icon: 'fa-list-alt',
                    Destination: 'contact',
                    Links: []
                },
                {
                    Id: '1204B301',
                    Name: 'RESTAPI',
                    Icon: 'fa-wifi',
                    Destination: 'restapi',
                    Links: []
                },
                {
                    Id: '1304B301',
                    Name: 'SMS',
                    Icon: 'fa-fax',
                    Destination: 'sms',
                    Links: []
                },
                {
                    Id: '0D04B301',
                    Name: 'MQTT',
                    Icon: 'fa-send-o',
                    Destination: 'mqtt',
                    Links: []
                }
            ]
        },
        {
            Name: '',
            IconColor: 'fg-warning',
            Elements: [
                {
                    Id: '1501FF01',
                    Name: 'USERS',
                    Icon: 'fa-user',
                    Destination: 'user',
                    Links: []
                },
                {
                    Id: '0303FF01',
                    Name: 'COMPANIES',
                    Icon: 'fa-building-o',
                    Destination: 'company',
                    Links: []
                },
                {
                    Id: '1001FF01',
                    Name: 'PROJECTS',
                    Icon: 'fa-archive',
                    Destination: 'project',
                    Links: []
                },
                {
                    Id: '1503FF01',
                    Name: 'USERGROUPS',
                    Icon: 'fa-users',
                    Destination: 'usergroup',
                    Links: []
                },
                {
                    Id: '0603FF01',
                    Name: 'FUNCGROUPS',
                    Icon: 'fa-puzzle-piece',
                    Destination: 'functiongroup',
                    Links: []
                },
                {
                    Id: '0C01FF01',
                    Name: 'LICENSES',
                    Icon: 'fa-legal',
                    Destination: 'license',
                    Links: []
                },
                {
                    Id: '0C01FF01',
                    Name: 'LOG',
                    Icon: 'fa-list-alt',
                    Destination: 'log',
                    Links: []
                },
                {
                    Id: '0101FF01',
                    Name: 'APP',
                    Icon: 'fa-wrench',
                    Destination: 'app',
                    Links: []
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

app.controller("InstallController", ["$scope", "$http", "$interval", InstallController]);

function InstallController($scope, $http, $interval) {
    $scope.CreateAlias = function () {
        if ($scope.Data.AdministratorFirstName.length > 3 && $scope.Data.AdministratorLastName.length > 3) {
            var alias = $scope.Data.AdministratorLastName.substring(0, 3) + $scope.Data.AdministratorFirstName.substring(0, 3) + "01";
            $scope.Data.AdministratorAlias = alias.toLowerCase();
        }
    }

    $scope.Data = {
        OrganizationName: "",
        AdministratorFirstName: "",
        AdministratorLastName: "",
        AdministratorAlias: "",
        AdministratorEmail: "",
        Password: "",
        PasswordCheck: ""
    };

    $interval(function () {
        var inputValid = true;
        inputValid = $scope.Data.OrganizationName.length < 1 ? false : true;
        inputValid = $scope.Data.AdministratorFirstName.length < 1 ? false : true;
        inputValid = $scope.Data.AdministratorLastName.length < 1 ? false : true;
        inputValid = $scope.Data.AdministratorEmail.length < 1 ? false : true;
        inputValid = $scope.Data.Password.length < 1 ? false : true;
        if ($scope.ValidPassword && inputValid === true) {
            $scope.HideButton = false;
        } else {
            $scope.HideButton = true;
        }
    }, 500);

    $scope.HideButton = true;
    $scope.ValidPassword = false;

    $scope.CheckPassword = function () {
        if ($scope.Data.PasswordCheck === $scope.Data.Password) {
            $scope.ValidPassword = true;
            $("[data-password]").removeClass("fg-anthilla-orange");

        } else {
            $scope.ValidPassword = false;
            $("[data-password]").addClass("fg-anthilla-orange");
        }
    }

    $scope.Install = function (el) {
        var data = $.param({
            OrganizationName: $scope.Data.OrganizationName,
            AdministratorFirstName: $scope.Data.AdministratorFirstName,
            AdministratorLastName: el.AdministratorLastName,
            AdministratorEmail: el.AdministratorEmail,
            Password: el.Password
        });
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        $http.post("/install", data).then(function () {
            $window.location.href = "/";
        }, function (response) { console.log(response); });
    }
}