(function () {
    'use strict';

    angular
        .module('jiniV2bulk')
        .factory('jiniV2bulkService', ['$http', '$q', '__jiniEnv', function ($http, $q, __jiniEnv) {
            var service = {};

            service.getForretningsOmrade = function () {
                var deferred = $.Deferred();
                $http.get(__jiniEnv.jiniApiUrl + 'v2/JiniV2/GetDepartmentsAndEditorial').success(function (data) {
                    deferred.resolve(data);
                }).error(function (status) {
                    alert("Call to server failed. Error status : " + status);
                });
                return deferred.promise();
            };

            service.getMediaMaterialType = function () {
                var deferred = $.Deferred();
                $http.get(__jiniEnv.jiniApiUrl + 'v2/JiniV2/GetMediaAndMaterialeTypes').success(function (data) {
                    deferred.resolve(data);
                }).error(function (status) {
                    alert("Call to server failed. Error status : " + status);
                });
                return deferred.promise();
            };


            service.PriceChanged = function (source, price, selectedRows,rows) {
                var updateSource = angular.copy(source);
                $(rows).each(function (e) {
                    var rowId = $(this).attr("id");
                   
                    var data = UpDateSelectedRowsPrice(updateSource, price, rowId)

                    $(this).find('.pris-u-mom').val(data.price);
                    $(this).find('.pris-m-mom').val(data.priceVat);

                });

            }

            service.saveSalesConfiguration = function (request, scope) {
                var d = $.Deferred();
                var jsonconfig = JSON.stringify(request);

                $http.post(__jiniEnv.jiniApiUrl + "/v1/JiniBulk/SaveDeflatedPrices/", jsonconfig)
                    .success(function (response) {
                        if (response.IsUpdated) {
                            var isbn = response.Isbn;
                            var refConfigTypeCode = GetRefConfigTypeCode(response.RowId);
                            var count = salesConfigurationWithCount[isbn + "-" + refConfigTypeCode];
                            totalRecordsSaved += count;
                            savedRows.push({ id: isbn + "-" + refConfigTypeCode });
                        } else {
                            if (response.RowId != undefined) {
                                var isbn = response.Isbn;
                                var refConfigTypeCode = GetRefConfigTypeCode(response.RowId);
                                var count = salesConfigurationWithCount[isbn + "-" + refConfigTypeCode];
                                totalRecordsFailed += count;
                                faildedRows.push({ id: isbn + "-" + refConfigTypeCode  });
                                
                            }
                        }
                        d.resolve(response);
                    })
                    .error(function (data, status) {
                        alert("Error status : " + status);
                        d.reject("error");
                    });
                return d.promise();

            }

            service.PriceChangedIncTax = function (source, percentage, selectedRows, rows) {
                var updateSource = angular.copy(source);

                var updateSource = angular.copy(source);
                $(rows).each(function (e) {
                    var rowId = $(this).attr("id");
                    var data = UpDateSelectedRowsPriceByPercentage(updateSource, percentage, rowId)
                    $(this).find('.pris-u-mom').val(data.price);
                    $(this).find('.pris-m-mom').val(data.priceVat);

                });
               
                return updateSource;
            }

            service.PriceChangedIncTaxForIndiviualRow = function (source, percentage, selectedRows) {
                var updateSource = angular.copy(source);

                for (var i = 0; i < selectedRows.length; i++) {
                    UpDateSelectedRowsPriceByPercentageByPercentage(updateSource, percentage, selectedRows[i])
                }

                return updateSource;
            }
            service.getDataSource = function () {
                var deferred = $.Deferred();
                $http.get(__jiniEnv.jiniApiUrl + 'v1/Jini/GetMediaAndMaterialeTypes').success(function (data) {
                    deferred.resolve(data);
                }).error(function (status) {
                    alert("Call to server failed. Error status : " + status);
                });
                return deferred.promise();
            };



            return service;
        }]);

    function UpDateSelectedRowsPrice(source, price, rowId) {
        var UnitPrice = 0;
        var UnitPriceVat = 0;
        var splitedId = rowId.split("_");
        for (var i = 0; i < source.length; i++) {

            var RefSalesConfigTypeCode = (source[i].RefSalesConfigTypeCode == null) ? "null" : source[i].RefSalesConfigTypeCode;
            var RefSalesCode = (source[i].RefSalesCode == null) ? "null" : source[i].RefSalesCode;
            var RefAccessFormCode = (source[i].RefAccessFormCode == null) ? "null" : source[i].RefAccessFormCode;
            var RefPeriodTypeCode = (source[i].RefPeriodTypeCode == null) ? "null" : source[i].RefPeriodTypeCode;
            var RefPriceModelCode = (source[i].RefPriceModelCode == null) ? "null" : source[i].RefPriceModelCode;
            var UnitValue = (source[i].UnitValue == null) ? "null" : source[i].UnitValue;

            if (source[i].Isbn == splitedId[0] && RefSalesConfigTypeCode == splitedId[1] && RefSalesCode == splitedId[2]
                && RefAccessFormCode == splitedId[3] && RefPeriodTypeCode == splitedId[4] && RefPriceModelCode == splitedId[5]
                && UnitValue == splitedId[6]) {
                UnitPrice = source[i].UnitPrice + price;
                UnitPriceVat = (UnitPrice * 1.25).toFixed(2);
                break
            }
        }
        return { price: UnitPrice, priceVat: UnitPriceVat};
    }

    function UpDateSelectedRowsPriceByPercentage(source, price, rowId) {
        var UnitPrice = 0;
        var UnitPriceVat = 0;
        var splitedId = rowId.split("_");
        for (var i = 0; i < source.length; i++) {

            var RefSalesConfigTypeCode = (source[i].RefSalesConfigTypeCode == null) ? "null" : source[i].RefSalesConfigTypeCode;
            var RefSalesCode = (source[i].RefSalesCode == null) ? "null" : source[i].RefSalesCode;
            var RefAccessFormCode = (source[i].RefAccessFormCode == null) ? "null" : source[i].RefAccessFormCode;
            var RefPeriodTypeCode = (source[i].RefPeriodTypeCode == null) ? "null" : source[i].RefPeriodTypeCode;
            var RefPriceModelCode = (source[i].RefPriceModelCode == null) ? "null" : source[i].RefPriceModelCode;
            var UnitValue = (source[i].UnitValue == null) ? "null" : source[i].UnitValue;

            if (source[i].Isbn == splitedId[0] && RefSalesConfigTypeCode == splitedId[1] && RefSalesCode == splitedId[2]
                && RefAccessFormCode == splitedId[3] && RefPeriodTypeCode == splitedId[4] && RefPriceModelCode == splitedId[5]
                && UnitValue == splitedId[6]) {
                var perPrice = ((price / 100) * Math.round(source[i].UnitPrice)).toFixed(2);
                var newUnitPrice = parseFloat(Math.round(source[i].UnitPrice)) + parseFloat(perPrice);
                source[i].UnitPrice = Math.round(newUnitPrice);
                source[i].UnitPriceVat = (source[i].UnitPrice * 1.25).toFixed(2);
                UnitPrice = Math.round(newUnitPrice);
                UnitPriceVat = (source[i].UnitPrice * 1.25).toFixed(2);
                break;
            }
        }
        return { price: UnitPrice, priceVat: UnitPriceVat };
    }

    function UpDateSelectedRowsPriceByPercentageByPercentage(source, price, rowId) {
        var splitedId = rowId.split("_");
        for (var i = 0; i < source.length; i++) {

            var RefSalesConfigTypeCode = (source[i].RefSalesConfigTypeCode == null) ? "null" : source[i].RefSalesConfigTypeCode;
            var RefSalesCode = (source[i].RefSalesCode == null) ? "null" : source[i].RefSalesCode;
            var RefAccessFormCode = (source[i].RefAccessFormCode == null) ? "null" : source[i].RefAccessFormCode;
            var RefPeriodTypeCode = (source[i].RefPeriodTypeCode == null) ? "null" : source[i].RefPeriodTypeCode;
            var RefPriceModelCode = (source[i].RefPriceModelCode == null) ? "null" : source[i].RefPriceModelCode;
            var UnitValue = (source[i].UnitValue == null) ? "null" : source[i].UnitValue;

            if (source[i].Isbn == splitedId[0] && RefSalesConfigTypeCode == splitedId[1] && RefSalesCode == splitedId[2]
                && RefAccessFormCode == splitedId[3] && RefPeriodTypeCode == splitedId[4] && RefPriceModelCode == splitedId[5]
                && UnitValue == splitedId[6]) {
                var unitPrice = parseFloat(price) / parseFloat(1.25)
                source[i].UnitPrice = Math.round(unitPrice);
                source[i].UnitPriceVat = price;
            }
        }
        return source;
    }

    function GetRefConfigTypeCode(rowId) {
        var keys = rowId.split("_");
        return keys[1];
    }

})();