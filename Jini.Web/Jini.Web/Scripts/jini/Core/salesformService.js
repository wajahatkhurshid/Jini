'use strict';
/**************************************************************************

  * Validation for Jini Wizard. Validations user inputs based on each step of the wizard

  *************************************************************************/
jiniApp.factory('salesformService',
    function( periodService, jiniservice) {
        return {
            setSalesForm: function (displayName, code, scope) {
                var promise = $.Deferred();
                jiniservice.getSalesTypes().then(function (data) {
                    scope.LookupSalesTypes = data;

                    var salesObject = $.grep(scope.LookupSalesTypes, function (e) { return e.Code == code; });
                    salesObject = {
                        Code: salesObject[0].Code, DisplayName: salesObject[0].DisplayName, PeriodTypeName: salesObject[0].PeriodTypeName
                    };
                    var searchResult = $.grep(scope.SalesForm, function (e) { return e.Code == code; });
                    if (searchResult.length == 0) {
                        if (scope.SalesForm.length > 0)
                            var previousType = scope.SalesForm[0].PeriodTypeName;
                        for (var index = 0 ; index < scope.SalesForm.length; index++) {
                            removeObjectFromList(scope.SalesForm[index], scope.SalesForm, scope.SelectedSalesForm);
                        }

                        scope.SalesForm.push(salesObject);

                        // based upon the selected sales form, get the billing periods
                        if (scope.LookupDefaultPeriods.length === 0) {
                            jiniservice.getDefaultPeriods(code)
                                .then(function (data) {
                                    scope.LookupDefaultPeriods = data;
                                    periodService.addType(scope.LookupDefaultPeriods, scope.Periods, scope.SalesForm[0].PeriodTypeName);
                                    promise.resolve("true");
                                });
                        }
                        else {
                            periodService.convertType(scope.LookupDefaultPeriods, scope.Periods, code, scope.SalesForm[0].PeriodTypeName, previousType);
                            var stringifiedDefaultPeriods = JSON.stringify(scope.LookupDefaultPeriods);
                            var stringifiedPeriods = JSON.stringify(scope.Periods);
                            scope.LookupDefaultPeriods = [], scope.Periods = [];
                            scope.LookupDefaultPeriods = JSON.parse(stringifiedDefaultPeriods);
                            scope.Periods = JSON.parse(stringifiedPeriods);
                            //$timeout(function () {
                            //    $scope.$apply(function () {
                                    
                            //    });

                            //});

                        }
                        
                        return promise.promise();
                    }
                });

                return promise.promise();
            },
            clear: function (scope) {
                var lenght = scope.SalesForm.length;
                for (var index = 0 ; index < lenght; index++) {
                    removeObjectFromList(scope.SalesForm[index], scope.SalesForm, scope.SelectedSalesForm);
                }
            }
        };
    });