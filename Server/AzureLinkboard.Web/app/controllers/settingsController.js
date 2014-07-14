'use strict';
app.controller('settingsController', [
    '$scope', 'settingsService', function($scope, settings) {
        $scope.bookmarklet = "javascript:(function(){ window.open('" + settings.server + "bookmarklet.html?url=' + window.location.href + '&title=' + window.document.title); })();";
    }
]);