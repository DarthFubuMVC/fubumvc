"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

exports.UnaryExpression = UnaryExpression;
exports.__esModule = true;

var t = _interopRequireWildcard(require("../../../types"));

var optional = true;

exports.optional = optional;

function UnaryExpression(node, parent, scope, file) {
  this.skip();

  if (node.operator === "typeof") {
    var call = t.callExpression(file.addHelper("typeof"), [node.argument]);
    if (t.isIdentifier(node.argument)) {
      var undefLiteral = t.literal("undefined");
      return t.conditionalExpression(t.binaryExpression("===", t.unaryExpression("typeof", node.argument), undefLiteral), undefLiteral, call);
    } else {
      return call;
    }
  }
}