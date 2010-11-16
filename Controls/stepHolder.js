ST.stepHolder = function(section) {
    var holder = section;
    if (!$(section).hasClass('step-holder')) {
        holder = $('.step-holder', section).get(0);
    }

    if (holder == null) {
        holder = section;
    }

    $(holder).children('div.step').remove();

    holder.show = function() { $(holder).show(); }
    holder.hide = function() { $(holder).hide(); }

    section.eachStep = function(func) {
        $(holder).children('.step').each(function(i, x) {
            func(x);
        });
    };

    section.getStepNames = function() {
        var steps = [];
        section.eachStep(function(x) {
            var key = $(x).metadata().key;
            if (key == 'Comment') return;

            steps.push(key);
        });

        return steps;
    };

    section.hasNoSteps = function() {
        return section.getStepNames().length == 0;
    };

    return holder;
}


