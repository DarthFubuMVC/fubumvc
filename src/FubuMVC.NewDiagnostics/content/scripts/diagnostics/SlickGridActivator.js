$(document).ready(function () {
    $('.slick-grid').each(function (i, div) {
        makeSlickGrid(div);

        div.update({});
    });
});


function makeSlickGrid(div){
    var columnJson = $(div).data('columns');

    eval('var columns = ' + columnJson);

    var url = $(div).data('url');

    var options = {
        enableCellNavigation: true,
        enableColumnReorder: true
    };

    var data = [{ UniqueId: '1', FirstCallDescription: 'a.b()' }, { UniqueId: '2', FirstCallDescription: 'c.d()' }, { UniqueId: '1', FirstCallDescription: 'e.f()' } ]

    var grid = new Slick.Grid("#" + div.id, data, columns, options);

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
            }
        });
    }


}