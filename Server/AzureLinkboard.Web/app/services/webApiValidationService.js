'use strict';
app.factory('webApiValidationService', ['$q', function ($q) {
    return {
        defaultModel: function() {
            return {
                attemptedSave: false,
                status: 200,
                message: null,
                isModelError: false,
                errors: null,
                savedSuccessfully: false
            };
        },
        handleSuccess: function (response) {
            return {
                status: 200,
                attemptedSave: true,
                message: response.message,
                isModelError: false,
                savedSuccessfully: true,
                errors: {

                }
            };

        },
        handleError: function (response) {
            var result;
            if (response.status == 400 && response.data !== undefined) {
                // modelstate error
                result = {
                    attemptedSave: true,
                    status: response.status,
                    message: response.data.Message,
                    isModelError: true,
                    savedSuccessfully: false,
                    errors: {

                    }
                };
                for (var propertyName in response.data.ModelState) {
                    if (propertyName.indexOf('model.') === 0) {
                        var key = propertyName.substring(6);
                        result.errors[key] = response.data.ModelState[propertyName];
                    }
                }
            } else {
                // not a modelstate error
                result = {
                    attemptedSave: true,
                    status: response.status,
                    message: response.statusText,
                    isModelError: false,
                    savedSuccessfully: false,
                    errors: {

                    }
                };
            }
            return $q.reject(result);
        }
    };
}]);