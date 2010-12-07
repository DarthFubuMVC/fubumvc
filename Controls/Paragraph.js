ST.paragraph = function(div, metadata, step) {
    div.step = step;
    div.container = $('.section-container', div).get(0);

    $('.deleteStep', div).removable();

    ST.activateGrammars(div.container, step);

    div.update = function() {
        $(div.container).children('.step').each(function (i, child) {
            child.update();
        });

        return div.step;
    };
}

ST.registerGrammar('.paragraph', ST.paragraph);
