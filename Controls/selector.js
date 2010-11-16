ST.behaviors = {
    Single: function(section) {
        if (section.hasNoSteps()) {
            section.showSelections();
        }
        else {
            section.hideSelections();
        }
    },

    OneOrMore: function(section) {
        if (section.hasNoSteps()) {
            section.showSelections();
            //section.errors.hide();
        }
    }
};



ST.selector = function(section, metadata) {
    var grammarSelector = $('.grammar-selector', section).get(0);
    $('.add-link', grammarSelector).grammarLink(section);

    section.openSelectorLink = $('.add-section-activator', section);
    section.selector = $('.grammar-selector', section);

    section.hideSelections = function() {
        section.openSelectorLink.show();
        section.selector.hide();
    }

    section.showSelections = function() {
        section.openSelectorLink.hide();
        section.selector.show();
    }

    $('.closer', section).click(function() {
        section.hideSelections();
        return false;
    });

    section.openSelectorLink.click(function() {
        section.showSelections();
        return false;
    });

    var stepChangeHandler = ST.behaviors[metadata.selectionMode];
    if (stepChangeHandler){
        section.registerStepChangeHandler(stepChangeHandler);
    }
}


$.fn.grammarLink = function(section) {
    return this.each(function(i, link) {
        var key = $(link).metadata().key;
        $(link).click(function() {
            section.addStep(key);

            return false;
        });
    });
}
