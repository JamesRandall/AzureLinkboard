'use strict';
app.factory('tagService', ['$http', 'settingsService', function ($http, settings) {

    var serviceBase = settings.remoteServer;

    return {
        getPage: function (tag, continuationToken) {
            var queryUrl = serviceBase + 'api/tag/' + tag;
            if (continuationToken !== undefined) {
                queryUrl = queryUrl + "?continuationToken=" + encodeURIComponent(continuationToken);
            }
            return $http.get(queryUrl).then(function (results) {
                return results;
            });
        }
    }
}]);