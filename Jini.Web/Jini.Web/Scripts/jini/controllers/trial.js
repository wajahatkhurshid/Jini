'use strict';
/**************************************************************************

  * Controller for the trial Sales Configuration

  *************************************************************************/

jiniApp.controller('trial',
    function ($rootScope,
        $scope,
        accessformService,
        jiniservice,
        dataFactory) {
        //setup pages
        $scope.jiniData = dataFactory;
        $rootScope.state = 'trial';
        trialPageSetup();

        jiniservice.getTrialAccessForms().then(function (result) {
            $scope.lookupTrialAccessForms = result;
            initialize();
        });

        jiniservice.getTrialPeriodUnitTypes().then(function (result) {
            $scope.lookupTrialPeriodUnitTypes = result;
            var dropdownlist;
            if (Object.keys($scope.jiniData.Trial.Period).length !== 0 &&
                $scope.jiniData.Trial.Period.constructor === Object) {
                dropdownlist = $("#period-type").data("kendoDropDownList");
                if (dropdownlist != null) {
                    //bind the change event here and whenever it's changed set the value in variables as well
                    setTimeout(function () {
                        if ($scope.jiniData.Trial.Period.UnitType != null)
                            dropdownlist.value($scope.jiniData.Trial.Period.UnitType.Code);
                        else {
                            dropdownlist.select(0);
                            dropdownlist.trigger("change");
                        }
                        $scope.checkNullValue();
                    }, 50);
                }
            } else {
                dropdownlist = $("#period-type").data("kendoDropDownList");
                if (dropdownlist != null) {
                    $scope.jiniData.Trial.Period.UnitValue = 1;
                    //bind the change event here and whenever it's changed set the value in variables as well
                    //dropdownlist.bind("change", valueChangedForPeriodType);
                    setTimeout(function () {
                        dropdownlist.select(0);
                        dropdownlist.trigger("change"); //this will trigger change event
                    }, 50);
                }
            }
        });

        jiniservice.getCountUnitType().then(function (result) {
            $scope.lookupTrialCountUnitTypes = result;
            var dropdownlist;
            if (Object.keys($scope.jiniData.Trial.TrialAccessCount).length !== 0 && $scope.jiniData.Trial.TrialAccessCount.constructor === Object) {
                // Search in the datasource and bind the value
                dropdownlist = $("#trial-count").data("kendoDropDownList");
                if (dropdownlist != null) {
                    //bind the change event here and whenever it's changed set the value in variables as well
                    setTimeout(function () {
                        if ($scope.jiniData.Trial.TrialAccessCount.UnitType != null)
                            dropdownlist.value($scope.jiniData.Trial.TrialAccessCount.UnitType.Code);
                        else {
                            dropdownlist.select(0);
                            dropdownlist.trigger("change");
                        }
                        $scope.checkNullValue();
                    }, 50);
                }
            } else {
                dropdownlist = $("#trial-count").data("kendoDropDownList");
                $scope.jiniData.Trial.TrialAccessCount.UnitValue = 3;
                if (dropdownlist != null) {
                    //bind the change event here and whenever it's changed set the value in variables as well
                    setTimeout(function () {
                        dropdownlist.select(0);
                        dropdownlist.trigger("change"); //this will trigger change event
                    }, 50);
                }
            }
        });
        $scope.setSelectedTrialAccess = function (displayName, code) {
            initUnloadEvent();
            accessformService.setTrialAccessForm(displayName, code, $scope.jiniData);
        };

        $scope.maxTrials = function (selection) {
            initUnloadEvent();
            $scope.jiniData.Trial.MultipleTrialsPerUser = selection;
        };

        $scope.hasContactSales = function (selection) {
            initUnloadEvent();
            $scope.jiniData.Trial.TrialContactSales = selection;
        };

        $scope.checkNullValue = function () {
            initUnloadEvent();

            if ($scope.jiniData.Trial.Period.UnitValue === null || $scope.jiniData.Trial.Period.UnitValue <= 0) {
                $scope.jiniData.Trial.Period.UnitValue = 1;
            }

            if ($scope.jiniData.Trial.TrialAccessCount.UnitValue === null || $scope.jiniData.Trial.TrialAccessCount.UnitValue <= 0) {
                $scope.jiniData.Trial.TrialAccessCount.UnitValue = 3;
            }
        };

        function initialize() {
            if (Object.keys($scope.jiniData.Trial.AccessForm).length === 0 && $scope.jiniData.Trial.AccessForm.constructor === Object) {
                var access;
                for (var accessForm = 0; accessForm < $scope.jiniData.AccessForm.length; accessForm++) {
                    if ($scope.jiniData.AccessForm[accessForm].Code === _school ||
                        $scope.jiniData.AccessForm[accessForm].Code === _class ||
                        $scope.jiniData.AccessForm[accessForm].Code === _teacher) {
                        access = $.grep($scope.lookupTrialAccessForms,
                            function (e) { return e.Code === _school; })[0];
                        $scope.setSelectedTrialAccess(access.DisplayName, access.Code);
                        break;
                    }
                    if ($scope.jiniData.AccessForm[accessForm].Code === _singleUser) {
                        access = $.grep($scope.lookupTrialAccessForms,
                            function (e) { return e.Code === _singleUser; })[0];
                        $scope.setSelectedTrialAccess(access.DisplayName, access.Code);
                        break;
                    }
                    if ($scope.jiniData.AccessForm[accessForm].Code === _contactSales) {
                        access = $.grep($scope.lookupTrialAccessForms,
                            function (e) { return e.Code === _school; })[0];
                        $scope.setSelectedTrialAccess(access.DisplayName, access.Code);
                        break;
                    }
                }
                if ($scope.jiniData.AccessForm.length === 0) {
                    access = $.grep($scope.lookupTrialAccessForms,
                        function (e) { return e.Code === _school; })[0];
                    $scope.setSelectedTrialAccess(access.DisplayName, access.Code);
                }
            }

            $("#toggle-max-trials").data("kendoMobileButtonGroup").select(($scope.jiniData.Trial.MultipleTrialsPerUser === true) ? 0 : 1);
            $("#toggle-contact-sales-text").data("kendoMobileButtonGroup").select(($scope.jiniData.Trial.TrialContactSales === true) ? 0 : 1);
        };

        $scope.eventTypeDropDown = {
            placeholder: "Search ..."
        };

        $("#divLoading").removeClass("show");

        //variables
        $scope.lookupTrialAccessForms = [];
        $scope.lookupTrialPeriodUnitTypes = [];
        $scope.lookupTrialCountUnitTypes = [];
    });