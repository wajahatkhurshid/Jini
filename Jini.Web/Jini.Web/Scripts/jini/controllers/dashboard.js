'use strict';
/**************************************************************************

  * Dashboard controller

  *************************************************************************/

jiniApp.controller('Dashboard',
    function (
        $scope,
        dataFactory) {
        $scope.jiniData = dataFactory;
    });