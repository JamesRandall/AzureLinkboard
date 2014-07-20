'use strict';
(function(controllers) {
    controllers.controller('bookmarkletController', [
        '$scope', '$window', '$location', 'linkFeedService', function($scope, $window, $location, linkFeedService) {
            $scope.newLink = {
                "Url": $location.search().url,
                "Title": $location.search().title,
                "Description": "",
                "Tags": ""
            };

            $scope.saveResult = linkFeedService.defaultModel();

            $scope.cancel = function() {
                $window.close();
            };

            $scope.saveLink = function () {
                if ($scope.linkform.$invalid) {
                    return;
                }
                linkFeedService.saveLink($scope.newLink).then(function (results) {
                    $scope.saveResult = results;
                    $scope.$broadcast('newLink', results.data);
                    $window.close();
                },
                function (error) {
                    $scope.saveResult = error;
                });

            };
        }
    ]);
})(angular.module('bookmarkletControllers'));
