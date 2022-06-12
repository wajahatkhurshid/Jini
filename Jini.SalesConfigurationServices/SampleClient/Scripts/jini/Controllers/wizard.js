'use strict';
/**************************************************************************

  * Controller for the Jini Sales Configuration Wizard. Walks the user through the first step to the eventual approval or cancellation of sales configuration

  *************************************************************************/

jiniApp.controller('wizard', function ($scope, SalesConfigurationService) {

    $scope.IsPublished = function ()
    {
        SalesConfigurationService.IsPublished($scope.ISBN)
            .then(function (data)
            {
                alert(data);
            });
    }

    $scope.GetConfiguration = function ()
    {
        SalesConfigurationService.GetConfiguration($scope.ISBN, $scope.InstitutionNo)
            .then(function (data)
            {
                $scope.SalesConfiguration = data;
                
            });
    };

    $scope.CalculatePrice = function ()
    {
        for (var i = 0; i < $scope.SalesConfiguration.AccessForms.length; i++)
        {
            if ($scope.SalesConfiguration.AccessForms[i].Code == 1002 )
            {
                if ($scope.SalesConfiguration.AccessForms[i].PriceModels[0].Classes != "" || $scope.SalesConfiguration.AccessForms[i].PriceModels[0].Classes != undefined)
                {
                    $scope.SalesConfiguration.AccessForms[i].SelectedClasses = $scope.SelectedClasses.Classes;

                }

            }
        }
        SalesConfigurationService.GetPrice($scope.ISBN, $scope.InstitutionNo, $scope.SalesConfiguration)
            .then(function (data)
            {
                $scope.SalesConfiguration = data;
            });
    };

    $scope.Refresh = function ()
    {
        $scope.ISBN = "";
        $scope.InstitutionNo = "";
        $scope.SalesConfiguration = {};
        $scope.SelectedClasses = {};
    };

    $scope.ISBN;
    $scope.InstitutionNo;
    $scope.SalesConfiguration;
    $scope.SelectedClasses = {};

});