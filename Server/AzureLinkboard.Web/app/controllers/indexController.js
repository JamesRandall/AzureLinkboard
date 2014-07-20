'use strict';
(function(controllers) {
    controllers.controller('indexController', [
        '$scope', '$location', 'authService', 'linkFeedService', function($scope, $location, authService, linkFeedService) {

            function setNewLinkModel() {
                $scope.newLink = {
                    "Url": "",
                    "Title": "",
                    "Description": "",
                    "Tags": ""
                };
                $scope.saveResult = linkFeedService.defaultModel();
            }

            $scope.saveResult = linkFeedService.defaultModel();

            $scope.logOut = function() {
                authService.logOut();
                $location.path('/home');
            };

            $scope.showAddLink = function () {
                setNewLinkModel();
                $("#addLinkModal").modal();
            };

            $scope.saveLink = function () {
                if ($scope.linkform.$invalid) {
                    return;
                }
                linkFeedService.saveLink($scope.newLink).then(function (results) {
                    $scope.saveResult = results;
                    $scope.$broadcast('newLink', results.data);
                    $('#addLinkModal').modal('hide');
                },
                function (error) {
                    $scope.saveResult = error;
                });

            };

            $scope.authentication = authService.authentication;

        }
    ]);
}(angular.module('linkboardControllers')));