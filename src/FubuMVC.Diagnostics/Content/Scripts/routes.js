$(document).ready(function () {
    var $explorer = $('#route-explorer');
    var url = $explorer.metadata().url;
    $explorer.jqGrid({
        url: url,
        datatype: 'json',
        colNames: ['Route', 'Constraints', 'Action', 'InputModel', 'OutputModel'],
        colModel: [
            {name:'Route', index: 'Route', width:165},
            {name:'Constraints', index: 'Constraints', width:55},
            {name:'Action', index: 'Action', width:280},
            {name:'InputModel', index: 'InputModel', width:200},
            {name:'OutputModel', index: 'OutputModel', width:200}
        ],
        rowNum: -1,
        height: '100%',
        loadonce: true,
        sortorder: 'desc',
        sortname: 'Route',
        caption: 'Routes'
    });
});