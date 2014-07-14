'use strict';
app.controller('bookmarkletController', [
    '$scope', '$window', '$location', 'linkFeedService', function($scope, $window, $location, linkFeedService) {
        $scope.newLink = {
            "Url": $location.search().url,
            "Title": $location.search().title,
            "Description": "",
            "Tags": ""
        };

        $scope.cancel = function() {
            $window.close();
        };

        $scope.saveLink = function() {
            linkFeedService.saveLink($scope.newLink).then(function (results) {
                $scope.$broadcast('newLink', results.data);
                $('#addLinkModal').modal('hide');
                $window.close();
            });
        };
    }
]);