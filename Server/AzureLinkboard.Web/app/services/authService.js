'use strict';
app.factory('authService', ['$http', '$q', 'localStorageService', 'settingsService', 'webApiValidationService', function ($http, $q, localStorageService, settings, webApiValidationService) {

    var serviceBase = settings.remoteServer;
    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        userName: ""
    };

    var _saveRegistration = function (registration) {

        _logOut();
        var deferred = $q.defer();
        $http.post(serviceBase + 'api/account/register', registration, { withCredentials: true }).then(function (response) {
            deferred.resolve(webApiValidationService.handleSuccess(response));
        },
        function(error) {
            deferred.reject(webApiValidationService.handleError(error));
        });
        return deferred.promise;
    };

    var _login = function (loginData) {
        _logOut();
        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

        var deferred = $q.defer();

        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

            localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName });

            _authentication.isAuth = true;
            _authentication.userName = loginData.userName;

            deferred.resolve(webApiValidationService.handleSuccess(response));

        }).error(function (err, status) {
            _logOut();
            deferred.reject(webApiValidationService.handleError(err));
            //deferred.reject(err);
        });

        return deferred.promise;

    };

    var _logOut = function () {

        localStorageService.remove('authorizationData');

        _authentication.isAuth = false;
        _authentication.userName = "";

    };

    var _fillAuthData = function () {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
        }

    }

    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    authServiceFactory.defaultModel = webApiValidationService.defaultModel;

    return authServiceFactory;
}]);