'use strict';
describe('loginController tests', function () {
    var scope, ctrl, authServices, location;

    function authServicesMock() {
        var mock = {
            succeed: true
        };

        mock.defaultModel = function() {
            return null;
        };

        mock.login = function() {
            return {
                then: function(r, e) {
                    if (mock.succeed) {
                        r();
                    } else {
                        e({ attemptedSave: true, savedSuccessfully: false });
                    }
                }
            }
        };
        return mock;
    };

    beforeEach(function() {
        module('AzureLinkboardApp');
        module('linkboardControllers');

        inject(function ($rootScope, $injector, $controller) {
            authServices = authServicesMock();
            location = $injector.get('$location');
            scope = $rootScope.$new();
            scope.loginform = { $invalid: false };
            ctrl = $controller("loginController", {
                $scope: scope,
                $location: location,
                authService: authServices
            });
        });
    });
    
    it("should have empty login data", function () {
        expect(scope.loginData).toBeDefined();
        expect(scope.loginData.userName).toBe("");
        expect(scope.loginData.password).toBe("");
    });

    it("should start with an empty message", function () {
        expect(scope.message).toBe("");
    });

    it("login should go to links page", function () {
        scope.login();
        expect(location.path()).toBe('/links');
    });

    it("failed login should set save result", function () {
        authServices.succeed = false;

        scope.login();
        expect(scope.saveResult.attemptedSave).toBe(true);
        expect(scope.saveResult.savedSuccessfully).toBe(false);
    });
});