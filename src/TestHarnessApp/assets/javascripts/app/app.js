define(['angular'], function (angular) {

  var app = angular.module('myApp',[])

  app.controller('MainCtrl', ['$scope', function ($scope) {
    $scope.message = "message goes here"
  }])

  return app;
})
