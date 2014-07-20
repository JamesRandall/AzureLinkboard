'use strict';
(function(controllers) {
    controllers.controller('loginController', [
        '$scope', '$location', 'authService', function($scope, $location, authService) {
            $scope.loginData = {
                userName: "",
                password: ""
            };
            $scope.saveResult = authService.defaultModel();
            $scope.message = "";
            $scope.login = function () {
                if ($scope.loginform.$invalid) {
                    return;
                }
                authService.login($scope.loginData).then(function(response) {
                        $location.path('/links');
                    },
                    function(error) {
                        $scope.saveResult = error;
                    });
            };
        }
    ]);
}(angular.module('linkboardControllers')));