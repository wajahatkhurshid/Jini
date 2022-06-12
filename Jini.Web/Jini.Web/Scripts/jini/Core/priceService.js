'use strict';
/**************************************************************************

  * Price calculation module. Calculates price based on given tax rate

  *************************************************************************/

jiniApp.factory('priceService',
    function (jiniConfig) {

        return {
            priceChanged: function (period, caller, license) {
                //stateChanged = true;
                initUnloadEvent();
                var str;
                var taxablePrice;
                var price;

                str = license.Pricing[period].ExTax.toString();
                if (str == "") str = "0";
                price = kendo.parseFloat(str);
                license.Pricing[period].ExTax = kendo.toString(price, "n2");
                taxablePrice = price * jiniConfig.PercentValue;
                license.Pricing[period].IncTax = kendo.toString(taxablePrice, "n2");

            },

            priceChangedIncTax: function (period, caller, license) {
                //stateChanged = true;
                initUnloadEvent();
                var str;
                var originalPrice;
                var price;

                str = license.Pricing[period].IncTax.toString();
                if (str == "") str = "0";
                price = kendo.parseFloat(str);
                license.Pricing[period].IncTax = kendo.toString(price, "n2");
                originalPrice = price / jiniConfig.PercentValue;
                license.Pricing[period].ExTax = kendo.toString(originalPrice, "n2");
                if (str.slice(-1) !== ",") {
                    //$timeout(function () {
                    //    //any code in here will automatically have an apply run afterwards
                    //    $scope.$apply();
                    //});
                }
            }
        }

    });
function parseFloatOpts(str) {

    if (typeof str === "number") {
        return str;
    }

    var ar = str.split(/\.|,/);

    var value = '';
    for (var i in ar) {
        if (i > 0 && i == ar.length - 1) {
            value += ".";
        }
        value += ar[i];
    }
    return Number(value);
}