'use strict';
/**************************************************************************

  * Approve controller

  *************************************************************************/

jiniApp.controller('approve',
    function ($rootScope,
        $scope,
        $sce,
        salesconfiguration,
        dataFactory) {
        $rootScope.state = 'approve';

        $scope.Save = function (approved) {
            return salesconfiguration.Save(approved, $scope);
        };

        $scope.jiniData = dataFactory;

        // initializing html text from the user input, in case of new sale configuration.
        $scope.jiniData.Trial.ContactSalesTextHtml = $sce.trustAsHtml($scope.jiniData.Trial.ContactSales);
        $scope.jiniData.ContactSales.DescriptionTextHtml = $sce.trustAsHtml($scope.jiniData.ContactSales.DescriptionText);

        $("#divLoading").removeClass("show");

        wizardPageThree();
    });