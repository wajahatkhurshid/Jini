'use strict';
/**************************************************************************

  * Controller for the create of  sales configuration

  *************************************************************************/

jiniApp.controller('create',
    function ($rootScope,
        $scope,
        jiniservice,
        salesformService,
        accessformService,
        periodService,
        dataFactory) {
        $("#divLoading").removeClass('show');

        $scope.jiniData = dataFactory;
        $rootScope.state = 'create';
        /* Lookup variables will be used by Kendo Controls for population */

        jiniservice.getSalesTypes()
            .then(function (data) {
                $scope.jiniData.LookupSalesTypes = data;
            });
        jiniservice.getAccessTypes().then(function (data) {
            $scope.jiniData.LookupAccessTypes = data;
        });

        jiniservice.getSellerId(__jiniEnv.sellerName).then(function (data) {
            $scope.jiniData.Seller = data;
        });

        // init periodUnitType
        if ($scope.jiniData.SalesForm && $scope.jiniData.SalesForm.length > 0) {
            $scope.jiniData.lookupPeriodUnitType = periodService.getLookupPeriodUnitType($scope.jiniData.SalesForm[0].Code, 1);

          

            //When user move back in wizard and comeback again, this objects gets null
            //so If model has values for LookupDefaultPeriods then store it in
            //local storage for later use when user move back and forth in wizard
            if ($scope.jiniData.LookupDefaultPeriods.length > 0){

                sessionStorage.setItem("_defaultPeriodValues", JSON.stringify($scope.jiniData.LookupDefaultPeriods));

                sessionStorage.setItem("_selectedPeriods", JSON.stringify($scope.jiniData.Periods));
                
            }

            //if this object found null on any step in wizard then populate it from local storage
            if ($scope.jiniData.LookupDefaultPeriods.length <= 0) {
                $scope.jiniData.LookupDefaultPeriods = JSON.parse(sessionStorage.getItem("_defaultPeriodValues"));
                if ($scope.jiniData.Periods.length < 0) {
                    $scope.jiniData.Periods = localStorage.getItem("_selectedPeriods") == null ? [] : JSON.parse(localStorage.getItem("_selectedPeriods"));
                }
                //select multi select kendo with already selected values from model if any
                if ($scope.jiniData.Periods != null && $scope.jiniData.Periods.length > 0) {

                    selectPeriodsFromModel($scope.jiniData.Periods);
                }
            }
        }

        function selectPeriodsFromModel(selectedValues) {
            var displayNames = [];
            
            for (var i = 0; i < selectedValues.length; i++) {
                displayNames.push(selectedValues[i].DisplayName);
            }

            if (displayNames.length > 0) {

                //wait for 1 sec and let UI render multi select otherwise it will throw error
                setTimeout(function () {
                    $("#select-period").data("kendoMultiSelect").value(displayNames);
                }, 1000);
               
            }
        }

        /* Page 1 */
        $scope.selectOptions = {
            placeholder: "Tilføj perioder…"
        };

        $scope.setSelectedSales = function (displayName, code) {
            initUnloadEvent();
            salesformService.setSalesForm(displayName, code, $scope.jiniData);
            $scope.jiniData.lookupPeriodUnitType = periodService.getLookupPeriodUnitType(code, 1);
            periodService.setPeriodAccordingToSaleForm(code, $scope.jiniData.LookupDefaultPeriods, $scope.jiniData.Periods);
        };
        $scope.setSelectedAccess = function (displayName, code) {
            initUnloadEvent();
            accessformService.setAccessForm(displayName, code, $scope.jiniData);
        };
        $scope.OnPeriodOpen = function (e) {
            if ($scope.jiniData.SalesForm.length == 0) {
                e.preventDefault();
                $('#period-warning').data('kendoWindow').center().open();
            }
            else {
                var multiSelectInput = $(e.sender.element).prev().find(".k-input");
                multiSelectInput.attr("placeholder", "");
                multiSelectInput.css("width", "auto");
            }
        }

        $scope.OnPeriodClose = function (e) {
            var multiSelectInput = $(e.sender.element).prev().find(".k-input");
            if ($scope.jiniData.Periods.length > 0) {
                multiSelectInput.attr("placeholder", "Tilføj flere perioder…");
            }
            multiSelectInput.addClass("multiselect-input-override");
        }

        $scope.onHasTrial = function () {
            initUnloadEvent();
        };

        $scope.OnPeriodChange = function () {
            // Declared with other private variables at the bottom
            // ReSharper disable once VariableUsedInInnerScopeBeforeDeclared
            $scope.jiniData.Periods = $scope.jiniData.Periods;
            initUnloadEvent();
            var difference = [];
            var currentData = $scope.jiniData.Periods;

            if (window.previousData.length == 0) {
                difference.push($scope.jiniData.Periods);
            } else {
                jQuery.grep(window.previousData, function (el) {
                    if (jQuery.inArray(el, currentData) == -1) difference.push(el);
                });
            }

            window.previousData = $scope.jiniData.Periods;
        }

        //Custom Period Handling ...
        $scope.inititalizeCustomPeriods = function () {
            //Cleanup custom periods
            for (var index = 0; index < $scope.jiniData.CustomPeriods.length; index++) {
                var indexOfValidationMessage = $scope.jiniData.Validations.indexOf("customperiod:interconflict:duplicate" + index);
                if (indexOfValidationMessage > -1)
                    $scope.jiniData.Validations.splice(indexOfValidationMessage, 1);
                indexOfValidationMessage = $scope.jiniData.Validations.indexOf("customperiod:intraconflict:duplicate" + index);
                if (indexOfValidationMessage > -1)
                    $scope.jiniData.Validations.splice(indexOfValidationMessage, 1);
            }

            $scope.jiniData.CustomPeriods = [];

            if ($scope.jiniData.CustomPeriods.length == 0) {
                $scope.jiniData.CustomPeriods.push(createCustomPeriod(0));
            }
            else {
                angular.forEach($scope.jiniData.CustomPeriods, function (item, key) {
                    if (key == $scope.jiniData.CustomPeriods.length - 1)
                        item.showAddButton = true;
                    else
                        item.showAddButton = false;
                    if (key == 0 && $scope.jiniData.CustomPeriods.length > 1)
                        item.showRemoveButton = true;
                    else if (key == 0)
                        item.showRemoveButton = false;
                    else
                        item.showRemoveButton = true;
                });
            }
            $rootScope.bindTextBoxToAllSelect();
        };

        $scope.onAddCustomPeriod = function () {
            //stateChanged = true;
            initUnloadEvent();
            angular.forEach($scope.jiniData.CustomPeriods, function (item) {
                if (item.showAddButton == true)
                    item.showAddButton = false;
                if (item.showRemoveButton == false)
                    item.showRemoveButton = true;
            });
            $scope.jiniData.CustomPeriods.push(createCustomPeriod());
            $rootScope.bindTextBoxToAllSelect();
        };

        $scope.onCustomPeriodInputChange = function (item) {
            item.lookupPeriodUnitType = periodService.getLookupPeriodUnitType($scope.jiniData.SalesForm[0].Code, this.numeric.value());
            //$('#select-period').data('kendoMultiSelect').dataSource.read();
        };

        function createCustomPeriod(index) {
            var highest = 0;
            return { Id: highest, periodUnitCount: "1", periodUnitType: { Code: 1002, DisplayName: "" }, lookupPeriodUnitType: $scope.jiniData.lookupPeriodUnitType, showAddButton: true, showRemoveButton: index !== 0 }
        }

        $scope.onRemoveCustomPeriod = function (index) {
            var indexOfValidationMessage = $scope.jiniData.Validations.indexOf('customperiod:interconflict:duplicate' + index);
            if (indexOfValidationMessage > -1)
                $scope.jiniData.Validations.splice(indexOfValidationMessage, 1);
            indexOfValidationMessage = $scope.jiniData.Validations.indexOf('customperiod:intraconflict:duplicate' + index);
            if (indexOfValidationMessage > -1)
                $scope.jiniData.Validations.splice(indexOfValidationMessage, 1);

            for (var i = index + 1; i < $scope.jiniData.CustomPeriods.length; i++) {
                indexOfValidationMessage = $scope.jiniData.Validations.indexOf('customperiod:interconflict:duplicate' + i);
                if (indexOfValidationMessage > -1) {
                    $scope.jiniData.Validations.splice(indexOfValidationMessage, 1);
                    $scope.jiniData.Validations.push('customperiod:interconflict:duplicate' + (i - 1));
                }

                indexOfValidationMessage = $scope.jiniData.Validations.indexOf('customperiod:intraconflict:duplicate' + i);
                if (indexOfValidationMessage > -1) {
                    $scope.jiniData.Validations.splice(indexOfValidationMessage, 1);
                    $scope.jiniData.Validations.push('customperiod:intraconflict:duplicate' + (i - 1));
                }
            }

            $scope.jiniData.CustomPeriods.splice(index, 1);
            if ($scope.jiniData.CustomPeriods.length == 1) {
                $scope.jiniData.CustomPeriods[0].showRemoveButton = false;
            }
            $scope.jiniData.CustomPeriods[$scope.jiniData.CustomPeriods.length - 1].showAddButton = true;
        };

        $scope.onAppendCustomPeriods = function () {
            //stateChanged = true;
            initUnloadEvent();
            $scope.jiniData.Validations = [];

            //Step 1: check inter-item Custom Period duplicates
            var array = $scope.jiniData.CustomPeriods;
            var valuesSoFar = [];
            var value;
            var i;
            for (i = 0; i < array.length; ++i) {
                value = array[i];
                if (value.periodUnitCount > 0 && valuesSoFar.filter(function (item) {
                    return (item.periodUnitCount == value.periodUnitCount &&
                        item.periodUnitType.Code == value.periodUnitType.Code);
                }).length > 0) {
                    //if ($rootScope.Validations.indexOf('customperiod:interconflict:duplicate'+i)==-1)
                    $scope.jiniData.Validations.push('customperiod:interconflict:duplicate' + i);
                }
                valuesSoFar.push(value);
            }
            //Step 2: check intra-item Custom Period duplicates with non-custom existing periods
            array = $scope.jiniData.CustomPeriods;
            valuesSoFar = [];
            for (i = 0; i < array.length; ++i) {
                value = array[i];
                if (value.periodUnitCount > 0 && $scope.jiniData.Periods.filter(function (item) {
                    return (item.UnitValue == value.periodUnitCount &&
                        item.RefPeriodUnitTypeCode == value.periodUnitType.Code);
                }).length > 0) {
                    //if ($rootScope.Validations.indexOf('customperiod:intraconflict:duplicate' + i) == -1)
                    $scope.jiniData.Validations.push("customperiod:intraconflict:duplicate" + i);
                }
                valuesSoFar.push(value);
            }

            // if no validation error then
            if ($scope.jiniData.Validations.length == 0) {
                //Append new custom periods
                angular.forEach($scope.jiniData.CustomPeriods, function (item) {
                    if (item.periodUnitCount > 0) {
                        var periodUnitTypeName = item.lookupPeriodUnitType.filter(function (p) { return p.Code == item.periodUnitType.Code })[0].DisplayName;
                        item.periodUnitType.DisplayName = periodUnitTypeName;
                        var x = {
                            PeriodTypeCode: $scope.jiniData.LookupDefaultPeriods[0].PeriodTypeCode,
                            UnitValue: item.periodUnitCount,
                            RefPeriodUnitTypeCode: item.periodUnitType.Code,
                            RefSalesFormCode: $scope.jiniData.LookupDefaultPeriods[0].RefSalesFormCode,
                            Id: item.Id,
                            "Price": null,
                            Code: 0,
                            DisplayName: item.periodUnitCount + " "
                                + periodUnitTypeName + " "
                                + (($scope.jiniData.SalesForm[0].Code == _subscription) ? "binding" : "adgang"),
                            IsCustomPeriod: true
                        };
                        // push in drop down list
                        $scope.jiniData.LookupDefaultPeriods.push(x);

                        $scope.jiniData.Periods.push(x);
                    }
                });
                //Reload the kendo drop down list
                $("#select-period").data("kendoMultiSelect").dataSource.read();
                //Close the kendo window
                $("#custom-period-popup").data("kendoWindow").close();
            }
        };
        $scope.closeCustomPeriodPopup = function () {
            if ($("#custom-period-popup").data("kendoWindow") != null)
                $("#custom-period-popup").data("kendoWindow").destroy();
        };

        wizardPageOne();
    });