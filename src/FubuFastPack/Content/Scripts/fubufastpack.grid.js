(function ($) {
    var containsAction = function (a, s) { return arrayContains(s.actions, a.key); }

    var OURGRID_DATA_KEY = "ourGrid";

    $.ourGrid = {
        from: function (jqGridSelector) {
            return $(jqGridSelector).data(OURGRID_DATA_KEY);
        },

        eachGrid: function (jqGridSelector, callback) {
            $(jqGridSelector).each(function () {
                callback($(this).data(OURGRID_DATA_KEY));
            });
        },

        toColumnNames: function (gridDefinition) {
            return $.map(gridDefinition.Columns, function (item, i) {
                return item.Header;
            });
        },

        toJqGridColumnModel: function (gridDefinition) {
            return $.map(gridDefinition.Columns, function (item, i) {
                var oneColumn = { name: item.Name, index: item.Name, hidden: item.Hidden, sortable: item.Sortable, width: item.Width, editable: !!item.EditType, edittype: item.EditType };

                return oneColumn;
            });
        },

        columnFormatters:
        {
            link: function (column, originalValue, subject) { return '<a href="' + subject[column.UrlPropertyName] + '" target="_top">' + originalValue + '</a>'; },
            linktoentity: function (column, originalValue, subject) { eval('var x = ' + originalValue); return '<a href="{Url}" target="_top">{DisplayText}</a>'.fillWith(x); },
            image: function (column, originalValue, subject) { return '<img src="' + subject[column.ImageUrlPropertyName] + '" border="0"/>'; },
            command: function (column, originalValue, subject) { return '<a href="' + originalValue + '" class="invoke">' + column.Header + '</a>'; },
            timeAgo: function (column, originalValue, subject) { return $.timeago(originalValue); }
        }
    };

    var OurGrid = function (jqGridInstance) {
        var self = this;
        this.jqGridInstance = jqGridInstance;
        this.onDoubleClick = function (rowid) { };

        this.selectedRowId = function () {
            return self.jqGridInstance.getGridParam("selrow");
        };

        this.getSelectedRowFindUrl = function () {
            return self.jqGridInstance.getSelectedRowFindUrl();
        }

        this.refresh = function (newSettings) {
            if (newSettings) {
                self.jqGridInstance.setGridParam(newSettings);
            }
            self.jqGridInstance.trigger("reloadGrid");
        };
    };

    $.fn.ourGrid = function (gridDefinition, initalPostData, userOptions, onGridComplete) {
        $(this).addClass("grid-container");
        var onCellSelect = null;

        try {
            onCellSelect = global.onGridCellSelect;
        } catch (e) { }

        var theGrid = this;
        var gridId = $(theGrid).attr("id");
        var ourGridInstance = new OurGrid(theGrid);
        theGrid.data(OURGRID_DATA_KEY, ourGridInstance);

        var columnNames = $.ourGrid.toColumnNames(gridDefinition);
        var columnModel = $.ourGrid.toJqGridColumnModel(gridDefinition);
        var pagerSelector = "#" + gridId + "_pager";

        var gridDefaultOptions =
        {
            height: "auto",
            imgpath: global.baseUrl + 'content/images/shared/grid',
            url: gridDefinition.Url,
            editurl: gridDefinition.EditUrl,
            datatype: 'json',
            mtype: 'GET',
            readjustHeaderWidths: true,
            colNames: columnNames,
            colModel: columnModel,
            onCellSelect: onCellSelect,
            rowNum: 10,
            rowList: [3, 10, 20, 30],
            loadui: "disable",
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
            gridComplete: function () {

                if (onGridComplete) {
                    onGridComplete();
                }
            },
            onPaging: function (pgButton) {
                if (pgButton == 'records') {
                    this.page = 1;
                }
            },
            postData: initalPostData,
            sortorder: "asc",
            //sortname: gridDefinition.DefaultSortColumn || '',
            viewrecords: true,
            pager: $(pagerSelector),
            jsonReader: {
                repeatitems: true,
                root: "items",
                cell: "cell",
                id: "id"
            }
        };

        if (gridDefinition.DefaultSortColumn) {
            gridDefaultOptions.sortname = gridDefinition.DefaultSortColumn;
        }
        var gridOptions = {};
        gridOptions = $.extend(gridOptions, gridDefaultOptions, userOptions || {});

        var userRowCount = dovetail.core.rememberWidgetState(gridId);
        if (userRowCount) {
            gridOptions.rowNum = userRowCount;
        }

        var createdGrid = this.jqGrid(gridOptions);

        $(pagerSelector + " select.selbox").change(function () {
            var selectedRowCount = $(this).val();
            dovetail.core.rememberWidgetState(gridId, selectedRowCount)
        });

        return createdGrid;
    }

    /* Add functionality to jqGrid */
    $.fn.setRowMetaData = function (rowid, key, val) {
        var ret = false;
        this.each(function () {
            var $t = this;
            if (!$t.grid) { return; }
            if (rowid) {
                var ind = $($t).getInd($t.rows, rowid);
                if (ind) {
                    ret = $($t.rows[ind]).data(key, val);
                }
            }
        });
        return ret;
    }

    $.fn.getRowMetaData = function (rowid, key) {
        var ret = false;
        this.each(function () {
            var $t = this;
            if (!$t.grid) { return; }
            if (rowid) {
                var ind = $($t).getInd($t.rows, rowid);
                if (ind) {
                    ret = $($t.rows[ind]).data(key);
                }
            }
        });
        return ret;
    }

    $.fn.getSelectedRowFindUrl = function () {
        var rowId = $(this).getGridParam("selrow");
        return $(this).getRowMetaData(rowId, "_dt_findUrl");
    }
    /* END Add functionality to jqGrid */

})(jQuery);
