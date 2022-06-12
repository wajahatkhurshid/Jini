'use strict';
/**************************************************************************

  * Controller for the Jini Sales Configuration Wizard. Walks the user through the first step to the eventual approval or cancellation of sales configuration

  *************************************************************************/

jiniApp.controller('config',
    function ($rootScope,
        $scope,
        $timeout,
        jiniservice,
        priceService,
        dataFactory) {
        $scope.jiniData = dataFactory;
        //Startup Code. Sort the user selections
        $scope.jiniData.SalesForm = $scope.jiniData.SalesForm.sort(function (a, b) { return a.Code - b.Code });
        $scope.jiniData.AccessForm = $scope.jiniData.AccessForm.sort(function (a, b) { return a.Code - b.Code });
        $scope.jiniData.Periods = $scope.jiniData.Periods.sort(function (a, b) { return a.UnitValue - b.UnitValue });
        $scope.jiniData.Periods = $scope.jiniData.Periods.sort(function (a, b) { return b.RefPeriodUnitTypeCode - a.RefPeriodUnitTypeCode });

        //Startup Code. Initialize Objects
        if ($.grep($scope.jiniData.AccessForm, function (e) { return e.Code == _school; }).length !== 0) {
            initializeViewModel($scope.jiniData.SchoolLicense, $scope.jiniData.Periods);
        }
        if ($.grep($scope.jiniData.AccessForm, function (e) { return e.Code == _class; }).length !== 0) {
            $timeout(function () {
                var dropdownlist = $("#ClassLicense-billing-model-select").data("kendoDropDownList");
                dropdownlist.select(0);
                dropdownlist.enable(false);
                $scope.jiniData.ClassLicense.BillingModel = $scope.jiniData.LookupBillingModels[_class][0];
            });

            initializeViewModel($scope.jiniData.ClassLicense, $scope.jiniData.Periods);
        }
        if ($.grep($scope.jiniData.AccessForm, function (e) { return e.Code == _teacher; }).length !== 0) {
            initializeViewModel($scope.jiniData.TeacherLicense, $scope.jiniData.Periods);
        }
        if ($.grep($scope.jiniData.AccessForm, function (e) { return e.Code == _singleUser; }).length !== 0) {
            initializeViewModel($scope.jiniData.SingleUserLicense, $scope.jiniData.Periods);
        }
        wizardPageTwo();

        if ($scope.jiniData.LookupBillingModels[_teacher] != null) {
            $timeout(function () {
                var dropdownlist = $("#teacherLicense-billing-model-select").data("kendoDropDownList");
                dropdownlist.select(0);
                dropdownlist.enable(false);
                $scope.jiniData.TeacherLicense.BillingModel = $scope.jiniData.LookupBillingModels[_teacher][0];
            });
        }
        $timeout(function () {
            $("#divLoading").removeClass('show');
            $rootScope.bindTextBoxToAllSelect();
        });
        //END Startup Code

        $scope.onSpinner = function (period, caller) {
            initUnloadEvent();
            if (caller == _school)
                $scope.jiniData.SchoolLicense.Pricing[period].ExTax = String(this.numeric.value());

            if (caller == _class)
                $scope.jiniData.ClassLicense.Pricing[period].ExTax = String(this.numeric.value());

            if (caller == _teacher)
                $scope.jiniData.TeacherLicense.Pricing[period].ExTax = String(this.numeric.value());

            if (caller == _singleUser)
                $scope.jiniData.SingleUserLicense.Pricing[period].ExTax = String(this.numeric.value());

            priceService.priceChanged(period, caller);
        };

        $scope.onClassBillingFoundationChange = function () {
            initUnloadEvent();
            jiniservice.gradeLevelsExists($scope.jiniData.DigitalTitle.Isbn, $scope.jiniData.DigitalTitle.Version)
                .then(function (data) {
                    // ReSharper disable once QualifiedExpressionIsNull
                    if (data != undefined || data != null || data.length !== 0) {
                        var distinctGradeLevels = [];

                        $.each(data, function (index, event) {
                            var events = $.grep(distinctGradeLevels, function (e) {
                                return event.SortLevel == e.SortLevel;
                            });
                            if (events.length == 0) {
                                distinctGradeLevels.push(event);
                            }
                        });

                        var gradeLevelString = $.map(distinctGradeLevels, function (obj) {
                            return obj.SortLevel;
                        }).join(',');
                        $scope.jiniData.ClassLicense.BillingModel.GradeLevels = gradeLevelString;
                    }
                    else {
                        $scope.jiniData.ClassLicense.BillingModel.GradeLevels = null;
                    }
                });
        };

        $scope.onSchoolBillingFoundationChange = function (billingFoundationCode) {
            initUnloadEvent();
            if (billingFoundationCode == _noOfStudentsInReleventClasses || billingFoundationCode == _noOfReleventClasses) {
                jiniservice.gradeLevelsExists($scope.jiniData.DigitalTitle.Isbn, $scope.jiniData.DigitalTitle.Version)
                    .then(function (data) {
                        if (data == undefined || data == null || data == "") {
                            $scope.jiniData.SchoolLicense.BillingModel = undefined;
                            $("#schoolBillingFoundations").data("kendoDropDownList").select(0);
                            $("#no-grade-warning-popup").data("kendoWindow").center().open();
                        } else {
                            //var distinctGradeLevels = [];

                            //$.each(data, function (index, event) {
                            //    var events = $.grep(distinctGradeLevels, function (e) {
                            //        return event.SortLevel == e.SortLevel;
                            //    });
                            //    if (events.length == 0) {
                            //        distinctGradeLevels.push(event);
                            //    }
                            //});

                            //var gradeLevelString = $.map(distinctGradeLevels, function (obj) {
                            //    return obj.SortLevel;
                            //}).join(',');
                            $scope.jiniData.SchoolLicense.BillingModel.GradeLevels = data;
                        }
                    });
            } else {
                $scope.jiniData.SchoolLicense.BillingModel.GradeLevels = null;
            }
        }
        var priceChangeWatch = function () {
            var timer = 0;
            return function (callback, ms) {
                clearTimeout(timer);
                timer = setTimeout(callback, ms);
            }
        }();

        $scope.onPriceChange = function (period, caller) {
            initUnloadEvent();
            priceChangeWatch(function () {
                switch (caller) {
                    case _school: priceService.priceChanged(period, caller, $scope.jiniData.SchoolLicense);
                        break;
                    case _class: priceService.priceChanged(period, caller, $scope.jiniData.ClassLicense);
                        break;
                    case _teacher: priceService.priceChanged(period, caller, $scope.jiniData.TeacherLicense);
                        break;
                    case _singleUser: priceService.priceChanged(period, caller, $scope.jiniData.SingleUserLicense);
                        break;
                }
            }, 0);
        }

        $scope.onPriceChangeIncTax = function (period, caller) {
            initUnloadEvent();
            priceChangeWatch(function () {
                switch (caller) {
                    case _school: priceService.priceChangedIncTax(period, caller, $scope.jiniData.SchoolLicense);
                        break;
                    case _class: priceService.priceChangedIncTax(period, caller, $scope.jiniData.ClassLicense);
                        break;
                    case _teacher: priceService.priceChangedIncTax(period, caller, $scope.jiniData.TeacherLicense);
                        break;
                    case _singleUser: priceService.priceChangedIncTax(period, caller, $scope.jiniData.SingleUserLicense);
                        break;
                }
            }, 0);
        }

        $scope.appendEmailHyperLink = function appendEmailHyperLink() {
            var editor = $("#contact-sales-desc").data("kendoEditor");

            var address = dataFactory.EmailHyperLink.address.indexOf("@") > -1 ? "mailto:" + dataFactory.EmailHyperLink.address.trim() + "?Subject=" + encodeURIComponent(dataFactory.EmailHyperLink.subject) : dataFactory.EmailHyperLink.address;
            var target = dataFactory.EmailHyperLink.openInNewTab ? "_blank" : "";

            var range = editor.getRange();
            var startOffset = range.startOffset, endOffset = range.endOffset, startingIndex, endingIndex, html, startNodeIndex = 0, endNodeIndex = 0, lastIndex = 0;
            var textNodes = [];
            getTextNodes(range.commonAncestorContainer, textNodes, range.startContainer, range.endContainer, false);
            var bodyHtml = editor.body.innerHTML;
            var done = false;
            for (startNodeIndex = 0; startNodeIndex < range.commonAncestorContainer.childNodes.length; startNodeIndex++) {
                if (range.commonAncestorContainer.childNodes[startNodeIndex].textContent === range.startContainer.textContent || $(range.commonAncestorContainer.childNodes[startNodeIndex]).find(range.startContainer)[0] != null)
                    break;
            }

            for (endNodeIndex = startNodeIndex; endNodeIndex < range.commonAncestorContainer.childNodes.length; endNodeIndex++) {
                if (range.commonAncestorContainer.childNodes[endNodeIndex].textContent === range.endContainer.textContent || $(range.commonAncestorContainer.childNodes[endNodeIndex]).find(range.endContainer)[0] != null)
                    break;
            }

            for (var innerIndex = 0; innerIndex < textNodes.length; innerIndex++) {
                var currentNode = textNodes[innerIndex];

                startingIndex = innerIndex === 0 ? startOffset : 0;
                endingIndex = innerIndex === textNodes.length - 1 ? endOffset : currentNode.textContent.length;

                if (currentNode.parentElement.tagName != undefined &&
                    currentNode.parentElement.tagName.toLowerCase() === 'a') {
                    currentNode.parentElement.title = dataFactory.EmailHyperLink.tooltip;
                    currentNode.parentElement.text = currentNode.parentElement.innerHTML.substring(startingIndex, endingIndex);
                    currentNode.parentElement.href = address;
                    currentNode.parentElement.target = target;
                    continue;
                }

                var previousLength = currentNode.parentElement.textContent.indexOf(currentNode.textContent);
                startingIndex = getInnerHtmlIndex(currentNode.parentElement.innerHTML, previousLength + startingIndex, false);
                endingIndex = getInnerHtmlIndex(currentNode.parentElement.innerHTML, previousLength + endingIndex, false);

                if (endingIndex <= startingIndex)
                    continue;
                var cleanedIndexes = cleanString(currentNode.parentElement.innerHTML, startingIndex, endingIndex);
                startingIndex = cleanedIndexes[0];
                endingIndex = cleanedIndexes[1];

                html = "<a href = '" + address + "' target='" + target + "' title='" + dataFactory.EmailHyperLink.tooltip + "'>" + currentNode.parentElement.innerHTML.substring(startingIndex, endingIndex) + "</a>";

                startingIndex = innerIndex === 0 ? startOffset : 0;
                endingIndex = innerIndex === textNodes.length - 1 ? endOffset : currentNode.textContent.length;
                var previousLength = editor.body.textContent.indexOf(currentNode.textContent, lastIndex);
                startingIndex = getInnerHtmlIndex(bodyHtml, previousLength + startingIndex, false);
                endingIndex = getInnerHtmlIndex(bodyHtml, previousLength + endingIndex, false);

                var y = bodyHtml.substring(startingIndex, endingIndex)
                var cleanedIndexes = cleanString(bodyHtml, startingIndex, endingIndex);
                startingIndex = cleanedIndexes[0];
                endingIndex = cleanedIndexes[1];

                var x = bodyHtml.substring(startingIndex, endingIndex)
                bodyHtml = replaceAtIndex(bodyHtml, html, startingIndex, endingIndex - startingIndex);

                lastIndex = previousLength + 1;
            }

            editor.body.innerHTML = bodyHtml;
            editor.update();
            editor.refresh();
            $scope.jiniData.ContactSales.DescriptionText = editor.value();

            var window = $("#email-hyperlink").data("kendoWindow");
            if (window != null)
                window.close();
        };

        $scope.select = function () {
            var selection = $("#contact-sales-desc").data("kendoEditor").getSelection();

            if (selection.focusNode != null) {
                $rootScope.showUnlink = findParent(selection.focusNode.parentNode, 'a') !== null ? true : false;//selection.focusNode.parentNode.tagName.toLowerCase() === 'a';
                dataFactory.applyRootScope();
            }
        }

        $scope.tools = [
            {
                name: "formatting",
                items: [].concat(
                    { text: "Brødtekst", value: "p" },
                    { text: "Citat", value: "q" },
                    { text: "Overskrift 1", value: "h1" },
                    { text: "Overskrift 2", value: "h2" },
                    { text: "Overskrift 3", value: "h3" },
                    { text: "Overskrift 4", value: "h4" },
                    { text: "Overskrift 5", value: "h5" },
                    { text: "Overskrift 6", value: "h6" }
                )
            }
            , "cleanFormatting", "bold", "italic", "underline",
            "justifyLeft", "justifyCenter", "justifyRight",
            "insertUnorderedList", "insertOrderedList", "indent", "outdent",

            {
                name: 'CreateLink',
                template: '<a  role="button" class="k-tool k-state-hover" unselectable="on" title="Opret link"><span unselectable="on" class="k-icon k-i-link-horizontal" style="background-position: -288px -48px; width: 24px; height: 24px; opacity: 1"></span><span class="k-tool-text">CreateLink</span></a>',
                exec: function (e) {
                    var editor = $("#contact-sales-desc").data("kendoEditor");
                    var selection = editor.getSelection();
                    var range = editor.createRange();
                    var startingIndex = 0, endingIndex = 0;
                    var window;

                    if (selection.focusNode == null || selection.focusNode.nodeValue == null) {
                        window = $("#email-hyperlink").data("kendoWindow");
                        dataFactory.EmailHyperLink.text = "";
                        dataFactory.EmailHyperLink.address = "http://";
                        dataFactory.EmailHyperLink.subject = "";
                        dataFactory.EmailHyperLink.tooltip = "";
                        dataFactory.EmailHyperLink.openInNewTab = false;
                        if (window != null)
                            window.center().open();
                        return;
                    }

                    if (selection.focusNode.parentNode.tagName.toLowerCase() === 'a') {
                        startingIndex = 0;
                        endingIndex = selection.focusNode.nodeValue.length;
                        dataFactory.EmailHyperLink.address = selection.focusNode.parentNode.href.indexOf("mailto:") > -1 ? selection.focusNode.parentNode.href.substring(7, selection.focusNode.parentNode.href.indexOf("?Subject=")) : selection.focusNode.parentNode.href;
                        dataFactory.EmailHyperLink.subject = selection.focusNode.parentNode.href.indexOf("mailto:") > -1 ? decodeURIComponent(selection.focusNode.parentNode.href.substring(selection.focusNode.parentNode.href.indexOf("?Subject=") + 9, selection.focusNode.parentNode.href.length)) : "";
                        dataFactory.EmailHyperLink.text = selection.focusNode.nodeValue;
                        dataFactory.EmailHyperLink.tooltip = selection.focusNode.parentNode.title;
                        dataFactory.EmailHyperLink.openInNewTab = selection.focusNode.parentNode.target === "_blank" ? true : false;

                        range.setStart(selection.focusNode, startingIndex);
                        range.setEnd(selection.focusNode, endingIndex);
                        editor.selectRange(range);
                    }
                    else if (selection.type === "Caret") {
                        startingIndex = selection.anchorOffset < selection.focusOffset ? selection.anchorOffset : selection.focusOffset;
                        endingIndex = selection.focusOffset > selection.anchorOffset ? selection.focusOffset : selection.anchorOffset;
                        startingIndex = selection.focusNode.nodeValue.substring(0, endingIndex).lastIndexOf(" ") + 1;
                        endingIndex = selection.focusNode.nodeValue.indexOf(" ", startingIndex);
                        startingIndex = startingIndex === -1 ? 0 : startingIndex;
                        endingIndex = endingIndex === -1 ? selection.focusNode.nodeValue.length : endingIndex;
                        dataFactory.EmailHyperLink.text = selection.focusNode.nodeValue.substring(startingIndex, endingIndex);

                        dataFactory.EmailHyperLink.address = "http://";
                        dataFactory.EmailHyperLink.subject = "";
                        dataFactory.EmailHyperLink.tooltip = "";
                        dataFactory.EmailHyperLink.openInNewTab = false;

                        range.setStart(selection.focusNode, startingIndex);
                        range.setEnd(selection.focusNode, endingIndex);
                        editor.selectRange(range);
                    }
                    else {
                        if (selection.baseOffset === selection.extentOffset) {
                            startingIndex = selection.anchorOffset < selection.focusOffset ? selection.anchorOffset : selection.focusOffset;
                            endingIndex = selection.focusOffset > selection.anchorOffset ? selection.focusOffset : selection.anchorOffset;
                        }
                        else {
                            startingIndex = selection.baseOffset < selection.extentOffset ? selection.baseOffset : selection.extentOffset;
                            endingIndex = selection.extentOffset > selection.baseOffset ? selection.extentOffset : selection.baseOffset;
                        }

                        dataFactory.EmailHyperLink.text = strip(editor.selectedHtml());

                        dataFactory.EmailHyperLink.address = "http://";
                        dataFactory.EmailHyperLink.subject = "";
                        dataFactory.EmailHyperLink.tooltip = "";
                        dataFactory.EmailHyperLink.openInNewTab = false;
                    }

                    dataFactory.applyRootScope();

                    window = $("#email-hyperlink").data("kendoWindow");

                    if (window != null)
                        window.center().open();
                }
            },
            {
                name: 'UnLink',
                template: '<a  role="button" ng-show="showUnlink" class="k-tool k-state-hover" unselectable="on" title="Fjern link"><span unselectable="on" class="k-icon k-i-unlink-horizontal" style="background-position: -288px -72px; width: 24px; height: 24px; opacity: 1"></span><span class="k-tool-text">Unlink</span></a>',
                exec: function (e) {
                    var editor = $("#contact-sales-desc").data("kendoEditor");
                    var selection = editor.getSelection();
                    var range = editor.createRange();

                    range.setStart(selection.focusNode, 0);
                    range.setEnd(selection.focusNode, selection.focusNode.nodeValue.length);
                    editor.selectRange(range);
                    var hyperLink = findParent(selection.focusNode, 'a');
                    hyperLink.outerHTML = hyperLink.innerHTML;
                    editor.update();
                    editor.refresh();
                    $scope.jiniData.ContactSales.DescriptionText = editor.value();
                }
            }
        ];

        $scope.closeEmailPopup = function () {
            var window = $("#email-hyperlink").data('kendoWindow');
            if (window != null)
                window.close();
        };

        $scope.resizeable = {
            content: true
        };
        $rootScope.showUnlink = false;
        $scope.paste = function (e) {
            if ((/^<img src="data:image/).test(e.html)) {
                e.html = "";
            }
        };
    });