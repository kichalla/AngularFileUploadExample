var User = (function () {
    function User(name, age, zipcode, curriculumVitae) {
        this.name = name;
        this.age = age;
        this.zipcode = zipcode;
        this.curriculumVitae = curriculumVitae;
    }
    return User;
}());

var myApp = angular.module('myApp', ['ngCookies']);
myApp.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);

myApp.service('userService', ['$http', 'tokenService', function ($http, tokenService) {
    this.createUser = function (user) {
        var fd = new FormData();
        fd.append('name', user.name);
        fd.append('age', user.age);
        fd.append('zipcode', user.zipcode);
        fd.append('curriculumVitae', user.curriculumVitae);

        return $http.post('/file/upload', fd, {
            transformRequest: angular.identity,
            headers: {
                'Content-Type': undefined
            }
        });
    }

    this.createUser1 = function (user) {
        if (tokenService.Token == undefined) {
            return $http.post('/users/create', user);
        }
        else {
            return $http.post(
                '/users/create',
                user,
                {
                    headers: { 'Authorization': ' Bearer ' + tokenService.Token.token }
                });
        }
    };
}]);

myApp.service('tokenService', ['$http', function ($http) {
    this.createToken = function () {
        return $http.get('/token/create');
    };

    this.createCSRFToken = function () {
        return $http.get(
            '/token/createCSRFToken',
            {
                headers: { 'Authorization': ' Bearer ' + this.Token.token }
            });
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

    $scope.createUser1 = function () {
        $scope.showStatus = false;
        $scope.showResponseData = false;

        var user = new User($scope.name, $scope.age, $scope.zipcode, $scope.curriculumVitae)

        userService.createUser1(user).then(function (response) { // success
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

myApp.controller('tokenCtrl', ['$scope', 'tokenService', function ($scope, tokenService, $cookies) {
    $scope.createToken = function () {
        $scope.showStatus = false;
        $scope.showResponseData = false;

        tokenService.createToken().then(function (response) { // success
            if (response.status == 200) {
                $scope.statusMessage = "Token created sucessfully.";
                $scope.responseData = response.data;
                tokenService.Token = response.data;
                $scope.showStatus = true;
                $scope.showResponseData = true;
            }
        },
            function (response) { // failure
                $scope.statusMessage = "Token creation failed with status code: " + response.status;
                $scope.showStatus = true;
                $scope.showResponseData = false;
            });
    };
    $scope.createCSRFToken = function () {
        $scope.showStatus = false;
        $scope.showResponseData = false;

        tokenService.createCSRFToken().then(function (response) { // success
            if (response.status == 200) {
                $scope.statusMessage = "CSRF Token created sucessfully.";
                $scope.showStatus = true;
            }
        },
            function (response) { // failure
                $scope.statusMessage = "CSRF Token creation failed with status code: " + response.status;
                $scope.showStatus = true;
            });
    };
}]);