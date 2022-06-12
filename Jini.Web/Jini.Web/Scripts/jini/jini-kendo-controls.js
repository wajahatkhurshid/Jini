// Generic function for creatinga Kendo UI window
function historyPage() {
    if ($('#custom-period-popup').data('kendoWindow') != null)
        $('#custom-period-popup').data('kendoWindow').destroy();

    if ($("#email-hyperlink").data('kendoWindow') != null)
        $("#email-hyperlink").data('kendoWindow').destroy();

    if ($("#trial-email-hyperlink").data('kendoWindow') != null)
        $("#trial-email-hyperlink").data('kendoWindow').destroy();

}


function wizardPageOne() {
    if ($("#email-hyperlink").data('kendoWindow') != null)
        $("#email-hyperlink").data('kendoWindow').destroy();


    $("#select-model").kendoMobileButtonGroup();

    $("#specify-custom-period").kendoButton({
        icon: "plus"
    });

    createPopup(
        "custom-period-popup",
        "specify-custom-period",
        true,
        "Opret brugerdefineret periode",
        540
    );
    createPopup(
        "period-warning",
        "",
        true,
        "",
        540
    );
    $(".period-add").kendoButton({
        icon: "plus"
    });
    //$(".period-unit-count").kendoNumericTextBox();
    //$(".period-unit-type").kendoDropDownList();

}
var previousData = [];

function createPopup(popupId, caller, isCallerId, popupTitle, popupWidth) {
    window.popUpRedirect = "";
    var w = $("#" + popupId);
    if (w.data("kendoWindow") != null)
        return;
    var c;

    if (isCallerId)
        c = $("#" + caller);
    else
        c = $("." + caller);

    c.click(function () {
        if (w != undefined && w.data("kendoWindow") != undefined) {
            w.data("kendoWindow").center().open();
        }
    });

    w.kendoWindow({
        width: popupWidth + "px",
        title: popupTitle,
        //open: function(e) { $("html, body").css("overflow", "hidden"); },
        //close: function(e) { $("html, body").css("overflow", ""); },
        modal: true,
        visible: false,
        resizable: false,
        maximize: false,
        open: function () {
        },
        close: function (e) {

            window.popUpRedirect = "";
            if (e.sender.element[0].id === "email-hyperlink") {
                $("#contact-sales-desc").data("kendoEditor").focus();
            }

        },
        actions: [
            "Close"
        ]

    });

}

function wizardPageTwo() {
    if ($("#trial-email-hyperlink").data('kendoWindow') != null)
        $("#trial-email-hyperlink").data('kendoWindow').destroy();

    if ($("#email-hyperlink").data('kendoWindow') === undefined || $("#email-hyperlink").data('kendoWindow') === null)
        createPopup("email-hyperlink", "", true, "Indsæt hyperlink", 400);



    createPopup(
        "no-grade-warning-popup",
        "",
        true,
        "Ingen relevante klassetrin",
        550
    );

    if ($('#custom-period-popup').data('kendoWindow') != null)
        $('#custom-period-popup').data('kendoWindow').destroy();
}

function trialPageSetup() {
    DestroyPage();
    if ($('#custom-period-popup').data('kendoWindow') != null)
        $('#custom-period-popup').data('kendoWindow').destroy();

    if ($("#trial-email-hyperlink").data('kendoWindow') != null)
        $("#trial-email-hyperlink").data('kendoWindow').destroy();

    if ($("#email-hyperlink").data('kendoWindow') != null)
        $("#email-hyperlink").data('kendoWindow').destroy();

    //if ($("#trial-email-hyperlink").data('kendoWindow') === undefined || $("#trial-email-hyperlink").data('kendoWindow') === null)
    //    createPopup("trial-email-hyperlink", "", true, "Indsæt hyperlink", 400);

    $("#select-access-type").kendoMobileButtonGroup({ index: 0 });
    $("#toggle-max-trials").kendoMobileButtonGroup({ index: 0 });
    $("#toggle-contact-sales-text").kendoMobileButtonGroup({ index: 1 });

}

function wizardPageThree() {

    DestroyPage();


    if ($('#trial-email-hyperlink').data('kendoWindow') != null)
        $('#trial-email-hyperlink').data('kendoWindow').destroy();

    $("#wizard-next-button").kendoButton();
    $(".product-preview-billing-period").kendoDropDownList();
    //$(".single-user-access-count").kendoNumericTextBox();
    $(".product-preview-licenses li").click(function () {
        $(this).siblings().removeClass("selected");
        $(this).addClass("selected");
    });
}

function wizardPageFour() {
    $("#goto-productlist").kendoButton();
}

function DestroyPage() {
    // Destroy Popup of Custom Periods as Kendo Create it outside our main div scope
    // When we come back to page one it creates another one and popup doesn't work properly, so destorying it on page two.
    $(".k-list-container.k-popup.k-group.k-reset").empty();
    $(".k-list-container.k-popup.k-group.k-reset").remove();

    var window = $("#email-hyperlink").data('kendoWindow');
    if (window != null)
        window.destroy();



    window = $("#product-preview-popup-single-user").data("kendoWindow");
    if (window != null) {
        window.destroy();
    }
    window = $("#product-preview-popup").data("kendoWindow");
    if (window != null) {
        window.destroy();
    }
    window = $("#custom-period-popup").data("kendoWindow");
    if (window != null) {
        window.destroy();
    }
}


function viewSalesConfig() {
    $("#view-sales-config-tabstrip").kendoTabStrip({
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    });

    createPopup(
        "product-preview-popup",
        "preview-products",
        false,
        "Shopopsætning",
        670
    );

    createPopup(
        "product-preview-popup-single-user",
        "preview-products-single-user",
        false,
        "Shopopsætning",
        670
    );

    createPopup(
        "new-version-warning-popup",
        "create-new-version",
        false,
        "Ny version oprettet",
        490
    );
}

