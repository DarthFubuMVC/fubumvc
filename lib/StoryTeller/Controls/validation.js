ST.applyValidation = function(section) {
    section.validationSteps = [];
    section.errors = $('.errorMessage', section);

    var group = $(section);
    for (clazz in ST.validators) {
        if (group.hasClass(clazz)) {
            section.validationSteps.push(ST.validators[clazz]);
        }
    }

    section.findValidationMessage = function() {
        var steps = section.holder.getStepNames();
        for (var i = 0; i < section.validationSteps.length; i++) {
            var message = section.validationSteps[i](steps);
            if (message) {
                return message;
            }
        }

        return null;
    };

    section.validate = function() {
        if (section.errors.is(":visible")) {
            section.errors.hide();
        }

        var message = section.findValidationMessage();
        if (message == null) {
            return true;
        }

        section.errors.html(message).show();

        return false;
    };

    section.stepChangeHandlers.push(function(x) { x.validate(); });
}

ST.validators['Single'] = function(steps) {
    if (steps.length != 1) {
        return 'Must choose one option';
    }

    return null;
}

ST.validators['OneOrMore'] = function(steps) {
    if (steps.length == 0) {
        return 'Must choose one or more options';
    }

    return null;
}

ST.validators['NoRepeatSteps'] = function(steps) {
    var seen = [];
    for (i = 0; i < steps.length; i++) {
        var step = steps[i];
        if ($.inArray(step, seen) > -1) {
            return 'Each option can only appear once';
        }

        seen.push(step);
    }

    return null;
}









