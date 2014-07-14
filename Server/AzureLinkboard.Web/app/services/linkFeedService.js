'use strict';
app.factory('linkFeedService', ['$http', 'settingsService', function ($http, settings) {

    var serviceBase = settings.remoteServer;

    return {
        getPage: function (continuationToken) {
            var queryUrl = serviceBase + 'api/url';
            if (continuationToken !== undefined) {
                queryUrl = queryUrl + "?continuationToken=" + encodeURIComponent(continuationToken);
            } 
            return $http.get(queryUrl).then(function (results) {
                return results;
            });
        },
        saveLink: function(link) {
            var queryUrl = serviceBase + 'api/url';
            return $http.post(queryUrl, link).then(function(results) {
                return results;
            });
        }
    }
}]);