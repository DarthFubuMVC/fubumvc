ST.sentence = function(holder, metadata, step) {
    ST.activateCells(holder);

    holder.step = step;

    $('.deleteStep', holder).removable();

    holder.each = function(func) {
        $('.cell', holder).each(function(i, part) {
            func(part);
        });
    };

    holder.each(function(part) {
        part.bindTo(step);
    });

    holder.update = function() {
        holder.each(function(part) {
            part.update(step);
        });

        return step;
    };

    return holder;
}

ST.registerGrammar('.sentence', ST.sentence);
