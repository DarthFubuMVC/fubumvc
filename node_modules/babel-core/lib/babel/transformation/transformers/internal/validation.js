"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

exports.ForOfStatement = ForOfStatement;
exports.Property = Property;
exports.BlockStatement = BlockStatement;
exports.__esModule = true;

var messages = _interopRequireWildcard(require("../../../messages"));

var t = _interopRequireWildcard(require("../../../types"));

function ForOfStatement(node, parent, scope, file) {
  var left = node.left;
  if (t.isVariableDeclaration(left)) {
    var declar = left.declarations[0];
    if (declar.init) throw file.errorWithNode(declar, messages.get("noAssignmentsInForHead"));
  }
}

exports.ForInStatement = ForOfStatement;

function Property(node, parent, scope, file) {
  if (node.kind === "set") {
    if (node.value.params.length !== 1) {
      throw file.errorWithNode(node.value, messages.get("settersInvalidParamLength"));
    }

    var first = node.value.params[0];
    if (t.isRestElement(first)) {
      throw file.errorWithNode(first, messages.get("settersNoRest"));
    }
  }
}

exports.MethodDefinition = Property;

function BlockStatement(node) {
  for (var i = 0; i < node.body.length; i++) {
    var bodyNode = node.body[i];
    if (t.isExpressionStatement(bodyNode) && t.isLiteral(bodyNode.expression)) {
      bodyNode._blockHoist = Infinity;
    } else {
      return;
    }
  }
}

exports.Program = BlockStatement;