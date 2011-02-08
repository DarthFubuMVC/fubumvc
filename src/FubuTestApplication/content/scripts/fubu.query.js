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
        div.grid = $('#' + $(div).metadata().filters.gridId).get(0);

        var queryRunner = function () {
            var criterion = div.getCriteria();

            div.grid.runQuery(criterion);

            return false;
        }

        div.getCriteria = function () {
            return $('tr', div.tbody).map(function () { return this.getCriteria(); });
        }

        $(options.runQuerySelector).click(queryRunner);

        if (this.context == document) {
            // We cannot wire up the Enter key if a context was used in the selector
            $("#" + div.id + "input[type=text]").live("keypress", function (keyEvent) {
                if (keyEvent.which == 13) {
                    queryRunner();
                }
            });
        }

        var templates = $('.templates', div).get(0);

        div.addFilterRow = function () {
            $('<tr class="filter"><td></td><td></td><td></td><td></td></tr>').appendTo(div.tbody).asFilterRow(templates, options);
            return false;
        }

        $(options.addCriteriaSelector).click(div.addFilterRow);

        $(options.clearCriteriaSelector).click(function () {
            $(div.tbody).empty();

            $(div).trigger('filters-cleared');
            return false;
        });



        // THIS MAY NEED TO CHANGE, MAYBE A SERVER SIDE MODEL?
        //        if (options.initialFilters) {
        //            table.loadQuery(options.initialFilters);
        //        }

        div.addFilterRow();
        return false;
    });


}



$.fn.asQueryBuilder.defaults =
{
    clearCriteriaSelector: "#search-criteria-cancel",
    addCriteriaSelector: "#add",
    removeCriteriaTemplateSelector: "#removeFilter",
    runQuerySelector: "#search-criteria-search",
    clearCriteriaSelector: "#clear"
};

function buildQueryOptions(onFormClear) {search-criteria-search
    return {
        runQuerySelector: "#search-criteria-search",
        clearCriteriaSelector: "#search-criteria-cancel",
        runQuery: runQuery,
        onFormClear: onFormClear
    }
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
        return false;
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

        return false;
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
            op: row.operator(),
            value: row.editor.getValue(),
            property: row.property()
        }
    };

    row.changeProperty();
    row.changeOperator();
}






