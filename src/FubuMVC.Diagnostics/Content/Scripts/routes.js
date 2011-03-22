$(document).ready(function () {
    var $explorer = $('#route-explorer');
    var url = $explorer.metadata().url;
    $explorer.jqGrid({
        url: url,
        datatype: 'json',
        colNames: ['Route', 'Constraints', 'Action', 'InputModel', 'OutputModel', 'ChainUrl'],
        colModel: [
            { name: 'Route', index: 'Route', width: 165 },
            { name: 'Constraints', index: 'Constraints', width: 55 },
            { name: 'Action', index: 'Action', width: 280 },
            { name: 'InputModel', index: 'InputModel', width: 200 },
            { name: 'OutputModel', index: 'OutputModel', width: 200 },
            { name: 'ChainUrl', index: 'ChainUrl', hidden: true, hidedlg: true }
        ],
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
});