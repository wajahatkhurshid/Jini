'use strict';
/**************************************************************************

  * Wizard controller

  *************************************************************************/

jiniApp.controller('wizard', function ($rootScope, $scope, $timeout, $state, jiniservice, validation, salesconfiguration, dataFactory) {
    $scope.jiniData = dataFactory;
    $("#divLoading").addClass("show");
    startup();
    /* Lookup variables will be used by Kendo Controls for population */

    /*Page 3*/
    $scope.searchArray = function (code, arrayName) {
        if (arrayName == "SalesForm")
            return $.grep(dataFactory.SalesForm, function (e) { return e.Code == code; });
        else if (arrayName == "AccessForm")
            return $.grep(dataFactory.AccessForm, function (e) { return e.Code == code; });
        return null;
    }

    /*Shared*/
    $rootScope.state = '';

    /*Navigation Buttons*/
    $("#wizard-next-button").click(function () {
        $("#divLoading").addClass('show');
        switch ($rootScope.state) {
            case 'history':
                break;
            case 'create':

                $scope.jiniData.Validations = [];

                if ($scope.jiniData.HasTrial && $scope.jiniData.SalesForm.length == 0 && $scope.jiniData.AccessForm.length == 0 && $scope.jiniData.Periods.length == 0) {
                    $rootScope.state = 'trial';
                    $state.go('trial');
                    break;
                }

                //Validation code of Tab 1 here
                $scope.jiniData.Validations = validation.validateStageOne($scope.jiniData.SalesForm, $scope.jiniData.AccessForm, $scope.jiniData.Periods, $scope.jiniData.HasTrial);

                if ($scope.jiniData.Validations.length > 0) {
                    $scope.$apply();
                    $("#divLoading").removeClass('show');
                    return;
                }

                $("#WizardContainer").addClass("wizard-set-prices");
                $state.go('config');
                $rootScope.state = 'config';
                break;
            case 'config':
                $scope.jiniData.Validations = [];
                //Validation code of Tab 2 here
                $scope.jiniData.Validations = validation.validateStageTwo($scope.jiniData.AccessForm, $scope.jiniData.Periods, $scope.jiniData.SchoolLicense, $scope.jiniData.ClassLicense, $scope.jiniData.TeacherLicense, $scope.jiniData.SingleUserLicense, $scope.jiniData.ContactSales, $scope.jiniData.Validations);

                //If validation errors are there - just stop
                if ($scope.jiniData.Validations.length > 0) {
                    $("#divLoading").removeClass("show");
                    $scope.$apply();
                    return;
                }

                $("#WizardContainer").removeClass("wizard-set-prices");

                if ($scope.jiniData.HasTrial) {
                    $rootScope.state = 'trial';
                    $state.go('trial');
                } else {
                    $rootScope.state = 'approve';
                    $state.go('approve');
                }
                break;
            case 'trial':
                $scope.jiniData.Validations = [];
                //Validation code of Tab 2 here
                $scope.jiniData.Validations = validation.validateTrialLicense($scope.jiniData.Trial);

                //If validation errors are there - just stop
                if ($scope.jiniData.Validations.length > 0) {
                    $("#divLoading").removeClass("show");
                    $scope.$apply();
                    return;
                }

                $state.go('approve');
                $rootScope.state = 'approve';
                break;
            case 'approve':
                $state.go('published');
                $rootScope.state = 'published';
                break;
            case 'published':
                break;
        }

        window.scrollTo(0, 0);
        if (!$scope.$$phase && !$scope.$root.$$phase) { $scope.$apply(); }
        $rootScope.bindTextBoxToAllSelect();
    });

    $("#wizard-prev-button").click(function () {
        $("#divLoading").addClass("show");

        if ($scope.state == 'config') {
            $rootScope.state = 'create';
            $state.go('create');
            $("#divLoading").removeClass("show");
        }

        else if ($scope.state == 'trial') {
            if ($scope.jiniData.HasTrial && $scope.jiniData.SalesForm.length == 0 && $scope.jiniData.AccessForm.length == 0 && $scope.jiniData.Periods.length == 0) {
                $rootScope.state = 'create';
                $state.go('create');
            }
            else {
                $rootScope.state = 'config';
                $state.go('config');
            }

            $("#divLoading").removeClass("show");
        }
        else if ($scope.state == 'approve') {
            $("#stage1").addClass("current");
            if ($scope.jiniData.HasTrial) {
                $rootScope.state = 'trial';
                $state.go('trial');
            } else {
                $rootScope.state = 'config';
                $state.go('config');
            }
            $("#divLoading").removeClass("show");
        }
        $("#divLoading").removeClass("show");
        window.scrollTo(0, 0);
        $scope.$apply();
    });

    $scope.SaveDraft = function () {

        if ($scope.jiniData.AccessForm.length == 1 && $scope.jiniData.AccessForm[0].DisplayName == "Friteksfelt") {

            sessionStorage.setItem("_defaultPeriodValues", JSON.stringify($scope.jiniData.LookupDefaultPeriods));

            sessionStorage.removeItem("_selectedPeriods");
            $scope.jiniData.Periods = [];
            $scope.jiniData.SalesForm = [];
        } else {

            sessionStorage.setItem("_defaultPeriodValues", JSON.stringify($scope.jiniData.LookupDefaultPeriods));

            if ($scope.jiniData.Periods.length > 0) {
                sessionStorage.setItem("_selectedPeriods", JSON.stringify($scope.jiniData.Periods));
            }
        }

        if (validation.validateDraft($scope.jiniData))
            salesconfiguration.Save(false, $scope.jiniData).then(function (data) {
                if (data == true)
                    $("#wizard-next-button").click();
                else if (data == false) {
                    window.onbeforeunload = null;

                }

                stateChanged = false;
                $scope.jiniData.DigitalTitle.HasDraft = true;
                $('#confirmation-popup').data('kendoWindow').center().close();
                $scope.openAbandonSales("product-history");
            });
    };
    $scope.Save = function (approved) {

        if ($scope.jiniData.AccessForm.length == 1 && $scope.jiniData.AccessForm[0].DisplayName == "Friteksfelt") {

            sessionStorage.setItem("_defaultPeriodValues", JSON.stringify($scope.jiniData.LookupDefaultPeriods));

            sessionStorage.removeItem("_selectedPeriods");
            $scope.jiniData.Periods = [];
            $scope.jiniData.SalesForm = [];
        } else {

            sessionStorage.setItem("_defaultPeriodValues", JSON.stringify($scope.jiniData.LookupDefaultPeriods));

            sessionStorage.setItem("_selectedPeriods", JSON.stringify($scope.jiniData.Periods));
        }

        salesconfiguration.Save(approved, $scope.jiniData).then(function (data) {
            if (data == true)
                $("#wizard-next-button").click();
            else if (data == false) {
                window.onbeforeunload = null;
            }
            stateChanged = false;
            $scope.jiniData.DigitalTitle.HasDraft = !approved;
            $('#confirmation-popup').data('kendoWindow').center().close();
            $scope.openAbandonSales("product-history");
        });
    };

    $scope.gobacktoDashboard = function () {
        stateChanged = false;
        $scope.jiniData.DigitalTitle.HasDraft = false;
        $('#confirmation-popup').data('kendoWindow').center().close();
        $scope.openAbandonSales("product-history");
    }
    function initUnloadEvent() {
        stateChanged = true;
        window.onbeforeunload = function () {
            return "Ændringer du har foretaget gemmes muligvis ikke.";
        };
    }

    function startup() {
        $("#divLoading").addClass("show");
        var getSalesConfiguratioPromise;
        $scope.jiniData.DigitalTitle = ViewModel;
        if ($scope.jiniData.DigitalTitle.SubTitle == null || $scope.jiniData.DigitalTitle.SubTitle == undefined) {
            $scope.jiniData.DigitalTitle
                .TitleAndMediaType = $scope.jiniData.DigitalTitle.Title +
                " - " +
                $scope.jiniData.DigitalTitle.MediaTypeName;
        } else {
            $scope.jiniData.DigitalTitle
                .TitleAndMediaType = $scope.jiniData.DigitalTitle.Title +
                " - " +
                $scope.jiniData.DigitalTitle.SubTitle +
                " - " +
                $scope.jiniData.DigitalTitle.MediaTypeName;
        }

        $scope.jiniData.DigitalTitle.Cover = __jiniEnv.mmsUrl + $scope.jiniData.DigitalTitle.Isbn;
        jiniservice.exists($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName, false).then(function (existingConfiguration) {
            $rootScope.existingConfiguration = existingConfiguration;
            //if (existingConfiguration.indexOf(_approveCode) > -1) {
            //    $state.go('dashboard');

            //}
            //else {
            //    if (existingConfiguration.indexOf(_draftCode) > -1) {
            //        getSalesConfiguratioPromise = jiniservice
            //        .getSalesConfiguration($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName, false);
            //        getSalesConfiguratioPromise.then(function (data) {
            //            $scope.refreshDataStructures();
            //            salesconfiguration.parseSalesConfiguration(data, $scope.jiniData)
            //                .then(function () {
            //                    $state.go("dashboard");
            //                });
            //        });
            //    }
            //    else
            $rootScope.state = "dashboard";
            $state.go("dashboard");
            //}
        });
    };

    $rootScope.bindTextBoxToAllSelect = function () {
        $timeout(function () {
            var inputs = $("input[type=text]");
            inputs.bind("focus", function () {
                var input = $(this);
                clearTimeout(input.data("selectTimeId")); //stop started time out if any

                var selectTimeId = setTimeout(function () {
                    input.select();
                });

                input.data("selectTimeId", selectTimeId);
            }).blur(function () {
                clearTimeout($(this).data("selectTimeId")); //stop started timeout
            });
            inputs.bind("focusout",
                function () {
                    var textbox = $(this);
                    if (textbox.val() == "" && textbox.parents("#email-hyperlink").length === 0) {
                        textbox.val("0,00");
                    }
                });
        });
    }

    $scope.parseSalesConfiguration = function (config) {
        return salesconfiguration.parseSalesConfiguration(config, $scope.jiniData);
    };

    $scope.hidePopops = function () {
        initUnloadEvent();
        window.popUpCaller = "";
        if ($("#confirmation-popup").data("kendoWindow") != null)
            $("#confirmation-popup").data("kendoWindow").close();

        if ($("#new-version-warning-popup").data("kendoWindow") != null)
            $("#new-version-warning-popup").data("kendoWindow").close();
    }

    $scope.openAbandonSales = function (redirect) {
        if (redirect == undefined) {
            if ($rootScope.Approved == true) {
                redirect = "product-history";
            }
            else {
                redirect = "product-history";
            }
        }
        window.popUpRedirect = redirect;
        window.onbeforeunload = null;
        if (stateChanged == true)
            $('#confirmation-popup').data('kendoWindow').center().open();

        else if (redirect == "product-history") {
            if ($scope.state == 'approve') {
                $rootScope.state = 'dashboard';
                $state.go('dashboard');
            }
            else if ($scope.state == 'published') {
                $rootScope.state = 'dashboard';
                $state.go('dashboard');
            }
            else {
                $rootScope.state = 'dashboard';
                $state.go('dashboard');
            }
        }
        else if (redirect == "product-list") {
            //Destroy session when user leave page
            sessionStorage.clear();
            if ($scope.jiniData.DigitalTitle.Version == "V1") {
                window.location = "/";
            }
            else {
                window.location = "/JiniV2";
            }
        }
    }

    $scope.GemIkke = function () {
        window.onbeforeunload = null;
        if (window.popUpRedirect == "product-list") {

            //Destroy session when user leave page
            sessionStorage.clear();
            if ($scope.jiniData.DigitalTitle.Version == "V1") {
                window.location = "/";
            }
            else {
                window.location = "/JiniV2";
            }
            return;
        }
        else if (window.popUpRedirect == "product-history") {
            var getSalesConfiguratioPromise = jiniservice
                .getSalesConfiguration($scope.jiniData.DigitalTitle.Isbn, __jiniEnv.sellerName, false, $scope.jiniData.DigitalTitle.Version);
            getSalesConfiguratioPromise.then(function (data) {
                $scope.refreshDataStructures();
                $scope.hidePopops();
                $rootScope.state = 'dashboard';
                $state.go('dashboard');
                salesconfiguration.parseSalesConfiguration(data, $scope.jiniData)
                    .then(function () {
                    });
            });
        }
    }

    $scope.disableKendoGroupButton = function (condition) {
        if (condition == _teacher)
            return "disabled";
        return "";
    }

    ////Upon change of Sales Type, the entire configuration is refreshed
    $scope.refreshDataStructures = function () {
        $scope.jiniData.SalesForm = [];
        $scope.jiniData.AccessForm = [];
        $scope.jiniData.SchoolLicense = {};
        $scope.jiniData.TeacherLicense = {};
        $scope.jiniData.ClassLicense = {};
        $scope.jiniData.SingleUserLicense = {};
        $scope.jiniData.ContactSales.DescriptionText = "";
        $scope.jiniData.ContactSales.DescriptionTextHtml = "";
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
});