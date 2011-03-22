$(document).ready(function () {
    var $explorer = $('#route-explorer');
    var url = $explorer.metadata().url;
    var colModel = [
        { name: 'Route', index: 'Route', width: 165 },
        { name: 'Constraints', index: 'Constraints', width: 55 },
        { name: 'Action', index: 'Action', width: 280 },
        { name: 'InputModel', index: 'InputModel', width: 200 },
        { name: 'OutputModel', index: 'OutputModel', width: 200 },
        { name: 'ChainUrl', index: 'ChainUrl', hidden: true, hidedlg: true }
    ];

    var reloadGrid = function() {
    };

    var viewModel = {
        filters: [],
        findFilter: function(type, value) {
        },
        addFilter: function(type, value) {
            reloadGrid();
        },
        removeFilter: function(type, value) {
            reloadGrid();
        }
    };

    

    $explorer.jqGrid({
        url: url,
        datatype: 'json',
        colNames: ['Route', 'Constraints', 'Action', 'InputModel', 'OutputModel', 'ChainUrl'],
        colModel: colModel,
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
            var url = $('#route-explorer').jqGrid('getCell', rowId, 'ChainUrl');
            window.location = url;
        }
    });

    $(window).bind('resize', function() {
        // don't really want to keep that jqGrid reference lying around
        $('#route-explorer').setGridWidth($('#route-heading').width());
    });

    $('#filter-dialog').dialog({
        bgiframe: true,
        autoOpen: false,
        height: 150,
        width: 500,
        modal: true
    });

    $(document).bind('keydown', 'Ctrl+f', function(evt) {
        $('#filter-dialog').dialog('open');
        return false;
    });
});