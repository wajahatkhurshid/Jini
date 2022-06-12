
(function () {
    'use strict';

    angular
        .module('jiniV2bulk')
        .controller('configurationV2Ctrl', ['$scope', 'jiniV2bulkService', '__jiniEnv', '$q',
            function ($scope, jiniV2bulkService, __jiniEnv, $q) {
                $scope.ListSalesConfigurations = null;  //load configurations
                $scope.ForretningsOmrade = null;        //business areas
                $scope.MediaMaterialtype = null;        //media or media types
                $scope.Konfiguret = [{ Id: '1', Name: 'Konfigureret' }, { Id: '2', Name: 'Afventer loginopsætning' }];               //type (configured or awaiting login setup)
                $scope.selectedForretningsOmrade = "";
                $scope.selectedMediaMaterialtype = "";
                $scope.selectedKonfiguret = "1";
                $scope.profit = 0;
                $scope.percentage = 0;
                $scope.isPercentagedisabled = false;
                $scope.IsDisabled = true;
                $scope.OrignalDataSource = null;
                $scope.progressBarPercent = 0;
                $scope.isPercentageApplied = false;
                $scope.isExactAmountApplied = false;
                $scope.firstTimePageLoad = false;
                $scope.totalRowSaved = 0;
                $scope.totalRowsFailed = 0;
                $scope.failedSalesConfiguration = [];

                $scope.EnableDisable = function () {
                    //If TextBox is disabled it will be enabled and vice versa.
                    $scope.IsDisabled = false;
                }

                $scope.PriceChanged = function (e) {
                    var addPrice = $('#txtPrice').val();
                    var grid = $("#bulkEditGrid").data("kendoGrid");
                    var rows = grid.select().toArray();
                    var selectedRowIds = getSelectedRows(grid);


                    $scope.OrignalDataSource = angular.copy($("#bulkEditGrid").data("kendoGrid").dataSource.data());

                    if (isNaN(addPrice)) {
                        return;
                    }
                    if (addPrice == "") {
                        addPrice = 0;

                    }
                    jiniV2bulkService.PriceChanged(angular.copy($scope.OrignalDataSource), parseInt(addPrice), selectedRowIds, rows)
                    if (addPrice > 0) {

                        //grayed out percentage filed, send current active field name as param
                        grayedOutInactiveTextBox('exactAmount');
                        $scope.isExactAmountApplied = true;
                        var isChangedFoundInPercentage = false;
                        $('#txtPercentage').val('');

                        for (var i = 0; i < rows.length; i++) {

                            var currentRow = rows[i];

                            var id = currentRow.getAttribute("id");
                            if (!$('#' + id).hasClass('changed')) {

                                $('#' + id).addClass('changed');
                                isChangedFoundInPercentage = true;

                            }
                        }

                        if (isChangedFoundInPercentage) {
                            $('#btnGemAendringer').prop('disabled', false);
                            $('#btnGemAendringer').removeClass('gem-btn-inactive').addClass('gem-btn-active');
                        }
                    } else {
                        $scope.isExactAmountApplied = false;
                        for (var i = 0; i < rows.length; i++) {

                            var currentRow = rows[i];

                            var id = currentRow.getAttribute("id");
                            if ($('#' + id).hasClass('changed')) {

                                $('#' + id).removeClass('changed');

                            }
                        }
                        var isChangedFoundInPercentage = false;

                        $("#bulkEditGrid tbody tr.changed").each(function (index, item) {

                            isChangedFoundInPercentage = true;
                        });

                        if (!isChangedFoundInPercentage) {
                            $('#btnGemAendringer').prop('disabled', true);
                            $('#btnGemAendringer').addClass('gem-btn-inactive').removeClass('gem-btn-active');
                        }
                    }
                }

                $scope.IndividualRowPriceChanged = function (e) {

                    var price = e.target.value;

                    var cell = e.target;
                    var row = $(cell).parent().parent()[0];

                    var rowId = row.id;

                    if (isNaN(price) || price == 0) {
                        return;
                    }
                    var checkBoxOfCurrentRow = $(row).find('td input[type="checkbox"]');

                    checkBoxOfCurrentRow.attr('checked', true);

                    var oldObj = prisOld.find(x => x.Id === rowId);
                    var oldValue = oldObj.prisUMom;

                    if (oldValue != price && !$(row).hasClass('changed')) {
                        $(row).addClass('changed');
                        $('#btnGemAendringer').removeClass('gem-btn-inactive').addClass('gem-btn-active');
                        $('#btnGemAendringer').prop('disabled', false);
                        isAllSelected = true;

                    } else if (oldValue == price) {
                        $(row).removeClass('changed');
                    }

                    if ($(row).find('.pris-u-mom')) {
                        $(row).find('.pris-u-mom').removeClass('input-error');
                    }

                    var isChangedFoundInPercentage = false;
                    var isAnyChangedFound = false;
                    $("#bulkEditGrid tbody tr.changed").each(function (index, item) {

                        isChangedFoundInPercentage = true;
                    });



                    if (isChangedFoundInPercentage) {
                        $('#btnGemAendringer').removeClass('gem-btn-inactive').addClass('gem-btn-active');
                        $('#btnGemAendringer').prop('disabled', false);
                    } else {
                        $('#btnGemAendringer').addClass('gem-btn-inactive').removeClass('gem-btn-active');
                        $('#btnGemAendringer').prop('disabled', true);
                    }
                    var target = angular.element(event.target);

                    var grid = $("#bulkEditGrid").data("kendoGrid");

                    var row = target.closest("tr");

                    var txtInputId = e.target.getAttribute("data-item");

                    var priceVatValue = (price * 1.25).toFixed(2);

                    $('#' + txtInputId).val(priceVatValue);
                    grid.select(row);
                    $('#txtPercentage').prop("disabled", false);
                    $('#txtPrice').prop("disabled", false);
                }

                $scope.IndividualRowMMomsChanged = function (e) {

                    var price = e.target.value;

                    var cell = e.target;
                    var row = $(cell).parent().parent()[0];

                    var rowId = row.id;

                    var checkBoxOfCurrentRow = $(row).find('td input[type="checkbox"]');
                    checkBoxOfCurrentRow.attr('checked', true);

                    var oldObj = prisOld.find(x => x.Id === rowId);
                    var oldValue = oldObj.prisUMom;



                    if (oldValue !== price && !$(row).hasClass('changed')) {
                        $(row).addClass('changed');

                    } else if (oldValue === price) {
                        $(row).removeClass('changed');
                    }

                }

                $scope.PriceChangedIncTax = function (e) {

                    var percentage = $('#txtPercentage').val();

                    if (percentage > 100) {
                        return;
                    }

                    var grid = $("#bulkEditGrid").data("kendoGrid");
                    var rows = grid.select().toArray();
                    var selectedRowIds = [];
                    for (var i = 0; i < rows.length; i++) {

                        var currentRow = rows[i];

                        var id = currentRow.getAttribute("id");
                        selectedRowIds.push(id);

                    }

                    $scope.OrignalDataSource = angular.copy($("#bulkEditGrid").data("kendoGrid").dataSource.data());

                    if (isNaN(percentage)) {

                        return;
                    }
                    if (percentage == "") {
                        percentage = 0;
                        var isChangedFoundOnRemovePercentage = false;

                        $("#bulkEditGrid tbody tr.changed").each(function (index, item) {

                            isChangedFoundOnRemovePercentage = true;
                        });

                        if (!isChangedFoundOnRemovePercentage) {
                            $('#btnGemAendringer').prop('disabled', true);
                            $('#btnGemAendringer').addClass('gem-btn-inactive').removeClass('gem-btn-active');
                        }


                    }
                    jiniV2bulkService.PriceChangedIncTax(angular.copy($scope.OrignalDataSource), parseInt(percentage), selectedRowIds, rows)
                    if (percentage > 0) {


                        //grayed out exact amount filed, send current active field name as param
                        grayedOutInactiveTextBox('percentage');
                        var isChangedFoundInPercentage = false;
                        $scope.isPercentageApplied = true;
                        $('#txtPrice').val('');
                        for (var i = 0; i < rows.length; i++) {

                            var currentRow = rows[i];

                            var id = currentRow.getAttribute("id");
                            var currentRowPriceUMOM = $('#' + id).find('.pris-u-mom');
                            var currentRowPriceMMOM = $('#' + id).find('.pris-m-mom');

                            var oldPriceForCurrentRow = prisOld.find(x => x.Id === $('#' + id).attr('id'));
                            if (!$('#' + id).hasClass('changed')) {

                                if (typeof oldPriceForCurrentRow != 'undefined') {

                                    if (oldPriceForCurrentRow.prisUMom != currentRowPriceUMOM.val()) {
                                        $('#' + id).addClass('changed');
                                    }

                                }

                            } else {
                                if (oldPriceForCurrentRow.prisUMom == currentRowPriceUMOM.val()) {
                                    $('#' + id).removeClass('changed');
                                }
                            }
                        }

                        $("#bulkEditGrid tbody tr.changed").each(function (index, item) {

                            isChangedFoundInPercentage = true;
                        });

                        if (isChangedFoundInPercentage) {
                            $('#btnGemAendringer').prop('disabled', false);
                            $('#btnGemAendringer').removeClass('gem-btn-inactive').addClass('gem-btn-active');
                        } else {
                            $('#btnGemAendringer').prop('disabled', true);
                            $('#btnGemAendringer').addClass('gem-btn-inactive').removeClass('gem-btn-active');
                        }
                    } else {
                        $scope.isPercentageApplied = false;
                        for (var i = 0; i < rows.length; i++) {

                            var currentRow = rows[i];

                            var id = currentRow.getAttribute("id");

                            if ($('#' + id).hasClass('changed')) {

                                $('#' + id).removeClass('changed');

                                isChangedFoundInPercentage = false;
                            }




                        }

                        isChangedFoundInPercentage = false;

                        $("#bulkEditGrid tbody tr.changed").each(function (index, item) {

                            isChangedFoundInPercentage = true;
                        });

                        if (!isChangedFoundInPercentage) {
                            $('#btnGemAendringer').prop('disabled', true);
                            $('#btnGemAendringer').addClass('gem-btn-inactive').removeClass('gem-btn-active');
                        }
                    }
                }

                $scope.disabledPerCentageTextBox = function (element) {
                    var ele = element.target.attr("disabled");
                    if (!ele) {
                        $('#txtPercentage').prop("disabled", true);
                    }
                }

                $scope.disabledPriceTextBox = function (element) {
                    var ele = eelement.target.attr("disabled");
                    if (!ele) {
                        $('#txtPrice').prop("disabled", true);
                    }
                }

                $scope.$on("kendoWidgetCreated", function (ev, widget) {
                    if (widget === $scope.Windowforretning) {
                        $scope.Windowforretning.content("<Text><div id=\'departmentTreeview\' style=\'overflow: auto;\'></div></Text>");
                        getForretningsOmrade();


                    }

                    if (widget === $scope.Windowmaterialetype) {
                        $scope.Windowmaterialetype.content("<Text><div id=\'mediaTypeTreeview\' style=\'overflow: auto;\'></div></Text>");
                        getMediaMaterialType();
                    }

                    if (widget === $scope.Windowstatus) {
                        $scope.Windowstatus.content("<Text><div id=\'statusTreeview\' style=\'overflow: auto;\'></div></Text>");
                        getStatus();
                    }
                });

                $scope.reloadGrid = function () {


                    $("#bulkEditGrid").data("kendoGrid").dataSource.read();


                };

                saveBulkConfiguration = function () {
                    var rowChanged = 0;

                    var emptyPriceFound = false;

                    $('#bulkEditGrid table[role="grid"] tr.changed').each(function (e) {
                        rowChanged++

                        var currentPrisField = $(this).find('.pris-u-mom');

                        if (currentPrisField.val() == "") {
                            currentPrisField.addClass('input-error');
                            emptyPriceFound = true;
                        }
                    });


                    if (emptyPriceFound) {
                        emptyPriceFound = false;
                        dialog.kendoDialog({

                            width: "450px",
                            title: "Pris mangler på en eller flere salgsopsætninger",
                            closable: false,
                            dragable: true,
                            modal: true,
                            close: function () {

                            },
                            content: `
                                  <p>Du mangler at udfylde prisen på en eller flere salgsopsætninger på denne side.<p>
                                    <p>Før du kan gemme dine ændringer, skal disse være udfyldt.<p>
                                <br/>
                                `,
                            actions: [
                                {
                                    text: "OK",
                                    action: function () {
                                        dialog.data("kendoDialog").close();
                                        $('.k-widget.k-window.k-dialog').hide();

                                    },
                                    primary: true,
                                }],

                        });
                        dialog.data("kendoDialog").open();

                        return;
                    }


                    if (rowChanged > 0) {
                        //do nothing
                    }
                    else {
                        kendo.alert("No change found on grid, please update a record to proceed.");
                        return;
                    }

                    dialog.kendoDialog({

                        width: "450px",
                        title: "Ønsker du at gemme dine ændringer?",
                        closable: false,
                        dragable: true,
                        modal: true,
                        close: function () {

                        },
                        content: `<p>Du har foretaget ændringer.<p>
                               <br/>
                                  <p>Klik <span style ='font-weight: bold;'>GEM</span> for at gemme dine ændringerne.<p>
                                    <p>Klik <span style ='font-weight: bold;'>Annuller</span> hvis du ikke ønsker dine ændringer gemt.<p>
                                <br/>
                                `,
                        actions: [{
                            text: "Annuller",
                            action: function () {
                                allowReleaseDateFilter = false;
                                dialog.data("kendoDialog").close();
                                $('.k-widget.k-window.k-dialog').hide();
                            },
                        },
                        {
                            text: "GEM",
                            action: function () {
                                dialog.data("kendoDialog").close();
                                $('.k-widget.k-window.k-dialog').hide();
                                $.blockUI();
                                $('#loadernew').show();
                                var grid = $("#bulkEditGrid").data("kendoGrid");

                                var selectedRow = grid.select().toArray();

                                var salesConfiguration = [];

                                var totalSelectRecords = 0;

                                for (var i = 0; i < selectedRow.length; i++) {

                                    var uniqueId = selectedRow[i].getAttribute("id");

                                    if ($('#' + uniqueId).hasClass('changed')) {
                                        totalSelectRecords++;
                                    }
                                }

                                if (totalSelectRecords == 0) {
                                    alert("Please select a row.");
                                    return;
                                }



                                for (var i = 0; i < selectedRow.length; i++) {

                                    var uniqueId = selectedRow[i].getAttribute("id");
                                    var _refSalesconfigTypeCode = uniqueId.split('_')[1];
                                    if ($('#' + uniqueId).hasClass('changed')) {
                                        var id = selectedRow[i].getAttribute("data-item");
                                        var unitPrice = $('.' + id).val();
                                        var unitPriceVat = $('#' + id).val();
                                        var isbn = selectedRow[i].children[1].outerText;
                                        var request = {
                                            Isbn: isbn,
                                            DeflatedPrice: {
                                                Id: uniqueId,
                                                CreatedBy: window.__jiniEnv.userName,
                                                UnitPrice: unitPrice,
                                                UnitPriceVat: unitPriceVat,

                                            },
                                            RefSalesconfigTypeCode: _refSalesconfigTypeCode
                                        };

                                        salesConfiguration.push(request);
                                    }


                                }
                                $scope.Save(salesConfiguration, _refSalesconfigTypeCode);
                            },
                            primary: true,
                        }],

                    });
                    dialog.data("kendoDialog").open();

                };

                $scope.Save = function (salesConfigurations) {
                    var configurations = [];
                    totalRecordsSaved = 0;
                    totalRecordsFailed = 0;
                    faildedRows = [];
                    salesConfigurationWithCount = {};
                    savedRows = [];
                    $scope.totalRowSaved = 0;
                    $scope.totalRowsFailed = 0;

                    for (var i = 0; i < salesConfigurations.length; i++) {
                        var isbn = salesConfigurations[i].Isbn;
                        var refSalesconfigTypeCode = salesConfigurations[i].RefSalesconfigTypeCode == 'null' ? "1001" : salesConfigurations[i].RefSalesconfigTypeCode;
                        //Grouping SalesConfigurations on the bases of isbn and refSalesconfigTypeCode
                        var id = isbn + "-" + refSalesconfigTypeCode;
                        if (!configurations[id]) {
                            configurations[id] = [];
                        }
                        configurations[id].push(salesConfigurations[i].DeflatedPrice);

                    }
                    salesConfigurations = [];
                    for (var configuration in configurations) {
                        var count = configurations[configuration].length;
                        salesConfigurationWithCount[configuration] = count;
                        //splitting the id of configuration to get Isbn and refSalesconfigTypeCode
                        var result = configuration.split("-")
                        salesConfigurations.push({ Isbn: result[0], DeflatedPrice: configurations[configuration], RefSalesconfigTypeCode: result[1]});
                    }


                    var rowsCompleted = 1;
                    var Percent = 0;


                    var totalSelectRecords = salesConfigurations.length;
                    var promiseArray = [];
                    var now = new Date(); //Current DateTime
                    for (var m = 0; m < salesConfigurations.length; m++) {
                        // Every new Sales Configuration will be saved with the time 
                        // decremented by a second ----------------------------------
                        now.setSeconds(now.getSeconds() - m);
                        var datetime = now.getFullYear() + "-" +
                            ("0" + (now.getMonth() + 1)).substr(-2) + "-" +
                            ("0" + now.getDate()).substr(-2) + "T" +
                            ("0" + now.getHours()).substr(-2) + ":" +
                            ("0" + now.getMinutes()).substr(-2) + ":" +
                            ("0" + now.getSeconds()).substr(-2) + ".000Z";
                        salesConfigurations[m].DeflatedPrice.CreatedDate = datetime;
                        //-------------------------------------------------------------
                        var currentPercent = 0;

                        if (rowsCompleted > 0) {
                            currentPercent = (rowsCompleted / totalSelectRecords) * 100;
                        }
                        if (rowsCompleted > 0 && rowsCompleted == totalSelectRecords) {
                            currentPercent = 100;
                        }
                        rowsCompleted++;


                        promiseArray.push(jiniV2bulkService.saveSalesConfiguration(salesConfigurations[m]), $scope);


                    }

                    $q.all(promiseArray).then(function (res) //<-- it should be function in .then
                    {
                        var Percent = 0;
                        var currentPercent = 40;
                        var rowsCompleted = 1;
                        var total = res.length
                        res.forEach(function (res) {
                            if (rowsCompleted > 0) {
                                currentPercent = (rowsCompleted / total) * 100;
                            }
                            if (rowsCompleted > 0 && rowsCompleted == total) {
                                Percent = 100;
                            }
                            if (Percent < currentPercent) {

                                Percent = currentPercent;
                                if (res.IsUpdated == false) {
                                    $scope.failedSalesConfiguration.push(res.Isbn);
                                }
                            }
                            rowsCompleted = rowsCompleted + 1;

                            if (Percent === 100) {
                                $('#txtPercentage').val("");
                                $scope.txtPercentage = '';
                                $scope.txtPrice = '';
                                $('#txtPrice').val("");
                                $('#txtPercentage').prop("disabled", true);
                                $('#txtPrice').prop("disabled", true);
                                $('#btnGemAendringer').addClass('gem-btn-inactive').removeClass('gem-btn-active');
                                $('#btnGemAendringer').prop('disabled', true);
                                $("#txtPrice").addClass('gray-background');
                                $("#txtPercentage").addClass('gray-background');
                                $('#loadernew').hide();

                                $("#bulkEditGrid").data("kendoGrid").dataSource.read();
                                $("#bulkEditGrid").data("kendoGrid").clearSelection();
                                $.unblockUI();
                                $scope.OrignalDataSource = [];



                                $('#btnGemAendringer').prop('disabled', true);
                                $('#btnGemAendringer').removeClass('gem-btn-active').addClass('gem-btn-inactive');


                                
                                if (totalRecordsSaved > 0) {
                                    $scope.totalRowSaved = totalRecordsSaved;
                                    $('#savedCount').show();
                                }
                                if (totalRecordsFailed > 0) {
                                    $scope.totalRowsFailed = totalRecordsFailed;
                                    $('#failedCount').show();
                                }
                                

                                console.log(totalRecordsSaved);
                                $('#successAlert').show();
                                //$('#successAlert').fadeOut(10000);
                                //setTimeout(function () {
                                //    $('#divSavedRecordNotes').fadeIn(3000);
                                //}, 8000);

                               

                                checkedIds = [];
                                isAllSelected = false;
                            }
                        });


                    });
                };

                function getMediaMaterialType() {

                    var mediaTypesUrl = "/JiniV2/GetMediaAndMaterialTypes";
                    FetchAndBindTreeView("#mediaTypeTreeview", getHierarchicalData(mediaTypesUrl), mediaTypeChecked);
                }

                function getForretningsOmrade() {

                    var deptUrl = "/JiniV2/GetDepartmentsAndEditorial";
                    FetchAndBindTreeView("#departmentTreeview", getHierarchicalData(deptUrl), departmentChecked);
                }

                function getStatus() {

                    const status = new kendo.data.HierarchicalDataSource({
                        data: [
                            {
                                "Id": "-1",
                                "Text": "Vælg alle",
                                "HasChildren": true,
                                "Items": [
                                    {
                                        "Id": "1",
                                        "Text": "Konfigureret",
                                        "HasChildren": false,
                                        "Items": []
                                    },
                                    {
                                        "Id": "2",
                                        "Text": "Afventer loginopsætning",
                                        "HasChildren": false,
                                        "Items": []
                                    },
                                ]
                            }
                        ],
                        schema: {
                            model: {
                                id: "Id",
                                hasChildren: "HasChildren",
                                children: "Items"
                            }
                        }
                    });

                    FetchAndBindTreeView("#statusTreeview", status, statusChecked);
                }

                function getHierarchicalData(url) {
                    return new kendo.data.HierarchicalDataSource({
                        transport: {
                            read: {
                                url: url,
                                dataType: "json"
                            }
                        },
                        schema: {
                            model: {
                                id: "Id",
                                hasChildren: "HasChildren",
                                children: "Items"
                            }
                        }
                    });
                }

                // Fetch data and bind to TreeView Filter
                function FetchAndBindTreeView(ctrl, hierarchicalData, checkEvt) {
                    $(ctrl).kendoTreeView({
                        dataSource: hierarchicalData,
                        checkboxes: {
                            checkChildren: true
                        },
                        check: checkEvt,
                        dataBound: treeViewBound,
                        loadOnDemand: false,
                        dataTextField: "Text",


                    });


                }

                $scope.mainGridOptions = {

                    dataSource: {
                        transport: {
                            read: {
                                url: __jiniEnv.jiniApiUrl + "v2/JiniBulkV2/GetDeflatedSalesConfigurations",
                                type: "POST",
                                contentType: "application/json",
                                data: function () {

                                    var sort = $("#bulkEditGrid").data("kendoGrid").dataSource.sort();

                                    if (sort != null && sort.length > 0) {

                                        var sortFiled = sort[0].field;
                                        var sortOrder = sort[0].dir;


                                        if (sortFiled.length > 0) {

                                            gridDataColumns.SortField = sortFiled;
                                            gridDataColumns.SortOrder = sortOrder;
                                        }
                                        else {
                                            gridDataColumns.SortField = "";
                                            gridDataColumns.SortOrder = "";
                                        }
                                    }
                                    else {
                                        gridDataColumns.SortField = "";
                                        gridDataColumns.SortOrder = "";
                                    }

                                    return gridDataColumns;
                                },

                                complete: function (responseData) {

                                    $('#isbnFilter').parent().removeClass('k-state-active');
                                    $('#titleFilter').parent().removeClass('k-state-active');
                                    //set data columns values from filters
                                    for (var f in filtersArray) {


                                        if (filtersArray[f].name === "Isbn") {
                                            $('#isbnFilter').parent().addClass('k-state-active');
                                            $('.isbnFilterInput').val(filtersArray[f].value);
                                            gridDataColumns.Isbn = filtersArray[f].value;
                                        }

                                        if (filtersArray[f].name === "Title") {
                                            gridDataColumns.Title = filtersArray[f].value;
                                            $('.titleFilterInput').val(filtersArray[f].value);
                                            $('#titleFilter').parent().addClass('k-state-active');
                                        }
                                    }


                                    var treeviewMediaType = $("#mediaTypeTreeview").data("kendoTreeView");
                                    if (checkedNodesMediaType.length == 0) {
                                        checkedNodesMediaType = getTreeViewItems(treeviewMediaType, true);
                                    }
                                    var treeviewDepartment = $("#departmentTreeview").data("kendoTreeView");
                                    if (checkedNodesDepartment.length == 0) {
                                        checkedNodesDepartment = getTreeViewItems(treeviewDepartment, true);
                                    }
                                    var treeviewStatus = $("#statusTreeview").data("kendoTreeView");
                                    if (checkedNodesStatus.length == 0) {
                                        checkedNodesStatus = getTreeViewItems(treeviewStatus, true);
                                    }

                                    $('#spnTotalProduct').text(responseData.responseJSON.ProductCount);
                                    $('#spnTotalSalesConfigurations').text(responseData.responseJSON.Total);

       
                                 
                                    faildedRows.forEach(function (value, index, array) {
                                        var row = $('.' + value.id);
                                        row.addClass('failed-row');
                                    });

                                    savedRows.forEach(function (value, index, array) {
                                        var row = $('.' + value.id);
                                        row.removeClass('failed-row');
                                    })
                                }
                            },
                            parameterMap: function (options, type) {
                                return kendo.stringify(options);
                            }
                        },
                        error: onGridError,
                        schema: {

                            data: "Data", total: "Total", model: { id: 'Id' }
                        },
                        batch: true,
                        pageSize: 50,
                        serverPaging: true,
                        change: function (e) {

                        },
                        serverFiltering: true,
                        serverSorting: true
                    },
                    sortable: true,
                    pageable: {
                        alwaysVisible: true,
                        numeric: true,
                    },
                    noRecords: {
                        template: "Ingen resultater fundet"
                    },
                    filterable: true,
                    resizable: true,
                    columnResize: function (e) {
                        adjustLastColumnOnResize();
                    },

                    dataBound: function (e) {

                        if (!$scope.firstTimePageLoad && !($scope.isExactAmountApplied || $scope.isPercentageApplied)) {

                            adjustLastColumn();
                            $scope.firstTimePageLoad = true;
                        }

                        this.expandRow(this.tbody.find("tr.k-master-row").first());
                        this.tbody.find("tr").height(35);


                        if (!window.StartupComplete) {
                            InitBulkEditGridControls();
                        }
                        var perviousRow = null;
                        var rows = e.sender.tbody.children();

                        prisOld = [];
                        for (var j = 0; j < rows.length; j++) {
                            var row = $(rows[j]);

                            if (j > 0) {
                                perviousRow = $(rows[j - 1]);
                            }
                            var dataItem = e.sender.dataItem(row);
                            var perviousDataItem = null;
                            var pervoiusRowIsbn = "";
                            if (perviousRow != null) {
                                perviousDataItem = e.sender.dataItem(perviousRow);
                                pervoiusRowIsbn = perviousDataItem.get("Isbn");
                            }

                            var currentIsbn = dataItem.get("Isbn");
                            var RefSalesConfigTypeCode = (dataItem.get("RefSalesConfigTypeCode") == null) ? "null" : dataItem.get("RefSalesConfigTypeCode");
                            var RefSalesCode = (dataItem.get("RefSalesCode") == null) ? "null" : dataItem.get("RefSalesCode");
                            var RefAccessFormCode = (dataItem.get("RefAccessFormCode") == null) ? "null" : dataItem.get("RefAccessFormCode");
                            var RefPeriodTypeCode = (dataItem.get("RefPeriodTypeCode") == null) ? "null" : dataItem.get("RefPeriodTypeCode");
                            var RefPriceModelCode = (dataItem.get("RefPriceModelCode") == null) ? "null" : dataItem.get("RefPriceModelCode");
                            var UnitValue = (dataItem.get("UnitValue") == null) ? "null" : dataItem.get("UnitValue");
                            var State = (dataItem.get("State") == null) ? "null" : dataItem.get("State");

                            var id = currentIsbn + "_" + RefSalesConfigTypeCode + "_" + RefSalesCode + "_" + RefAccessFormCode + "_" + RefPeriodTypeCode + "_" + RefPriceModelCode + "_" + UnitValue + "_" + State;
                            row.attr("id", id);
                            row.attr("data-item", dataItem.get("Id"));
                            row.addClass(currentIsbn + "-" + RefSalesConfigTypeCode);
                            if (perviousRow != null) {
                                if (currentIsbn == pervoiusRowIsbn) {
                                    perviousRow.hasClass("IsbnGroup") ? row.addClass("IsbnGroup") : row.addClass("default");
                                }
                                else {
                                    perviousRow.hasClass("IsbnGroup") ? row.addClass("default") : row.addClass("IsbnGroup");
                                }
                            }

                            var oldObj = prisOld.find(x => x.Id === id);

                            if (typeof oldObj == 'undefined') {

                                prisOld.push({ Id: id, prisUMom: dataItem.get("UnitPrice"), prisMMom: dataItem.get("UnitPriceVat") })

                            }
                        }

                        Bind();

                        // Enable Filter On dataBound
                        $('#btnWindowforretning').prop("disabled", false);
                        $('#btnWindowMaterialtype').prop("disabled", false);
                        $('#btnWindowStatus').prop("disabled", false);
                    },
                    filterMenuInit: function (e) {
                        if (e.field === "Isbn" || e.field === "Title") {
                            var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");

                            setTimeout(function () {
                                firstValueDropDown.wrapper.hide();
                            });
                        }

                    },
                    selectable: "multiple, row",
                    persistSelection: true,
                    columns: [
                        {
                            selectable: true,
                            width: "55px",
                            filterable: false,
                        },
                        {
                            field: "Isbn",
                            title: "ISBN",
                            headerAttributes: {
                                "class": "k-header k-filterable k-with-icon"
                            },
                            headerTemplate: "<a class='k-grid-filter' id='isbnhyperlink' href='javascript:void(0)' tabindex='-1'><span class='k-icon k-i-filter' id='isbnFilter' title='Filtrer' style='cursor: pointer;'></span></a><span class='k-link'>ISBN</span>",
                            width: "150px",
                            filterable: {
                                extra: false,

                                messages: {

                                    filter: "FILTRER",
                                    clear: "NULSTIL"
                                },
                                operators: {
                                    string: {
                                        contains: "Indehold"
                                    }
                                },
                                cell: {
                                    showOperators: false,
                                    operator: "contains",

                                }
                            }
                        },
                        {
                            field: "Title",
                            title: "Titel og undertitel",
                            width: "230px",
                            headerTemplate: "<a class='k-grid-filter' id='titlehyperlink' href='javascript:void(0)' tabindex='-1'><span class='k-icon k-i-filter' id='titleFilter' title='Filtrer' style='cursor: pointer;'></span></a><span class='k-link'>Titel og undertitel</span>",
                            filterable: {
                                extra: false,
                                messages: {
                                    filter: "FILTRER",
                                    clear: "NULSTIL"
                                },
                                operators: {
                                    string: {
                                        contains: "Indehold..."
                                    }
                                },
                                cell: {
                                    showOperators: false,
                                    operator: "contains"
                                }
                            }
                        },
                        {
                            field: "Salgsform",
                            title: "Salgsform",
                            headerAttributes: { "style": "font-size:16px;" },
                            width: "200px",
                            template: "<span>#= RefSalesConfigTypeCode == 1002 ? 'GUA' : RefAccessFormDisplayName + (RefPriceModelDisplayName != null ? ' / ' + RefPriceModelDisplayName : '' )  #</span>",
                            filterable: false,
                            sortable: false

                        },
                        {
                            field: "RefSalesDisplayName",
                            title: "Abo./Leje",
                            width: "100px",
                            template: '#if(RefSalesDisplayName  == "Abonnement")  {#Abo.# } else {##:RefSalesDisplayName##}#',
                            filterable: false,
                            sortable: true
                        },
                        {
                            field: "RefPeriodTypeDisplayName",
                            title: "Periode",
                            width: "150px",
                            filterable: false,
                            sortable: true,
                            template: "<span>#=UnitValue# #=RefPeriodTypeDisplayName# #if(RefSalesDisplayName  == 'Abonnement')  {#binding# } else {#adgang#}#</span>",

                        },
                        {
                            field: "ReleaseDate",
                            title: "Udgivelsesdato",
                            template: "#= kendo.toString(kendo.parseDate(ReleaseDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #",
                            width: "150px",
                            filterable: false,
                            sortable: true,
                            headerTemplate: "<a class='k-grid-filter' id='releasedatehyperlink' href='javascript:void(0)' tabindex='-1'><span class='k-icon k-i-filter' id='releasedateFilter' title='Filtrer' style='cursor: pointer;'></span></a><span class='k-link'>Udgivelsesdato</span>"
                        },
                        {
                            field: "LastModified",
                            title: "Sidst ændret",
                            template: "#= kendo.toString(kendo.parseDate(LastModified, 'yyyy-MM-dd'), 'dd-MM-yyyy') #",
                            width: "150px",
                            filterable: false,
                            sortable: true,
                            headerTemplate: "<a class='k-grid-filter' id='lastmodifiedhyperlink' href='javascript:void(0)' tabindex='-1'><span class='k-icon k-i-filter' id='lastmodifiedFilter' title='Filtrer' style='cursor: pointer;'></span></a><span class='k-link'>Sidst ændret</span>"
                        },
                        {
                            field: "UnitPrice",
                            title: "Pris u. moms kr.",
                            template: "<input data-item='#=Id#' class ='#=Id# alignCenter pris-u-mom' value='#=UnitPrice#' onkeypress='return event.charCode >= 48 && event.charCode <= 57'  ng-keyup='IndividualRowPriceChanged($event)'  data-bind='value:UnitPrice' type='number' data-role='numerictextbox' style='width:75px; margin:0px;'/>",
                            width: "140px",
                            filterable: false,
                            sortable: true,
                        },
                        {
                            field: "UnitPriceVat",
                            title: "Pris m. moms kr.",
                            template: "<input id='#=Id#' class ='alignCenter pris-m-mom' value='#=UnitPriceVat#' readonly  ng-keyup='IndividualRowMMomsChanged($event)' data-bind='value:UnitPriceVat' type='number' data-role='numerictextbox' style='width:75px; margin:0px;' />",
                            width: "140px",
                            filterable: false,
                            sortable: true,
                        }]
                };

                //Event Handler for MediaType filter change
                mediaTypeChecked = function (e) {


                    var checkedNode = e.node;
                    var treeview = $("#mediaTypeTreeview").data("kendoTreeView");
                    var dataNode = treeview.dataItem(checkedNode);

                    var currentNode = $(e.node).find('input[type="checkbox"]').first();



                    var isChecked = currentNode.prop('checked');


                    if (allowMediaTypetFilter == false && !mediaTypeFilterCancelClicked) {
                        clearSelectionOnMediaTypeFilter(e);
                    }


                    if (allowMediaTypetFilter) {

                        allowMediaTypetFilter = false;

                        var field = "MediaMaterialName";

                        createAndApplyFilters(e.sender, field, "Text", true);
                        checkedNodesMediaType = [];


                        checkedNodesMediaType = getTreeViewItems(treeview, true);
                        $scope.reloadGrid();
                    } else if (mediaTypeFilterCancelClicked) {
                        if (dataNode.Text == "Vælg alle") {
                            var barParent = treeview.findByUid(dataNode.uid);
                            treeview.dataItem(barParent).set("checked", false);
                            for (var i = 0; i < checkedNodesMediaType.length; i++) {
                                var currntNode = treeview.findByUid(checkedNodesMediaType[i].uid);
                                var currentDataNode = treeview.dataItem(currntNode);
                                if (typeof currentDataNode !== 'undefined' && currentDataNode.Text !== "Vælg alle") {
                                    if (typeof checkedNodesMediaType.find(x => x.uid == currentDataNode.uid) !== 'undefined') {
                                        isChecked = checkedNodesMediaType.find(x => x.uid == currentDataNode.uid).Checked;
                                        var bar = treeview.findByUid(currentDataNode.uid);

                                        treeview.dataItem(bar).set("checked", isChecked);
                                    }

                                }
                            }

                        } else {
                            if (dataNode.hasChildren) {
                                for (var i = 0; i < dataNode.Items.length; i++) {
                                    var item = dataNode.Items[i];
                                    var bar = treeview.findByUid(item.uid);



                                    if (typeof checkedNodesMediaType.find(x => x.uid == item.uid) !== 'undefined') {
                                        isChecked = checkedNodesMediaType.find(x => x.uid == item.uid).Checked;
                                    } else {
                                        isChecked = false;
                                    }
                                    treeview.dataItem(bar).set("checked", isChecked);
                                }
                            } else {
                                var bar = treeview.findByUid(dataNode.uid);
                                treeview.dataItem(bar).set("checked", !isChecked);
                            }
                        }

                        mediaTypeFilterCancelClicked = false;
                    }




                }


                //Event Handler for department change
                departmentChecked = function (e) {

                    var checkedNode = e.node;
                    var treeview = $("#departmentTreeview").data("kendoTreeView");
                    var dataNode = treeview.dataItem(checkedNode);

                    var currentNode = $(e.node).find('input[type="checkbox"]').first();
                    var isChecked = currentNode.prop('checked');


                    if (allowDepartmentFilter == false && !departmentFilterCancelClicked) {
                        clearSelectionOnDepartmentFilter(e);
                    }


                    if (allowDepartmentFilter) {
                        var field = "SectionCode";
                        allowDepartmentFilter = false;
                        createAndApplyFilters(e.sender, field, "Id", false);
                        checkedNodesDepartment = [];


                        checkedNodesDepartment = getTreeViewItems(treeview, true);
                        $scope.reloadGrid();
                    } else if (departmentFilterCancelClicked) {
                        if (dataNode.Text == "Vælg alle") {
                            var barParent = treeview.findByUid(dataNode.uid);
                            treeview.dataItem(barParent).set("checked", false);
                            for (var i = 0; i < checkedNodesDepartment.length; i++) {
                                var currntNode = treeview.findByUid(checkedNodesDepartment[i].uid);
                                var currentDataNode = treeview.dataItem(currntNode);
                                if (typeof currentDataNode !== 'undefined' && currentDataNode.Text !== "Vælg alle") {
                                    if (typeof checkedNodesDepartment.find(x => x.Id == currentDataNode.Id) !== 'undefined') {
                                        if (currentDataNode.Text == "Hans Reitzel red.") {

                                        }

                                        if (!currentDataNode.hasChildren) {
                                            var isChecked = checkedNodesDepartment.find(x => x.Id == currentDataNode.Id).Checked;
                                            var bar = treeview.findByUid(currentDataNode.uid);
                                            treeview.dataItem(bar).set("checked", true);
                                        }
                                    }

                                }
                            }

                        } else {
                            if (dataNode.hasChildren) {
                                var barParent = treeview.findByUid(dataNode.uid);
                                treeview.dataItem(barParent).set("checked", false);
                                for (var i = 0; i < dataNode.Items.length; i++) {
                                    var item = dataNode.Items[i];
                                    var bar = treeview.findByUid(item.uid);



                                    if (typeof checkedNodesDepartment.find(x => x.Id == item.Id) !== 'undefined') {
                                        isChecked = checkedNodesDepartment.find(x => x.Id == item.Id).Checked;
                                        var bar = treeview.findByUid(item.uid);
                                        treeview.dataItem(bar).set("checked", true);
                                    }

                                }
                            }
                            else {
                                var barParent = treeview.findByUid(dataNode.uid);
                                treeview.dataItem(barParent).set("checked", false);
                                if (typeof checkedNodesDepartment.find(x => x.Id == dataNode.Id) !== 'undefined') {
                                    var bar = treeview.findByUid(dataNode.uid);
                                    treeview.dataItem(bar).set("checked", true);
                                }

                            }
                        }

                        departmentFilterCancelClicked = false;
                    }

                }

                //Event Handler for status change
                statusChecked = function (e) {



                    var checkedNode = e.node;
                    var treeview = $("#statusTreeview").data("kendoTreeView");
                    var dataNode = treeview.dataItem(checkedNode);

                    var currentNode = $(e.node).find('input[type="checkbox"]').first();
                    var isChecked = currentNode.prop('checked');


                    if (allowStatusFilter == false && !statusFilterCancelClicked) {
                        clearSelectionOnStatusFilter(e);
                    }


                    if (allowStatusFilter) {
                        var field = "status";
                        allowStatusFilter = false;

                        createAndApplyFilters(e.sender, field, "Id", false);
                        checkedNodesStatus = [];


                        checkedNodesStatus = getTreeViewItems(treeview, true);
                        $scope.reloadGrid();
                    } else if (statusFilterCancelClicked) {
                        if (dataNode.Text == "Vælg alle") {
                            var barParent = treeview.findByUid(dataNode.uid);
                            treeview.dataItem(barParent).set("checked", false);
                            for (var i = 0; i < checkedNodesStatus.length; i++) {
                                var currntNode = treeview.findByUid(checkedNodesStatus[i].uid);
                                var currentDataNode = treeview.dataItem(currntNode);
                                if (typeof currentDataNode !== 'undefined' && currentDataNode.Text !== "Vælg alle") {
                                    if (typeof checkedNodesStatus.find(x => x.uid == currentDataNode.uid) !== 'undefined') {
                                        isChecked = checkedNodesStatus.find(x => x.uid == currentDataNode.uid).Checked;
                                        var bar = treeview.findByUid(currentDataNode.uid);

                                        treeview.dataItem(bar).set("checked", true);
                                    }

                                }
                            }

                        } else {
                            if (dataNode.hasChildren) {
                                for (var i = 0; i < dataNode.Items.length; i++) {
                                    var item = dataNode.Items[i];
                                    var bar = treeview.findByUid(item.uid);
                                    isChecked = checkedNodesStatus.find(x => x.uid == item.uid).Checked;
                                    treeview.dataItem(bar).set("checked", isChecked);
                                }
                            } else {
                                var bar = treeview.findByUid(dataNode.uid);
                                treeview.dataItem(bar).set("checked", !isChecked);
                            }
                        }
                        statusFilterCancelClicked = false;
                    }




                }

                // Check All Treeview Nodes
                function checkNodes(nodes) {
                    for (var i = 0; i < nodes.length; i++) {
                        nodes[i].checked = true;
                        if (nodes[i].hasChildren) {
                            checkNodes(nodes[i].children.view());
                        }
                    }
                }

                // Perform additional operations after department TreeView Data Bound
                function treeViewBound(e) {
                    $("#departmentTreeview").children().unbind("click");
                    $("#mediaTypeTreeview").children().unbind("click");
                    $("#statusTreeview").children().unbind("click");

                    var treeView = e.sender;
                    treeView.expand("> .k-group > .k-item");

                    e.sender.element.find('input[type="checkbox"]').attr('checked', 'checked');
                    var items = treeView.dataSource.view();
                    checkNodes(items);


                }
            }]
        );


})();

function uncheckNodes(nodes) {
    for (var i = 0; i < nodes.length; i++) {
        nodes[i].checked = false;
        if (nodes[i].hasChildren) {
            uncheckNodes(nodes[i].children.view());
        }
    }
}


var checkedNodesMediaType = [];
var checkedNodesDepartment = [];
var checkedNodesStatus = [];
var _field = "";
var _isAllMediaTypedChecked = false;
var _isAllDepartmentsChecked = false;
var statusChecked;
var allowStatusFilter = false;
var statusFilterCancelClicked = false;
var mediaTypeChecked;
var allowMediaTypetFilter = false;
var mediaTypeFilterCancelClicked = false;
var departmentChecked;
var allowDepartmentFilter = false;
var departmentFilterCancelClicked = false;
function createAndApplyFilters(sender, field, filterProperty, includeParent) {

    _isAllMediaTypedChecked = false;
    _isAllDepartmentsChecked = false;
    var checkedNodes = getTreeViewItems(sender, true);


    _field = field;
    if (field === "SectionCode") {

        gridDataColumns.DepartmentsSections = [];
        for (var i = 0; i < checkedNodes.length; i++) {

            var objTemp = { DepartmentCode: checkedNodes[i].parentNode().Id, SectionCode: checkedNodes[i].Id }

            gridDataColumns.DepartmentsSections.push(objTemp);
        }

        gridDataColumns.IsAllDepartmentChecked = _isAllDepartmentsChecked;



    }

    if (field === "MediaMaterialName") {

        gridDataColumns.MediaMaterialTypes = [];
        for (var i = 0; i < checkedNodes.length; i++) {

            var objTemp = { MediaTypeCode: checkedNodes[i].parentNode().Id, MaterialTypeCode: checkedNodes[i].Id }

            gridDataColumns.MediaMaterialTypes.push(objTemp);
        }

        gridDataColumns.IsAllMediaTypesChecked = _isAllMediaTypedChecked;
    }

    if (field === "status") {
        var statuses = [];
        for (var i = 0; i < checkedNodes.length; i++) {

            statuses.push(checkedNodes[i].Id);

        }

        gridDataColumns.SalesConfigurationStates = statuses;

    }

    var unCheckedNodes = getTreeViewItems(sender, false);

    if (unCheckedNodes.length > 0) {
        if (_field == "MediaMaterialName") {
            $('#btnWindowMaterialtype').addClass('btn-red');
        }
        else if (_field == "SectionCode") {
            $('#btnWindowforretning').addClass('btn-red');
        }
        else if (_field == "status") {
            $('#btnWindowStatus').addClass('btn-red');
        }
    }
    if (unCheckedNodes.length < 1) {
        if (_field == "MediaMaterialName") {
            $('#btnWindowMaterialtype').removeClass('btn-red');
        }
        else if (_field == "SectionCode") {
            $('#btnWindowforretning').removeClass('btn-red');
        }
        else if (_field == "status") {
            $('#btnWindowStatus').removeClass('btn-red');
        }
    }
}
// Get Treeview Items
function getTreeViewItems(treeview, checked) {

    var nodes = treeview.dataSource.view();

    return getChildrenNodesOfTreeView(nodes, checked);
}

function getChildrenNodesOfTreeView(nodes, checked) {
    var node, childNodes;
    var nodesArr = [];

    for (var i = 0; i < nodes.length; i++) {
        node = nodes[i];


        // Get only children
        if (node.checked === checked && !node.hasChildren) {

            nodesArr.push(node);
        }

        if (node.hasChildren) {
            if (_field === "MediaMaterialName") {
                if (node.Id === '-1' && node.checked === true) {
                    _isAllMediaTypedChecked = true;
                }
            }

            if (_field === "SectionCode") {
                if (node.Id === '-1' && node.checked === true) {
                    _isAllDepartmentsChecked = true;
                }
            }
            childNodes = getChildrenNodesOfTreeView(node.children.view(), checked);

            if (childNodes.length > 0) {
                nodesArr = nodesArr.concat(childNodes);
            }
        }
    }
    return nodesArr;
}

var allowFilter = false;
function clearSelectionOnReleaseDateFilter() {



    if (checkedIds.length > 0 || isAllSelected) {
        dialog.kendoDialog({

            width: "450px",
            title: "Ønsker du at forlade denne side?",
            closable: false,
            dragable: true,
            modal: true,
            close: function () {
            },
            content: `<p>Hvis du fortsætter, vil de produkter, du har sat flueben ved på denne side blive fjernet, når du forlader siden.<p>
                               <br/>
                                Klik OK for at fortsætte.<br/>Klik Annuller hvis du ønsker at blive på denne side`,
            actions: [{
                text: "Annuller",
                action: function () {
                    allowReleaseDateFilter = false;
                    dialog.data("kendoDialog").close();
                    $('.k-widget.k-window.k-dialog').hide();
                },
            },
            {
                text: "OK",
                action: function () {

                    clearSelection();
                    disableTextBoxs();
                    checkedIds = [];
                   

                    isAllSelected = false;
                    restoreOldPrices();
                    allowReleaseDateFilter = true;

                    dialog.data("kendoDialog").close();
                    $('.k-widget.k-window.k-dialog').hide();
                    releaseDateFilterApplied();
                },
                primary: true,
            }],

        });
        dialog.data("kendoDialog").open();

    } else {
        allowReleaseDateFilter = true;
        releaseDateFilterApplied();
    }
}

function clearSelectionOnStatusFilter(e) {

    if (checkedIds.length > 0 || isAllSelected) {

        dialog.kendoDialog({

            width: "450px",
            title: "Ønsker du at forlade denne side?",
            closable: false,
            dragable: true,
            modal: true,
            close: function () {

            },
            content: `<p>Hvis du fortsætter, vil de produkter, du har sat flueben ved på denne side blive fjernet, når du forlader siden.<p>
                               <br/>
                                Klik OK for at fortsætte.<br/>Klik Annuller hvis du ønsker at blive på denne side`,
            actions: [{
                text: "Annuller",
                action: function () {
                    allowStatusFilter = false;
                    statusFilterCancelClicked = true;
                    dialog.data("kendoDialog").close();
                    $('.k-widget.k-window.k-dialog').hide();
                    statusChecked(e);
                },
            },
            {
                text: "OK",
                action: function () {

                    clearSelection();
                    disableTextBoxs();
                    checkedIds = [];
                    dialog.data("kendoDialog").close();
                    $('.k-widget.k-window.k-dialog').hide();

                    isAllSelected = false;
                    restoreOldPrices();
                    allowStatusFilter = true;
                    statusChecked(e);


                },
                primary: true,
            }],

        });
        dialog.data("kendoDialog").open();

    } else {
        allowStatusFilter = true;

        statusChecked(e);
    }
}

function clearSelectionOnMediaTypeFilter(e) {

    if (checkedIds.length > 0 || isAllSelected) {


        dialog.kendoDialog({

            width: "450px",
            title: "Ønsker du at forlade denne side?",
            closable: false,
            dragable: true,
            modal: true,
            close: function () {

            },
            content: `<p>Hvis du fortsætter, vil de produkter, du har sat flueben ved på denne side blive fjernet, når du forlader siden.<p>
                               <br/>
                                Klik OK for at fortsætte.<br/>Klik Annuller hvis du ønsker at blive på denne side`,
            actions: [{
                text: "Annuller",
                action: function () {
                    allowMediaTypetFilter = false;
                    mediaTypeFilterCancelClicked = true;

                    mediaTypeChecked(e);
                    dialog.data("kendoDialog").close();
                    $('.k-widget.k-window.k-dialog').hide();
                },
            },
            {
                text: "OK",
                action: function () {
                    clearSelection();
                    disableTextBoxs();
                    checkedIds = [];

                    dialog.data("kendoDialog").close();
                    $('.k-widget.k-window.k-dialog').hide();
                    isAllSelected = false;
                    restoreOldPrices();
                    allowMediaTypetFilter = true;
                    mediaTypeChecked(e);

                },
                primary: true,
            }],

        });
        dialog.data("kendoDialog").open();

    } else {
        allowMediaTypetFilter = true;

        mediaTypeChecked(e);
    }
}


function clearSelectionOnDepartmentFilter(e) {



    if (checkedIds.length > 0 || isAllSelected) {


        dialog.kendoDialog({

            width: "450px",
            title: "Ønsker du at forlade denne side?",
            closable: false,
            dragable: true,
            modal: true,
            close: function () {

            },
            content: `<p>Hvis du fortsætter, vil de produkter, du har sat flueben ved på denne side blive fjernet, når du forlader siden.<p>
                               <br/>
                                Klik OK for at fortsætte.<br/>Klik Annuller hvis du ønsker at blive på denne side`,
            actions: [{
                text: "Annuller",
                action: function () {
                    allowDepartmentFilter = false;
                    departmentFilterCancelClicked = true;
                    dialog.data("kendoDialog").close();
                    $('.k-widget.k-window.k-dialog').hide();
                    departmentChecked(e);
                },
            },
            {
                text: "OK",
                action: function () {

                    clearSelection();
                    disableTextBoxs();
                    checkedIds = [];

                    dialog.data("kendoDialog").close();
                    $('.k-widget.k-window.k-dialog').hide();
                    isAllSelected = false;
                    restoreOldPrices();
                    allowDepartmentFilter = true;
                    departmentChecked(e);

                },
                primary: true,
            }],

        });
        dialog.data("kendoDialog").open();

    } else {
        allowDepartmentFilter = true;

        departmentChecked(e);
    }
}

function clearSelectionOnModifiedDateFilter() {



    if (checkedIds.length > 0 || isAllSelected) {



        dialog.kendoDialog({

            width: "450px",
            title: "Ønsker du at forlade denne side?",
            closable: false,
            dragable: true,
            modal: true,
            close: function () {
            },
            content: `<p>Hvis du fortsætter, vil de produkter, du har sat flueben ved på denne side blive fjernet, når du forlader siden.<p>
                               <br/>
                                Klik OK for at fortsætte.<br/>Klik Annuller hvis du ønsker at blive på denne side`,
            actions: [{
                text: "Annuller",
                action: function () {
                    allowModifiedDateFilter = false;
                    dialog.data("kendoDialog").close();
                    $('.k-widget.k-window.k-dialog').hide();
                    departmentChecked(e);
                },
            },
            {
                text: "OK",
                action: function () {
                    clearSelection();
                    disableTextBoxs();
                    checkedIds = [];

                    dialog.data("kendoDialog").close();
                    $('.k-widget.k-window.k-dialog').hide();
                    isAllSelected = false;
                    restoreOldPrices();
                    allowModifiedDateFilter = true;
                    lastModifiedFilterApplied();

                },
                primary: true,
            }],

        });
        dialog.data("kendoDialog").open();

    } else {
        allowModifiedDateFilter = true;
        lastModifiedFilterApplied();
    }
}

function applyFilters(e) {


    if (e.filter == null) {

        //Remove current clreared filter from filters arrayd
        for (var cf in filtersArray) {

            if (filtersArray[cf].name == e.field) {

                delete filtersArray[cf];
                gridDataColumns[e.field] = "";

            }
        }
        //remove empty items from filters array
        filtersArray = filtersArray.filter(function (el) {
            return el != null;
        });

    } else {   //if any filter changed or applied first time

        var isFilterUpdated = false;

        //Loop through all filter if alreay applied then update value
        for (var f in filtersArray) {

            if (filtersArray[f].name === e.filter.filters[0].field) {
                //upated value of applied filter in filters array
                filtersArray[f].value = e.filter.filters[0].value;

                var isFilterUpdated = true;
                break; //Stop this loop, we found it!
            }
        }
        if (!isFilterUpdated) {
            filtersArray.push({ name: e.filter.filters[0].field, value: e.filter.filters[0].value });
        }

    }

    $('#isbnFilter').parent().removeClass('k-state-active');
    $('#titleFilter').parent().removeClass('k-state-active');
    //set data columns values from filters
    for (var f in filtersArray) {

        if (filtersArray[f].name === "Isbn") {
            $('#isbnFilter').parent().addClass('k-state-active');
            gridDataColumns.Isbn = filtersArray[f].value;
        }

        if (filtersArray[f].name === "Title") {
            gridDataColumns.Title = filtersArray[f].value;
            $('#titleFilter').parent().addClass('k-state-active');
        }
    }

}

function clearSelection() {
    $("#bulkEditGrid").data("kendoGrid").clearSelection();
}

//create array to hold all applied filters
var filtersArray = [];


var gridDataColumns = {
    Isbn: "",
    Title: "",
    ReleaseDate: "",
    ModifiedDate: "",
    DepartmentCodes: "",
    SectionCodes: "",
    MaterialTypeCodes: "",
    MediaTypeCodes: "",
    ReleaseStartDate: "",
    ReleaseEndDate: "",
    ModifiedStartDate: "",
    ModifiedEndDate: "",
    SalesConfigurationStates: "",
    MediaMaterialTypes: [],
    DepartmentsSections: [],
    IsAllMediaTypesChecked: true,
    IsAllDepartmentChecked: true

}
function enableTextBoxs() {
    $('#txtPercentage').prop("disabled", false).removeClass('gray-background');
    $('#txtPrice').prop("disabled", false).removeClass('gray-background');
}

var isAllSelected = false;

function disableTextBoxs() {
    $('#txtPercentage').val("");
    $('#txtPrice').val("");
    $('#txtPercentage').prop("disabled", true);
    $('#txtPrice').prop("disabled", true);

    $("#txtPrice").addClass('gray-background');
    $("#txtPercentage").addClass('gray-background');

    $('#btnGemAendringer').prop('disabled', true);
    $('#btnGemAendringer').removeClass('gem-btn-active').addClass('gem-btn-inactive');

    var scope = angular.element($("select")).scope();
    scope.$apply(function () {
        scope.txtPercentage = '';
        scope.txtPrice = '';
    });


}
var checkedIds = [];
function selectRow(el) {
    var rowId = el.target.parentElement.parentElement.id;
    isAllSelected = false;
    isAnySelected = true;
    if (el.currentTarget.checked) {
        if (!checkedIds.includes(rowId)) {
            checkedIds.push(rowId);
        }
        enableTextBoxs();
        $('#txtPercentage').removeClass('gray-background');
        $('#txtPrice').removeClass('gray-background');

        var percentage = $('#txtPercentage').val();
        var price = $('#txtPrice').val();
        if (percentage != "" || price != "") {
            priceChangeOnIndiviualRow(el.target.parentElement.parentElement,
                percentage == "" ? price : percentage, percentage != "");
        }

    } else {

        var row = $('#' + rowId);
        if (row.hasClass('changed')) {
            row.removeClass('changed');

            var currentRowPriceUMOM = row.find('.pris-u-mom');
            var currentRowPriceMMOM = row.find('.pris-m-mom');
            if (currentRowPriceUMOM.hasClass('input-error')) {
                currentRowPriceUMOM.removeClass('input-error');
            }
            var oldPriceForCurrentRow = prisOld.find(x => x.Id === rowId);

            if (typeof oldPriceForCurrentRow != 'undefined') {
                currentRowPriceUMOM.val(oldPriceForCurrentRow.prisUMom);
                currentRowPriceMMOM.val(oldPriceForCurrentRow.prisMMom);
            }
        }
        var isAnyChangedFound = false;
        $("#bulkEditGrid tbody tr.changed").each(function (index, item) {

            isAnyChangedFound = true;
        });

        if (!isAnyChangedFound) {
            $('#btnGemAendringer').prop('disabled', true);
            $('#btnGemAendringer').removeClass('gem-btn-active').addClass('gem-btn-inactive');
        }


        var index = checkedIds.indexOf(rowId);
        if (index > -1) {
            checkedIds.splice(index, 1);
        }
        if (checkedIds.length <= 0) {
            disableTextBoxs();

        }
    }
}

function selectAll(el) {
    if (el.currentTarget.checked) {
        enableTextBoxs();
        isAllSelected = true;
        $('#txtPercentage').removeClass('gray-background');
        $('#txtPrice').removeClass('gray-background');

        $("#bulkEditGrid tbody tr").each(function () {

            var rowElement = this;
            var row = $(rowElement);
            checkedIds.push(row.attr('id'));
        });

        var percentage = $('#txtPercentage').val();
        var price = $('#txtPrice').val();
        if (percentage != "" || price != "") {
            priceChangeOnAllRows(percentage == "" ? price : percentage, percentage != "");
        }

    } else {
        disableTextBoxs();
        checkedIds = [];
        restoreOldPrices();


    }


}
var prisOld = [];

var prisUMomsNew = [
    { id: '', value: '' }
]


function Bind() {
    $('#bulkEditGrid').on('click', 'table tr td input[type="checkbox"]', function (e) {
        selectRow(e);
        e.stopImmediatePropagation();
    });

    $('#bulkEditGrid').on('click', 'table thead tr th input[type="checkbox"]', function (e) {
        selectAll(e);
        e.stopImmediatePropagation();
    });

    $('#bulkEditGrid').on('focusin', 'table tr td input[type="number"]', function (el) {
        var cell = el.target;
        var row = $(cell).parent().parent()[0];
        var rowId = row.Id;

        var prisUMoms = row.cells[8].children[0].value;
        var prisMMoms = row.cells[9].children[0].value;

        $(".pris-u-mom").keydown(function (e) {
            if (!$(this).val() || (parseInt($(this).val()) > 0))
                $(this).data("old", $(this).val());
        });
        $(".pris-u-mom").keyup(function (e) {
            if (!$(this).val() || (parseInt($(this).val()) > 0));
            else
                $(this).val($(this).data("old"));
        });

    });

    $("#bulkEditGrid tbody").on("click", "tr", function (e) {

        var rowElement = this;
        var row = $(rowElement);

        if (!$(e.target).hasClass('pris-m-mom') && !$(e.target).hasClass('k-checkbox') && !$(e.target).hasClass('pris-u-mom')) {
            checkedIds = [];
            checkedIds.push(row.attr('id'));
            row.addClass('k-state-selected');
            row.find('td input[type="checkbox"]').prop('checked', true)
        }

        if (!$(e.target).hasClass('pris-m-mom') && !$(e.target).hasClass('pris-u-mom') && !$(e.target).hasClass('k-checkbox')) {
            $("#bulkEditGrid tbody tr").each(function (index, item) {


                var thisRow = $(item);
                if (thisRow.attr('id') !== row.attr('id')) {
                    if (thisRow.hasClass('changed')) {
                        thisRow.removeClass('changed');

                        var currentRowPriceUMOM = thisRow.find('.pris-u-mom');
                        var currentRowPriceMMOM = thisRow.find('.pris-m-mom');
                        if (currentRowPriceUMOM.hasClass('input-error')) {
                            currentRowPriceUMOM.removeClass('input-error');
                        }
                        var oldPriceForCurrentRow = prisOld.find(x => x.Id === thisRow.attr('id'));

                        if (typeof oldPriceForCurrentRow != 'undefined') {

                            currentRowPriceUMOM.val(oldPriceForCurrentRow.prisUMom);
                            currentRowPriceMMOM.val(oldPriceForCurrentRow.prisMMom);
                        }
                    }
                }

            });
        }

        if ($(e.target).hasClass('pris-u-mom')) {
            row.addClass("k-state-selected");
            row.find('td input[type="checkbox"]').prop('checked', true);
            if (!checkedIds.includes(row.attr('id'))) {
                checkedIds.push(row.attr('id'));
            }
            enableTextBoxs();
        }

        if (row.hasClass("k-state-selected")) {
            if (!checkedIds.includes(row.attr('id'))) {
                checkedIds.push(row.attr('id'));
            }
            enableTextBoxs();
        } else {

        }

        var isAnyChangedFound = false;
        $("#bulkEditGrid tbody tr.changed").each(function (index, item) {

            isAnyChangedFound = true;
        });

        if (!isAnyChangedFound) {
            $('#btnGemAendringer').prop('disabled', true);
            $('#btnGemAendringer').removeClass('gem-btn-active').addClass('gem-btn-inactive');

        }

    });


    var isFilterValid = false;
    var isFiltered = false;
    var isFilterCanceled = false;
    function filterData(el) {

        var showAlert = false;

        if (el.filter == null) {
            if (el.field == "Title" && $('#titleFilter').parent().hasClass('k-state-active')) {
                if ($('.titleFilterInput').val().length > 0) {
                    showAlert = true;
                }
            }
            else {
                el.preventDefault();
            }
            if (el.field == "Isbn" && $('#isbnFilter').parent().hasClass('k-state-active')) {
                if ($('.isbnFilterInput').val().length > 0) {
                    showAlert = true;
                }

            } else {
                el.preventDefault();
            }
        } else {
            showAlert = true;
        }

        if (checkedIds.length > 0 || isFilterValid || isAllSelected) {
            if (!isFilterValid && showAlert) {
                dialog.kendoDialog({

                    width: "450px",
                    title: "Ønsker du at forlade denne side?",
                    closable: false,
                    dragable: true,
                    modal: true,
                    close: function () {

                    },
                    content: `<p>Hvis du fortsætter, vil de produkter, du har sat flueben ved på denne side blive fjernet, når du forlader siden.<p>
                               <br/>
                                Klik OK for at fortsætte.<br/>Klik Annuller hvis du ønsker at blive på denne side`,
                    actions: [{
                        text: "Annuller",
                        action: function () {
                            isFilterCanceled = true;
                            dialog.data("kendoDialog").close();
                            $('.k-widget.k-window.k-dialog').hide();
                        },
                    },
                    {
                        text: "OK",
                        action: function () {
                            isFilterValid = true;
                            isFiltered = false;
                            dialog.data("kendoDialog").close();
                            $('.k-widget.k-window.k-dialog').hide();

                            clearSelection();
                            disableTextBoxs();
                            checkedIds = [];


                            restoreOldPrices();
                            filterData(el);
                        },
                        primary: true,
                    }],

                });
                dialog.data("kendoDialog").open();


            } else if (!isFiltered) {

                if (!isFilterCanceled) {
                    isFiltered = true;
                    isAllSelected = false;
                    isFilterValid = false;
                    applyFilters(el);
                    if (el.filter == null) {
                        $("#bulkEditGrid").data("kendoGrid").dataSource.filter();
                        $("#bulkEditGrid").data("kendoGrid").dataSource.read();
                    } else {
                        $("#bulkEditGrid").data("kendoGrid").dataSource.filter({ logic: "and", filters: el.filter });
                    }
                }
                else {
                    isFilterCanceled = false;
                }

            }
        } else {
            if (!isFilterCanceled) {
                applyFilters(el);
                if (el.filter == null) {
                    $("#bulkEditGrid").data("kendoGrid").dataSource.filter();
                    $("#bulkEditGrid").data("kendoGrid").dataSource.read();
                } else {
                    $("#bulkEditGrid").data("kendoGrid").dataSource.filter({ logic: "and", filters: el.filter });
                }
            }
            else {
                isFilterCanceled = false;
            }


        }


    }


    var isValid = false;
    var paggerCalled = false;
    var pageNumber = 0;

    function pagerCall(e) {
        counter = 0;

        if (checkedIds.length > 0 || isValid || isAllSelected) {
            e.preventDefault();
            if (!isValid) {

                dialog.kendoDialog({

                    width: "450px",
                    title: "Ønsker du at forlade denne side?",
                    closable: true,
                    dragable: true,
                    modal: true,
                    close: function (e) {
                        $(this.element).empty();
                    },
                    content: `<p>Hvis du fortsætter, vil de produkter, du har sat flueben ved på denne side blive fjernet, når du forlader siden.<p>
                               <br/>
                                Klik OK for at fortsætte.<br/>Klik Annuller hvis du ønsker at blive på denne side`,
                    actions: [{
                        text: "Annuller",
                        action: function () {


                            dialog.data("kendoDialog").close();
                            $('.k-widget.k-window.k-dialog').hide();
                        },
                    },
                    {
                        text: "OK",
                        action: function () {
                            isValid = true;
                            paggerCalled = false;
                            pageNumber = e.page;
                            dialog.data("kendoDialog").close();
                            $('.k-widget.k-window.k-dialog').hide();
                            clearSelection();
                            disableTextBoxs();
                            checkedIds = [];


                            restoreOldPrices();


                            pagerCall(e);
                        },
                        primary: true,
                    }],

                });
                dialog.data("kendoDialog").open();

            } else if (!paggerCalled) {

                paggerCalled = true;
                isAllSelected = false;
                isValid = false;
                $("#bulkEditGrid").data("kendoGrid").dataSource.page(pageNumber);
                $("#bulkEditGrid").data("kendoGrid").dataSource.read();

            }
        }
    }

    var grid = $("#bulkEditGrid").data("kendoGrid");
    grid.bind("page", pagerCall);
    grid.bind("filter", filterData);

    //show tooptip on hover
    $("#bulkEditGrid").kendoTooltip({
        filter: "td:nth-child(3)", //this filter selects the 3rd column's cells i.e Title coloumn
        position: "right",
        content: function (e) {
            var dataItem = $("#bulkEditGrid").data("kendoGrid").dataItem(e.target.closest("tr"));
            var content = dataItem.Title;
            return content;
        }
    }).data("kendoTooltip");

    $("#bulkEditGrid").kendoTooltip({
        filter: "td:nth-child(4)", //this filter selects the 4th column's cells i.e Salgsform coloumn
        position: "right",
        content: function (e) {
            var dataItem = $("#bulkEditGrid").data("kendoGrid").dataItem(e.target.closest("tr"));
            //var content = dataItem.RefPriceModelDisplayName != null ? dataItem.RefAccessFormDisplayName + ' / ' + dataItem.RefPriceModelDisplayName : //dataItem.RefAccessFormDisplayName;

            var content = dataItem.RefSalesConfigTypeCode == 1002 ? 'GUA' : dataItem.RefAccessFormDisplayName + (dataItem.RefPriceModelDisplayName != null ? ' / ' + dataItem.RefPriceModelDisplayName : '')
            return content;
        }
    }).data("kendoTooltip");


    //Add class to isbn filter text box so that we can hookup events on.
    $('#isbnFilter').click(function () {
        if ($('form.k-filter-menu').first().find('input.k-textbox').hasClass('titleFilterInput')) {


            setTimeout(function () {
                $('form.k-filter-menu').last().find('input.k-textbox').addClass('isbnFilterInput');
                $('form.k-filter-menu').last().find('button[type="reset"]').addClass('btnClearIsbn');
            }, 500);


        } else {

            setTimeout(function () {
                $('form.k-filter-menu').first().find('input.k-textbox').addClass('isbnFilterInput');
                $('form.k-filter-menu').first().find('button[type="reset"]').addClass('btnClearIsbn');
            }, 500);


        }

    });


    $('#titleFilter').click(function () {

        if ($('form.k-filter-menu').first().find('input.k-textbox').hasClass('isbnFilterInput')) {


            setTimeout(function () {
                $('form.k-filter-menu').last().find('input.k-textbox').addClass('titleFilterInput');
                $('form.k-filter-menu').last().find('button[type="reset"]').addClass('btnClearTitle');
            }, 500);


        } else {

            setTimeout(function () {
                $('form.k-filter-menu').first().find('input.k-textbox').addClass('titleFilterInput');
                $('form.k-filter-menu').first().find('button[type="reset"]').addClass('btnClearTitle');
            }, 500);


        }

    });


}


function getSelectedRows(grid) {
    var rows = grid.select().toArray();
    var selectedRowIds = [];

    for (var i = 0; i < rows.length; i++) {
        var id = rows[i].getAttribute("id");
        selectedRowIds.push(id);
    }
    return selectedRowIds;
}

var progressBarPercentMain = 0;

var saveBulkConfiguration;


function getMasterColumnsWidth(tbl) {
    var result = 0;
    tbl.children("colgroup").find("col").not(":last").each(function (idx, element) {
        result += parseInt($(element).outerWidth() || 0, 10);
    });

    return result;
}
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

var isSortedApplied = false;

function adjustLastColumn() {


    var sort = $("#bulkEditGrid").data("kendoGrid").dataSource.sort();

    if (sort != null && sort.length > 0) {

        isSortedApplied = true;
        return false;
    }
    else if (isSortedApplied) {

        isSortedApplied = false;

        return false;
    }


    var grid = $("#bulkEditGrid").data("kendoGrid");
    var contentDiv = grid.wrapper.children(".k-grid-content");
    var masterHeaderTable = grid.thead.parent();
    var masterBodyTable = contentDiv.children("table");
    var gridDivWidth = contentDiv.width() - kendo.support.scrollbar();

    masterHeaderTable.width("");
    masterBodyTable.width("");

    var headerWidth = getMasterColumnsWidth(masterHeaderTable),
        thirdColElement = grid.thead.parent().find("col").eq(2),
        thirdDataColElement = grid.tbody.parent().children("colgroup").find("col").eq(2),

        forthColElement = grid.thead.parent().find("col").eq(2),
        forthDataColElement = grid.tbody.parent().children("colgroup").find("col").eq(2),
        delta = parseInt(gridDivWidth, 10) - parseInt(headerWidth, 10);

    if (delta > 0) {
        delta = Math.abs(delta);
        if (delta <= 200) {
            delta = 200;
        }
        thirdColElement.width(delta);
        thirdDataColElement.width(delta);
    } else {
        thirdColElement.width(0);
        thirdDataColElement.width(0);
    }

    contentDiv.scrollLeft(contentDiv.scrollLeft() - 1);
    contentDiv.scrollLeft(contentDiv.scrollLeft() + 1);
}

var isSortedAppliedonResieze = false;

function adjustLastColumnOnResize() {

    var grid = $("#bulkEditGrid").data("kendoGrid");
    var contentDiv = grid.wrapper.children(".k-grid-content");
    var masterHeaderTable = grid.thead.parent();
    var masterBodyTable = contentDiv.children("table");
    var gridDivWidth = contentDiv.width() - kendo.support.scrollbar();

    masterHeaderTable.width("");
    masterBodyTable.width("");
    var headerWidth = getMasterColumnsWidth(masterHeaderTable),
        lastHeaderColElement = grid.thead.parent().find("col").last(),
        lastDataColElement = grid.tbody.parent().children("colgroup").find("col").last(),

        delta = parseInt(gridDivWidth, 10) - parseInt(headerWidth, 10);

    if (delta > 0) {
        delta = Math.abs(delta);
        lastHeaderColElement.width(delta);
        lastDataColElement.width(delta);
    } else {
        lastHeaderColElement.width(0);
        lastDataColElement.width(0);
    }

    contentDiv.scrollLeft(contentDiv.scrollLeft() - 1);
    contentDiv.scrollLeft(contentDiv.scrollLeft() + 1);
}

function grayedOutInactiveTextBox(currentActiveField) {



    if (currentActiveField == "percentage") {
        $("#txtPercentage").removeClass('gray-background');
        $("#txtPrice").addClass('gray-background');
    }
    if (currentActiveField == "exactAmount") {
        $("#txtPrice").removeClass('gray-background');
        $("#txtPercentage").addClass('gray-background');
    }

}

function restoreOldPrices() {
    isAllSelected = false;
    $("#bulkEditGrid").data("kendoGrid").clearSelection();
    $('#bulkEditGrid tr.changed').each(function (e) {

        var row = $(this);
        var rowId = $(this).attr('id');
        if (row.hasClass('changed')) {
            row.removeClass('changed');

            var currentRowPriceUMOM = row.find('.pris-u-mom');
            var currentRowPriceMMOM = row.find('.pris-m-mom');
            if (currentRowPriceUMOM.hasClass('input-error')) {
                currentRowPriceUMOM.removeClass('input-error');
            }
            var oldPriceForCurrentRow = prisOld.find(x => x.Id === rowId);

            if (typeof oldPriceForCurrentRow != 'undefined') {
                currentRowPriceUMOM.val(oldPriceForCurrentRow.prisUMom);
                currentRowPriceMMOM.val(oldPriceForCurrentRow.prisMMom);
            }
        }
    });

    $('#btnGemAendringer').addClass('gem-btn-inactive').removeClass('gem-btn-active');
    $('#btnGemAendringer').prop('disabled', true);
}

function onGridError() {
    $('#loader').hide();
    $('#bulkEditGrid').css('min-height', '389px');
    var grid = $('#bulkEditGrid');
    if ($(".k-grid-norecords-template").length === 0) {
        grid
            .append('<div class="k-grid-norecords" style="position: fixed; top :53%; left : 37%; padding-left: 40px;width :27em;"<div class="k-grid-norecords-template"><b>Data kan ikke hentes lige nu. Prøv venligst igen senere</b></div></div>');
    }
}

function priceChangeOnIndiviualRow(row, changeValue, isPercent) {
    var id = row.getAttribute("data-item");
    var currentPriceValue = $("." + id).val();
    if (isPercent) {
        var perPrice = ((parseInt(changeValue) / 100) * Math.round(currentPriceValue)).toFixed(2);
        var newUnitPrice = parseFloat(Math.round(currentPriceValue)) + parseFloat(perPrice);

        var newPrice = Math.round(newUnitPrice);
        var UnitPriceVat = (newPrice * 1.25).toFixed(2);
        $("." + id).val(newPrice);
        $("#" + id).val(UnitPriceVat);
    }
    else {
        var newPrice = parseInt(currentPriceValue) + parseInt(changeValue);
        var UnitPriceVat = (newPrice * 1.25).toFixed(2);
        $("." + id).val(newPrice);
        $("#" + id).val(UnitPriceVat);
    }
    var oldObj = prisOld.find(x => x.Id === row.id);
    var oldValue = oldObj.prisUMom;
    if (oldValue != $("." + id).val() && !$(row).hasClass('changed')) {
        $(row).addClass('changed');
    }
}

function priceChangeOnAllRows(changeValue, isPercent) {
    $("#bulkEditGrid tbody tr").each(function (index, item) {
        if (!$("#" + item.id).hasClass("changed")) {
            priceChangeOnIndiviualRow(item, changeValue, isPercent)
        }
    });
}







