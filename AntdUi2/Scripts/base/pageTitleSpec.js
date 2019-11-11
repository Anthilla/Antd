describe('ngPageTitle', function () {
    var DEFAULT_TITLE = 'Foo Bar',
        $window, provider, $pageTitle;

    beforeEach(module('ngPageTitle'));

    beforeEach(function () {
        angular.module('testPageTitleMdl', ['ngRoute', 'ngPageTitle'])
            .config(function ($routeProvider, $pageTitleProvider) {
                provider = $pageTitleProvider;
                $pageTitleProvider.setDefault(DEFAULT_TITLE);
                $routeProvider
                    .when('/test/default/title', {})
                    .when('/test/custom/title', {
                        pageTitle: 'Custom route title'
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
                console.log($window.document.title)
                expect($pageTitle.get()).toBe($window.document.title);
            });
        });

        describe('#set(value)', function () {
            it('should be defined', function () {
                expect($pageTitle.set).toBeDefined();
                expect(typeof $pageTitle.set).toBe('function');
            });

            it('should set the document title', function () {
                $pageTitle.set('Test');
                expect($pageTitle.get()).toBe('Test');
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
        });
    });
});
