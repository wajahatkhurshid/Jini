var Util = {
    parseSalesConfiguration : function (config) {
        var d = $.Deferred();
        var periodPromise, billingModelPromise = [];
        for (var index = 0; index < config.SalesForms.length; index++) {
            periodPromise = $scope.setSelectedSales(config.SalesForms[index].DisplayName, config.SalesForms[index].Code);
        }

        var periods = true;
        periodPromise.then(function (data) {
            $timeout(function () {
                for (var index = 0; index < config.AccessForms.length; index++) {

                    billingModelPromise[index] = $scope.setSelectedAccess(config.AccessForms[index].DisplayName, config.AccessForms[index].Code);


                    if (periods) {

                        for (var copyPeriods = 0; copyPeriods < config.AccessForms[0].BillingPeriods.length; copyPeriods++) {
                            var temp = JSON.parse(JSON.stringify(config.AccessForms[0].BillingPeriods[copyPeriods]))
                            delete temp['Price'];
                            $scope.Periods.push(temp);
                            var results = $.grep($scope.LookupDefaultPeriods, function (e) {
                                var result = e.DisplayName == config.AccessForms[0].BillingPeriods[copyPeriods]['DisplayName'];
                                return result;
                            });
                            if (results.length == 0) {
                                $scope.LookupDefaultPeriods.push(config.AccessForms[0].BillingPeriods[copyPeriods]);
                            }
                            periods = false;
                        }

                    }
                    switch ($scope.AccessForm[index].Code) {
                        case _school:
                            $scope.SchoolLicense.Pricing = [];
                            if (config.AccessForms[index].PriceModels[0] != null) {
                                $scope.SchoolLicense.BillingModel = config.AccessForms[index].PriceModels[0];
                                $scope.SchoolLicense.BillingPercentage = $scope.SchoolLicense.BillingModel.PercentageValue;
                                $scope.SchoolLicense.BillingModel.GradeLevels = config.AccessForms[index].PriceModels[0].GradeLevels;
                            }
                            for (var periodIndex = 0; periodIndex < config.AccessForms[index].BillingPeriods.length; periodIndex++) {
                                $scope.SchoolLicense.Pricing[$scope.Periods[periodIndex]['DisplayName']] = {};
                                $scope.SchoolLicense.Pricing[$scope.Periods[periodIndex]['DisplayName']].ExTax = String(config.AccessForms[index].BillingPeriods[periodIndex].Price.UnitPrice);
                                priceChanged($scope.Periods[periodIndex]['DisplayName'], _school);
                                if (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue != 0)
                                    $scope.PercentValue = (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue + 100) / 100;

                            }
                            break;
                        case _class:
                            $scope.ClassLicense.Pricing = [];
                            if (config.AccessForms[index].PriceModels[0] != null)
                                $scope.ClassLicense.BillingModel = config.AccessForms[index].PriceModels[0];
                            for (var periodIndex = 0; periodIndex < $scope.Periods.length; periodIndex++) {
                                $scope.ClassLicense.Pricing[$scope.Periods[periodIndex]['DisplayName']] = {};
                                $scope.ClassLicense.Pricing[$scope.Periods[periodIndex]['DisplayName']].ExTax = String(config.AccessForms[index].BillingPeriods[periodIndex].Price.UnitPrice);
                                priceChanged($scope.Periods[periodIndex]['DisplayName'], _class);
                                if (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue != 0)
                                    $scope.PercentValue = (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue + 100) / 100;
                                else $scope.PercentValue = parseFloat(__jiniEnv.vat);
                            }
                            break;
                        case _teacher:
                            $scope.TeacherLicense.Pricing = [];
                            if (config.AccessForms[index].PriceModels[0] != null)
                                $scope.TeacherLicense.BillingModel = config.AccessForms[index].PriceModels[0];
                            for (var periodIndex = 0; periodIndex < $scope.Periods.length; periodIndex++) {
                                $scope.TeacherLicense.Pricing[$scope.Periods[periodIndex]['DisplayName']] = {};
                                $scope.TeacherLicense.Pricing[$scope.Periods[periodIndex]['DisplayName']].ExTax = String(config.AccessForms[index].BillingPeriods[periodIndex].Price.UnitPrice);
                                priceChanged($scope.Periods[periodIndex]['DisplayName'], _teacher);
                                if (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue != 0)
                                    $scope.PercentValue = (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue + 100) / 100;
                                else $scope.PercentValue = parseFloat(__jiniEnv.vat);
                            }
                            break;
                        case _singleUser:
                            $scope.SingleUserLicense.Pricing = [];
                            for (var periodIndex = 0; periodIndex < $scope.Periods.length; periodIndex++) {
                                $scope.SingleUserLicense.Pricing[$scope.Periods[periodIndex]['DisplayName']] = {};
                                $scope.SingleUserLicense.Pricing[$scope.Periods[periodIndex]['DisplayName']].ExTax = String(config.AccessForms[index].BillingPeriods[periodIndex].Price.UnitPrice);
                                priceChanged($scope.Periods[periodIndex]['DisplayName'], _singleUser);
                                if (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue != 0)
                                    $scope.PercentValue = (config.AccessForms[index].BillingPeriods[periodIndex].Price.VatValue + 100) / 100;
                                else $scope.PercentValue = parseFloat(__jiniEnv.vat);
                            }
                            break;
                        case _contactSales:
                            $scope.ContactSales.DescriptionText = config.AccessForms[index].DescriptionText;
                            break;
                    }

                }
                window.previousData = $scope.Periods;
                d.resolve();
                if ($('#select-period').data('kendoMultiSelect') != null) {
                    $('#select-period').data('kendoMultiSelect').dataSource.read();
                    $('#select-period').data('kendoMultiSelect').refresh();
                }

                stateChanged = false;
            });

        });
        return d.promise();
    }
};
