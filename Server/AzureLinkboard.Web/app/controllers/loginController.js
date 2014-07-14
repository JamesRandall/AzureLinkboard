'use strict';
app.controller('loginController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {
    $scope.loginData = {
        userName: "",
        password: ""
    };
    $scope.message = "";
    $scope.login = function() {
        authService.login($scope.loginData).then(function(response) {
                $location.path('/links');
            },
            function(error) {
                $scope.message = error.error_description;
            });
    };
}]);