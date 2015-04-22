"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

exports.Expression = Expression;
exports.Identifier = Identifier;
exports.__esModule = true;

var t = _interopRequireWildcard(require("../../../types"));

var optional = true;

exports.optional = optional;

function Expression(node, parent, scope) {
  var res = t.evaluate(node, scope);
  if (res.confident) return t.valueToNode(res.value);
}

function Identifier() {}

// override Expression