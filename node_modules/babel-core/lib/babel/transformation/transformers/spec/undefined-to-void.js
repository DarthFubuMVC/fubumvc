"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

exports.Identifier = Identifier;
exports.__esModule = true;

var t = _interopRequireWildcard(require("../../../types"));

var optional = true;

exports.optional = optional;

function Identifier(node, parent) {
  if (node.name === "undefined" && this.isReferenced()) {
    return t.unaryExpression("void", t.literal(0), true);
  }
}