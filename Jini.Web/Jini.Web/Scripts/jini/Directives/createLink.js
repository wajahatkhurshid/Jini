jiniApp.directive('createlink', ['$rootScope', 'dataFactory', function (
    $rootScope,
    dataFactory) {
    return {
        restrict: 'AE',
        replace: true,
        scope: {
            model: '=ngModel'
        },
        templateUrl: '/Wizard/CreateLink',
        link: function ($scope) {
            $scope.jiniData = dataFactory;

            $scope.kendoEditorChange = function () {
                initUnloadEvent();
            };

            $scope.resizeable = {
                content: true
            };

            $scope.paste = function (e) {
                if ((/^<img src="data:image/).test(e.html)) {
                    e.html = "";
                }
            };
            $scope.appendEmailHyperLink = function appendEmailHyperLink() {
                var editor = $("#contact-sales-desc").data("kendoEditor");
                var address = dataFactory.EmailHyperLink.address.indexOf("@") > -1 ? "mailto:" + dataFactory.EmailHyperLink.address.trim() + "?Subject=" + encodeURIComponent(dataFactory.EmailHyperLink.subject) : dataFactory.EmailHyperLink.address;
                var target = dataFactory.EmailHyperLink.openInNewTab ? "_blank" : "";

                var range = editor.getRange();
                var startOffset = range.startOffset, endOffset = range.endOffset, startingIndex, endingIndex, html, startNodeIndex = 0, endNodeIndex = 0, lastIndex = 0;

                for (startNodeIndex = 0; startNodeIndex < range.commonAncestorContainer.childNodes.length; startNodeIndex++) {
                    if (range.commonAncestorContainer.childNodes[startNodeIndex].textContent === range.startContainer.textContent || $(range.commonAncestorContainer.childNodes[startNodeIndex]).find(range.startContainer)[0] != null)
                        break;
                }

                for (endNodeIndex = startNodeIndex; endNodeIndex < range.commonAncestorContainer.childNodes.length; endNodeIndex++) {
                    if (range.commonAncestorContainer.childNodes[endNodeIndex].textContent === range.endContainer.textContent || $(range.commonAncestorContainer.childNodes[endNodeIndex]).find(range.endContainer)[0] != null)
                        break;
                }

                var bodyHtml = editor.body.innerHTML;

                var node = range.commonAncestorContainer;
                var selectedNodeIndex = 0;
                var selectedNodeStartingIndex = 0;
                while ((node = node.previousSibling) != null) {
                    selectedNodeIndex++;
                    if (node.textContent) {
                        selectedNodeStartingIndex += node.textContent.length;
                    }
                }

                var textNodes = [];
                getTextNodes(range.commonAncestorContainer, textNodes, range.startContainer, range.endContainer, false);

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

                    html = "<a href = '" + address + "' target='" + target + "' title='" +
                        dataFactory.EmailHyperLink.tooltip + "'>" +
                        currentNode.textContent.substring(range.startOffset, range.endOffset) + "</a>";

                    startingIndex = innerIndex === 0 ? startOffset : 0;
                    endingIndex = innerIndex === textNodes.length - 1 ? endOffset : currentNode.textContent.length;

                    var previousLength = innerIndex === 0 ? selectedNodeStartingIndex : editor.body.textContent.indexOf(currentNode.textContent, lastIndex);
                    startingIndex = getInnerHtmlIndex(bodyHtml, previousLength + startingIndex, false);
                    endingIndex = getInnerHtmlIndex(bodyHtml, previousLength + endingIndex, false);

                    var cleanedIndexes = cleanString(bodyHtml, startingIndex, endingIndex);
                    startingIndex = cleanedIndexes[0];
                    endingIndex = cleanedIndexes[1];
                    bodyHtml = replaceAtIndex(bodyHtml, html, startingIndex, endingIndex - startingIndex);
                    lastIndex = previousLength + 1;
                }

                if (range.commonAncestorContainer.childNodes.length === 0) {
                    startingIndex = getInnerHtmlIndex(range.commonAncestorContainer.parentElement.innerHTML, selectedNodeStartingIndex + range.startOffset);
                    endingIndex = getInnerHtmlIndex(range.commonAncestorContainer.parentElement.innerHTML, selectedNodeStartingIndex + range.endOffset);
                    html = "<a href = '" + address + "' target='" + target + "' title='" + dataFactory.EmailHyperLink.tooltip + "'>" + range.commonAncestorContainer.parentElement.innerHTML.substring(startingIndex, endingIndex) + "</a>";
                    if (range.commonAncestorContainer.parentElement.tagName != undefined &&
                        range.commonAncestorContainer.parentElement.tagName.toLowerCase() === 'a') {
                        range.commonAncestorContainer.parentElement.title = dataFactory.EmailHyperLink.tooltip;
                        range.commonAncestorContainer.parentElement.text = range.commonAncestorContainer.parentElement.innerHTML.substring(startingIndex, endingIndex);
                        range.commonAncestorContainer.parentElement.href = address;
                        range.commonAncestorContainer.parentElement.target = target;
                    } else {
                        range.commonAncestorContainer.parentElement.innerHTML = replaceAtIndex(range.commonAncestorContainer.parentElement.innerHTML, html, startingIndex, endingIndex - startingIndex);
                    }

                    bodyHtml = editor.body.innerHTML;
                }
                editor.body.innerHTML = bodyHtml;
                editor.update();
                editor.refresh();
                $scope.model = editor.value();

                var window = $("#trial-email-hyperlink").data("kendoWindow");
                if (window != null)
                    window.close();
            };

            $scope.select = function () {
                var selection = $("#contact-sales-desc").data("kendoEditor").getSelection();

                if (selection.focusNode != null) {
                    $rootScope.showUnlink = findParent(selection.focusNode.parentNode, 'a') !== null ? true : false;
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
                }, "cleanFormatting", "bold", "italic", "underline",
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
                            window = $("#trial-email-hyperlink").data("kendoWindow");
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
                            startingIndex = selection.focusNode.nodeValue.substring(0, selection.extentOffset).lastIndexOf(" ");
                            endingIndex = selection.focusNode.nodeValue.indexOf(" ", selection.baseOffset);
                            startingIndex = startingIndex === -1 ? 0 : startingIndex + 1;
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
                        $scope.$apply();

                        window = $("#trial-email-hyperlink").data("kendoWindow");

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
                        $scope.model = editor.value();
                        //$scope.jiniData.Trial.ContactSales = editor.value();
                    }
                }
            ];

            $scope.closeEmailPopup = function () {
                var window = $("#trial-email-hyperlink").data('kendoWindow');
                if (window != null)
                    window.close();
            };

            var x = $("#trial-email-hyperlink").data('kendoWindow');

            if ($("#trial-email-hyperlink").data('kendoWindow') === undefined || $("#trial-email-hyperlink").data('kendoWindow') === null)
                createPopup("trial-email-hyperlink", "", true, "Indsæt hyperlink", 400);
        }
    }
}]);