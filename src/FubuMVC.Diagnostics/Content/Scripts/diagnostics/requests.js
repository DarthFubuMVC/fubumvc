$(document).ready(function () {

    function grid() {
        return $('#request-explorer');
    }

    function filterDialog() {
        return $('#filter-dialog');
    }

    var columnModel = $('#column-model').metadata({type:'elem', name:'script'});
    function colNames() {
        var cols = [];
        for(var i = 0; i < columnModel.length; i++) {
            var col = columnModel[i];
            cols.push(col.name);
        }

        return cols;
    }

    function filterColumns() {
        var cols = [];
        for(var i = 0; i < columnModel.length; i++) {
            var col = columnModel[i];
            if(col.hideFilter) {
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

            var value = this.filterValue();
            this.explicitAddFilter(type, value);

            this.closeDialog();
            this.reloadGrid();
        },
        explicitAddFilter: function(type, value) {
            var self = this;
            this.filters.push({
                type: type,
                value: value,
                remove: function() {
                    self.removeFilter(type, value);
                }
            });
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
                width: 510,
                modal: true
            });

        $(document).bind('keydown', 'Ctrl+f', function(evt) {
            filterDialog().dialog('open');
            return false;
        });

        var startupFilters = $('#filters').metadata({type:'elem', name:'script'});
        if(startupFilters.Values) {
            var value = '';
            if(startupFilters.Values.length != 0) {
                value = startupFilters.Values[0];
            }
            
            viewModel.explicitAddFilter(startupFilters.ColumnName, value);
        }
    }

    function setupAutocomplete() {
        var filterInput = $('#filter-value');
        var filterUrl = filterInput.metadata().url;

        filterInput.autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: filterUrl,
                    dataType: 'json',
                    type: 'POST',
                    data: {
                        Column: viewModel.selectedFilter().Name,
                        Query: request.term
                    },
                    success: function(data) {
                        response($.map( data.Values, function( item ) {
							return {
								label: item.ColumnName,
								value: item.Value
							}
						}));
                    }
                });
            },
            minLength: 2,
            select: function(event, ui) {
                filterInput.val(ui.item.value);
            }
        });
    }

    function getData(gridData) {
        var params = {};
        params.page = gridData.page;
        params.rows = gridData.rows;
        params.sidx = gridData.sidx;
        params.sord = gridData.sord;
        params.Filters = viewModel.buildFilters();

        $.ajax({
            type: "POST",
            url: grid().metadata().url,
            data: JSON.stringify(params),
			contentType: "application/json",
            dataType: "json",
            success: function(data) {
                $('#NoData').hide();
                grid()[0].addJSONData(data);
            },
            error: function() {
                if(window.console) {
                    console.log('An error occurred loading the grid');
                }
            }
        });
    }
    
    function setupGrid() {
        if(!columnModel.length) {
            $('#NoData').show();
            return;
        }
        
        grid()
            .jqGrid({
                datatype: function(gridData) {
                    getData(gridData);
                },
                url: grid().metadata().url,
                colNames: colNames(),
                colModel: columnModel,
                jsonReader: $.fubu.jsonReader,
                rowNum: 50,
                autowidth: true,
                height: '100%',
                mtype: 'POST',
                sortorder: 'asc',
                sortname: 'Route',
                caption: 'Routes',
                pager: '#pager',
                onCellSelect: function (rowId, iRow, content, e) {
                    window.location = rowId;
                },
                ondblClickRow: function (rowId, iRow, iCol, e) {
                    window.location = rowId;
                },
                sortable: true
            });

        $(window).bind('resize', function() {
            grid().setGridWidth($('#explorer-heading').width());
        });
    }

    ko.applyBindings(viewModel);
    setupFilters();
    setupGrid();
    setupAutocomplete();
});