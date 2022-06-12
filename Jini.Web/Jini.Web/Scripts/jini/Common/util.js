'use strict';

/**************************************************************************

  * Utility functions. Not part of AngularJS controllers.

  *************************************************************************/


function removeObjectFromList(obj, list, styleArray) {

    $.each(list, function (listIndex) {
        var flag = true;
        for (var prop in obj) {
            if (obj.hasOwnProperty(prop)) {
                if (list[listIndex][prop] !== obj[prop])
                    flag = false;
            }
        }

        if (flag) {
            if (styleArray != null)
                styleArray[obj.Code] = "";
            list.splice(listIndex, 1);
            return false;
        }
        return true;
    });
}

function hasDuplicates(array) {
    var valuesSoFar = Object.create(null);
    for (var i = 0; i < array.length; ++i) {
        var value = array[i];
        if (value in valuesSoFar) {
            return true;
        }
        valuesSoFar[value] = true;
    }
    return false;
}

function replaceAll(str, find, replace) {
    return str.replace(new RegExp(find, "g"), replace);
}

function UTCToLocalTimeString(d, format) {
    
    var timeOffsetInHours = (new Date().getTimezoneOffset() / 60) * (-1);
    
    d.setHours(d.getHours() + timeOffsetInHours);

    return d.strftime(format);
}

var stateChanged = false;

function initUnloadEvent() {
    stateChanged = true;
    window.onbeforeunload = function () {
        return "If you want to Save Draft Please click 'Stay on this Page' and then Click Save Button";
    };
}

function initializeViewModel(model, periods) {
    if (model.Pricing == null)
        model.Pricing = [];
    for (var period in periods) {
        if (periods.hasOwnProperty(period)) {
            if (model.Pricing[periods[period]['UnitValue'] + " " + periods[period]['RefPeriodUnitTypeCode']] == null) {
                model.Pricing[periods[period]['UnitValue'] + " " + periods[period]['RefPeriodUnitTypeCode']] = {};
                model.Pricing[periods[period]['UnitValue'] + " " + periods[period]['RefPeriodUnitTypeCode']]
                    .ExTax = "0,00";
                model.Pricing[periods[period]['UnitValue'] + " " + periods[period]['RefPeriodUnitTypeCode']]
                    .IncTax = "0,00";
            }
        }
    }
}

function copyToClipboard(value) {
    // create temp element
    var copyElement = document.createElement("span");
    copyElement.appendChild(document.createTextNode(value));
    copyElement.id = 'tempCopyToClipboard';
    angular.element(document.body.append(copyElement));

    // select the text
    var range = document.createRange();
    range.selectNode(copyElement);
    window.getSelection().removeAllRanges();
    window.getSelection().addRange(range);

    // copy & cleanup
    document.execCommand('copy');
    window.getSelection().removeAllRanges();
    copyElement.remove();
}

function strip(html) {
    var tmp = document.createElement("DIV");
    tmp.innerHTML = html;
    return tmp.textContent || tmp.innerText || "";
    //return html.indexOf("<") === 0 ? html.substring(4, html.length - 4) : html;
}

var entityMap = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#39;',
    '/': '&#x2F;',
    '`': '&#x60;',
    '=': '&#x3D;'
};

function escapeHtml(string) {
    return String(string).replace(/[&<>"'`=\/]/g, function (s) {
        return entityMap[s];
    });
}

function getInnerHtmlIndex(string, position) {
    var stack = [];
    var count = 0, index = 0, nextIndex = 0, totalCount = 0;
    for (index = 0; index < string.length; index++) {
        if (totalCount >= position) {
            if (string[index - 1] === '&') {
                index = string.indexOf(';', index) + 1;
            }
            break;
        }
        if (string[index - 1] === '&') {
            index = string.indexOf(';', index);
            continue;
        }
        
        if (string[index] === '<') {
            if (string[index + 1] === '/') {
                nextIndex = string.indexOf('>', index);
                if (string.substring(index + 2, 2) !== "br" && string.substring(index + 2, index + 2) !== "p" && string.substring(index + 2, index + 3) !== "li")
                    stack.pop();
                index = nextIndex;
                
                continue;
            }
            else {
                if (string.substring(index + 1, 2) !== "br" && string.substring(index + 1, index + 2) !== "p" && string.substring(index + 1, index + 3) !== "li")
                    stack.push(string.substring(index, nextIndex));
                nextIndex = string.indexOf('>', index);

                index = nextIndex;
                continue;

            }
        }

        
        if (stack.length === 0) {
            count++;
        }
        totalCount++;

        
    }
    return index;
}

function cleanString(bodyHtml, startingIndex, endingIndex) {
    var startCleaned = false;
    for (var index = startingIndex; index < bodyHtml.length && index < endingIndex; index++)
    {
        if (bodyHtml[index] === '<') {
            if (startCleaned) {
                endingIndex = index;
                break;
            }
            index = bodyHtml.indexOf('>', index);
            startingIndex = index + 1;
            continue;
        }
        else {
            startCleaned = true;
            
        }


    }
    return [startingIndex, endingIndex];
}

function findParent(a, tagName) {
    var els = [];
    
    while (a) {
        els.unshift(a);
        if (a.tagName != null)
        if (a.tagName.toLowerCase() === tagName)
            return a;
        a = a.parentNode;
    }
    return null;
}

function findChild(node, textContent) {
    var els = [];
    
    while (node) {
        els.unshift(node);
        if (node.tagName != null)
            if (node.textContent === textContent)
                return true;
        node = node.parentNode;
    }
    return false;
}

function replaceAtIndex(string, replaceString, startingIndex, numberofCharacters) {
    
    string = string.split('');
    string.splice(startingIndex, numberofCharacters, replaceString);
    string = string.join('');
    return string;
}


function getTextNodes(elem, queue, startNode, endNode, endFlag) {
    Array.from(elem.childNodes)
        .forEach(child => {
            endFlag = getTextNodes(child, queue, startNode, endNode, endFlag);
        });

    if (elem.nodeType === 3 && ($(elem).is(startNode) || queue.length > 0) && endFlag === false) {
        queue.push(elem);
    }
    if ($(elem).is(endNode)) {
        endFlag = true;
    }
    return endFlag;
}
