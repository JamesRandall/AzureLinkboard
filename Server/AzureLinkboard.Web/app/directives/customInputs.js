'use strict';
(function (directives) {
    directives.directive("input", function () {
        return {
            restrict: "E",
            require: "?ngModel",
            link: function ($scope, element, attributes, modelController) {
                if (!modelController)
                    return;
                $scope.$watch(function () {
                    return modelController.$invalid;
                }, function () {
                    if (modelController.$invalid && modelController.$dirty) $scope.$emit("inputError");
                    else $scope.$emit("inputValid");
                });
                $scope.$watch(function () {
                    return modelController.$dirty;
                }, function () {
                    if (modelController.$invalid && modelController.$dirty) $scope.$emit("inputError");
                    else $scope.$emit("inputValid");
                });
            }
        };
    });

    directives.directive('afTextInput', function() {
        var d = {
            restrict: 'E',
            templateUrl: '/app/directives/afEmailInput.html',
            scope: {
                afName: '=',
                afLabel: '=',
                ngModel: '=',
                afType: '='
            },
        };
        d.link = function(scope, element, attrs, modelController) {
            scope.$on("inputError", function () {
                angular.element(element[0].children[0]).addClass("has-error");
            });
            scope.$on("inputValid", function () {
                angular.element(element[0].children[0]).removeClass("has-error");
            });

            scope.autofocus = attrs.hasOwnProperty('autofocus');
            if (scope.autofocus) {
                element[0].removeAttribute('autofocus');
                //angular.element(element.find("input")[0]).setAttribute('autofocus', 'autofocus');
                //d.scope.autofocus = true;
                
            }
        };
        return d;
    });

}(angular.module('afCustomInputs', [])));