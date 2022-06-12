'use strict';
/**************************************************************************

  * Period domain service

  *************************************************************************/

jiniApp.factory('periodService', ['$filter', function ($filter) {
    var removeInvalidPeriodForSubscription = function (periods) {
        var invalidPeriods = $filter("filter")(periods, function (obj) {
            return obj.RefPeriodUnitTypeCode == 0 || obj.RefPeriodUnitTypeCode == 1003 || obj.RefPeriodUnitTypeCode == 1004;
        });
        if (invalidPeriods.length >= 1) {
            invalidPeriods.forEach(function (element) {
                var idx = periods.indexOf(element);
                if (idx !== -1) {
                    periods.splice(idx, 1);
                }
            });
        }
    }
    return {
        convertType: function (lookupPeriods, periods, type, typeName, replaceWord) {
            var index = 0;
            for (index = 0; index < lookupPeriods.length; index++) {
                lookupPeriods[index].DisplayName = lookupPeriods[index].DisplayName.replace(replaceWord, typeName);
            }

            for (index = 0; index < periods.length; index++) {
                periods[index].DisplayName = periods[index].DisplayName.replace(replaceWord, typeName);
            }
        },
        addType: function (lookupPeriods, periods, type) {
            var index = 0;
            for (index = 0; index < lookupPeriods.length; index++) {
                lookupPeriods[index].DisplayName = lookupPeriods[index].DisplayName + ' ' + type;
            }
            for (index = 0; index < periods.length; index++) {
                periods[index].DisplayName = periods[index].DisplayName + ' ' + type;
            }
        },
        clear: function (scope) {
            var lenght = scope.Periods.length;
            for (var index = 0; index < lenght; index++) {
                removeObjectFromList(scope.Periods[index], scope.Periods);
            }
        },
        getLookupPeriodUnitType: function (saleFormCode, unitValue) {
            var lookupPeriodUnitTypeForOne = [{ "Code": 1001, "DisplayName": "års" }, { "Code": 1002, "DisplayName": "måneds" }, { "Code": 1003, "DisplayName": "dags" }, { "Code": 1004, "DisplayName": "times" }];
            var lookupPeriodUnitTypeForMultiple = [{ "Code": 1001, "DisplayName": "års" }, { "Code": 1002, "DisplayName": "måneders" }, { "Code": 1003, "DisplayName": "dages" }, { "Code": 1004, "DisplayName": "timers" }];

            // Removing days and times in case of subscription saleForm.
            if (saleFormCode == 1001) {
                lookupPeriodUnitTypeForOne.splice(-2);
                lookupPeriodUnitTypeForMultiple.splice(-2);
            }

            if (unitValue == 1) {
                return lookupPeriodUnitTypeForOne;
            }
            else {
                return lookupPeriodUnitTypeForMultiple;
            }
        },
        setPeriodAccordingToSaleForm: function (saleFormCode, lookupDefaultPeriods, periods) {
            // Removing days and times in case of subscription saleForm.
            if (saleFormCode == 1001) {
                // Removing from lookUp period list
                removeInvalidPeriodForSubscription(lookupDefaultPeriods);

                // Removing from selected period list
                removeInvalidPeriodForSubscription(periods);
            }
        }
    };
}]);