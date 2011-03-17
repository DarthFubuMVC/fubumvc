(function ($) {
    jQuery.extend($.fn.fmatter, {
        link: function (cellValue, options, rowObject) {
            var linkName = options.colModel.linkName;
            var url = rowObject[0][linkName];

            return '<a href="' + url + '" target="_top">' + cellValue + '</a>';
        },

        command: function (cellValue, options, rowObject) {
            var linkName = options.colModel.linkName;
            var url = rowObject[0][linkName];

            return '<a href="' + url + '" class="invoke">' + cellValue + '</a>';
        },

        timeAgo: function (cellValue, options, rowObject) {
            return $.timeago(cellValue);
        },

        trimmedLink: function (cellValue, options, rowObject) {
            var linkName = options.colModel.linkName;
            var url = rowObject[0][linkName];

            var length = parseInt(options.colModel.trim-length);
            var displayValue = cellValue;
            if (displayValue.length > length){
                displayValue = displayValue.substr(0, length) + "&hellip;";
            }

            return '<a href="' + url + '" target="_top" title="' + cellValue + '">' + displayValue + '</a>';
        },
    });

    $.fn.smartGrid = function (userOptions) {
        return this.each(function (i, div) {
            SmartGrid(div, userOptions);
        });
    }



    var SmartGrid = function (div, userOptions) {
        var model = $(div).metadata();
        var definition = model.definition;
        div.isGridDisabled = model.disabled == true;

        div.selectedRow = null;
        div.grid = $('table', div);

        div.getData = function (rowid) {
            var item = div.grid.getRowData(rowid);

            var index = parseInt(rowid) - 1;
            var rowData = div.data[index];
            item.data = rowData.cell[0];
            return item;
        }

        var gridDefaultOptions =
        {
            height: "auto",
            url: definition.url,
            datatype: 'json',
            mtype: 'POST',
            altRows: true,
            autowidth: true,
            gridview: true,
            colNames: definition.headers,
            colModel: definition.colModel,
            /*onCellSelect: onCellSelect,*/
            rowNum: model.initialRows || 10,
            rowList: [3, 10, 20, 30],
            loadui: "disable",
            sortorder: definition.sortorder,
            sortname: definition.sortname,
            viewrecords: true,
            jsonReader: {
                repeatitems: true,
                root: "items",
                cell: "cell",
                id: "id"
            },
            postData: { criterion: definition.initialCriteria },
            pager: $('#' + definition.pagerId),
            onPaging: function (pgButton) {
                if (pgButton == 'records') {
                    this.page = 1;
                }
            },
            gridComplete: function (data) {
                $(div).trigger("grid-refreshed", data);
                div.selectedRow = null;
            },
            ondblClickRow: function (rowId, iCol, cellcontent, e) {
                var row = div.getData(rowId);
                $(div).trigger("row-doubleclicked", row);
            },
            onSelectRow: function (rowid, status) {
                div.selectedRow = div.getData(rowid);
                $(div).trigger("row-selected", div.selectedRow);
            },
            loadComplete: function (data) {
                div.data = data.items;
                return true;
            }
        };

        var gridOptions = {};
        gridOptions = $.extend(gridOptions, gridDefaultOptions, userOptions || {});

        div.refresh = function () {
            if (!div.isGridDisabled) {
                div.grid.trigger("reloadGrid");
            }
        }

        div.runQuery = function (criterion) {
            div.grid.setGridParam({ page: 1 });
            div.grid.setPostDataItem("criterion", criterion);
            div.refresh();
        }

        div.setArgument = function(key, value){
            div.grid.setPostDataItem(key, value);
        }

        div.activateUrl = function (url) {
            div.grid.setGridParam({ url: url });
            div.isGridDisabled = false;
            div.refresh();
        }

        div.grid.jqGrid(gridOptions);
        div.refresh();
    }

})(jQuery);

$.fn.activateGrid = function(url){
    return this.each(function(i, div){
        div.activateUrl(url);
    });
}

$(document).ready(function () {
    $(".grid-container").smartGrid();
});