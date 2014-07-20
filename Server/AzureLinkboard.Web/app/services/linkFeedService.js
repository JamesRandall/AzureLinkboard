'use strict';
app.factory('linkFeedService', ['$http', '$q', 'settingsService', 'afWebApiValidationService', function ($http, $q, settings, webApiValidationService) {

    var serviceBase = settings.remoteServer;

    return {
        getPage: function(continuationToken) {
            var queryUrl = serviceBase + 'api/url';
            if (continuationToken !== undefined) {
                queryUrl = queryUrl + "?continuationToken=" + encodeURIComponent(continuationToken);
            }
            return $http.get(queryUrl).then(function(results) {
                return results;
            });
        },
        saveLink: function(link) {
            var queryUrl = serviceBase + 'api/url';
            var deferred = $q.defer();
            $http.post(queryUrl, link).then(function(results) {
                deferred.resolve(webApiValidationService.handleSuccess(results));
            }, function(error) {
                deferred.reject(webApiValidationService.handleError(error));
            });
            return deferred.promise;
        },
        defaultModel: function() {
            return webApiValidationService.defaultModel();
        }
    };
}]);