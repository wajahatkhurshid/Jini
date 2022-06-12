'use strict';
/**************************************************************************

  * Create, Delete and View Trial License for Produkter

  *************************************************************************/
jiniApp.factory('trialLicense',
    function (jiniservice,
        dataFactory
    ) {
        return {
            getShareLink: function (isbn, seller) {
                jiniservice.getTrialLicenseShareLink(isbn, seller).then(function (data) {
                    dataFactory.TrialLicenseShareLink = data;
                    window.shareLink = data;
                });
            }
        }
    });