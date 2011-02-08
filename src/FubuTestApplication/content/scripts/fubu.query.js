/*
var queryControl = null;
var grid = null;
var columns = null;
*/

$.fn.textboxEditor = function () {
    this.each(function (i, box) {
        
    });
}

$.fn.asQueryBuilder = function (userOptions) {
    var options = {};
    $.extend(options, $.fn.asQueryBuilder.defaults, userOptions || {});

    return this.each(function (i, div) {
        div.tbody = $('tbody', div).get(0);



        var queryRunner = function () {
            // What to do?
            // Execute the query against the grid
        }

        if (this.context == document) {
            // We cannot wire up the Enter key if a context was used in the selector
            $(this.selector + " input[type=text]").live("keypress", function (keyEvent) {
                if (keyEvent.which == 13) {
                    queryRunner();
                }
            });
        }

        var templates = $('.templates', div).get(0);

        div.addFilterRow = function () {
            $('<tr class="filter"><td></td><td></td><td></td><td></td></tr>').appendTo(div.tbody).asFilterRow(templates, options);
        }

        $(options.addCriteriaSelector).click(div.addFilterRow);

        /*
        FilterTable(table, options.filterChoices, options.removeCriteriaTemplateSelector);
        var queryRunner = function () {
        var filters = table.createFilters();
        options.runQuery(filters);
        return false;
        };

        table.runQuery = queryRunner;

        $(options.addCriteriaSelector).click(function () { table.addFilterRow(); return false; });
        $(options.runQuerySelector).click(queryRunner);



        $(options.clearCriteriaSelector).click(function () {
        table.clear();
        table.addFilterRow();

        options.onFormClear();
        return false;
        });
        */

        // THIS MAY NEED TO CHANGE, MAYBE A SERVER SIDE MODEL?
        //        if (options.initialFilters) {
        //            table.loadQuery(options.initialFilters);
        //        }

        div.addFilterRow();
        return false;
    });




    /*
    if (window.parent.createAutoQuery) {
    var criterion = window.parent.createAutoQuery(options.autoQueryPrefix);
    table.loadQuery(criterion);
    if (criterion.length > 0) {
    queryRunner();
    }
    }
    */
}

$(document).ready(function () {
    $('.smart-grid-filter').asQueryBuilder({});
});

$.fn.asQueryBuilder.editors = {
    textbox: function (textbox) {
        textbox.setValue = function (value) { $(textbox).val(value); }
        textbox.getValue = function (value) { return $(textbox).val(); }
    }
}

$.fn.asFilterRow = function (templates, options) {
    var row = this.get(0);
    row.operatorCell = row.childNodes[1];
    row.editorCell = row.childNodes[2];

    $(options.removeCriteriaTemplateSelector).clone().appendTo(row.childNodes[3]).click(function () {
        $(row).remove();
        return true;
    });

    row.findEditorTemplate = function () {
        for (var i = 0; i < arguments.length; i++) {
            var finder = $(arguments[i], templates);
            if (finder.length > 0) return finder.clone(true);
        }

        return $('<input type="text"></input>');
    }

    row.changeProperty = function () {
        var propName = $(row.propertySelector).val();
        var selector = 'div.smart-grid-operators > select.' + propName;

        $(row.operatorCell).empty();
        row.operatorSelector = $(selector, templates).clone(true).appendTo(row.operatorCell).change(function () {
            row.changeOperator();
        });

        return true;
    }

    row.property = function () { return $(row.propertySelector).val(); }
    row.operator = function () { return $(row.operatorSelector).val(); }
    row.value = function () { return row.editor.getValue(); }

    row.propertySelector = $('select.smart-grid-properties', templates)
        .clone(true)
        .appendTo(row.childNodes[0])
        .change(row.changeProperty).get(0);

    row.changeOperator = function () {
        $(row.editorCell).empty();

        // go find the editor
        var prop = row.property();
        var oper = row.operator();

        var template = row.findEditorTemplate('.' + prop + '-' + oper, '.' + oper).appendTo(row.editorCell);
        var metadata = template.metadata();
        var editorName = "textbox";
        if (metadata != null && metadata.formatter != null) {
            editorName = metadata.editor;
        }

        row.editor = template.get(0);
        $.fn.asQueryBuilder.editors[editorName](row.editor);
    }

    row.getCriteria = function () {
        return {
        // put properties here
    }
};

row.changeProperty();
row.changeOperator();
}



$.fn.asQueryBuilder.defaults =
{
    filterChoices: [],
    autoQueryPrefix: "",
    runQuery: function () { },
    addCriteriaSelector: "#add",
    removeCriteriaTemplateSelector: "#removeFilter",
    runQuerySelector: "#query",
    clearCriteriaSelector: "#clear",
    onFormClear: function () { }
};



/*
function getInitialFilters() {
    var filters = dovetail.windowManager.retrieve('filters');
    return filters == null ? initialCriteria.filters : filters;
}

function buildQueryOptions(onFormClear) {
    return {
        filterChoices: modelFilters,
        removeCriteriaTemplateSelector: "#removeFilter",
        autoQueryPrefix: prefix,
        runQuerySelector: "#search-criteria-search",
        clearCriteriaSelector: "#search-criteria-cancel",
        initialFilters: getInitialFilters(),
        runQuery: runQuery,
        onFormClear: onFormClear
    }
}
*/

FilterTable = function (table, filters, removeTemplate) {
    LayoutTable(table);
    table.properties = filters;
    table.removeTemplate = removeTemplate;
    table.lastRow = null;

    table.addFilterRow = function () {
        var row = new FilterRow(this.removeTemplate);
        row.loadProperties(this.properties);
        this.tBody.appendChild(row);

        this.lastRow = row;

        return row;
    }

    table.removeFilterRow = function (row) {
        this.tBody.removeChild(row);
    }
/*
    table.loadQuery = function (criterion) {
        if (criterion.length == 0) return;

        this.clear();
        for (var i = 0; i < criterion.length; i++) {
            var row = this.addFilterRow();
            row.loadCriteria(criterion[i]);
        }
    }

    table.createFilters = function () {
        var sendFilters = new Array();

        $(this.tBody.childNodes).each(function (i, row) {
            sendFilters.push(row.createCriteria());
        });

        return sendFilters;
    }
*/

    table.addFilterRow();
}


function FilterDropdown(width) {
    var select = $('<select></select>').dropdown()[0];
    width = width ? width : 0;
    select.style.width = width + 'px';

    return select;
}

function Property(display, value, operators) {
    this.display = display;
    this.value = value;
    this.operators = operators;

    return this;
}

function Operator(display, value, inputStyle) {
    this.display = display;
    this.value = value;
    this.inputStyle = inputStyle;

    return this;
}



function nothing() { }

function FilterRow(removeTemplate) {
    var row = new LayoutRow();
    $(row).addClass('filter');

    row.propertySelect = row.addChild(new FilterDropdown(150));
    row.operatorSelect = row.addChild(new FilterDropdown(150));
    row.valueTextbox = $('<input type="text"></input>').appendTo(row.addCell()).get(0);

    var removeLink = $(removeTemplate).clone()
    removeLink.appendTo(row.addCell());

    row.removeLink = removeLink.get(0);

    row.select = function (field, operator, value) {
        this.propertySelect.value = field;
        this.propertySelect.onchange();
        this.operatorSelect.value = operator;
        this.operatorSelect.onchange();
        this.valueTextbox.value = value;
    }

    row.removeLink.onclick = function () {
        this.parentNode.parentNode.remove();
        return false;
    }

    row.propertySelect.onSelect = function (property) {
        if (property) {
            var row = this.parentNode.parentNode;
            row.operatorSelect.setOptions(property.operators);
        }
    }

    row.operatorSelect.onSelect = function (operator) {
        var row = this.parentNode.parentNode;
        var newInput = $('<input type="text"></input>');
        $(row.valueTextbox).datepicker("destroy").replaceWith(newInput);

        //This is smelly, if this needs to change we'll probably want to revisit this design later and make it more robust.
        if (operator.inputStyle == 'DateTime') {
            newInput.datepicker({
                showOn: "button",
                buttonImage: global.calendarIcon,
                constrainInput: true
            });
        }
        row.valueTextbox = newInput.get(0);
    }

    row.remove = function () {
        this.parentNode.parentNode.removeFilterRow(this);
    }

    row.createCriteria = function () {
        return { property: this.propertySelect.value, op: this.operatorSelect.value, value: this.valueTextbox.value };
    }

    row.loadCriteria = function (criteria) {
        $(this.propertySelect).val(criteria.property);
        this.propertySelect.onchange();

        $(this.operatorSelect).val(criteria.op);
        $(this.valueTextbox).val(criteria.value);
    }

    row.loadProperties = function (properties) {
        row.propertySelect.setOptions(properties);
        row.propertySelect.selectedIndex = -1;
    }

    return row;
}

function LayoutTable(layoutTable) {
    if (!layoutTable) {
        layoutTable = document.createElement("table");
    }

    $('<thead></thead>').appendTo(layoutTable)[0];

    layoutTable.tBody = $('<tbody></tbody>').appendTo(layoutTable)[0];

    // Properties
    layoutTable.lastRow = null;
    layoutTable.headerRow = $('<tr></tr>').appendTo(layoutTable.tHead)[0];

    // Methods
    layoutTable.addHeading = function (text) {
        return $('<th></th>').appendTo(this.headerRow).attr('innerHTML', text)[0];
    }

    layoutTable.addRow = function () {
        var row = new LayoutRow();
        this.tBody.appendChild(row);
        this.lastRow = row;

        return row;
    }

    layoutTable.addCell = function (child) {
        var cell = document.createElement("td");
        this.lastRow.appendChild(cell);

        if (child) {
            if (typeof child == 'string') {
                cell.innerHTML = child;
            }
            else {
                cell.appendChild(child);
            }
        }

        return cell;
    }

    layoutTable.addElement = function (tag) {
        var cell = this.addCell();
        var element = document.createElement(tag);
        cell.appendChild(element);

        return element;
    }

    layoutTable.addHeaderCell = function (text) {
        var headerCell = document.createElement("th");
        headerCell.innerHTML = text;
        this.lastRow.appendChild(headerCell);

        return headerCell;
    }

    layoutTable.clear = function () {
        $(this.headerRow).empty();
        $(this.tBody).empty();
    }

    return layoutTable;
}

function LayoutRow() {
    var row = document.createElement("tr");

    row.addCell = function (child) {
        var cell = document.createElement("td");
        this.appendChild(cell);

        if (child) {
            if (typeof child == 'string') {
                cell.innerHTML = child;
            }
            else {
                cell.appendChild(child);
            }
        }

        return cell;
    }

    row.addChild = function (child) {
        this.addCell(child);
        return child;
    }

    return row;
}

