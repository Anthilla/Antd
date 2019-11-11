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
    'angularFileUpload',
    "ngFileUpload",
    "chart.js",
    "ngCookies",
    "rzModule",
    "NgSwitchery",
    "dbaq.emoji",
    "720kb.tooltips",
    "tooltips",
    "ngScrollbars",
    "angularAudioRecorder",
    "g1b.calendar-heatmap",
    "com.2fdevs.videogular",
    "com.2fdevs.videogular.plugins.controls",
    "com.2fdevs.videogular.plugins.overlayplay",
    "com.2fdevs.videogular.plugins.poster",
    "com.2fdevs.videogular.plugins.dash",
    "ngPageTitle",
    "pdfjsViewer",
    "mgo-angular-wizard",
    "infinite-scroll",
    "ng-context-menu",
    "ngTagsInput",
    "color.picker",
    "ngWebsocket"
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

//http audio recording config
app.config(function (recorderServiceProvider) {
    //.setSwfUrl('/lib/recorder.swf')
    recorderServiceProvider.forceSwf(false).withMp3Conversion(true);
});

//route config
app.config(function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise("/");

    $stateProvider
        .state("home", {
            cache: false,
            url: "/",
            views: {
                'content': { templateUrl: "pg/dashboard.min.html?v=" + new Date() }
            }
        })

        .state("dashboard", {
            cache: false,
            url: "/dashboard",
            views: {
                'content': { templateUrl: "pg/dashboard.min.html?v=" + new Date() }
            }
        })

        .state("message_received", {
            cache: false,
            url: "/message/received",
            views: {
                'content': { templateUrl: "pg/message_received.min.html?v=" + new Date() }
            }
        })
        .state("message_sent", {
            cache: false,
            url: "/message/sent",
            views: {
                'content': { templateUrl: "pg/message_sent.min.html?v=" + new Date() }
            }
        })
        .state("message_archive", {
            cache: false,
            url: "/message/archive",
            views: {
                'content': { templateUrl: "pg/message_archive.min.html?v=" + new Date() }
            }
        })

        .state("chat", {
            cache: false,
            url: "/chat",
            views: {
                'content': { templateUrl: "pg/chat.min.html?v=" + new Date() }
            }
        })
        .state("mail", {
            cache: false,
            url: "/mail",
            views: {
                'content': { templateUrl: "pg/mail.min.html?v=" + new Date() }
            }
        })
        .state("mail_account", {
            cache: false,
            url: "/mail/account",
            views: {
                'content': { templateUrl: "pg/mail_account.min.html?v=" + new Date() }
            }
        })
        .state("mail_contact", {
            cache: false,
            url: "/message/contact",
            views: {
                'content': { templateUrl: "pg/mail_contact.min.html?v=" + new Date() }
            }
        })
        .state("note", {
            cache: false,
            url: "/note",
            views: {
                'content': { templateUrl: "pg/note.min.html?v=" + new Date() }
            }
        })
        .state("notification", {
            cache: false,
            url: "/notification",
            views: {
                'content': { templateUrl: "pg/notification.min.html?v=" + new Date() }
            }
        })
        .state("sharing", {
            cache: false,
            url: "/sharing",
            views: {
                'content': { templateUrl: "pg/sharing.min.html?v=" + new Date() }
            }
        })
        .state("sharing_drive", {
            cache: false,
            url: "/drive",
            views: {
                'content': { templateUrl: "pg/sharing_drive.min.html?v=" + new Date() }
            }
        })
        //.state("sharing_upload", {
        //    cache: false,
        //    url: "/sharing/upload",
        //    views: {
        //        'content': { templateUrl: "pg/sharing_upload.min.html?v=" + new Date() }
        //    }
        //})

        .state("task", {
            cache: false,
            url: "/task",
            views: {
                'content': { templateUrl: "pg/task.min.html?v=" + new Date() }
            }
        })
        .state("task_production", {
            cache: false,
            url: "/task/production",
            views: {
                'content': { templateUrl: "pg/task_production.min.html?v=" + new Date() }
            }
        })
        .state("task_preproduction", {
            cache: false,
            url: "/task/preproduction",
            views: {
                'content': { templateUrl: "pg/task_preproduction.min.html?v=" + new Date() }
            }
        })
        .state("task_vieworders", {
            cache: false,
            url: "/task/vieworders",
            views: {
                'content': { templateUrl: "pg/task_vieworders.min.html?v=" + new Date() }
            }
        })
        .state("task_salestats", {
            cache: false,
            url: "/task/salestats",
            views: {
                'content': { templateUrl: "pg/task_salestats.min.html?v=" + new Date() }
            }
        })
        .state("task_saletrend", {
            cache: false,
            url: "/task/saletrend",
            views: {
                'content': { templateUrl: "pg/task_saletrend.min.html?v=" + new Date() }
            }
        })
        .state("task_orders", {
            cache: false,
            url: "/task/orders",
            views: {
                'content': { templateUrl: "pg/task_orders.min.html?v=" + new Date() }
            }
        })
        .state("task_orders_import", {
            cache: false,
            url: "/task/orders/import",
            views: {
                'content': { templateUrl: "pg/task_orders_import.min.html?v=" + new Date() }
            }
        })
        .state("task_orders_web", {
            cache: false,
            url: "/task/orders/web",
            views: {
                'content': { templateUrl: "pg/task_orders_web.min.html?v=" + new Date() }
            }
        })
        .state("task_orders_supplier", {
            cache: false,
            url: "/task/orders/supplier",
            views: {
                'content': { templateUrl: "pg/task_orders_supplier.min.html?v=" + new Date() }
            }
        })
        .state("task_purchases", {
            cache: false,
            url: "/task/purchases",
            views: {
                'content': { templateUrl: "pg/task_purchases.min.html?v=" + new Date() }
            }
        })

        .state("resourceregister", {
            cache: false,
            url: "/resource/register",
            views: {
                'content': { templateUrl: "pg/inventory.min.html?v=" + new Date() }
            }
        })

        .state("inventory_resource", {
            cache: false,
            url: "/inventory/resource",
            views: {
                'content': { templateUrl: "pg/inventory_resource.min.html?v=" + new Date() }
            }
        })
        .state("inventory_product", {
            cache: false,
            url: "/inventory/product",
            views: {
                'content': { templateUrl: "pg/inventory_product.min.html?v=" + new Date() }
            }
        })
        .state("inventory_materials", {
            cache: false,
            url: "/inventory/materials",
            views: {
                'content': { templateUrl: "pg/inventory_materials.min.html?v=" + new Date() }
            }
        })
        .state("inventory_product_new", {
            cache: false,
            url: "/inventory/product/new",
            views: {
                'content': { templateUrl: "pg/inventory_product_new.min.html?v=" + new Date() }
            }
        })
        .state("inventory_product_docs", {
            cache: false,
            url: "/inventory/product/docs",
            views: {
                'content': { templateUrl: "pg/inventory_product_docs.min.html?v=" + new Date() }
            }
        })
        .state("inventory_product_find", {
            cache: false,
            url: "/inventory/product/find",
            views: {
                'content': { templateUrl: "pg/inventory_product_find.min.html?v=" + new Date() }
            }
        })

        .state("inventory_product_detail", {
            cache: false,
            url: "/inventory/product/:guid",
            views: {
                'content': { templateUrl: "pg/inventory_product_detail.min.html?v=" + new Date() }
            }
        })

        .state("inventory_prodgroup", {
            cache: false,
            url: "/inventory/prodgroup",
            views: {
                'content': { templateUrl: "pg/inventory_prodgroup.min.html?v=" + new Date() }
            }
        })
        .state("inventory_proddisco", {
            cache: false,
            url: "/inventory/discontinued",
            views: {
                'content': { templateUrl: "pg/inventory_proddisco.min.html?v=" + new Date() }
            }
        })
        .state("inventory_type", {
            cache: false,
            url: "/inventory/type",
            views: {
                'content': { templateUrl: "pg/inventory_type.min.html?v=" + new Date() }
            }
        })
        .state("inventory_depot", {
            cache: false,
            url: "/inventory/depot",
            views: {
                'content': { templateUrl: "pg/inventory_depot.min.html?v=" + new Date() }
            }
        })
        .state("inventory_depot_list", {
            cache: false,
            url: "/inventory/depot/list",
            views: {
                'content': { templateUrl: "pg/inventory_depot_list.min.html?v=" + new Date() }
            }
        })
        .state("inventory_processing", {
            cache: false,
            url: "/inventory/processing",
            views: {
                'content': { templateUrl: "pg/inventory_processing.min.html?v=" + new Date() }
            }
        })
        .state("supplier", {
            cache: false,
            url: "/supplier",
            views: {
                'content': { templateUrl: "pg/r/supplier.min.html?v=" + new Date() }
            }
        })
        .state("inventory_pricelist", {
            cache: false,
            url: "/inventory/pricelist",
            views: {
                'content': { templateUrl: "pg/inventory_pricelist.min.html?v=" + new Date() }
            }
        })
        .state("inventory_production_stat", {
            cache: false,
            url: "/inventory/production/stat",
            views: {
                'content': { templateUrl: "pg/inventory_production_stat.min.html?v=" + new Date() }
            }
        })
        .state("inventory_machinery", {
            cache: false,
            url: "/inventory/machinery",
            views: {
                'content': { templateUrl: "pg/inventory_machinery.min.html?v=" + new Date() }
            }
        })

        .state("resource_depot", {
            cache: false,
            url: "/resource/depot",
            views: {
                'content': { templateUrl: "pg/resource_depot.min.html?v=" + new Date() }
            }
        })
        .state("resource_type", {
            cache: false,
            url: "/resource/type",
            views: {
                'content': { templateUrl: "pg/resource_type.min.html?v=" + new Date() }
            }
        })
        .state("resource", {
            cache: false,
            url: "/resource",
            views: {
                'content': { templateUrl: "pg/resource.min.html?v=" + new Date() }
            }
        })
        .state("ticket", {
            cache: false,
            url: "/ticket",
            views: {
                'content': { templateUrl: "pg/ticket.min.html?v=" + new Date() }
            }
        })

        .state("analysis_db", {
            cache: false,
            url: "/analysis/db",
            views: {
                'content': { templateUrl: "pg/a/analysis_db.min.html?v=" + new Date() }
            }
        })
        .state("analysis_ds", {
            cache: false,
            url: "/analysis/ds",
            views: {
                'content': { templateUrl: "pg/a/analysis_ds.min.html?v=" + new Date() }
            }
        })
        .state("analysis_history_db", {
            cache: false,
            url: "/analysis/history/db",
            views: {
                'content': { templateUrl: "pg/a/dbanalysis_history.min.html?v=" + new Date() }
            }
        })
        .state("analysis_history_ds", {
            cache: false,
            url: "/analysis/history/ds",
            views: {
                'content': { templateUrl: "pg/a/dsanalysis_history.min.html?v=" + new Date() }
            }
        })
        .state("analysis_compounds", {
            cache: false,
            url: "/analysis/compounds",
            views: {
                'content': { templateUrl: "pg/a/compounds.min.html?v=" + new Date() }
            }
        })
        .state("analysis_customer", {
            cache: false,
            url: "/analysis/customer",
            views: {
                'content': { templateUrl: "pg/a/analysis_customer.min.html?v=" + new Date() }
            }
        })
        .state("analysis_kit", {
            cache: false,
            url: "/analysis/kit",
            views: {
                'content': { templateUrl: "pg/a/analysis_kit.min.html?v=" + new Date() }
            }
        })




        .state("testmqtt", {
            cache: false,
            url: "/",
            views: {
                'content': { templateUrl: "pg/testmqtt.min.html?v=" + new Date() }
            }
        })

        .state("event", {
            cache: false,
            url: "/event",
            views: {
                'content': { templateUrl: "pg/event.min.html?v=" + new Date() }
            }
        })
        .state("video", {
            cache: false,
            url: "/video",
            views: {
                'content': { templateUrl: "pg/d/video.min.html?v=" + new Date() }
            }
        })
        .state("guest", {
            cache: false,
            url: "/guest",
            views: {
                'content': { templateUrl: "pg/guest.min.html?v=" + new Date() }
            }
        })
        .state("live", {
            cache: false,
            url: "/live",
            views: {
                'content': { templateUrl: "pg/live.min.html?v=" + new Date() }
            }
        })
        .state("command", {
            cache: false,
            url: "/command",
            views: {
                'content': { templateUrl: "pg/command.min.html?v=" + new Date() }
            }
        })
        .state("warning", {
            cache: false,
            url: "/warning",
            views: {
                'content': { templateUrl: "pg/warning.min.html?v=" + new Date() }
            }
        })

        .state("zone", {
            cache: false,
            url: "/zone",
            views: {
                'content': { templateUrl: "pg/d/zone.min.html?v=" + new Date() }
            }
        })
        //.state("camera", {
        //    cache: false,
        //    url: "/camera",
        //    views: {
        //        'content': { templateUrl: "pg/d/camera.min.html?v=" + new Date() }
        //    }
        //})
        //.state("relay", {
        //    cache: false,
        //    url: "/relay",
        //    views: {
        //        'content': { templateUrl: "pg/d/relay.min.html?v=" + new Date() }
        //    }
        //})
        //.state("access", {
        //    cache: false,
        //    url: "/access",
        //    views: {
        //        'content': { templateUrl: "pg/d/access.min.html?v=" + new Date() }
        //    }
        //})
        .state("device", {
            cache: false,
            url: "/device",
            views: {
                'content': { templateUrl: "pg/d/device.min.html?v=" + new Date() }
            }
        })

        .state("volumeconfig", {
            cache: false,
            url: "/volumeconfig",
            views: {
                'content': { templateUrl: "pg/c/volumeconfig.min.html?v=" + new Date() }
            }
        })
        .state("volume", {
            cache: false,
            url: "/volume",
            views: {
                'content': { templateUrl: "pg/c/volume.min.html?v=" + new Date() }
            }
        })
        .state("contact", {
            cache: false,
            url: "/contact",
            views: {
                'content': { templateUrl: "pg/c/contact.min.html?v=" + new Date() }
            }
        })
        .state("account", {
            cache: false,
            url: "/account",
            views: {
                'content': { templateUrl: "pg/c/account.min.html?v=" + new Date() }
            }
        })
        .state("restapi", {
            cache: false,
            url: "/restapi",
            views: {
                'content': { templateUrl: "pg/c/restapi.min.html?v=" + new Date() }
            }
        })
        .state("sms", {
            cache: false,
            url: "/sms",
            views: {
                'content': { templateUrl: "pg/c/sms.min.html?v=" + new Date() }
            }
        })
        .state("mqtt", {
            cache: false,
            url: "/mqtt",
            views: {
                'content': { templateUrl: "pg/c/mqtt.min.html?v=" + new Date() }
            }
        })

        .state("company", {
            cache: false,
            url: "/company",
            views: {
                'content': { templateUrl: "pg/r/company.min.html?v=" + new Date() }
            }
        })
        .state("functiongroup", {
            cache: false,
            url: "/functiongroup",
            views: {
                'content': { templateUrl: "pg/r/functiongroup.min.html?v=" + new Date() }
            }
        })

        //.state("mailaccounts", {
        //    cache: false,
        //    url: "/mailaccounts",
        //    views: {
        //        'content': { templateUrl: "pg/r/mailaccounts.min.html?v=" + new Date() }
        //    }
        //})
        .state("project", {
            cache: false,
            url: "/project",
            views: {
                'content': { templateUrl: "pg/r/project.min.html?v=" + new Date() }
            }
        })
        //.state("remotefs", {
        //    cache: false,
        //    url: "/remotefs",
        //    views: {
        //        'content': { templateUrl: "pg/r/remotefs.min.html?v=" + new Date() }
        //    }
        //})
        .state("user", {
            cache: false,
            url: "/user",
            views: {
                'content': { templateUrl: "pg/r/user.min.html?v=" + new Date() }
            }
        })
        .state("user_importer", {
            cache: false,
            url: "/user/importer",
            views: {
                'content': { templateUrl: "pg/r/user_importer.min.html?v=" + new Date() }
            }
        })
        .state("user_detail", {
            cache: false,
            url: "/user/:guid",
            views: {
                'content': { templateUrl: "pg/r/user_detail.min.html?v=" + new Date() }
            }
        })
        .state("global_contact", {
            cache: false,
            url: "/global/contact",
            views: {
                'content': { templateUrl: "pg/r/global_contact.min.html?v=" + new Date() }
            }
        })
        .state("usergroup", {
            cache: false,
            url: "/usergroup",
            views: {
                'content': { templateUrl: "pg/r/usergroup.min.html?v=" + new Date() }
            }
        })
        .state("syslog_entry", {
            cache: false,
            url: "/syslog/entry",
            views: {
                'content': { templateUrl: "pg/r/syslog_entry.min.html?v=" + new Date() }
            }
        })
        .state("syslog_filter", {
            cache: false,
            url: "/syslog/filter",
            views: {
                'content': { templateUrl: "pg/r/syslog_filter.min.html?v=" + new Date() }
            }
        })
        .state("log", {
            cache: false,
            url: "/log",
            views: {
                'content': { templateUrl: "pg/r/log.min.html?v=" + new Date() }
            }
        })
        .state("log_device", {
            cache: false,
            url: "/log/device",
            views: {
                'content': { templateUrl: "pg/r/log_device.min.html?v=" + new Date() }
            }
        })
        .state("app", {
            cache: false,
            url: "/app",
            views: {
                'content': { templateUrl: "pg/r/app.min.html?v=" + new Date() }
            }
        })
        .state("profile", {
            cache: false,
            url: "/profile/:guid",
            views: {
                'content': { templateUrl: "pg/u/profile.min.html?v=" + new Date() }
            }
        })

        .state("asset", {
            cache: false,
            url: "/asset",
            views: {
                'content': { templateUrl: "pg/s/asset.min.html?v=" + new Date() }
            }
        })

        .state("scenario", {
            cache: false,
            url: "/scenario",
            views: {
                'content': { templateUrl: "pg/d/scenario.min.html?v=" + new Date() }
            }
        })

        .state("bluetooth", {
            cache: false,
            url: "/bluetooth",
            views: {
                'content': { templateUrl: "pg/d/bt.min.html?v=" + new Date() }
            }
        })

        .state("monitoring", {
            cache: false,
            url: "/monitoring",
            views: {
                'content': { templateUrl: "pg/d/monitoring.min.html?v=" + new Date() }
            }
        })

        .state("managezone", {
            cache: false,
            url: "/zone/manage/:guid",
            views: {
                'content': { templateUrl: "pg/d/zone_manage.min.html?v=" + new Date() }
            }
        })

        .state("projectdetails", {
            cache: false,
            url: "/project/:guid",
            views: {
                'content': { templateUrl: "pg/r/project_details.min.html?v=" + new Date() }
            }
        })

        //.state("settings", {
        //    cache: false,
        //    url: "/settings",
        //    template: 'pg/settings.min.html',
        //    views: {
        //        'content': { templateUrl: "pg/u/profile.min.html?v=" + new Date() }
        //    }
        //})
        .state("report", {
            cache: false,
            url: "/report",
            views: {
                'content': { templateUrl: "pg/report.min.html?v=" + new Date() }
            }
        })
        .state("homologation_euro3", {
            cache: false,
            url: "/homologation/euro3",
            views: {
                'content': { templateUrl: "pg/sc/homologation_euro3.min.html?v=" + new Date() }
            }
        })
        .state("homologation_euro4", {
            cache: false,
            url: "/homologation/euro4",
            views: {
                'content': { templateUrl: "pg/sc/homologation_euro4.min.html?v=" + new Date() }
            }
        })
        .state("homologation_issuer", {
            cache: false,
            url: "/homologation/issuer",
            views: {
                'content': { templateUrl: "pg/sc/homologation_issuer.min.html?v=" + new Date() }
            }
        })

        .state("shop", {
            cache: false,
            url: "/shop",
            views: {
                'content': { templateUrl: "pg/ps/shop.min.html?v=" + new Date() }
            }
        })

        .state("vnc", {
            cache: false,
            url: "/vnc",
            views: {
                'content': { templateUrl: "pg/t/vnc.min.html?v=" + new Date() }
            }
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

app.directive("convertToBoolean", function () {
    return {
        require: "ngModel",
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (val) {
                return val.toLowerCase() === 'true' ? true : false;
            });
            ngModel.$formatters.push(function (val) {
                return val !== null ? "" + val : null;
            });
        }
    };
});

app.directive('scrollToBottom', function ($timeout) {
    return {
        scope: {
            scrollToBottom: "="
        },
        restrict: 'A',
        link: function (scope, element) {
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
});

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

app.directive('addressForm', function () {
    return {
        restrict: 'E',
        templateUrl: '/tpl/address_line.min.html', // markup for template
        scope: {
            address: '=' // allows data to be passed into directive from controller scope
        }
    };
});

app.factory('asyncHttp', function ($http) {
    var data = function (value) {
        return $http.get(value, { timeout: 3600000 });
    };
    return { data: data };
});

app.factory('asyncHttpPost', function ($http) {
    var data = function (value, data) {
        $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        return $http.post(value, data, { timeout: 3600000 });
    };
    return { data: data };
});

app.factory('asyncHttpUpload', function (Upload) {
    var data = function (value, data) {
        return Upload.upload({
            url: value,
            data: data,
            timeout: 3600000
        });
    };
    return { data: data };
});

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

//app.factory('authHttpResponseInterceptor', ['$q', '$location', function ($q, $location) {
//    return {
//        response: function (response) {
//            if (response.status === 401) {
//                console.log("Response 401");
//            }
//            return response || $q.when(response);
//        },
//        responseError: function (rejection) {
//            if (rejection.status === 401) {
//                console.log("Response Error 401", rejection);
//                $location.path('/login').search('returnTo', $location.path());
//            }
//            return $q.reject(rejection);
//        }
//    };
//}]);

//app.config(['$httpProvider', function ($httpProvider) {
//    //Http Intercpetor to check auth failures for xhr requests
//    $httpProvider.interceptors.push('authHttpResponseInterceptor');
//}]);

app.service('UnloadEventService', function () {
    this.Add = function () {
        if (window.addEventListener) {
            window.addEventListener("beforeunload", handleUnloadEvent);
        } else {
            window.attachEvent("onbeforeunload", handleUnloadEvent);
        }
    };

    function handleUnloadEvent(event) {
        event.returnValue = "You are leaving the page!";
    }

    this.Remove = function () {
        if (window.removeEventListener) {
            window.removeEventListener("beforeunload", handleUnloadEvent);
        } else {
            window.detachEvent("onbeforeunload", handleUnloadEvent);
        }
    };
});

app.service('UploadService', ['FileUploader', function (FileUploader) {
    this.Uploader = function (config) {
        var uploader = new FileUploader(config);
        //uploader.filters.push({
        //    name: 'syncFilter',
        //    fn: function (item, options) {
        //        return this.queue.length < 10;
        //    }
        //});
        //uploader.filters.push({
        //    name: 'asyncFilter',
        //    fn: function (item, options, deferred) {
        //        setTimeout(deferred.resolve, 1e3);
        //    }
        //});
        return uploader;
    };

    //// CALLBACKS
    //uploader.onWhenAddingFileFailed = function (item, filter, options) { };
    //uploader.onAfterAddingFile = function (fileItem) { };
    //uploader.onAfterAddingAll = function (addedFileItems) { };
    //uploader.onBeforeUploadItem = function (item) {
    //    item.formData = [
    //        { 'data': data }
    //    ];
    //};
    //uploader.onProgressItem = function (fileItem, progress) { };
    //uploader.onProgressAll = function (progress) { };
    //uploader.onSuccessItem = function (fileItem, response, status, headers) { };
    //uploader.onErrorItem = function (fileItem, response, status, headers) { };
    //uploader.onCancelItem = function (fileItem, response, status, headers) { };
    //uploader.onCompleteItem = function (fileItem, response, status, headers) { };
    //uploader.onCompleteAll = function (fileItem, response, status, headers) { };
}]);

app.service('TableService', [function () {

    this.TblBegin = 0;
    this.TblSpan = 20;

    this.TblNavBegin = function () {
        this.TblBegin = 0;
    };

    this.TblNavPrev = function () {
        var newBegin = this.TblBegin - this.TblSpan;
        this.TblBegin = newBegin < 0 ? 0 : newBegin;
    };

    this.TblNavEnd = function (max) {
        this.TblBegin = max - this.TblSpan;
    };

    this.TblNavNext = function (max) {
        var newBegin = this.TblBegin + this.TblSpan;
        this.TblBegin = newBegin > max ? max - this.TblSpan : newBegin;
    };

    this.TblCeil = function (n) {
        return Math.ceil(n);
    }

}]);

app.service('LoadingService', [function () {

    this.IsLoading = false;

}]);

app.filter('range', function () {
    return function (input, total) {
        total = parseInt(total);

        for (var i = 0; i < total; i++) {
            input.push(i);
        }

        return input;
    };
});

app.filter('hrsize', function () {
    return function (input, si) {
        input = parseInt(input);
        var thresh = si ? 1000 : 1024;
        if (Math.abs(input) < thresh) {
            return input + ' B';
        }
        var units = si
            ? ['kB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB']
            : ['KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB'];
        var u = -1;
        do {
            input /= thresh;
            ++u;
        } while (Math.abs(input) >= thresh && u < units.length - 1);
        return input.toFixed(1) + ' ' + units[u];
    };
});

app.filter('numberFixedLen', function () {
    return function (a, b) {
        return (1e4 + "" + a).slice(-b);
    };
});

app.controller("CookieController", ["$scope", "$window", "$cookies", "HttpService", "$pageTitle", CookieController]);

function CookieController($scope, $window, $cookies, HttpService, $pageTitle) {
    $scope.showCookieMessage = false;

    $scope.manageCookie = function () {
        var cookieInfoCookie = $cookies.get('hoplite_cookie_notice_accepted');
        if (cookieInfoCookie !== "true") {
            $scope.showCookieMessage = true;
        }
        else {
            $scope.showCookieMessage = false;
        }
    };
    $scope.manageCookie();

    $scope.acceptCookie = function () {
        var expiration = moment().add(30, 'days').toDate();
        $cookies.put('hoplite_cookie_notice_accepted', true, { 'expires': expiration });
        $scope.manageCookie();
    };

    $scope.cookiePolicyInfo = function () {
        $window.location.href = "/policy/cookie";
    };

    $scope.loadPageTitle = function () {
        HttpService.GET("/app/name").then(function (r) {
            var data = r.data;
            $pageTitle.set(data);
        }).catch(function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { notificationService.error('Error! ' + r.data.message); } else { notificationService.error('Error!'); } }
        });
    };
    $scope.loadPageTitle();
}

app.controller("LoginController", ["$scope", "HttpService", "$window", "$cookies", "notificationService", LoginController]);

function LoginController($scope, HttpService, $window, $cookies, notificationService) {

    $scope.Username = "";
    $scope.Password = "";

    $scope.UserExists = false;

    $scope.VerifyUsername = function () {
        $scope.UserExists = $scope.Username.length > 0;
    };

    //function sha(input) {
    //    console.info("i", input);
    //    var shaObj = new jsSHA("SHA-256", "TEXT");
    //    shaObj.update(input);
    //    var hash = shaObj.getHash("HEX");
    //    console.info("h", hash);
    //    return hash;
    //}

    $scope.submitForm = function () {
        if (!$scope.doSubmit) {
            return;
        }
        $scope.doSubmit = false;
        //console.log($scope.Password);
        //var pwd = sha($scope.Password);
        //console.log(pwd);
        var data = $.param({
            Username: $scope.Username,
            Password: $scope.Password
        });
        HttpService.POST("/login", data).then(function () {
            notificationService.success('Ok');
        }, function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { notificationService.error('Error! ' + r.data.message); } else { notificationService.error('Error!'); } }
        });
    };

    $scope.ShowInstall = false;
    $scope.ShowInstallButton = false;

    $scope.checkStatus = function () {
        HttpService.GET("/install/status").then(function (r) {
            var data = r.data;
            if (data < 0) {
                $scope.ShowInstallButton = true;
            }
        }).catch(function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { notificationService.error('Error! ' + r.data.message); } else { notificationService.error('Error!'); } }
        });
    };
    $scope.checkStatus();

    $scope.submitInstall = function () {
        var data = $.param({
            Key: $scope.Install.Key,
            Vector: $scope.Install.Vector,
            OrganizationName: $scope.Install.OrganizationName,
            AdministratorFirstName: $scope.Install.AdministratorFirstName,
            AdministratorLastName: $scope.Install.AdministratorLastName,
            AdministratorEmail: $scope.Install.AdministratorEmail,
            Password: $scope.Install.Password
        });
        HttpService.POST("/install", data).then(function () {
            notificationService.success('Ok');
            $window.location.href = "/";
        }, function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { notificationService.error('Error! ' + r.data.message); } else { notificationService.error('Error!'); } }
        });
    };

    $scope.showEulaMessage = false;

    $scope.manageEulaCookie = function () {
        var cookieInfoCookie = $cookies.get('hoplite_eula_accepted');
        if (cookieInfoCookie !== "true") {
            $scope.showEulaMessage = true;
        }
        else {
            $scope.showEulaMessage = false;
        }
    };
    $scope.manageEulaCookie();

    $scope.acceptEula = function () {
        var expiration = moment().add(30, 'days').toDate();
        $cookies.put('hoplite_eula_accepted', true, { 'expires': expiration });
        $scope.manageEulaCookie();
    };

    $scope.viewEula = function () {
        $window.location.href = "/eula";
    };

    $scope.slider = {
        minValue: 0,
        maxValue: 9,
        options: {
            floor: 0,
            ceil: 9,
            step: 1,
            showTicks: false,
            labelOverlapSeparator: '',
            translate: function () {
                return '';
            }
        }
    };

    $scope.CaptchaData = [];
    $scope.CaptchaReady = false;
    $scope.HintUrl = '';

    $scope.load = function () {
        $scope.CaptchaData = [];
        $scope.CaptchaReady = false;
        HttpService.GET("/captcha/session_request").then(function (r) {
            var data = r.data;
            $scope.HintUrl = "/captcha/session_hint";
            $scope.CaptchaData = data.data;
            $scope.CaptchaReady = true;
        }).catch(function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { notificationService.error('Error! ' + r.data.message); } else { notificationService.error('Error!'); } }
        });
    };

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
        HttpService.POST("/captcha/session_verify", data).then(function () {
            notificationService.success('Ok');
        }, function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { notificationService.error('Error! ' + r.data.message); } else { notificationService.error('Error!'); } }
        });
    };
}

app.controller("MinosController", ["$scope", "HttpService", "$window", "$timeout", MinosController]);

function MinosController($scope, HttpService, $window, $timeout) {
    $scope.UserEmail = "";
    $scope.ResetCode = "";
    $scope.ShowResetPassword = false;
    $scope.ShowResetPasswordStep = 0;
    $scope.UserNP = "";
    $scope.UserNPC = "";

    $scope.minosSendResetWip = false;
    $scope.minosSendReset = function () {
        $scope.minosSendResetWip = true;
        var data = $.param({
            Email: $scope.UserEmail
        });
        HttpService.POST("/minos/sendreset", data).then(function () {
            //$scope.ShowResetPasswordStep = 1;
            //$scope.minosSendResetWip = false;
        }, function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { console.log('Error! ' + r.data.message); } else { console.log('Error!'); } }
            //$scope.minosSendResetWip = false;
        });
        $timeout(function () {
            $scope.ShowResetPasswordStep = 1;
            $scope.minosSendResetWip = false;
        }, 2500);
    };

    $scope.minosFetchCodeWip = false;
    $scope.minosFetchCode = function () {
        $scope.minosFetchCodeWip = true;
        var data = $.param({
            Code: $scope.ResetCode
        });
        HttpService.POST("/minos/fetchcode", data).then(function () {
            $scope.ShowResetPasswordStep = 2;
            $scope.minosFetchCodeWip = false;
        }, function (r) {
            $scope.ShowResetPasswordStep = -1;
            if (r.status > 0) { console.log(r); if (r.data.message) { console.log('Error! ' + r.data.message); } else { console.log('Error!'); } }
            $scope.minosFetchCodeWip = false;
        });
    };

    $scope.minosResetPasswordWip = false;
    $scope.minosResetPassword = function () {
        $scope.minosResetPasswordWip = true;
        var data = $.param({
            Code: $scope.ResetCode,
            NewPassword: $scope.UserNP
        });
        HttpService.POST("/minos/resetpassword", data).then(function () {
            $scope.ShowResetPasswordStep = 3;
            $scope.minosResetPasswordWip = false;
        }, function (r) {
            $scope.ShowResetPasswordStep = -1;
            if (r.status > 0) { console.log(r); if (r.data.message) { console.log('Error! ' + r.data.message); } else { console.log('Error!'); } }
            $scope.minosResetPasswordWip = false;
        });
    };

    $scope.minosDone = function () {
        $window.location.href = "/";
    };
}

app.controller("AuthenticationController", ["$scope", "HttpService", "$window", "authenticationService", AuthenticationController]);

function AuthenticationController($scope, HttpService, $window, authenticationService) {

    $scope.UserName = "";
    $scope.UserGuid = "";

    $scope.Login = function () {
        if ($scope.UserName.length > 0) {
            return;
        }
        HttpService.GET("/login/user").then(function (r) {
            var data = r.data;
            if (data === null) {
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
        }, function (r) {
            console.log("session expired: " + r);
            $window.location.href = "/logout";
        });
    };
    $scope.Login();

    $scope.AclHide = function (functionCode) {
        if (functionCode === "FFFFFFFF") { return true; }
        var hasAcl = false;
        while ($scope.AuthenticatedUser !== null && $scope.AuthenticatedUser !== undefined) {
            hasAcl = $scope.AuthenticatedUser.FunctionCodes.indexOf(functionCode) > -1;
            break;
        }
        return hasAcl;
    };

    $scope.LockScreenHidden = true;

    $scope.ShowLicMesg = false;
    $scope.LicMesgLeft = 50;

    $scope.checkLicStatus = function () {
        HttpService.GET("/install/status").then(function (r) {
            var data = r.data;
            if (data < 2) {
                $scope.ShowLicMesg = true;
            }
        }).catch(function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { notificationService.error('Error! ' + r.data.message); } else { notificationService.error('Error!'); } }
        });
    };
    $scope.checkLicStatus();

    $scope.closeLicMesg = function () {
        $scope.ShowLicMesg = true;
        var n = Math.round(Math.random() * 100);
        var np = $scope.LicMesgLeft + n;
        if (np > 300) {
            $scope.LicMesgLeft = 50;
        }
        else {
            $scope.LicMesgLeft = np;
        }
    };
}

app.controller("LanguageSwitchController", ["$scope", "$rootScope", "$translate", "$cookies", LanguageSwitchController]);

function LanguageSwitchController($scope, $rootScope, $translate, $cookies) {

    $scope.SelectedLanguage = "it";

    if ($cookies.get("lang") === null) {
        $translate.use("it");
    }
    else {
        var lang = $cookies.get("lang");
        $translate.use(lang);
    }

    $scope.slang = function () {
        if ($cookies.get("lang") !== null) {
            var lang = $cookies.get("lang");
            $scope.SelectedLanguage = lang;
        }
    };
    $scope.slang();

    $scope.changeLanguage = function (langKey) {
        $translate.use(langKey);
        $scope.SelectedLanguage = langKey;
    };

    $rootScope.$on("$translateChangeSuccess", function (event, data) {
        var language = data.language;
        $rootScope.lang = language;
        var now = new Date();
        var exp = new Date(now.getFullYear() + 1, now.getMonth(), now.getDate());
        $cookies.put("lang", $rootScope.lang, { 'expires': exp });
    });
}

app.controller("NavbarController", ["$scope", "HttpService", "$interval", "notificationService", "$cookies", "$state", NavbarController]);

function NavbarController($scope, HttpService, $interval, notificationService, $cookies, $state) {
    $scope.LastState = "home";
    $scope.getLastState = function () {
        var s = $cookies.get("last_state");
        if (s) {
            if (s === $state.current.name) {
                $scope.LastState = "home";
            } else {
                $scope.LastState = s;
            }
        }
    };
    $scope.getLastState();

    $scope.LastNotifications = [];
    $scope.NotificationsCount = 0;

    $scope.getNofications = function () {
        HttpService.GET("/repo/notification/last").then(function (r) {
            var data = r.data;
            $scope.LastNotifications = data.List;
            $scope.NotificationsCount = data.Count;
        }).catch(function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { notificationService.error('Error! ' + r.data.message); } else { notificationService.error('Error!'); } }
        });
    };
    $scope.getNofications();

    $scope.readThisNofitication = function (el) {
        var data = $.param({
            Guid: el.Guid
        });
        HttpService.POST("/repo/notification/read", data).then(function () {
            $state.go(el.Context);
            $scope.getNofications();
        }, function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { notificationService.error('Error! ' + r.data.message); } else { notificationService.error('Error!'); } }
        });
    };

    $scope.readAllNofitication = function () {
        HttpService.POST("/repo/notification/read/all").then(function () {
            notificationService.success('Ok');
            $scope.getNofications();
        }, function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { notificationService.error('Error! ' + r.data.message); } else { notificationService.error('Error!'); } }
        });
    };

    $scope.date = {
        ServerTime: new Date(),
        UserTime: new Date()
    };

    $scope.getDate = function () {
        HttpService.GET("/time").then(function (r) {
            $scope.date = r.data;
        }, function (r) {
            if (r.status > 0) { console.log(r); if (r.data.message) { notificationService.error('Error! ' + r.data.message); } else { notificationService.error('Error!'); } }
        });
    };
    $scope.getDate();

    $scope.updateTime = function () {
        var st = moment($scope.date.ServerTime);
        if (st.isValid()) {
            $scope.date.ServerTime = st.add(30, 'seconds').toDate();
        }
        else {
            console.log("st non valid");
        }
        var ut = moment($scope.date.UserTime);
        if (ut.isValid()) {
            $scope.date.UserTime = ut.add(30, 'seconds').toDate();
        }
        else {
            console.log("ut non valid");
        }
    };

    $interval(function () {
        $scope.updateTime();
        //$scope.getNofications();
    }, 30000);
}

app.controller("SidebarController", ["$scope", "HttpService", "$cookies", SidebarController]);

function SidebarController($scope, HttpService, $cookies) {

    $scope.Menu = [];
    $scope.GetMenu = function () {
        HttpService.GET("/menu").success(function (data) {
            $scope.Menu = data;
        });
    };
    $scope.GetMenu();

    $scope.itemClicked = function (index, elements) {
        for (var i = 0; i < elements.length; i++) {
            elements[i].ActiveClass = '';
        }
        elements[index] = "active";
    };

    $scope.setState = function (lastState) {
        $cookies.put("last_state", lastState);
    };
}

app.controller("InstallController", ["$scope", "HttpService", "$interval", InstallController]);

function InstallController($scope, HttpService, $interval) {
    $scope.CreateAlias = function () {
        if ($scope.Data.AdministratorFirstName.length > 3 && $scope.Data.AdministratorLastName.length > 3) {
            var alias = $scope.Data.AdministratorLastName.substring(0, 3) + $scope.Data.AdministratorFirstName.substring(0, 3) + "01";
            $scope.Data.AdministratorAlias = alias.toLowerCase();
        }
    };

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
    };

    $scope.Install = function (el) {
        var data = $.param({
            OrganizationName: $scope.Data.OrganizationName,
            AdministratorFirstName: $scope.Data.AdministratorFirstName,
            AdministratorLastName: el.AdministratorLastName,
            AdministratorEmail: el.AdministratorEmail,
            Password: el.Password
        });
        HttpService.POST("/install", data).then(function () {
            $window.location.href = "/";
        }, function (response) {
            console.log(response);
        });
    };
}