// Force namespace qualification on global functions (e.g., $.fubu.method)
$.extend({
    fubu: {
        jsonReader: {
            root:'Rows',
            page: 'PageNr',
            total: 'TotalPages',
            records: 'TotalRecords',
            cell: function(row) {
                var columns = [];
                for(var i = 0; i < row.Columns.length; i++) {
                    var col = row.Columns[i];
                    if(col.IsIdentifier) {
                        continue;
                    }
                    //columns[col.Name] = col.Value;
                    columns.push(col.Value);
                }
                return columns;
            },
            id: 'Id'
        }
    }
});

$(document).ready(function () {
    var toggleHover = function () {
        $(this).toggleClass('ui-state-hover');
    };
    $('.ui-button').hover(toggleHover, toggleHover);

    $('.grid > thead > tr > th').each(function() {
        $(this)
            .addClass('ui-state-default')
            .addClass('ui-th-column')
            .addClass('ui-th-ltr');

        $(this).hover(toggleHover, toggleHover);
    });

    //$('button').button();
});