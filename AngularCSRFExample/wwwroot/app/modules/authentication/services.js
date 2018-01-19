'use strict';

angular.module('Authentication')

    .factory('AuthenticationService', ['$http', '$cookieStore', '$rootScope', '$timeout',
        function ($http, $cookieStore, $rootScope, $timeout) {
            var service = {};

            service.Login = function (email, password, callback) {
                return $http
                    .post('/auth/login', { email: email, password: password })
                    .then(function (response) {
                        callback(response);
                    });
            };

            service.Logout = function (callback) {
                return $http
                    .get('/auth/logout')
                    .then(function (response) {
                        callback(response);
                    });
            };

            service.SetCredentials = function (email) {
                $rootScope.globals = {
                    currentUser: {
                        email: email
                    }
                };
                $cookieStore.put('globals', $rootScope.globals);
            }

            service.ClearCredentials = function () {
                $rootScope.globals = {};
                $cookieStore.remove('globals');
            };

            return service;
        }]);