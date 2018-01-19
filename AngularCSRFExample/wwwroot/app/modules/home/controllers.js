'use strict';

angular.module('Home')

    .controller('HomeController',
    ['$scope', '$location', 'AuthenticationService', '$http',
        function ($scope, $location, AuthenticationService, $http) {

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
            };

            $scope.createProduct = function () {
                $scope.showStatus = false; 
                $scope.showResponseData = false;

                $http.post('/products/create', { name: $scope.name })
                    .then(function (response) {
                        if (response.status == 200) {
                            $scope.statusMessage = "Product created sucessfully.";
                            $scope.responseData = response.data;
                            $scope.showStatus = true;
                            $scope.showResponseData = true;
                        }
                    }, function (response) {
                        $scope.statusMessage = "Production creation failed with status code: " + response.status;
                        $scope.showStatus = true;
                        $scope.showResponseData = false;
                    });
            };
}]);