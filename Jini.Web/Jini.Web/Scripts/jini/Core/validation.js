'use strict';
/**************************************************************************

  * Validation for Jini Wizard. Validations user inputs based on each step of the wizard

  *************************************************************************/
jiniApp.factory('validation', function () {
    var validations = [];

    //Validation for Only Draft
    function onlyDraft(salesForm, accessForm, periods, hasTrial) {
        if (hasTrial && salesForm.length == 0 && accessForm.length == 0 && periods.length == 0) {
            return true;
        }
        return false;
    }

    //Validation for Stage one of wizard
    function stageOne(salesForm, accessForm, periods, hasTrial) {
        validations = [];

        if (accessForm.length === 0 && hasTrial === false) {
            validations.push("tab1:noselection");
            //no need for further validations
            return validations;
        }

        if (salesForm.length == 0 && !(accessForm.length === 1 && accessForm[0].Code == _contactSales)) {
            validations.push("tab1:salesform");
        }
        if (accessForm.length == 0) {
            validations.push("tab1:accessform");
        }
        if (periods.length == 0) {
            if (!(accessForm.length == 1 && accessForm[0].Code == "1005")) {
                validations.push("tab1:periods");
            }
        }
        return validations;
    };

    function trialValidation(trialLicense) {

        validations = [];

        if (Object.keys(trialLicense.AccessForm).length === 0 && trialLicense.AccessForm.constructor === Object) {
            validations.push("tab4:accessform");
        }

        if (Object.keys(trialLicense.Period).length === 0 && trialLicense.Period.constructor === Object) {
            validations.push("tab4:trialperiod");
        }

        if (trialLicense.MultipleTrialsPerUser && Object.keys(trialLicense.TrialAccessCount).length === 0 && trialLicense.TrialAccessCount.constructor === Object) {
            validations.push("tab4:multipletrials");
        }

        if (trialLicense.TrialContactSales && trialLicense.ContactSales === "") {
            validations.push("tab4:contactsales");
        }

        return validations;
    };

    //Validation for Stage two of wizard
    function stageTwo(accessForm, periods, schoolLicense, classLicense,teacherLicense, singleUserLicense, contactSales) {
        validations = [];
        for (var index = 0; index < accessForm.length; index++) {
            
            if (accessForm[index].Code == _school) schoolValidation(schoolLicense,periods);
            else if (accessForm[index].Code == _class) classValidation(classLicense, periods);
            else if (accessForm[index].Code == _teacher) teacherValidation(teacherLicense, periods);
            else if (accessForm[index].Code == _singleUser) singleUserValidation(singleUserLicense, periods);
            else if (accessForm[index].Code == _contactSales) contactSalesValidation(contactSales, periods);
            
        }
        return validations;
    };
    // School Control Validation - if school is selected in access forms
    function schoolValidation(schoolLicense,periods) {
        if (schoolLicense.BillingModel == undefined || schoolLicense.BillingModel.Code == undefined || schoolLicense.BillingModel.Code == '') {
            validations.push("tab2:school:billingModel");
        }
        else {
            if (schoolLicense.BillingModel.ShowPercentage == true) {
                if (schoolLicense.BillingPercentage < 1 || schoolLicense.BillingPercentage > 100 || schoolLicense.BillingPercentage == undefined) {
                    validations.push("tab2:school:billingPercentage");
                }
            }

        }
        if (schoolLicense.Pricing == undefined) {
            validations.push("tab2:school:pricing");
        }
        else {
            var pricings = schoolLicense.Pricing;

            angular.forEach(periods, function (value) {
                if (!pricings.hasOwnProperty(value.UnitValue + ' ' + value.RefPeriodUnitTypeCode)) {
                    //$scope.Validations.push('tab2:school:pricing:' + value.label);
                    validations.push("tab2:school:pricing");
                }
            });

            angular.forEach(periods, function (value) {
                if (pricings[value.UnitValue + ' ' + value.RefPeriodUnitTypeCode].ExTax == "0,00")
                    //$scope.Validations.push('tab2:school:pricing:' + key);
                    validations.push('tab2:school:pricing');
            });
           
        }
    };

    //// Class Control Validation - if class is selected
    function classValidation(classLicense, periods) {
        if (classLicense.BillingModel == undefined || classLicense.BillingModel.Code == undefined || classLicense.BillingModel.Code == "") {
            validations.push('tab2:class:billingModel');
        }
        if (classLicense.Pricing == undefined) {
            validations.push("tab2:class:pricing");
        }
        else {
            var pricings = classLicense.Pricing;
            angular.forEach(periods, function (value) {
                if (!pricings.hasOwnProperty(value.UnitValue + ' ' + value.RefPeriodUnitTypeCode)) {
                    //$scope.Validations.push('tab2:school:pricing:' + value.label);
                    validations.push("tab2:class:pricing");
                }
            });
            angular.forEach(periods, function (value) {
                if (pricings[value.UnitValue + ' ' + value.RefPeriodUnitTypeCode].ExTax == "0,00")
                    //$scope.Validations.push('tab2:school:pricing:' + key);
                    validations.push("tab2:class:pricing");
            });
        }
    };

    //// Teacher Control Validation - if teacher is selected
    function teacherValidation(teacherLicense,periods) { 
        if (teacherLicense.BillingModel == undefined || teacherLicense.BillingModel.Code == undefined || teacherLicense.BillingModel.Code == '') {
            validations.push('tab2:teacher:billingModel');
        }
        else {
            if (teacherLicense.BillingModel.ShowPercentage == true) {
                if (teacherLicense.BillingPercentage < 1 || teacherLicense.BillingPercentage > 99 || teacherLicense.BillingPercentage == undefined) {
                    validations.push('tab2:teacher:billingPercentage');
                }
            }

        }
        if (teacherLicense.Pricing == undefined) {
            validations.push("tab2:teacher:pricing");
        }
        else {
            var pricings = teacherLicense.Pricing;

            angular.forEach(periods, function (value) {
                if (!pricings.hasOwnProperty(value.UnitValue + ' ' + value.RefPeriodUnitTypeCode)) {
                    //$scope.Validations.push('tab2:school:pricing:' + value.label);
                    validations.push("tab2:teacher:pricing");
                }
            });
            angular.forEach(periods, function (value) {
                if (pricings[value.UnitValue + ' ' + value.RefPeriodUnitTypeCode].ExTax == "0,00")
                    //$scope.Validations.push('tab2:school:pricing:' + key);
                    validations.push("tab2:teacher:pricing");
            });

        }
    };

    ////// Single-User Control Validation - if sungle user is selected
    function singleUserValidation(singleUserLicense, periods) {
        if (singleUserLicense.Pricing == undefined) {
            validations.push('tab2:singleuser:pricing');
        }
        else {
            var pricings = singleUserLicense.Pricing;
            angular.forEach(periods, function (value) {
                if (!pricings.hasOwnProperty(value.UnitValue + ' ' + value.RefPeriodUnitTypeCode)) {
                    //$scope.Validations.push('tab2:school:pricing:' + value.label);
                    validations.push("tab2:singleuser:pricing");
                }
            });
            angular.forEach(periods, function (value) {
                if (pricings[value.UnitValue + ' ' + value.RefPeriodUnitTypeCode].ExTax == "0,00")
                    //$scope.Validations.push('tab2:school:pricing:' + key);
                    validations.push("tab2:singleuser:pricing");
            });

        }
    };

    // Contact Sales Validation - if contact sales is selected in access forms
    function contactSalesValidation() {
        var text = $("<p>").html($("#contact-sales-desc").data("kendoEditor").value()).text();
        if (!/\S/.test(text)) {
            validations.push("tab2:contactsales");
        }

    };

    function draftValidation(dataFactory) {
        if (dataFactory.AccessForm.length === 1 && dataFactory.AccessForm[0].Code === _contactSales) {
            return true;
        }
        else if (dataFactory.HasTrial && dataFactory.AccessForm.length === 0) {
            return true;
        }
        else if (dataFactory.SalesForm.length === 0 && (!dataFactory.HasTrial)) {
            dataFactory.Validations.push("tab1:noselection");
            return false;
        }
        return true;
    };


    return {
        validateOnlyDraft: onlyDraft,
        validateStageOne: stageOne,
        validateStageTwo: stageTwo,
        validateSchooLicense: schoolValidation,
        validateClassLicense: classValidation,
        validateTrialLicense: trialValidation,
        validateTeacherLicense: teacherValidation,
        validateSingleUserLicense: singleUserValidation,
        validateContactSalesLicense: contactSalesValidation,
        validateDraft: draftValidation


    };
});