$(document).ready(function () {

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
                        Column: 'OutputModel',
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

    setupAutocomplete();

    $('#filter-value').focus();
});