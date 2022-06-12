'use strict';
/**************************************************************************

  * SaleConfiguration service

  *************************************************************************/
jiniApp.factory('salesconfiguration',
    function (priceService,
        jiniservice,
        $sce,
        salesformService,
        accessformService,
        dataFactory) {
        return {
            Save: function (approved, scopeObject) {
                $("#divLoading").addClass('show');
                scopeObject.SalesConfiguration.SalesForms = [];
                scopeObject.SalesConfiguration.AccessForms = [];
                //if (scopeObject.SalesForm.length == 0) {
                //    scopeObject.Validations.push('tab1:salesform');
                //    return null;
                //}
                var index;
                for (index = 0; index < scopeObject.SalesForm.length; index++) {
                    scopeObject.SalesConfiguration.SalesForms
                        .push({
                            Code: scopeObject.SalesForm[index].Code,
                            DisplayName: scopeObject.SalesForm[index].DisplayName
                        });
                }
                var accessFormObject = {};
                for (index = 0; index < scopeObject.AccessForm.length; index++) {
                    accessFormObject = {};
                    accessFormObject.PriceModels = [];
                    accessFormObject.BillingPeriods = [];
                    accessFormObject.Code = scopeObject.AccessForm[index].Code;
                    accessFormObject.DisplayName = scopeObject.AccessForm[index].DisplayName;
                    var periodIndex;
                    switch (scopeObject.AccessForm[index].Code) {
                        case _school:
                            if (scopeObject.SchoolLicense.BillingModel != null) {
                                accessFormObject.PriceModels[0] = scopeObject.SchoolLicense.BillingModel;
                                if (scopeObject.SchoolLicense.BillingModel.GradeLevels != null) {
                                    accessFormObject.PriceModels[0]
                                        .GradeLevels = scopeObject.SchoolLicense.BillingModel.GradeLevels;
                                }
                                if (accessFormObject.PriceModels[0].ShowPercentage == true) {
                                    accessFormObject.PriceModels[0]
                                        .PercentageValue = scopeObject.SchoolLicense.BillingPercentage;
                                } else {
                                    accessFormObject.PriceModels[0].PercentageValue = null;
                                }
                            }
                            for (periodIndex = 0; periodIndex < scopeObject.Periods.length; periodIndex++) {
                                accessFormObject.BillingPeriods[periodIndex] = $
                                    .extend(true, {}, scopeObject.Periods[periodIndex]);
                                if (scopeObject.SchoolLicense.Pricing != null)
                                    if (scopeObject.SchoolLicense
                                        .Pricing[scopeObject.Periods[periodIndex]['UnitValue'] +
                                        ' ' +
                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']] !=
                                        null)
                                        accessFormObject.BillingPeriods[periodIndex]
                                            .Price = {
                                                UnitPrice: kendo
                                                    .parseFloat(scopeObject.SchoolLicense
                                                        .Pricing[
                                                        scopeObject.Periods[periodIndex]['UnitValue'] +
                                                        ' ' +
                                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']]
                                                        .ExTax)
                                                    .toFixed(2),
                                                UnitPriceVat: kendo
                                                    .parseFloat(scopeObject.SchoolLicense
                                                        .Pricing[
                                                        scopeObject.Periods[periodIndex]['UnitValue'] +
                                                        ' ' +
                                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']].IncTax)
                                                    .toFixed(2),
                                                VatValue: ((scopeObject.PercentValue) * 100) - 100
                                            };
                            }
                            break;
                        case _class:
                            if (scopeObject.ClassLicense.BillingModel != null)
                                accessFormObject.PriceModels[0] = scopeObject.ClassLicense.BillingModel;
                            for (periodIndex = 0; periodIndex < scopeObject.Periods.length; periodIndex++) {
                                accessFormObject.BillingPeriods[periodIndex] = $
                                    .extend(true, {}, scopeObject.Periods[periodIndex]);;
                                if (scopeObject.ClassLicense.Pricing != null)
                                    if (scopeObject.ClassLicense
                                        .Pricing[scopeObject.Periods[periodIndex]['UnitValue'] +
                                        ' ' +
                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']] !=
                                        null)
                                        accessFormObject.BillingPeriods[periodIndex]
                                            .Price = {
                                                UnitPrice: kendo
                                                    .parseFloat(scopeObject.ClassLicense
                                                        .Pricing[
                                                        scopeObject.Periods[periodIndex]['UnitValue'] +
                                                        ' ' +
                                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']]
                                                        .ExTax)
                                                    .toFixed(2),
                                                UnitPriceVat: kendo
                                                    .parseFloat(scopeObject.ClassLicense
                                                        .Pricing[
                                                        scopeObject.Periods[periodIndex]['UnitValue'] +
                                                        ' ' +
                                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']].IncTax)
                                                    .toFixed(2),
                                                VatValue: ((scopeObject.PercentValue) * 100) - 100
                                            };
                            }
                            break;
                        case _teacher:
                            if (scopeObject.TeacherLicense.BillingModel != null)
                                accessFormObject.PriceModels[0] = scopeObject.TeacherLicense.BillingModel;
                            for (periodIndex = 0; periodIndex < scopeObject.Periods.length; periodIndex++) {
                                accessFormObject.BillingPeriods[periodIndex] = $
                                    .extend(true, {}, scopeObject.Periods[periodIndex]);;
                                if (scopeObject.TeacherLicense.Pricing != null)
                                    if (scopeObject.TeacherLicense
                                        .Pricing[scopeObject.Periods[periodIndex]['UnitValue'] +
                                        ' ' +
                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']] !=
                                        null)
                                        accessFormObject.BillingPeriods[periodIndex]
                                            .Price = {
                                                UnitPrice: kendo
                                                    .parseFloat(scopeObject.TeacherLicense
                                                        .Pricing[
                                                        scopeObject.Periods[periodIndex]['UnitValue'] +
                                                        ' ' +
                                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']]
                                                        .ExTax)
                                                    .toFixed(2),
                                                UnitPriceVat: kendo
                                                    .parseFloat(scopeObject.TeacherLicense
                                                        .Pricing[
                                                        scopeObject.Periods[periodIndex]['UnitValue'] +
                                                        ' ' +
                                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']].IncTax)
                                                    .toFixed(2),
                                                VatValue: ((scopeObject.PercentValue) * 100) - 100
                                            };
                            }
                            break;
                        case _singleUser:
                            for (periodIndex = 0; periodIndex < scopeObject.Periods.length; periodIndex++) {
                                accessFormObject.BillingPeriods[periodIndex] = $
                                    .extend(true, {}, scopeObject.Periods[periodIndex]);;
                                if (scopeObject.SingleUserLicense.Pricing != null)
                                    if (scopeObject.SingleUserLicense
                                        .Pricing[scopeObject.Periods[periodIndex]['UnitValue'] +
                                        ' ' +
                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']] !=
                                        null)
                                        accessFormObject.BillingPeriods[periodIndex]
                                            .Price = {
                                                UnitPrice: kendo
                                                    .parseFloat(scopeObject.SingleUserLicense
                                                        .Pricing[
                                                        scopeObject.Periods[periodIndex]['UnitValue'] +
                                                        ' ' +
                                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']]
                                                        .ExTax)
                                                    .toFixed(2),
                                                UnitPriceVat: kendo
                                                    .parseFloat(scopeObject.SingleUserLicense
                                                        .Pricing[
                                                        scopeObject.Periods[periodIndex]['UnitValue'] +
                                                        ' ' +
                                                        scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']].IncTax)
                                                    .toFixed(2),
                                                VatValue: ((scopeObject.PercentValue) * 100) - 100
                                            };
                            }
                            break;
                        case _contactSales:
                            accessFormObject.DescriptionText = scopeObject.ContactSales.DescriptionText;

                            break;
                    }

                    scopeObject.SalesConfiguration.AccessForms.push(accessFormObject);
                }

                scopeObject.SalesConfiguration.HasTrialAccess = scopeObject.HasTrial;
                if (scopeObject.HasTrial) {
                    scopeObject.SalesConfiguration.TrialAccess = scopeObject.Trial;
                }

                scopeObject.SalesConfiguration.Isbn = scopeObject.DigitalTitle.Isbn;
                scopeObject.SalesConfiguration.Approved = approved;
                scopeObject.SalesConfiguration.CreatedBy = __jiniEnv.userName;
                scopeObject.SalesConfiguration.Seller = dataFactory.Seller;
                return jiniservice.saveSalesConfiguration(scopeObject.SalesConfiguration, approved);
                //.then(function (data) {
                //    if (data == true)
                //        $("#wizard-next-button").click();
                //    else if (data == false) {
                //        window.onbeforeunload = null;
                //        //if (window.popUpRedirect == "product-list") {
                //        //    window.location = "/";
                //        //} else if (window.popUpRedirect == "product-history") {
                //        //    //$state.go("");
                //        //    scopeObject.hidePopops();
                //        //} else window.location = "/";
                //    }

                //});
                //return scopeObject.SalesConfiguration;
            },

            //Parse and display already existing sales configuration
            parseSalesConfiguration: function (config, scopeObject) {
                var d = $.Deferred();
                var periodPromise = {}, billingModelPromise = [];
                scopeObject.LookupDefaultPeriods = [];
                for (var index = 0; config && config.SalesForms && index < config.SalesForms.length; index++) {
                    periodPromise = salesformService
                        .setSalesForm(config.SalesForms[index].DisplayName, config.SalesForms[index].Code, scopeObject);
                }

                var periods = true;

                if (periodPromise.then != undefined)
                    periodPromise.then(function () {
                        for (var index = 0; index < config.AccessForms.length; index++) {
                            billingModelPromise[index] = accessformService
                                .setAccessForm(config.AccessForms[index].DisplayName,
                                    config.AccessForms[index].Code,
                                    scopeObject);

                            if (periods) {
                                for (var copyPeriods = 0;
                                    copyPeriods < config.AccessForms[0].BillingPeriods.length;
                                    copyPeriods++) {
                                    var temp = JSON.parse(JSON
                                        .stringify(config.AccessForms[0].BillingPeriods[copyPeriods]));
                                    delete temp["Price"];
                                    scopeObject.Periods.push(temp);
                                    var results = $.grep(scopeObject.LookupDefaultPeriods,
                                        function (e) {
                                            var result = e.DisplayName ==
                                                config.AccessForms[0].BillingPeriods[copyPeriods]['DisplayName'];
                                            return result;
                                        });
                                    if (results.length == 0) {
                                        scopeObject.LookupDefaultPeriods
                                            .push(config.AccessForms[0].BillingPeriods[copyPeriods]);
                                    }
                                    periods = false;
                                }
                            }
                            var periodIndex;
                            switch (scopeObject.AccessForm[index].Code) {
                                case _school:
                                    scopeObject.SchoolLicense.Pricing = [];
                                    if (config.AccessForms[index].PriceModels[0] != null) {
                                        scopeObject.SchoolLicense.BillingModel = config.AccessForms[index].PriceModels[0];
                                        scopeObject.SchoolLicense
                                            .BillingPercentage = scopeObject.SchoolLicense.BillingModel.PercentageValue;
                                        scopeObject.SchoolLicense.BillingModel
                                            .GradeLevels = config.AccessForms[index].PriceModels[0].GradeLevels;
                                    }
                                    for (periodIndex = 0; periodIndex < config.AccessForms[index].BillingPeriods.length; periodIndex++) {
                                        scopeObject.SchoolLicense.Pricing[scopeObject.Periods[periodIndex]['UnitValue'] + ' ' + scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']] = {};
                                        scopeObject.SchoolLicense.Pricing[scopeObject.Periods[periodIndex]['UnitValue'] + ' ' + scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']].ExTax = kendo.toString(config.AccessForms[index].BillingPeriods[periodIndex].Price.UnitPrice, "n2");
                                        priceService.priceChanged(scopeObject.Periods[periodIndex]['UnitValue'] + ' ' + scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode'], _school, scopeObject.SchoolLicense);
                                        if (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue !== 0)
                                            scopeObject.PercentValue = (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue + 100) / 100;
                                    }
                                    break;
                                case _class:
                                    scopeObject.ClassLicense.Pricing = [];
                                    if (config.AccessForms[index].PriceModels[0] != null)
                                        scopeObject.ClassLicense.BillingModel = config.AccessForms[index].PriceModels[0];
                                    for (periodIndex = 0; periodIndex < scopeObject.Periods.length; periodIndex++) {
                                        scopeObject.ClassLicense
                                            .Pricing[scopeObject.Periods[periodIndex]['UnitValue'] +
                                            ' ' +
                                            scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']] = {};
                                        scopeObject.ClassLicense
                                            .Pricing[scopeObject.Periods[periodIndex]['UnitValue'] +
                                            ' ' +
                                            scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']]
                                            .ExTax = kendo.toString(config.AccessForms[index].BillingPeriods[periodIndex].Price
                                                .UnitPrice, "n2");
                                        priceService.priceChanged(scopeObject.Periods[periodIndex]['UnitValue'] +
                                            ' ' +
                                            scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode'],
                                            _class,
                                            scopeObject.ClassLicense);
                                        if (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue !== 0)
                                            scopeObject
                                                .PercentValue = (config.AccessForms[index].BillingPeriods[periodIndex].Price
                                                    .VatValue +
                                                    100) /
                                                100;
                                        else scopeObject.PercentValue = parseFloat(__jiniEnv.vat);
                                    }
                                    break;
                                case _teacher:
                                    scopeObject.TeacherLicense.Pricing = [];
                                    if (config.AccessForms[index].PriceModels[0] != null)
                                        scopeObject.TeacherLicense.BillingModel = config.AccessForms[index].PriceModels[0];
                                    for (periodIndex = 0; periodIndex < scopeObject.Periods.length; periodIndex++) {
                                        scopeObject.TeacherLicense
                                            .Pricing[scopeObject.Periods[periodIndex]['UnitValue'] +
                                            ' ' +
                                            scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']] = {};
                                        scopeObject.TeacherLicense
                                            .Pricing[scopeObject.Periods[periodIndex]['UnitValue'] +
                                            ' ' +
                                            scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']]
                                            .ExTax = kendo.toString(config.AccessForms[index].BillingPeriods[periodIndex].Price
                                                .UnitPrice, "n2");
                                        priceService.priceChanged(scopeObject.Periods[periodIndex]['UnitValue'] +
                                            ' ' +
                                            scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode'],
                                            _teacher,
                                            scopeObject.TeacherLicense);
                                        if (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue !== 0)
                                            scopeObject
                                                .PercentValue = (config.AccessForms[index].BillingPeriods[periodIndex].Price
                                                    .VatValue +
                                                    100) /
                                                100;
                                        else scopeObject.PercentValue = parseFloat(__jiniEnv.vat);
                                    }
                                    break;
                                case _singleUser:
                                    scopeObject.SingleUserLicense.Pricing = [];
                                    for (periodIndex = 0; periodIndex < scopeObject.Periods.length; periodIndex++) {
                                        scopeObject.SingleUserLicense
                                            .Pricing[scopeObject.Periods[periodIndex]['UnitValue'] +
                                            ' ' +
                                            scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']] = {};
                                        scopeObject.SingleUserLicense
                                            .Pricing[scopeObject.Periods[periodIndex]['UnitValue'] +
                                            ' ' +
                                            scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode']]
                                            .ExTax = kendo.toString(config.AccessForms[index].BillingPeriods[periodIndex].Price
                                                .UnitPrice, "n2");
                                        priceService.priceChanged(scopeObject.Periods[periodIndex]['UnitValue'] +
                                            ' ' +
                                            scopeObject.Periods[periodIndex]['RefPeriodUnitTypeCode'],
                                            _singleUser,
                                            scopeObject.SingleUserLicense);
                                        if (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue !== 0)
                                            scopeObject
                                                .PercentValue = (config.AccessForms[index].BillingPeriods[periodIndex].Price
                                                    .VatValue +
                                                    100) /
                                                100;
                                        else scopeObject.PercentValue = parseFloat(__jiniEnv.vat);
                                    }
                                    break;
                                case _contactSales:
                                    scopeObject.ContactSales.DescriptionText = config.AccessForms[index].DescriptionText;
                                    scopeObject.ContactSales.DescriptionTextHtml = $sce.trustAsHtml(scopeObject.ContactSales.DescriptionText);
                                    break;
                            }
                        }
                        window.previousData = scopeObject.Periods;

                        d.resolve();

                        if ($("#select-period").data("kendoMultiSelect") != null) {
                            $("#select-period").data("kendoMultiSelect").dataSource.read();
                            $("#select-period").data("kendoMultiSelect").refresh();
                        }

                        stateChanged = false;
                        window.onbeforeunload = null;
                    });
                else {
                    d.resolve();
                }
                scopeObject.HasTrial = config.HasTrialAccess;
                if (config.HasTrialAccess) {
                    scopeObject.Trial = config.TrialAccess;
                    scopeObject.Trial.ContactSales = config.TrialAccess.ContactSales;
                    scopeObject.Trial.TrialContactSales = (config.TrialAccess.ContactSales == "" ? false : true);

                    if (scopeObject.Trial.TrialContactSales) {
                        scopeObject.Trial.ContactSalesTextHtml = $sce.trustAsHtml(scopeObject.Trial.ContactSales);
                    }
                }

                // For explicitly handling the case in which saleConfiguration only have a single license of the ContactSales
                if (config.AccessForms.length === 1 && config.AccessForms[0].Code === _contactSales) {
                    accessformService.setAccessForm(config.AccessForms[0].DisplayName, config.AccessForms[0].Code, scopeObject);
                    scopeObject.ContactSales.DescriptionText = config.AccessForms[0].DescriptionText;
                    scopeObject.ContactSales.DescriptionTextHtml = $sce.trustAsHtml(scopeObject.ContactSales.DescriptionText);
                }

                return d.promise();
            }
        }
    });