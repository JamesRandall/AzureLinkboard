String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

var app = angular.module('BookmarkletApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar']);

/*app.config(function ($routeProvider) {

    $routeProvider.when("/bookmarklet", {
        controller: "bookmarkletController",
        templateUrl: "/app/views/bookmarklet.html"
    });

    $routeProvider.otherwise({ redirectTo: "/bookmarklet" });
});*/

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);

app.config(function($locationProvider) {
    $locationProvider.html5Mode(true);
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

/*app.run(['$rootScope', '$location', 'authService', function($rootScope, $location, authService) {
    $rootScope.$on("$locationChangeStart", function (event, next, current) {
        var root = $location.url('/').absUrl().substring(0, $location.url('/').absUrl().length - 2);
        if (authService.authentication.isAuth && (next.endsWith('/home') || next == root)) {
            $location.path('/links');
        }
    });
}]);*/

