"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

exports.check = check;
exports.ObjectExpression = ObjectExpression;
exports.__esModule = true;

var ReplaceSupers = _interopRequire(require("../../helpers/replace-supers"));

var t = _interopRequireWildcard(require("../../../types"));

function check(node) {
  return t.isIdentifier(node, { name: "super" });
}

function Property(node, scope, getObjectRef, file) {
  if (!node.method) return;

  var value = node.value;
  var thisExpr = scope.generateUidIdentifier("this");

  var replaceSupers = new ReplaceSupers({
    topLevelThisReference: thisExpr,
    getObjectRef: getObjectRef,
    methodNode: node,
    isStatic: true,
    scope: scope,
    file: file
  });

  replaceSupers.replace();

  if (replaceSupers.hasSuper) {
    value.body.body.unshift(t.variableDeclaration("var", [t.variableDeclarator(thisExpr, t.thisExpression())]));
  }
}

function ObjectExpression(node, parent, scope, file) {
  var objectRef;
  var getObjectRef = function () {
    return (!objectRef && (objectRef = scope.generateUidIdentifier("obj")), objectRef);
  };

  for (var i = 0; i < node.properties.length; i++) {
    Property(node.properties[i], scope, getObjectRef, file);
  }

  if (objectRef) {
    scope.push({ id: objectRef });
    return t.assignmentExpression("=", objectRef, node);
  }
}