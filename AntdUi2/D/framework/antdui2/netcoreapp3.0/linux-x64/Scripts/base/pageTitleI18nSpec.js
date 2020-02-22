describe('ngPageTitleI18n', function () {
    var DEFAULT_TITLE = 'Foo Bar',
        $window, provider, $pageTitle;

    beforeEach(module('ngPageTitleI18n'));

    beforeEach(function () {
        angular.module('testPageTitleMdl', ['ngRoute', 'ngPageTitleI18n'])
            .config(function ($routeProvider, $i18nextProvider, $pageTitleProvider) {
                $i18nextProvider.options = {
                    resStore: {
                        'dev': {
                            'translation': {
                                'title': {
                                    'default': DEFAULT_TITLE,
                                    'route': 'Custom route title',
                                    'variable': '__varName__ title'
                                }
                            }
                        }
                    }
                };

                provider = $pageTitleProvider;
                $pageTitleProvider.setDefault('title.default');

                $routeProvider
                    .when('/test/default/title', {})
                    .when('/test/custom/title', {
                        pageTitle: 'title.route'
                    })
                    .when('/test/promise/title', {
                        pageTitle: 'title.variable',
                        resolve: {
                            data: function ($q, $pageTitle) {
                                var deferred = $q.defer();
                                deferred.resolve('Variable');
                                return deferred.promise.then(function (data) {
                                    $pageTitle.update({ varName: data });
                                });
                            }
                        }
                    });
            });
        // Load the test module.
        module('testPageTitleMdl');
    });

    beforeEach(inject(function (_$window_, _$pageTitle_) {
        $window = _$window_;
        $pageTitle = _$pageTitle_;
    }));

    describe('$pageTitleProvider', function () {
        it('should be defined', function () {
            expect(provider).toBeDefined();
        });

        describe('#setDefault(value)', function () {
            it('should be defined', function () {
                expect(provider.setDefault).toBeDefined();
                expect(typeof provider.setDefault).toBe('function');
            });
        });
    });

    describe('$pageTitle', function () {
        it('should be defined', function () {
            expect($pageTitle).toBeDefined();
        });

        it('should set the default title to the document', function () {
            expect($window.document.title).toBe(DEFAULT_TITLE);
        });

        describe('#get()', function () {
            it('should be defined', function () {
                expect($pageTitle.get).toBeDefined();
                expect(typeof $pageTitle.get).toBe('function');
            });

            it('should return the current document title', function () {
                expect($pageTitle.get()).toBe($window.document.title);
            });
        });

        describe('#set(value, variables)', function () {
            it('should be defined', function () {
                expect($pageTitle.set).toBeDefined();
                expect(typeof $pageTitle.set).toBe('function');
            });

            it('should set the document title', function () {
                $pageTitle.set('Test');
                expect($pageTitle.get()).toBe('Test');
            });

            it('should replace variables', function () {
                $pageTitle.set('__myVar__', { myVar: 'Test' });
                expect($pageTitle.get()).toBe('Test');
            });
        });

        describe('#update(variables)', function () {
            it('should be defined', function () {
                expect($pageTitle.update).toBeDefined();
                expect(typeof $pageTitle.update).toBe('function');
            });
        });

        describe('on $routeChangeSuccess event', function () {
            var navigateTo;

            beforeEach(inject(function ($location, $route, $rootScope) {
                navigateTo = function (path) {
                    $location.path(path);
                    $route.reload();
                    $rootScope.$apply();
                }
            }));

            it('should change the page title according to route config', function () {
                navigateTo('/test/custom/title');
                expect($pageTitle.get()).toBe('Custom route title');
            });

            it('should use the default if no page title is defined on route', function () {
                navigateTo('/test/default/title');
                expect($pageTitle.get()).toBe(DEFAULT_TITLE);
            });

            it('should change page title after resolving variables', function () {
                navigateTo('/test/promise/title');
                expect($pageTitle.get()).toBe('Variable title');
            });
        });
    });
});
