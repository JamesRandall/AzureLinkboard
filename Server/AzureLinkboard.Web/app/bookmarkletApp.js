String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

var linkboardControllers = angular.module('bookmarkletControllers', []);
var app = angular.module('BookmarkletApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar', 'bookmarkletControllers']);

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);

app.config(function($locationProvider) {
    $locationProvider.html5Mode(true);
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});