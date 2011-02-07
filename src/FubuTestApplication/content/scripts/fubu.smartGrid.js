(function ($) {
    jQuery.extend($.fn.fmatter, {

    link: function (cellValue, options, rowObject) { 
        var linkName = options.colModel.linkName;
        var url = rowObject[linkName];
        
        return '<a href="' + url + '" target="_top">' + cellValue + '</a>'; 
    },
});

/*
columnFormatters:
{
link: function (column, originalValue, subject) { return '<a href="' + subject[column.UrlPropertyName] + '" target="_top">' + originalValue + '</a>'; },
linktoentity: function (column, originalValue, subject) { eval('var x = ' + originalValue); return '<a href="{Url}" target="_top">{DisplayText}</a>'.fillWith(x); },
image: function (column, originalValue, subject) { return '<img src="' + subject[column.ImageUrlPropertyName] + '" border="0"/>'; },
command: function (column, originalValue, subject) { return '<a href="' + originalValue + '" class="invoke">' + column.Header + '</a>'; },
timeAgo: function (column, originalValue, subject) { return $.timeago(originalValue); }
}
*/

$.fn.smartGrid = function (userOptions) {
    this.each(function (i, div) {
        SmartGrid(div, userOptions);
    });
}

var SmartGrid = function (div, userOptions) {
    var model = $(div).metadata();
    var definition = model.definition;

    var gridDefaultOptions =
    {
        height: "auto",
        imgpath: definition.imageBaseUrl,
        url: definition.url,
        datatype: 'json',
        mtype: 'POST',
        readjustHeaderWidths: true,
        colNames: definition.headers,
        colModel: definition.colModel,
        /*onCellSelect: onCellSelect,*/
        rowNum: 10,
        rowList: [3, 10, 20, 30],
        loadui: "disable",
        sortorder: "asc",
        viewrecords: true,
        jsonReader: {
            repeatitems: true,
            root: "items",
            cell: "cell",
            id: "id"
        },
        pager: $('#' + definition.pagerId),
        onPaging: function (pgButton) {
            if (pgButton == 'records') {
                this.page = 1;
            }
        }
        /*
        ondblClickRow: function (rowid) { ourGridInstance.onDoubleClick(rowid, ourGridInstance); },
        afterInsertRow: function (rowid, rowdata, rowelem) {
        for (var i = 0; i < gridDefinition.Columns.length; i++) {
        var columnDefinition = gridDefinition.Columns[i];

        var columnFormatter = $.ourGrid.columnFormatters[columnDefinition.DisplayType];
        if (columnFormatter) {
        try {
        theGrid.setCell(rowid, i, columnFormatter(columnDefinition, rowdata[columnDefinition.Name], rowelem));
        } catch (formatError) { }
        }
        }

        if (rowelem.findUrl) {
        theGrid.setRowMetaData(rowid, "_dt_findUrl", rowelem.findUrl);
        }

        if (rowelem.viewUrl) {
        theGrid.setRowMetaData(rowid, "_dt_viewUrl", rowelem.viewUrl);
        }
        },
        */
        /*            gridComplete: function () {

        if (onGridComplete) {
        onGridComplete();
        }
        },
        onPaging: function (pgButton) {
        if (pgButton == 'records') {
        this.page = 1;
        }
        },  */
        /*            postData: initalPostData,   */
        //,
        /*            pager: $(pagerSelector),*/

    };

    var gridOptions = {};
    gridOptions = $.extend(gridOptions, gridDefaultOptions, userOptions || {});

    $('table', div).jqGrid(gridOptions);
}

})(jQuery);



$(document).ready(function () {
    $(".grid-container").smartGrid();
});