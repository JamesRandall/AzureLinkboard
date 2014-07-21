'use strict';
(function(controllers) {
    controllers.controller('settingsController', [
        '$scope', 'settingsService', 'authService', function($scope, settings, authService) {
            $scope.bookmarklet = "javascript:(function(){ window.open('" + settings.server + "bookmarklet.html?url=' + window.location.href + '&title=' + window.document.title); })();";

            function setDefaultModel() {
                $scope.changePassword = {
                    oldPassword: '',
                    newPassword: '',
                    confirmPassword: ''
                };
            };

            setDefaultModel();

            $scope.saveResult = authService.defaultModel();

            $scope.doChangePassword = function() {
                if ($scope.changePasswordForm.$invalid) {
                    return;
                }

                authService.changePassword($scope.changePassword).then(function (response) {
                    setDefaultModel();
                    $scope.saveResult = response;
                    $scope.saveResult.message = "You have successfully changed your password.";
                }, function (error) {
                    $scope.saveResult = error;
                });
            };
        }
    ]);
})(angular.module('linkboardControllers'));