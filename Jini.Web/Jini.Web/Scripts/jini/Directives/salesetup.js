'use strict';
/**************************************************************************

  * SaleSetup directive

  *************************************************************************/

jiniApp.directive('salesetup', ['trialLicense', 'salesconfiguration', 'jiniservice', '$state', '$timeout', 'dataFactory', function (

    trialLicense,
    salesconfiguration,
    jiniservice,
    $state,
    $timeout,
    dataFactory) {
    return {
        restrict: 'AE',
        replace: true,
        templateUrl: '/Wizard/salesetup',
        link: function ($scope) {
            $scope.jiniData = dataFactory;
            $("#divLoading").addClass('show');

            $scope.state = 'dashboard';
            stateChanged = false;
            //Startup Code
            $scope.Approved = true;
            $timeout(function () {
                viewSalesConfig();
                wizardPageThree();
                $("#ApproveHeader").hide();
            }, 0);

            $(document).ready(function () {

                //=======External history starts here================
                var getSalesConfigurationPromise = jiniservice
                    .getSalesConfiguration($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName, !$scope.jiniData.DigitalTitle.HasDraft, $scope.jiniData.DigitalTitle.Version);
                var getSalesConfigurationHistoryPromise = jiniservice
                    .getSalesConfigurationHistory($scope.jiniData.DigitalTitle.Isbn);

                getSalesConfigurationPromise.then(function (data) {
                    // $scope.refreshDataStructures();
                    if (data != null) {
                        salesconfiguration.parseSalesConfiguration(data, $scope.jiniData)
                            .then(function () {
                                $("#loader_jinisalesetup").css({ "display": "none" });

                                trialLicense.getShareLink($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName);

                                $("#divLoading").removeClass('show');
                            });
                    }
                });

                getSalesConfigurationHistoryPromise.then(
                    function (historyInfo) {
                        $timeout(function () { $scope.jiniData.isLDatafill = true; }, 3000);

                        $scope.jiniData.VersionHistory = historyInfo
                            .sort(function (a, b) { return b.VersionNo - a.VersionNo });
                        for (var index = 0; index < $scope.jiniData.VersionHistory.length; index++) {
                            $scope.jiniData.VersionHistory[index].CreatedDate = new Date($scope.jiniData.VersionHistory[index].CreatedDate);
                            //$scope.VersionHistory[index].CreatedDate.setMinutes($scope.VersionHistory[index].CreatedDate.getMinutes() + new Date().getTimezoneOffset());
                        }
                    }
                );

                 //=======External history ends here================

                //=======Gua history starts here================
                var getGuaSalesConfigurationPromise = jiniservice
                    .getSalesConfiguration($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName, !$scope.jiniData.DigitalTitle.HasDraft, $scope.jiniData.DigitalTitle.Version);
                var getGuaSalesConfigurationHistoryPromise = jiniservice
                    .getGuaSalesConfigurationHistory($scope.jiniData.DigitalTitle.Isbn);

                getGuaSalesConfigurationPromise.then(function (data) {
                    // $scope.refreshDataStructures();
                    if (data != null) {
                        salesconfiguration.parseSalesConfiguration(data, $scope.jiniData)
                            .then(function () {
                                $("#loader_jinisalesetup").css({ "display": "none" });

                                trialLicense.getShareLink($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName);

                                $("#divLoading").removeClass('show');
                            });
                    }
                });

                getGuaSalesConfigurationHistoryPromise.then(
                    function (historyInfo) {
                        $timeout(function () { $scope.jiniData.isLDatafill = true; }, 3000);

                        $scope.jiniData.GuaVersionHistory = historyInfo
                            .sort(function (a, b) { return b.VersionNo - a.VersionNo });
                        for (var index = 0; index < $scope.jiniData.VersionHistory.length; index++) {
                            $scope.jiniData.VersionHistory[index].CreatedDate = new Date($scope.jiniData.VersionHistory[index].CreatedDate);
                            //$scope.VersionHistory[index].CreatedDate.setMinutes($scope.VersionHistory[index].CreatedDate.getMinutes() + new Date().getTimezoneOffset());
                        }
                    }
                );

                 //=======External history ends here================
            });

            $scope.onSuccess = function (e) {
                console.info('Action:', e.action);
                console.info('Text:', e.text);
                console.info('Trigger:', e.trigger);

                e.clearSelection();
            };

            $scope.onError = function (e) {
                console.error('Action:', e.action);
                console.error('Trigger:', e.trigger);
            }

            //END Startup Code
            $scope.parseSalesConfiguration = function (config) {
                return salesconfiguration.parseSalesConfiguration(config, $scope.jiniData);
            };

            $scope.ResumeDraftVersion = function () {
                var newVersionWindow = $("#new-version-warning-popup").data("kendoWindow");
                if (newVersionWindow != null) {
                    newVersionWindow.center().open();
                }
            }

            $scope.CreateSaleConfiguartion = function () {
                $state.go("create");
            }

            $scope.TransitionToWizard = function () {
                $state.go("create");
                if ($("#new-version-warning-popup").data("kendoWindow") != null) {
                    $("#new-version-warning-popup").data("kendoWindow").destroy();
                }
            }

            historyPage();
            $scope.hidePopops = function () {
                initUnload + Event();
                window.popUpCaller = "";
                if ($("#confirmation-popup").data("kendoWindow") != null)
                    $("#confirmation-popup").data("kendoWindow").close();

                if ($("#new-version-warning-popup").data("kendoWindow") != null)
                    $("#new-version-warning-popup").data("kendoWindow").close();
            }

            $scope.refreshDataStructures = function () {
                $scope.jiniData.SalesForm = [];
                $scope.jiniData.AccessForm = [];
                $scope.jiniData.SchoolLicense = {};
                $scope.jiniData.TeacherLicense = {};
                $scope.jiniData.ClassLicense = {};
                $scope.jiniData.SingleUserLicense = {};

                //This line have been commented because it were causing to remove fritekst on sales wizard
                //$scope.jiniData.ContactSales.DescriptionText = "";
                //$scope.jiniData.ContactSales.DescriptionTextHtml = "";
                $scope.jiniData.Trial.ContactSalesTextHtml = "";

                $scope.jiniData.AccessForm = [];
                $scope.jiniData.SalesForm = [];
                $scope.jiniData.Periods = [];
                $scope.jiniData.Validations = [];
                $scope.jiniData.SelectedSalesForm = [];
                $scope.jiniData.SelectedAccessForm = [];
                $scope.jiniData.PreviewPeriodSelection = {};
                $scope.jiniData.SingleUserPeriods = [];
            };
        }
    }
}]);