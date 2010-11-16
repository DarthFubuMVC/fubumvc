String.prototype.fillWith = function(parameters) {
    var newValue = this;
    for (parameter in parameters) {
        var placeHolder = new RegExp('\{(' + parameter + ')\}', 'g');
        newValue = newValue.replace(placeHolder, function(match, parameterName) {
            return parameters[parameterName];
        });
    }
    return newValue;
}

$.fn.removable = function() {
    return this.each(function(i, link) {
        $(link).click(function() {
            var section = $(link).closest('.section').get(0);

            $(link).closest('.step').remove();

            section.stepsChanged();

            return false;
        });
    });
}

$.fn.applyBehavior = function(extender, target) {
    return this.each(function(index, element) {
        extender(element, $(element).metadata(), target);
    });
}

$.fn.dirtyable = function() {
    this.each(function(i, elem) {
        $(elem).change(function() {
            $(elem).markDirty();
        });
    });
}

$.fn.markDirty = function () {
    return this.each(function (index, element) {
        $('.test-editor').each(function(y, editor) {
            if ($.isFunction(editor.markDirty)) editor.markDirty();
        });

        //        $(element).closest('.test-editor').each(function(y, editor) {
        //            alert('markDirty');
        //            alert(editor.markDirty);
        //            alert($(editor).html());
        //            if ($.isFunction(editor.markDirty)) editor.markDirty();
        //        });
    });
}

function Matchers() {
    var self = this;
    self.matchers = [];

    self.register = function(match, extender) {
        self.matchers.push({ match: match, extender: extender });
    }

    self.applyAll = function(holder, target) {
        if (!(holder instanceof jQuery)) {
            holder = $(holder);
        }

        for (var i = 0; i < self.matchers.length; i++) {
            var matcher = self.matchers[i];
            holder.children(matcher.match).applyBehavior(matcher.extender, target);
        }
    }

    self.applyToAllDescendents = function(holder, target) {
        if (!(holder instanceof jQuery)) {
            holder = $(holder);
        }

        for (var i = 0; i < self.matchers.length; i++) {
            var matcher = self.matchers[i];
            $(matcher.match, holder).applyBehavior(matcher.extender, target);
        }
    }

    self.apply = function(div, step) {
        var jQ = $(div);

        for (var i = 0; i < self.matchers.length; i++) {
            var matcher = self.matchers[i];

            if (jQ.is(matcher.match)) {
                jQ.applyBehavior(matcher.extender, step);
            }
        }
    }

    return self;
}



var ST = {
    cellMatchers: new Matchers(),
    grammarMatchers: new Matchers(),
    validators: {},

    registerCell: function(match, extender) {
        ST.cellMatchers.register(match, extender);
    },

    registerGrammar: function(match, extender) {
        ST.grammarMatchers.register(match, extender);
    },

    activateCells: function(holder) {
        ST.cellMatchers.applyAll(holder);
    },

    activateGrammars: function(holder, step) {
        ST.grammarMatchers.applyAll(holder, step);
    },

    activateGrammar: function(div, step) {
        ST.grammarMatchers.apply(div, step);
    }


}


