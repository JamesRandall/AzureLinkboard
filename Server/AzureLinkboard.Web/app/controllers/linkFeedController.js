'use strict';
(function(controllers) {
    controllers.controller('linkFeedController', [
        '$scope', 'linkFeedService', 'settingsService', function($scope, linkFeedService, settings) {
            $scope.savedLinks = [];
            $scope.apibusy = false;
            $scope.continuationTokens = [];
            $scope.dataLoaded = false;
            $scope.bookmarklet = "javascript:(function(){ window.open('" + settings.server + "bookmarklet.html?url=' + window.location.href + '&title=' + window.document.title); })();";
            $scope.$on('newLink', function(ev, savedLink) {
                if ($scope.continuationTokens.length == 1) { // if on first page insert the new item
                    $scope.savedLinks.unshift(savedLink);
                }
            });
            $scope.nextPage = function() {
                if ($scope.continuationTokens.length == 0) return;
                var continuationToken = $scope.continuationTokens[$scope.continuationTokens.length - 1];
                if (continuationToken !== null) {
                    $scope.apibusy = true;
                    linkFeedService.getPage(continuationToken).then(function(results) {
                        $scope.continuationTokens.push(results.data.ContinuationToken);
                        $scope.savedLinks.push.apply($scope.savedLinks, results.data.Items);
                        $scope.apibusy = false;
                    }, function(error) {
                        $scope.apibusy = false;
                        alert(error.data.message);
                    });
                }
            };

            linkFeedService.getPage().then(function(results) {
                $scope.continuationTokens.push(results.data.ContinuationToken);
                $scope.savedLinks = results.data.Items;
                $scope.dataLoaded = true;

            }, function(error) {
                alert(error.data.message);
            });
        }
    ]);
}(angular.module('linkboardControllers')));
