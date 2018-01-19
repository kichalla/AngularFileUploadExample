'use strict';

angular.module('Authentication')

    .controller('LoginController',
    ['$scope', '$rootScope', '$location', 'AuthenticationService',
        function ($scope, $rootScope, $location, AuthenticationService) {
            // reset login status
            AuthenticationService.ClearCredentials();

            $scope.login = function () {
                $scope.dataLoading = true;
                AuthenticationService.Login($scope.email, $scope.password, function (response) {
                    if (response.status == 200) {
                        AuthenticationService.SetCredentials($scope.email);
                        $location.path('/');
                    } else {
                        $scope.error = response.message;
                        $scope.dataLoading = false;
                    }
                });
            };

            $scope.logout = function () {
                AuthenticationService.Logout(function (response) {
                    if (response.status == 200) {
                        AuthenticationService.ClearCredentials();
                        $location.path('/login');
                    } else {
                        $scope.error = response.message;
                        $scope.dataLoading = false;
                    }
                });
            }
        }]);