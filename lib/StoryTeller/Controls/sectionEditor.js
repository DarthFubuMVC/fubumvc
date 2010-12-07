// section -- holding div
// metadata -- directives
// step -- the step we're binding to
ST.sectionEditor = function (section, metadata, step) {
    section.step = step;
    section.fixture = metadata.fixture;

    $('.deleteStep', section).removable();

    if (step.key() == "Test" || $(section).hasClass('top-node')) {
        section.leaf = step;
    }
    else {
        section.leaf = step.childFor(metadata.leafName);
    }

    section.holder = ST.stepHolder(section);
    section.holder.hide();

    section.stepChangeHandlers = [];

    section.registerStepChangeHandler = function (func) {
        section.stepChangeHandlers.push(func);
    }

    ST.selector(section, metadata);
    //    ST.applyValidation(section);

    section.isLatched = true;
    section.stepsChanged = function () {
        for (var i = 0; i < section.stepChangeHandlers.length; i++) {
            section.stepChangeHandlers[i](section);
        }

        if (section.isLatched) return;
        $(section).markDirty();
    }

    // "key" may be a string denoting the grammar or a 
    // child Step object
    section.addStep = function (key) {
        if (key == null || key == '') return;

        var step = new Step(key);
        if (key.isStep) {
            step = key;
            key = step.key();
        }

        var search = "#{fixture} > .{key}".fillWith({ fixture: section.fixture, key: key });

        var node = $(search).clone().appendTo(section.holder);

        ST.activateGrammar(node, step);

        section.stepsChanged();

        section.isLatched = false;

        return node;
    };

    section.update = function () {
        section.leaf.children = [];

        section.eachStep(function (x) {
            section.leaf.children.push(x.update());
        });

        return step;
    };

    for (var i = 0; i < section.leaf.children.length; i++) {
        section.addStep(section.leaf.children[i]);
    }

    if (section.hasNoSteps()) {
        if (metadata.autoSelectKey) {
            var control = section.addStep(metadata.autoSelectKey);
            var link = $('a.deleteStep', control);
            if (link.length > 0) {
                $(link.get(0)).addClass("auto-select-remover");
            }
        }
    }

    section.stepsChanged();
    section.isLatched = false;
    section.holder.show();
}

ST.registerGrammar('.section', ST.sectionEditor);

$.fn.sectionEditor = function(metadata, step) {
    return this.each(function(i, section) {
        ST.sectionEditor(section, metadata, step);
    });
}


$.fn.stepsChanged = function() {
    return this.each(function(i, section) {
        section.stepsChanged();
    });
}

