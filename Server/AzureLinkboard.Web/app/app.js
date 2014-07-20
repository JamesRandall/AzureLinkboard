String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

var linkboardControllers = angular.module('linkboardControllers', []);
var app = angular.module('AzureLinkboardApp', ['ngRoute',
    'LocalStorageModule',
    'angular-loading-bar',
    'infinite-scroll',
    'afCustomInputs',
    'linkboardControllers',
    'afServerValidation']);

var requireAuthentication = function() {
    return [
        'authService', '$location', '$q', function(authService, $location, $q) {

            var deferred = $q.defer();
            deferred.resolve();
            if (authService.authentication.isAuth) {
                return deferred.promise;
            } else {
                $location.path('/login');
                return $q.reject('Not authenticated');
            }
        }
    ];
};

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);

app.config(['$routeProvider', function ($routeProvider) {

    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "/app/views/home.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

    $routeProvider.when("/signup", {
        controller: "signupController",
        templateUrl: "/app/views/signup.html"
    });

    $routeProvider.when("/links", {
        controller: "linkFeedController",
        templateUrl: "/app/views/linkFeed.html",
        resolve: requireAuthentication()
    });

    $routeProvider.when("/settings", {
        controller: "settingsController",
        templateUrl: "/app/views/settings.html"
    });

    $routeProvider.when("/tag/:tag", {
        controller: "tagController",
        templateUrl: "/app/views/tag.html",
        resolve: requireAuthentication()
    });

    $routeProvider.otherwise({ redirectTo: "/home" });
}]);

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.config(function ($compileProvider) {
    $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|ftp|mailto|file|javascript):/);
});