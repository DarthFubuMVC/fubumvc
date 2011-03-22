$(document).ready(function () {

    function grid() {
        return $('#route-explorer');
    }

    function filterDialog() {
        return $('#filter-dialog');
    }

    // TODO -- maybe generate this in metadata from our JsonGridModel?
    var columnModel = [
        { name: 'Route', index: 'Route', width: 165 },
        { name: 'Constraints', index: 'Constraints', width: 55 },
        { name: 'Action', index: 'Action', width: 280 },
        { name: 'InputModel', index: 'InputModel', width: 200 },
        { name: 'OutputModel', index: 'OutputModel', width: 200 },
        { name: 'ChainUrl', index: 'ChainUrl', hidden: true, hidedlg: true }
    ];

    function filterColumns() {
        var cols = [];
        for(var i = 0; i < columnModel.length; i++) {
            var col = columnModel[i];
            if(col.hidden) {
                continue;
            }

            cols.push({Name: col.name});
        }

        return cols;
    }

    var viewModel = {
        filters: ko.observableArray([]),
        showFilters: function() {
            return this.filters().length != 0;
        },
        selectedFilter: ko.observable(),
        filterValue: ko.observable(),
        findFilter: function(type, value) {
            for(var i = 0; i < this.filters().length; i++) {
                var filter = this.filters()[i];
                if(filter.type == type && filter.value == value) {
                    return filter;
                }
            }

            return null;
        },
        addFilter: function() {
            var type = this.selectedFilter().Name;
            if(this.findFilter(type, this.filterValue())) {
                return;
            }

            var self = this;
            var value = this.filterValue();
            this.filters.push({
                type: type,
                value: value,
                remove: function() {
                    self.removeFilter(type, value);
                }
            });

            this.closeDialog();
            this.reloadGrid();
        },
        closeDialog: function() {
            filterDialog().dialog('close');
            this.selectedFilter(null);
            this.filterValue('');
        },
        removeFilter: function(type, value) {
            var filter = this.findFilter(type, value);
            if(!filter) {
                return;
            }

            this.filters.remove(filter);
            this.reloadGrid();
        },
        buildFilters: function() {
            var gridFilters = [];
            var findFilter = function(colName) {
                for(var i = 0; i < gridFilters.length; i++) {
                    var f = gridFilters[i];
                    if(f.ColumnName == colName) {
                        return f;
                    }
                }

                return null;
            };

            for(var i = 0; i < this.filters().length; i++) {
                var filter = this.filters()[i];
                var gridFilter = findFilter(filter.type);
                if(!gridFilter) {
                    gridFilter = {
                        ColumnName: filter.type,
                        Values: []
                    };
                    gridFilters.push(gridFilter);
                }

                gridFilter.Values.push(filter.value);
            }

            return gridFilters;
        },
        reloadGrid: function() {
            grid()
              .jqGrid()
              .trigger('reloadGrid');
        },
        helloWorld: function() {
            alert('hi');
        },
        availableFilters: ko.observableArray(filterColumns())
    };

    function setupFilters() {
        filterDialog()
            .dialog({
                bgiframe: true,
                autoOpen: false,
                height: 150,
                width: 500,
                modal: true
            });

        $(document).bind('keydown', 'Ctrl+f', function(evt) {
            filterDialog().dialog('open');
            return false;
        });
    }

    function getData(gridData) {
        var params = {};
        params.page = gridData.page;
        params.rows = gridData.rows;
        params.sidx = gridData.sidx;
        params.sord = gridData.sord;
        params.Filters = viewModel.buildFilters();

        var postData = {
            Body: JSON.stringify(postData)
        };

        $.ajax({
            type: "POST",
            url: grid().metadata().url,
            data: 'Body=' + JSON.stringify(params),
            dataType: "json",
            success: function(data) {
                grid()[0].addJSONData(data);
            },
            error: function() {
                alert('Uh oh');
            }
        });
    }
    
    function setupGrid() {
        grid()
            .jqGrid({
                datatype: function(gridData) {
                    getData(gridData);
                },
                url: grid().metadata().url,
                colNames: ['Route', 'Constraints', 'Action', 'InputModel', 'OutputModel', 'ChainUrl'],
                colModel: columnModel,
                jsonReader: $.fubu.jsonReader,
                rowNum: 20,
                autowidth: true,
                height: '100%',
                mtype: 'POST',
                sortorder: 'asc',
                sortname: 'Route',
                caption: 'Routes',
                pager: '#pager',
                ondblClickRow: function (rowId, iRow, iCol, e) {
                    var url = grid().jqGrid('getCell', rowId, 'ChainUrl');
                    window.location = url;
                }
            });

        $(window).bind('resize', function() {
            grid().setGridWidth($('#route-heading').width());
        });
    }

    ko.applyBindings(viewModel);
    setupFilters();
    setupGrid();
});