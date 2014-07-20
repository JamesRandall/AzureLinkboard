'use strict';
(function(controllers) {
    controllers.controller('signupController', [
        '$scope', '$location', '$timeout', 'authService', function ($scope, $location, $timeout, authService) {

            $scope.message = "";

            $scope.registration = {
                email: "",
                password: "",
                confirmPassword: ""
            };

            $scope.saveResult = authService.defaultModel();

            $scope.signUp = function () {
                if ($scope.regform.$invalid) {
                    return;
                }

                authService.saveRegistration($scope.registration).then(function(response) {
                        $scope.saveResult = response;
                        $scope.saveResult.message = "You have been registered successfully and will be redicted to login page in 2 seconds.";
                        startTimer();
                    },
                    function(response) {
                        $scope.saveResult = response;
                    });
            };
            var startTimer = function() {
                var timer = $timeout(function() {
                    $timeout.cancel(timer);
                    $location.path('/login');
                }, 2000);
            }
        }
    ]);
})(angular.module('linkboardControllers'));