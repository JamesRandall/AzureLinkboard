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
                ngModel: '='
            },
        };
        d.link = function(scope, element, attrs, modelController) {
            scope.$on("inputError", function () {
                angular.element(element[0].children[0]).addClass("has-error");
            });
            scope.$on("inputValid", function () {
                angular.element(element[0].children[0]).removeClass("has-error");
            });
            scope.$watch(function () {
                return modelController.$invalid;
            }, function () {
                if (modelController.$invalid) scope.$emit("inputError");
                else scope.$emit("inputValid");
            });
        };
        return d;
    });

}(angular.module('afCustomInputs', [])));