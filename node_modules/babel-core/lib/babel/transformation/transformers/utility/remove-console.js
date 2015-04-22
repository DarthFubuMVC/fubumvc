"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

exports.CallExpression = CallExpression;
exports.__esModule = true;

var t = _interopRequireWildcard(require("../../../types"));

var isConsole = t.buildMatchMemberExpression("console", true);

var optional = true;

exports.optional = optional;

function CallExpression(node, parent) {
  if (isConsole(node.callee)) {
    if (t.isExpressionStatement(parent)) {
      this.parentPath.remove();
    } else {
      this.remove();
    }
  }
}