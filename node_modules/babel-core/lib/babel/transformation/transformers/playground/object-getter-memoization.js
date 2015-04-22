"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

exports.MethodDefinition = MethodDefinition;
exports.__esModule = true;

var t = _interopRequireWildcard(require("../../../types"));

var playground = true;

exports.playground = playground;
var visitor = {
  enter: function enter(node, parent, scope, state) {
    if (this.isFunction()) return this.skip();

    if (this.isReturnStatement() && node.argument) {
      node.argument = t.memberExpression(t.callExpression(state.file.addHelper("define-property"), [t.thisExpression(), state.key, node.argument]), state.key, true);
    }
  }
};

function MethodDefinition(node, parent, scope, file) {
  if (node.kind !== "memo") return;
  node.kind = "get";

  console.error("Object getter memoization is deprecated and will be removed in 5.0.0");

  var value = node.value;
  t.ensureBlock(value);

  var key = node.key;

  if (t.isIdentifier(key) && !node.computed) {
    key = t.literal(key.name);
  }

  var state = {
    key: key,
    file: file
  };

  scope.traverse(value, visitor, state);

  return node;
}

exports.Property = MethodDefinition;