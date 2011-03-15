$.fn.asQueryBuilder = function (userOptions) {
    var options = {};
    $.extend(options, $.fn.asQueryBuilder.defaults, userOptions || {});

    return this.each(function (i, div) {
        div.tbody = $('tbody', div).get(0);
        div.grid = $('#' + $(div).metadata().filters.gridId).get(0);

        var queryRunner = function () {
            var notification = div.validate();
            if (notification.errors.length > 0){
                $(div).trigger('invalid-filters', notification);
                return;
            }

            var criterion = div.getCriteria();

            $(div).trigger('valid-filters');
            div.grid.runQuery(criterion);

            return false;
        }

        div.getCriteria = function () {
            return $('tr', div.tbody).map(function () { return this.getCriteria(); });
        }

        div.validate = function() {
            var messages = [];
			
			$('tr', div.tbody).each(function(i, row){
				var message = row.validate();
				if (message == null || message.messages.length == 0) return;
				
				messages.push(message);
			});
			
            return {errors: messages};
        }

        div.hasCriteria = function () {
            return $('tr', div.tbody).length > 0;
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
            var row = $('<tr class="filter"><td></td><td></td><td></td><td></td></tr>').appendTo(div.tbody).asFilterRow(templates, options, div);
            $(div).trigger("filter-added", row);
            $(div).trigger("filters-changed");
            return row;
        }

        $(options.addCriteriaSelector).click(function () {
            div.addFilterRow();
            return false;
        });

        var clearFilters = function () {
            $(div.tbody).empty();
            $(div).trigger('filters-cleared');
            $(div).trigger("filters-changed");
            return false;
        }

        $(options.clearCriteriaSelector).click(clearFilters);

        div.loadQuery = function (criterion) {
            if (criterion.length == 0) return;

            clearFilters();

            for (var i = 0; i < criterion.length; i++) {
                var row = this.addFilterRow();
                row.setCriteria(criterion[i]);
            }
        }

        var metadata = $(div).metadata();
        if (metadata.filters.initialCriteria.length > 0) {
            div.loadQuery(metadata.filters.initialCriteria);
        }
        else {
            div.addFilterRow();
        }

        return false;
    });


}



$.fn.asQueryBuilder.defaults =
{
    clearCriteriaSelector: "#search-criteria-cancel",
    addCriteriaSelector: "#add",
    removeCriteriaTemplateSelector: "#removeFilter",
    runQuerySelector: "#search-criteria-search",
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


if( ! $.jgrid ){ 
	$.jgrid = {
		defaults: {
			missingvalue: 'Missing value'
		} 
	}
}


$.fn.asQueryBuilder.editors = {
    textbox: function (textbox) {
        textbox.setValue = function (value) { $(textbox).val(value); }
        textbox.getValue = function () { return $(textbox).val(); }
        textbox.validate = function() {
            var value = textbox.getValue();
            if ($.trim(value).length == 0){
                return [$.jgrid.defaults.missingvalue];
            }

            return [];
        }
    }
}

$.fn.asFilterRow = function (templates, options, div) {
    var row = this.get(0);
    row.operatorCell = row.childNodes[1];
    row.editorCell = row.childNodes[2];

    $(options.removeCriteriaTemplateSelector).clone().appendTo(row.childNodes[3]).click(function () {
        $(row).remove();
        $(div).trigger("filter-removed");
        $(div).trigger("filters-changed");
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

        row.changeOperator();

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
            editorName = metadata.formatter;
        }

        row.editor = template.get(0);
        $.fn.asQueryBuilder.editors[editorName](row.editor);
    }

    row.validate = function(){
		if ($.isFunction(row.editor.validate) == false) return false;
	
        return {
            header: $('option:selected', row.propertySelector).text(),
            messages: row.editor.validate(),
        };
    }

    row.getCriteria = function () {
        return {
            op: row.operator(),
            value: row.editor.getValue(),
            property: row.property()
        }
    };

    row.setCriteria = function (criteria) {
        $(row.propertySelector).val(criteria.property);
        row.changeProperty();
        row.operatorSelector.val(criteria.op);
        row.changeOperator();
        row.editor.setValue(criteria.value);
    }

    row.changeProperty();
    row.changeOperator();

    return row;
}






