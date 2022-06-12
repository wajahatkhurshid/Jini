jiniApp.directive('accessprovider', ['jiniservice', 'dataFactory', function (
    jiniservice,
    dataFactory) {
    return {
        restrict: 'AE',
        replace: true,
        scope: {
        },
        templateUrl: '/Wizard/accessprovider',
        link: function ($scope) {
            $scope.jiniData = dataFactory;
            $scope.jiniData.isRDatafill = false;

            $scope.ResetData = function () {
                $scope.jiniData.ProviderAccessForm = {
                    Code: "",
                    DisplayName: "",
                    Identifier: "",
                    ischecked: false
                }
            }
            $scope.ResetData();

            $scope.GetRefAccessProviders = function () {
                var getRefAccessProviders = jiniservice;
                getRefAccessProviders.GetRefAccessProviders().then(function (data) {
                    $scope.jiniData.ProviderAccessForm = data;
                    $scope.getProductAccessInformations();
                });
            }

            $scope.getProductAccessInformations = function () {
                var getRefAccessProvider = jiniservice;
                getRefAccessProvider.GetAllProductAccessProviders($scope.jiniData.DigitalTitle.Isbn).then(function (data) {
                    $scope.jiniData.ProductAccessProviderInformation = data;
                    $scope.ProductAccessChecks();
                });
            }
            $scope.GetRefAccessProviders();
            $scope.ProductAccessChecks = function () {
                for (var j = 0; j < $scope.jiniData.ProviderAccessForm.length; j++) {
                    if ($scope.jiniData.ProductAccessProviderInformation.AccessProvider)
                        for (var i = 0; i < $scope.jiniData.ProductAccessProviderInformation.AccessProvider.length; i++) {
                            if ($scope.jiniData.ProviderAccessForm[j].Code ==
                                $scope.jiniData.ProductAccessProviderInformation.AccessProvider[i]) {
                                $scope.jiniData.ProviderAccessForm[j].ischecked = true;
                                break;
                            } else {
                                if (!$scope.jiniData.ProviderAccessForm[j].ischecked)
                                    $scope.jiniData.ProviderAccessForm[j].ischecked = false;
                            }
                        }
                }
                $scope.jiniData.isRDatafill = true;
                $("#loader_jini").css({ "display": "none" });
            }

            $scope.togglestate = function (event, obj) {
                obj.ischecked = event.target.checked;
            }
            $scope.toggleExternalLogin = function (obj) {
                if (!obj.IsExternalLogin) {
                    obj.IsExternalLogin = false;
                } else {
                    obj.IsExternalLogin = true;
                }
            }
            $scope.filterData = function () {
                var temp = $scope.jiniData.ProviderAccessForm.filter(x => x.ischecked == true);
                $scope.jiniData.ProductAccessProviderInformation.AccessProvider = [];
                for (var i = 0; i < temp.length; i++) {
                    $scope.jiniData.ProductAccessProviderInformation.AccessProvider.push(temp[i].Code);
                }
            }
            $scope.SaveProductAccess = function () {
                //SaveProductAccessPInformation
                $scope.jiniData.isRDatafill = false;
                $scope.filterData();
                var getRefAccessProvider = jiniservice;
                if (!$scope.jiniData.ProductAccessProviderInformation.IsExternalLogin &&
                    $scope.jiniData.ProductAccessProviderInformation.AccessProvider &&
                    $scope.jiniData.ProductAccessProviderInformation.AccessProvider.length <= -1) {
                    $scope.jiniData.isRDatafill = true;
                    return false;
                }
                getRefAccessProvider.SaveProductAccessProviders($scope.jiniData.ProductAccessProviderInformation).then(function (data) {
                    $scope.isRDatafill = false;
                    if (!data) {
                        alert("Kan ikke gemme Adgang håndteres eksternt indstilling.");
                        $scope.jiniData.isRDatafill = true;
                    } else
                        $scope.GetRefAccessProviders();
                });
            }
        }
    }
}]);