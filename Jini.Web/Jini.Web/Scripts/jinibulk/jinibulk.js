(function () {
    'use strict';

    angular
        .module('jinibulk', ['kendo.directives', 'ngRoute'])
        .constant('__jiniEnv', __jiniEnv)
        .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
            $locationProvider.hashPrefix('');
            $routeProvider
                .when('/', {
                    controller: 'configurationCtrl',
                    templateUrl: '/Scripts/jinibulk/templates/Configuration.html'
                })
                .when('/Bulk', {
                    controller: 'configurationCtrl',
                    templateUrl: '/Scripts/jinibulk/templates/Configuration.html'
                })
                .otherwise({ redirectTo: '/' });
        }]);

})();

window.StartupComplete = false;

function InitBulkEditGridControls() {
    $("#loader").css({ "display": "none" });
    window.StartupComplete = true;
    ConfigureFilter("#releasedateWindow", "#releasedateFilter");
    ConfigureFilter("#lastmodifiedWindow", "#lastmodifiedFilter");

    // Hide Custom Filter Windows on Click
    $("*").not("#releasedateFilter").not("#lastmodifiedFilter").not(".k-i-calendar").click(function (x) {
        closeTreeViewModel(x);
    });

    (function ($) {
        var originalFilter = kendo.data.DataSource.fn.filter;

        kendo.data.DataSource.fn.filter = function (e) {
            if (e != null && e.filters.length > 0) {
                if (e.filters[0].value != null)
                    e.filters[0].value = e.filters[0].value.trim();
            }

            return originalFilter.apply(this, arguments);
        };
    })(jQuery);
    loadFilters();
}
//Event Handler for release date filter clear button
function releaseDateFilterCleared() {

    if (!$("#releasedatehyperlink").hasClass("k-state-active")) {
        return false;
    }

    $("#releasedatehyperlink").removeClass("k-state-active");
    var bulkGridData = $("#bulkEditGrid").data("kendoGrid");
    var currFilterObj = bulkGridData.dataSource.filter();
    var currentFilters = currFilterObj ? currFilterObj.filters : [];

    currentFilters = removeCurrentFilters(currentFilters, "ReleaseDate");
    gridDataColumns.ReleaseStartDate = "";
    gridDataColumns.ReleaseEndDate = "";
    bulkGridData.dataSource.filter({
        logic: "and",
        filters: currentFilters
    });
}
//Event handler for release Date filter apply button
var allowReleaseDateFilter = false;

function releaseDateFilterApplied() {

    if (allowReleaseDateFilter == false) {
        clearSelectionOnReleaseDateFilter();
    }

    if (allowReleaseDateFilter) {

        $("#releasedatehyperlink").addClass("k-state-active");
        var releasedateWindow = $("#releasedateWindow");
        releasedateWindow.data("kendoWindow").close();
        var field = "ReleaseDate";
        var endDate = $("#end").data("kendoDatePicker")._value,
            startDate = $("#start").data("kendoDatePicker")._value;

        //alert('start date: ' + startDate + ', End Date: ' + endDate);
        gridDataColumns.ReleaseStartDate = startDate;
        gridDataColumns.ReleaseEndDate = endDate;

        createAndApplyDateFilters(field, startDate, endDate);
        allowReleaseDateFilter = false;
    }
}
//Event Handler for last modified filter clear button
var allowModifiedDateFilter = false;
function lastModifiedFilterCleared() {

    if (!$("#lastmodifiedhyperlink").hasClass("k-state-active")) {
        return false;
    }

    $("#lastmodifiedhyperlink").removeClass("k-state-active");
    var bulkGridData = $("#bulkEditGrid").data("kendoGrid");
    var currFilterObj = bulkGridData.dataSource.filter();
    var currentFilters = currFilterObj ? currFilterObj.filters : [];

    gridDataColumns.ModifiedStartDate = "";
    gridDataColumns.ModifiedEndDate = "";

    currentFilters = removeCurrentFilters(currentFilters, "LastModified");

    bulkGridData.dataSource.filter({
        logic: "and",
        filters: currentFilters
    });

}
//Event handler for last modified filter apply button
function lastModifiedFilterApplied() {

    if (allowModifiedDateFilter == false) {
        clearSelectionOnModifiedDateFilter();
    }

    if (allowModifiedDateFilter) {

        $("#lastmodifiedhyperlink").addClass("k-state-active");
        var releasedateWindow = $("#lastmodifiedWindow");
        releasedateWindow.data("kendoWindow").close();
        var field = "LastModified";
        var endDate = $("#endLastModified").data("kendoDatePicker")._value,
            startDate = $("#startLastModified").data("kendoDatePicker")._value;
        //alert('start date: ' + startDate + ', End Date: ' + endDate);
        gridDataColumns.ModifiedStartDate = startDate;
        gridDataColumns.ModifiedEndDate = endDate;
        allowModifiedDateFilter = false;
        createAndApplyDateFilters(field, startDate, endDate);

    }
}
// Configure Click event of Filter
function ConfigureFilter(ctrlWin, ctrlFilterBtn) {

    var ctrlWindow = $(ctrlWin),
        ctrlFilter = $(ctrlFilterBtn);
    ctrlFilter.click(function (e) {
        e.stopPropagation();
        closeTreeViewModel(e);
        if (ctrlWindow.is(":hidden")) {
            SetWindowPosition($(this), ctrlWindow);
            ctrlWindow.data("kendoWindow").open();
        } else {
            ctrlWindow.data("kendoWindow").close();
        }
        return false;
    });
}
// Create and Apply Filter for date picker filter values
function createAndApplyDateFilters(field, startDate, endDate) {
    if (startDate === null || endDate === null) {
        startDate = null;
        endDate = null;
    }

    var gridData = $("#bulkEditGrid").data("kendoGrid");
    var currFilterObj = gridData.dataSource.filter();
    var currentFilters = currFilterObj ? currFilterObj.filters : [];

    currentFilters = removeCurrentFilters(currentFilters, field);

    var newFilters = [];

    newFilters.push({
        field: field,
        operator: "ge",
        value: startDate
    });

    newFilters.push({
        field: field,
        operator: "le",
        value: endDate
    });

    currentFilters.push({
        filters: newFilters,
        logic: "and"
    });

    gridData.dataSource.filter({
        logic: "and",
        filters: currentFilters
    });
}
// Clear currently Applied Filters
function removeCurrentFilters(currentFilters, field) {
    // iterate over current filters array. if a filter for "filterField" is already
    // defined, remove it from the array
    // once an entry is removed, we stop looking at the rest of the array.
    var i;
    if ((currentFilters && currentFilters.length > 0)) {
        for (i = 0; i < currentFilters.length; i++) {
            var existingFilters;
            if (currentFilters[i].filters === undefined) {
                if (currentFilters[i].field === field) {
                    currentFilters.splice(i, 1);
                    break;
                }
            } else {
                existingFilters = $.grep(currentFilters[i].filters, function (e) { return e.field === field; });
                if (existingFilters !== undefined && existingFilters !== null && existingFilters.length > 0) {
                    currentFilters.splice(i, 1);
                    break;
                }
            }
        }
    }
    return currentFilters;
}
//Any Change on Start Date on releasedate filter will be handled by this function
function startChange() {
    var endPicker = $("#end").data("kendoDatePicker"),
        startDate = this.value();

    if (startDate) {
        startDate = new Date(startDate);
        startDate.setDate(startDate.getDate());
        endPicker.min(startDate);
    }
}
//Any Change on Start Date on lastmodified filter will be handled by this function
function startLastModifiedChange() {
    var endPicker = $("#endLastModified").data("kendoDatePicker"),
        startDate = this.value();

    if (startDate) {
        startDate = new Date(startDate);
        startDate.setDate(startDate.getDate());
        endPicker.min(startDate);
    }
}
//Any Change on End Date on releasedate filter will be handled by this function
function endChange() {
    var startPicker = $("#start").data("kendoDatePicker"),
        endDate = this.value();

    if (endDate) {
        endDate = new Date(endDate);
        endDate.setDate(endDate.getDate());
        startPicker.max(endDate);
    }
}
//Any Change on End Date on lastmodified filter will be handled by this function
function endLastModifiedChange() {
    var startPicker = $("#startLastModified").data("kendoDatePicker"),
        endDate = this.value();

    if (endDate) {
        endDate = new Date(endDate);
        endDate.setDate(endDate.getDate());
        startPicker.max(endDate);
    }
}

function onCalenderFilterOpen(e) {
    e.sender.dateView.div.removeClass("k-popup");
}
// Close all open windows when mouse is clicked outside of their body
function closeTreeViewModel(e) {

    var releasedateWindow = $("#releasedateWindow");
    var lastmodifiedWindow = $("#lastmodifiedWindow");

    if (($(e.target).parents('#releasedateWindow').length === 0) && e.currentTarget.id !== 'start_dateview' && e.currentTarget.id !== 'end_dateview') {
        if (!releasedateWindow.is(":hidden") && ($(e.target).parents('#start_dateview').length === 0 && $(e.target).parents('#end_dateview').length === 0)) {
            releasedateWindow.data("kendoWindow").close();
        }
    } else if (e.currentTarget.id == 'releasedateApply' || e.currentTarget.id == 'releasedateClear') {
        if (!releasedateWindow.is(":hidden")) {
            releasedateWindow.data("kendoWindow").close();
        }
    } else if (e.currentTarget.id == 'start_dateview' || e.currentTarget.id == 'end_dateview') {
        e.stopPropagation();
    }

    if (($(e.target).parents('#lastmodifiedWindow').length === 0) && e.currentTarget.id !== 'startLastModified_dateview' && e.currentTarget.id !== 'endLastModified_dateview') {
        if (!lastmodifiedWindow.is(":hidden") && ($(e.target).parents('#startLastModified_dateview').length === 0 && $(e.target).parents('#endLastModified_dateview').length === 0)) {
            lastmodifiedWindow.data("kendoWindow").close();
        }
    } else if (e.currentTarget.id == 'lastmodifiedApply' || e.currentTarget.id == 'lastmodifiedClear') {
        if (!lastmodifiedWindow.is(":hidden")) {
            lastmodifiedWindow.data("kendoWindow").close();
        }
    } else if (e.currentTarget.id == 'startLastModified_dateview' || e.currentTarget.id == 'endLastModified_dateview') {
        e.stopPropagation();
    }

    return false;
}
//Position of Filter Windows on the basis of document window size
function SetWindowPosition(e, crtlWin) {
    var position;
    // Popup position shouldn't exceed Window's width
    if (e.offset().left + 200 > $(window).width()) {
        position = e.offset().left - 200;
    } else {
        position = e.offset().left;
    }
    crtlWin.closest(".k-window").css({
        top: e.offset().top + 25,
        left: position
    });
    return crtlWin;
}
// Save the selected filters
function saveFilters() {
    var grid = $("#bulkEditGrid").data("kendoGrid");
    var filters = grid.dataSource.filter();

    $.ajax({
        url: "/Bulk/SaveFilters",
        type: "POST",
        data: {
            filter: JSON.stringify(filters)
        }
    });
}
//Load and apply the previously selected filters
function loadFilters() {
    var grid = $("#bulkEditGrid").data("kendoGrid");
    $.ajax({
        url: "/Bulk/LoadFilters",
        success: function (state) {
            if (state === "No Filters") {
                window.StartupComplete = true;
                return;
            }
            else {
                var filters = JSON.parse(state);
                parseFilters(filters);
                grid.dataSource.filter(JSON.parse(state));
                window.StartupComplete = true;
            }
        },
        failure: function () {
        }
    });
}

function parseFilters(parsedFilters) {
    if (parsedFilters != null) {
        if (parsedFilters.filters.length > 0) {
            for (var index = 0; index < parsedFilters.filters.length; index++) {
                if (!isArray(parsedFilters.filters[index]) && parsedFilters.filters[index].filters === undefined) {
                    var listFilter = { filters: [] };
                    listFilter.filters.push(parsedFilters.filters[index]);
                    parsedFilters.filters[index] = listFilter;
                }
                if (parsedFilters.filters[index].filters[0].field === "ReleaseDate") {
                    $("#releasedatehyperlink").addClass("k-state-active");
                    $("#start").data("kendoDatePicker").value(new Date(parsedFilters.filters[index].filters[0].value));
                    $("#end").data("kendoDatePicker").value(new Date(parsedFilters.filters[index].filters[1].value));
                }
                if (parsedFilters.filters[index].filters[0].field === "LastModified") {
                    $("#lastmodifiedhyperlink").addClass("k-state-active");
                    $("#startLastModified").data("kendoDatePicker").value(new Date(parsedFilters.filters[index].filters[0].value));
                    $("#endLastModified").data("kendoDatePicker").value(new Date(parsedFilters.filters[index].filters[1].value));
                }
            }
        }
    }
}

function isArray(obj) {
    return !!obj && obj.constructor === Array;
}
