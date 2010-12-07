function Step(key) {
    var self = { children: [] };
    

    if (key.x_Name) {
        self = key;
        if (!self.children) {
            self.children = [];
        }

        $(self.children).each(function(i, child) {
            Step(child);
        });
    }
    else {
        self.x_Name = key;
        self.x_InnerText = '';
    }
    
    

    self.propertyNames = function() {
        var names = [];
        for (prop in self) {
            if (prop == 'x_Name' || prop == 'children' || prop == 'x_InnerText' || prop == 'isStep') continue;

            if ($.isFunction(self[prop])) continue;

            names.push(prop);
        }

        return names;
    }

    self.simpleClone = function() {
        var clone = new Step(self.key());

        $(self.propertyNames()).each(function(i, name) {
            clone[name] = self[name];
        });

        return clone;
    }

    self.remove = function(propName) {
        delete self[propName];
    }

    self.isStep = true;

    self.key = function() {
        return self.x_Name;
    };

    self.findChildren = function(name) {
        return $.grep(self.children, function(s) { return s.x_Name == name; });
    };

    self.childFor = function(leafName) {
        var matching = self.findChildren(leafName);
        if (matching.length > 0) {
            return matching[0];
        }
        else {
            var childStep = new Step(leafName);
            self.children.push(childStep);
            return childStep;
        }
    };

    self.text = function(text) {
        if (arguments.length == 0) {
            return self.x_InnerText;
        }

        self.x_InnerText = text;
    }

    return self;
}
