'use strict';

/**************************************************************************

  * Service for looking up and saving data from Jini service.

  *************************************************************************/

// ReSharper disable once InconsistentNaming
jiniApp.factory("dataFactory",
    function ($http, __jiniEnv, $rootScope) {

        this.jiniData = {
            isLDatafill: false,
            isRDatafill: false,
            /*lookup models*/
            LookupDefaultPeriods: [],
            LookupBillingModels: [],

            /*Preview Models*/
            PreviewUserPrice: {},
            PreviewUserCount: 1,

            /* Models  */
            Seller: 0,
            DigitalTitle: {},
            SchoolLicense: {},
            ClassLicense: {},
            TeacherLicense: {},
            SingleUserLicense: {},
            ContactSales: { DescriptionText: "", DescriptionTextHtml: "" },

            AccessForm: [],
            SalesForm: [],
            Periods: [],

            PercentValue: parseFloat(__jiniEnv.vat),
            SalesConfiguration: {},
            CustomPeriodsId: 5000,
            CustomPeriods: [],

            Validations: [],

            PreviewPeriodSelection: {},
            SingleUserPeriods: [],
            VersionHistory: [],
            GuaVersionHistory: [],

            /*Trial License*/

            HasTrial: true,
            Trial: {
                AccessForm: {},
                Period: {},
                MultipleTrialsPerUser: true,
                TrialAccessCount: {},
                TrialContactSales: false,
                ContactSales: "",
                ContactSalesTextHtml: ""
            },
            TrialLicenseShareLink: "",

            EmailHyperLink: {
                address: "http://",
                text: "",
                subject: "",
                tooltip: "",
                openInNewTab: false
            },

            /*ng-class models*/
            SelectedSalesForm: [],
            SelectedAccessForm: [],


            applyRootScope: function () {
                if ($rootScope.$root.$$phase != '$apply' && $rootScope.$root.$$phase != '$digest') {
                    $rootScope.$apply();
                }
            },
            ProductAccessProviderInformation: {
                Isbn:"",
                IsExternalLogin:false,
                AccessProvider:[]
            },
            ProviderAccessForm: [
                {
                    Code: "",
                    DisplayName: "",
                    Identifier:"",
                    ischecked: false
                }
            ]

        };
        return this.jiniData;
    });