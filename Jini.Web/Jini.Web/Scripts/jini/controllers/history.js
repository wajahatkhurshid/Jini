'use strict';
/**************************************************************************

  * Controller for the Jini Sales Configuration Wizard. Walks the user through the first step to the eventual approval or cancellation of sales configuration

  *************************************************************************/

jiniApp.controller('history',
    function ($rootScope,
        $scope,
        $timeout,
        $state,
        jiniservice,
        salesconfiguration,
        dataFactory,
        trialLicense) {
        $scope.jiniData = dataFactory;
        $("#divLoading").addClass('show');
        $rootScope.state = 'history';
        stateChanged = false;
        //Startup Code
        $rootScope.Approved = true;
        $timeout(function () {
            viewSalesConfig();
            wizardPageThree();
            $("#ApproveHeader").hide();
        }, 0);

        $(document).ready(function () {

            //==================External revision history starts here=====================
            var getSalesConfiguratioPromise = jiniservice
                .getSalesConfiguration($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName, true, $scope.jiniData.DigitalTitle.Version);
            var getSalesConfigurationHistoryPromise = jiniservice
                .getSalesConfigurationHistory($scope.jiniData.DigitalTitle.Isbn);
            getSalesConfiguratioPromise.then(function (data) {
                $scope.refreshDataStructures();
                salesconfiguration.parseSalesConfiguration(data, $scope.jiniData)
                    .then(function () {
                        if ($rootScope.existingConfiguration.indexOf(_draftCode) > -1) {
                            $state.go("history.draft");
                        }
                        else {
                            $state.go("history.new");
                        }

                        trialLicense.getShareLink($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName);

                        $("#divLoading").removeClass('show');
                    });
            });
            getSalesConfigurationHistoryPromise.then(
                function (historyInfo) {
                    $scope.jiniData.VersionHistory = historyInfo
                        .sort(function (a, b) { return b.VersionNo - a.VersionNo });
                    for (var index = 0; index < $scope.jiniData.VersionHistory.length; index++) {
                        $scope.jiniData.VersionHistory[index].CreatedDate = new Date($scope.jiniData.VersionHistory[index].CreatedDate);
                        //$scope.VersionHistory[index].CreatedDate.setMinutes($scope.VersionHistory[index].CreatedDate.getMinutes() + new Date().getTimezoneOffset());
                    }
                }
            );

            //================== External history ends here =================

            //================== Gua revision history starts here =====================
            var getGuaSalesConfiguratioPromise = jiniservice
                .getGuaSalesConfiguration($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName, true);
            var getGuaSalesConfigurationHistoryPromise = jiniservice
                .getGuaSalesConfigurationHistory($scope.jiniData.DigitalTitle.Isbn);
            getGuaSalesConfiguratioPromise.then(function (data) {
                $scope.refreshDataStructures();
                salesconfiguration.parseSalesConfiguration(data, $scope.jiniData)
                    .then(function () {
                        if ($rootScope.existingConfiguration.indexOf(_draftCode) > -1) {
                            $state.go("history.draft");
                        }
                        else {
                            $state.go("history.new");
                        }

                        trialLicense.getShareLink($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName);

                        $("#divLoading").removeClass('show');
                    });
            });
            getGuaSalesConfigurationHistoryPromise.then(
                function (historyInfo) {
                    $scope.jiniData.GuaVersionHistory = historyInfo
                        .sort(function (a, b) { return b.VersionNo - a.VersionNo });
                    for (var index = 0; index < $scope.jiniData.GuaVersionHistory.length; index++) {
                        $scope.jiniData.GuaVersionHistory[index].CreatedDate = new Date($scope.jiniData.GuaVersionHistory[index].CreatedDate);
                        
                    }
                }
            );

            //=============== External history ends here=================

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

        $scope.TransitionToWizard = function () {
            $state.go("create");
            if ($("#new-version-warning-popup").data("kendoWindow") != null) {
                $("#new-version-warning-popup").data("kendoWindow").destroy();
            }
        }

        $scope.TransitionToWizardWithDraft = function () {
            var getSalesConfiguratioPromise = jiniservice
                .getSalesConfiguration($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName, false, $scope.jiniData.DigitalTitle.Version);
            getSalesConfiguratioPromise.then(function (data) {
                $scope.refreshDataStructures();
                salesconfiguration.parseSalesConfiguration(data, $scope.jiniData)
                    .then(function () {
                        $state.go("create");
                        if ($("#new-version-warning-popup").data("kendoWindow") != null) {
                            $("#new-version-warning-popup").data("kendoWindow").destroy();
                        }
                    });
            });
        }

        historyPage();
    });