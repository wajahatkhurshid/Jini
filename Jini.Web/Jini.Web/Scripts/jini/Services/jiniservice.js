'use strict';

/**************************************************************************

  * Service for looking up and saving data from Jini service.

  *************************************************************************/

// ReSharper disable once InconsistentNaming
jiniApp.factory("jiniservice", function ($http, __jiniEnv) {

    return {
        //get lookup data from RefSalesForm table
        getSalesTypes: function () {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "/v1/SalesForm/GetRefSalesForms")
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Call to server failed. Error status : " + status);
                });;
            return d.promise();

        },

        //get lookup data from RefAccessForms table
        getAccessTypes: function () {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "/v1/AccessForm/GetRefAccessForms")
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                });;
            return d.promise();
        },

        //get lookup data from RefPeriods table based upon the selected SalesForm
        getDefaultPeriods: function (salesForm) {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "/v1/Period/GetRefPeriods/" + salesForm)
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                });;
            return d.promise();
        },

        //get trial accessforms from jini service
        getTrialAccessForms: function () {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "/v1/Trial/RefAccessForms/")
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                });;
            return d.promise();
        },

        //get trial period unit types from jini service
        getTrialPeriodUnitTypes: function () {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "/v1/Trial/RefPeriodUnitTypes/")
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                });;
            return d.promise();
        },

        //get lookup data from RefPriceModels table based upon the selected AccessForms
        getBillingModel: function (accessForm) {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "/v1/PriceModel/GetRefPriceModels/" + accessForm)
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });;
            return d.promise();
        },

        //get period unit types
        getPeriodUnitType: function () {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "/v1/Period/GetRefPeriodUnitTypes")
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });
            return d.promise();
        },

        //get period unit types
        getCountUnitType: function () {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "/v1/Trial/RefCountUnitTypes")
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });
            return d.promise();
        },

        //Save a draft or approved sales configuration to JiniService as JSON
        saveSalesConfiguration: function (configuration, approved) {
            var d = $.Deferred();
            var jsonconfig = JSON.stringify(configuration);
            $http.post(__jiniEnv.jiniApiUrl + "/v1/SalesConfiguration/Save/", jsonconfig)
                .success(function () {
                    d.resolve(approved);
                })
                .error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject("error");
                });
            return d.promise();
        },

        //Check from Service if Grade levels are Defined in RAP
        gradeLevelsExists: function (isbn,version) {
            var d = $.Deferred();
            if (version === "V2") {
                $http.get(__jiniEnv.jiniApiUrl + "v2/ProductV2/GetGradeLevels/" + isbn)
                    .success(function (data) {
                        d.resolve(data);
                    }).error(function (data) {
                        d.reject(data);
                    });
            } else {
                $http.get(__jiniEnv.jiniApiUrl + "v1/Product/GetGradeLevels/" + isbn)
                    .success(function (data) {
                        d.resolve(data);
                    }).error(function (data) {
                        d.reject(data);
                    });
            }
            return d.promise();
        },

        //Check from Service if SalesConfiguration of the given ISBN, for the given Seller exists in either approved or draft state.
        exists: function (isbn, seller, approved) {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "v1/SalesConfiguration/Exists/" + isbn + "/" + seller)
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });
            return d.promise();
        },

        //If configuration already exists, get the configuration Depending upon approved / draft status for the given product
        getSalesConfiguration: function (isbn, seller, approved, version) {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "v1/SalesConfiguration/Get/" + isbn + "/" + seller + "/" + version + "/" + approved)
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });
            return d.promise();
        },

        getSalesConfigurationHistory: function (isbn) {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "v1/SalesConfigurationRevisionHistory/Get/" + isbn)
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });
            return d.promise();
        },
        getGuaSalesConfigurationHistory: function (isbn) {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "v1/GuaSalesConfigurationRevisionHistory/Get/" + isbn)
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });
            return d.promise();
        },
        getSellerId: function (sellerName) {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "v1/Seller/Get/" + sellerName)
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });
            return d.promise();
        },
        getTrialLicenseShareLink: function (isbn, seller) {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "v1/Trial/ShareLink/Isbn/" + isbn + "/Seller/" + seller)
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });
            return d.promise();
        },
        GetRefAccessProviders: function () {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "v1/AccessProvider/GetRefAccessProviders")
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });
            return d.promise();
        },
        GetAllProductAccessProviders: function (isbn) {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "v1/Product/GetAllProductAccessProviders/" + isbn)
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });
            return d.promise();
        },
        SaveProductAccessProviders: function(data_) {
            var d = $.Deferred();
            $http.post(__jiniEnv.jiniApiUrl + "v1/Product/SaveProductAccessProviders/",JSON.stringify(data_))
                .success(function (data) {
                    d.resolve(data);
                }).error(function (data, status) {
                    alert("Error status : " + status);
                    d.reject();
                });
            return d.promise();
        }
    }
});