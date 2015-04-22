"use strict";

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

exports.__esModule = true;

var messages = _interopRequireWildcard(require("../../../messages"));

var build = _interopRequire(require("../../helpers/build-conditional-assignment-operator-transformer"));

var t = _interopRequireWildcard(require("../../../types"));

var playground = true;

exports.playground = playground;
build(exports, {
  is: function is(node, file) {
    if (t.isAssignmentExpression(node, { operator: "||=" })) {
      var left = node.left;
      if (!t.isMemberExpression(left) && !t.isIdentifier(left)) {
        throw file.errorWithNode(left, messages.get("expectedMemberExpressionOrIdentifier"));
      }
      return true;
    }
  },

  build: function build(node) {
    console.error("The mallet operator is deprecated and will be removed in 5.0.0");
    return t.unaryExpression("!", node, true);
  }
});