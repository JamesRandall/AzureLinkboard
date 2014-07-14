'use strict';
app.controller('homeController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {
    if (authService.authentication.isAuth) {
        $location.path('/links');
    }
}]);