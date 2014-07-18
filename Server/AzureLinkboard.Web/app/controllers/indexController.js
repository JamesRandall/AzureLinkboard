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
            }

            $scope.logOut = function() {
                authService.logOut();
                $location.path('/home');
            };

            $scope.showAddLink = function() {
                setNewLinkModel();
                $("#addLinkModal").modal();
            };

            $scope.saveLink = function() {
                linkFeedService.saveLink($scope.newLink).then(function(results) {
                    $scope.$broadcast('newLink', results.data);
                    $('#addLinkModal').modal('hide');
                });

            };

            $scope.authentication = authService.authentication;

        }
    ]);
}(angular.module('linkboardControllers')));