'use strict';
/**************************************************************************

  * Access Form for Jini Wizard.

  *************************************************************************/
jiniApp.factory('accessformService',
    function ( jiniservice, salesformService, periodService) {
        return {
            setAccessForm: function(displayName, code, scope) {
                var promise = $.Deferred();
                var accessFormObject = { Code: code, DisplayName: displayName };
                var searchResult = $.grep(scope.AccessForm, function (e) { return e.Code == code; });
                if (searchResult.length == 0) {
                    scope.AccessForm.push(accessFormObject);

                }
                // Skipping ContactSales for explicitly handling the case in which saleConfiguration only have a single license of the ContactSales
                else if (!(scope.AccessForm.length == 1 && code == _contactSales)) {
                    if (code == _contactSales) {
                        scope.ContactSales.DescriptionText = "";
                    }
                    for (var index = 0 ; index < scope.AccessForm.length; index++) {
                        if (scope.AccessForm[index].Code == searchResult[0].Code) {
                            
                            removeObjectFromList(scope.AccessForm[index], scope.AccessForm, scope.SelectedAccessForm);
                        }

                    }
                    if (scope.AccessForm.length === 0) {
                        salesformService.clear(scope);
                        periodService.clear(scope);
                    }
                }

                //each access type may or may not have a billing model, and may have a different one. 
                //Fetch a billing model relevant to the access type
                jiniservice.getBillingModel(code).then(function (data) {
                    scope.LookupBillingModels[code] = data;
                    promise.resolve("true");
                });
                
                return promise.promise();
            },
            setTrialAccessForm: function (displayName, code, scope) {
                scope.Trial.AccessForm = { Code: code, DisplayName: displayName };
            }
        };
    });