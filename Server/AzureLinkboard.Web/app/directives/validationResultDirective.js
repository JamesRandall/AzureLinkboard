angular.module('afValidationDirective', []).directive('afServerValidation', function() {
    return {
        restrict: 'E',
        templateUrl: '/app/views/afServerValidation.html',
        scope: {
            result: '=key'
        }
    }
});