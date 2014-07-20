'use strict';
app.factory('authService', ['$http', '$q', 'localStorageService', 'settingsService', 'webApiValidationService', function ($http, $q, localStorageService, settings, webApiValidationService) {

    var serviceBase = settings.remoteServer;
    var authServiceFactory = {
        authentication: {
            isAuth: false,
            userName: ""
        }
    };
    authServiceFactory.logOut = function () {
        localStorageService.remove('authorizationData');
        authServiceFactory.authentication.isAuth = false;
        authServiceFactory.authentication.userName = "";
    };
    authServiceFactory.fillAuthData = function() {
        var authData = localStorageService.get('authorizationData');
        if (authData) {
            authServiceFactory.authentication.isAuth = true;
            authServiceFactory.authentication.userName = authData.userName;
        }
    }
    authServiceFactory.saveRegistration = function (registration) {
        authServiceFactory.logOut();
        var deferred = $q.defer();
        $http.post(serviceBase + 'api/account/register', registration, { withCredentials: true }).then(function (response) {
            deferred.resolve(webApiValidationService.handleSuccess(response));
        },
        function(error) {
            deferred.reject(webApiValidationService.handleError(error));
        });
        return deferred.promise;
    };
    authServiceFactory.login = function (loginData) {
        authServiceFactory.logOut();
        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

        var deferred = $q.defer();

        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {
            localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName });
            authServiceFactory.authentication.isAuth = true;
            authServiceFactory.authentication.userName = loginData.userName;
            deferred.resolve(webApiValidationService.handleSuccess(response));

        }).error(function (err) {
            authServiceFactory.logOut();
            deferred.reject(webApiValidationService.handleError(err));
        });
        return deferred.promise;
    };

    authServiceFactory.defaultModel = webApiValidationService.defaultModel;

    return authServiceFactory;
}]);