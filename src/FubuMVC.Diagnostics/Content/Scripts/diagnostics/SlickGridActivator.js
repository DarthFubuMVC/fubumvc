(function ($) {
    // register namespace
    $.extend(true, window, {
        "Slick": {
            "Formatters": {
                "StringArray": StringArray,
                "DotNetType": DotNetType
            }
        }
    });

    function StringArray(row, cell, value, columnDef, dataContext) {
        if (value.length == 0) return "-";

        var inner = value[0];
        if (value.length > 1) {
            inner += ' (' + value.length - 1 + ' more)';
        }

        var qtipContent = '';

        var qtipContent = '<span class="qtip-text"><ul>';

        for (var i = 0; i < value.length; i++) {
            qtipContent += '<li>' + value[i] + '</li>';
        }

        qtipContent += '</ul></span>';

        return '<span data-header="' + columnDef.name + '" class="drilldown ui-icon ui-icon-newwin">' + qtipContent + '</span>' + inner;


    }

    function DotNetType(row, cell, value, columnDef, dataContext) {
        //return '<span data-values="' + JSON.stringify(value) +
    }
})(jQuery);


$(document).ready(function () {
    $('.slick-grid').each(function (i, div) {
        makeSlickGrid(div);

        div.update({});
    });


});


function makeSlickGrid(div) {
    var columnJson = $(div).data('columns');

    eval('var columns = ' + columnJson);

    var url = $(div).data('url');

    var options = {
        enableCellNavigation: true,
        enableColumnReorder: true
    };

    var grid = new Slick.Grid("#" + div.id, [], columns, options);

    div.update = function (query) {
        if (query == null) {
            query = {};
        }

        $.ajax({
            url: url,
            dataType: 'json',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(query),
            success: function (data) {
                grid.setData(data); // A different, empty or sorted array.
                grid.updateRowCount();
                grid.render();

                $('.drilldown', div).each(function (i, span) {
                    var header = $(span).data('header');

                    $(span).click(function () {
                        $('.qtip-text', span).dialog({
                            title: header
                        });
                    });




                });
            }


        });
    }


}