jiniApp.directive('revisionhistory', ['jiniservice', 'dataFactory', function (
    jiniservice,
    dataFactory) {
    return {
        restrict: 'AE',
        replace: true,
        scope: {
             version: "="
        },
        templateUrl: '/Wizard/revisionHistory',
        link: function ($scope) {
            $scope.jiniData = dataFactory;
        }
    }
}]);