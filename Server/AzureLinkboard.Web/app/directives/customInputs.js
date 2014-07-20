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
            templateUrl: '/app/directives/templates/afTextInput.html',
            scope: {
                afName: '=',
                afLabel: '=',
                ngModel: '=',
                afType: '='
            },
        };
        d.link = function(scope, element, attrs) {
            scope.$on("inputError", function () {
                angular.element(element[0].children[0]).addClass("has-error");
            });
            scope.$on("inputValid", function () {
                angular.element(element[0].children[0]).removeClass("has-error");
            });

            var inputElement = element.find("input")[0];

            if (attrs.hasOwnProperty('autofocus')) {
                element[0].removeAttribute('autofocus');
                inputElement.setAttribute('autofocus', 'autofocus');
            }

            scope.afRequired = attrs.hasOwnProperty('required');
            if (scope.afRequired) {
                element[0].removeAttribute('required');
                inputElement.setAttribute('required', 'required');
            }

            if (attrs.hasOwnProperty('maxlength')) {
                inputElement.setAttribute('maxlength', attrs.maxlength);
                element[0].removeAttribute('maxlength');
            }
        };
        return d;
    });

}(angular.module('afCustomInputs', [])));