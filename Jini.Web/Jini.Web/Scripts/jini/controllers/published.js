'use strict';
/**************************************************************************

  * Controller for the published Sales Configuration

  *************************************************************************/

jiniApp.controller('published',
    function () {
        wizardPageFour();
        stateChanged = false;
        window.onbeforeunload = null;
        $("#divLoading").removeClass("show");
    });