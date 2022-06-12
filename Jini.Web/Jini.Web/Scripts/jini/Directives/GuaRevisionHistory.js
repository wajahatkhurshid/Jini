jiniApp.directive('guarevisionhistory', ['jiniservice', 'dataFactory', function (
    jiniservice,
    dataFactory) {
    return {
        restrict: 'AE',
        replace: true,
        scope: {
            version: "="
        },
        templateUrl: '/Wizard/guaRevisionHistory',
        link: function ($scope) {
            $scope.jiniData = dataFactory;
        }
    }
}]);