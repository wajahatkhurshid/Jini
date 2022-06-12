'use strict';

////////////////////////////////
// Define AngularJS application
////////////////////////////////

var jiniApp = angular.module('jini', ['kendo.directives', 'ui.router', 'ncy-angular-breadcrumb']);


jiniApp.constant('__jiniEnv', __jiniEnv);

// routing function is in ~/Scripts/jini/Routing/routing.js
jiniApp.config(routing);

jiniApp.filter('unsafe', function ($sce) { return $sce.trustAsHtml; });

jiniApp.run(['$rootScope', '$state', function ($rootScope, $state) {

    $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
        if (fromState.name === '' && toState.name !== 'start') {
            event.preventDefault();
            $state.go('start');
        }
    });
}]);