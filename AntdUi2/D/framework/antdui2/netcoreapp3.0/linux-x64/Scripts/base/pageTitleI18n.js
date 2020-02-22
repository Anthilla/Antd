/**
 * $pageTitle
 * ==========
 *
 * Allows to control and localize the page title from the AngularJS route
 * system, controllers or any other component through an injectable service.
 *
 * ### ngPageTitleI18n - Page title service with internationalization support
 *
 * The iternationalization support is provided by the
 * [ng-i18next](https://github.com/archer96/ng-i18next), and all titles will be
 * parsed through the plugin before been set to the document, read the
 * [i18next documentation](http://i18next.com/pages/doc_features.html) to
 * understand all supported features.
 *
 * To get started add the module to your app and configure the page title
 * provider:
 *
 * ```js
 * angular.module('app', ['ngPageTitleI18n'])
 * .config(function($pageTitleProvider) {
 *   // Use the current doc title
 *   $pageTitleProvider.setDefaultTitle();
 *   // Set a custom default title
 *   $pageTitleProvider.setDefaultTitle('default.title');
 * });
 * ```
 *
 * Then setup the `$pageTitle` property along your routes:
 *
 * ```js
 * $routeProvider.when('path/to', {
 *   // ...
 *   pageTitle: 'path.title'
 * });
 * ```
 *
 * The document title will automatically update every time the route changes
 * with success.
 *
 * It's also possible to change the routes within any component that supports
 * injection through the `$pageTitle` service, with the `set`/`update` method:
 *
 * ```js
 * function MyAppCtrl($pageTitle) {
 *   $pageTitle.set('path.title');
 * }
 * ```
 *
 * The `$pageTitle` must be a valid i18next string to allow correct localization,
 * as the following JSON:
 *
 * ```json
 * {
 *   "path": {
 *     "title": "My path title",
 *     "variableTitle": "__varName__ path title"
 *   }
 * }
 * ```
 *
 * There's also the possibility to interpolate variables through the service,
 * we can set the value of `varName` giving `set` an variables object as the
 * second parameter.
 *
 * ```js
 * function MyAppCtrl($pageTitle) {
 *   $pageTitle.set('path.variableTitle', {varName: 'Variable'});
 *   // => Variable path title
 * }
 * ```
 *
 * Or just `update` the current title, if already been set within the route.
 * This is specially usefull when you have to resolve a promise before set the
 * title.
 * Notice that for the `update` method only the variables object is required.
 *
 * ```js
 * $routeProvider.when('path/to', {
 *   // ...
 *   pageTitle: 'path.title',
 *   resolve: {
 *     data: function($http, $pageTitle) {
 *       // Updates the page title after promise resolved
 *       return $http.get('/content/1.json')
 *         .success(function(data) {
 *           $pageTitle.update({varName: data.title});
 *         });
 *     }
 *   }
 * });
 * ```
 *
 * The route will be update only when the route is changed with success, so we
 * won't show non replaced title.
 */
angular.module('ngPageTitleI18n', ['ngRoute', 'jm.i18next'])
    .provider('$pageTitle', function () {
        var defaultTitle;

        /**
         * Sets the default title value, if none given it will use the document title
         * as the default, it must be called uppon config to allow the bindinds to the
         * route system.
         * @param {String} [value] - The default title value.
         */
        this.setDefault = function (value) {
            defaultTitle = value;
        };

        /**
         * @name $pageTitle
         * @requires $rootScope
         * @requires $window
         * @requires $i18next
         *
         * @description
         * The `$pageTitle` with i18n support service factory.
         *
         * ### Methods:
         *  - $pageTitle.get()
         *  - $pageTitle.set(value, variables)
         *  - $pageTitle.update(variables)
         */
        function PageTitleI18nFactory($rootScope, $window, $i18next) {
            var _currentTitle,
                _currentVariables = {};

            /**
             * Returns the current document title.
             * @return {string}
             */
            function _get() {
                return $window.document.title;
            }

            /**
             * Sets the document title, replacing the variable if has any.
             * @param {String} title - The i18n key.
             * @param {Object} [variables] - The variables object.
             */
            function _set(value, variables) {
                _currentTitle = value || defaultTitle;
                _update(variables || _currentVariables);
            }

            /**
             * Interpolates new set of variables in the current page title.
             * @param {Object} variables - The variables object.
             */
            function _update(variables) {
                _currentVariables = variables || {};
                $window.document.title = $i18next(_currentTitle, _currentVariables);
            }

            // Set up the default title to the document, if none provide use the current
            // document title as the default title.
            if (defaultTitle) {
                _set(defaultTitle);
            } else {
                defaultTitle = $window.document.title;
            }

            // Bind to angular route system.
            $rootScope.$on('$routeChangeSuccess', function (event, route) {
                var _pageTitle;
                if (route && angular.isDefined(route.$$route)) {
                    _pageTitle = route.$$route.pageTitle || null;
                }
                _set(_pageTitle);
            });

            return {
                get: _get,
                set: _set,
                update: _update
            }
        }

        this.$get = ['$rootScope', '$window', '$i18next', PageTitleI18nFactory];
    });
