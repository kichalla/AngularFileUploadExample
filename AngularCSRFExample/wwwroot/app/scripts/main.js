var User = (function () {
    function User(name, age, zipcode, curriculumVitae) {
        this.name = name;
        this.age = age;
        this.zipcode = zipcode;
        this.curriculumVitae = curriculumVitae;
    }
    return User;
}());

var myApp = angular.module('myApp', []);

myApp.service('userService', ['$http', 'loginService', function ($http, loginService) {
    this.createUser = function (user) {
        return $http.post('/home/create', user);
    };
}]);

myApp.service('loginService', ['$http', function ($http) {
    this.login = function () {
        return $http.post('/auth/login', {username: 'foo', password: 'bar'});
    };
    this.logout = function () {
        return $http.get('/auth/logout');
    };
}]);

myApp.controller('myCtrl', ['$scope', 'userService', function ($scope, userService) {
    $scope.createUser = function () {
        $scope.showStatus = false;
        $scope.showResponseData = false;

        var user = new User($scope.name, $scope.age, $scope.zipcode, $scope.curriculumVitae)

        userService.createUser(user).then(function (response) { // success
            if (response.status == 200) {
                $scope.statusMessage = "User created sucessfully.";
                $scope.responseData = response.data;
                $scope.showStatus = true;
                $scope.showResponseData = true;
            }
        },
            function (response) { // failure
                $scope.statusMessage = "User creation failed with status code: " + response.status;
                $scope.showStatus = true;
                $scope.showResponseData = false;
            });
    };
}]);

myApp.controller('authCtrl', ['$scope', 'loginService', function ($scope, loginService) {
    $scope.login = function () {
        $scope.showStatus = false;
        $scope.showResponseData = false;

        loginService.login().then(function (response) { // success
            if (response.status == 200) {
                $scope.statusMessage = "Logged into application sucessfully.";
                $scope.showStatus = true;
                $scope.showResponseData = false;
            }
        },
            function (response) { // failure
                $scope.statusMessage = "Login failed with status code: " + response.status;
                $scope.showStatus = true;
                $scope.showResponseData = false;
            });
    };

    $scope.logout = function () {
        $scope.showStatus = false;
        $scope.showResponseData = false;

        loginService.logout().then(function (response) { // success
            if (response.status == 200) {
                $scope.statusMessage = "Logged out of application sucessfully.";
                $scope.showStatus = true;
                $scope.showResponseData = false;
            }
        },
            function (response) { // failure
                $scope.statusMessage = "Logout failed with status code: " + response.status;
                $scope.showStatus = true;
                $scope.showResponseData = false;
            });
    };
}]);