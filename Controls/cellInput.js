ST.cellInput = function(input, metadata) {
    input.key = metadata.key;
    $(input).dirtyable();

    var defaultValue = metadata.defaultValue;

    input.bindTo = function(step) {
        var value = step[input.key] || defaultValue;

        if (value) {
            $(input).val(value);
        }
    };

    input.update = function(step) {
        step[input.key] = $(input).val();
    };
}

ST.registerCell('input.cell, select.cell', ST.cellInput);




