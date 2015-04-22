"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _toConsumableArray = function (arr) { if (Array.isArray(arr)) { for (var i = 0, arr2 = Array(arr.length); i < arr.length; i++) arr2[i] = arr[i]; return arr2; } else { return Array.from(arr); } };

exports.BindMemberExpression = BindMemberExpression;
exports.BindFunctionExpression = BindFunctionExpression;
exports.__esModule = true;

var t = _interopRequireWildcard(require("../../../types"));

var playground = true;

exports.playground = playground;

function BindMemberExpression(node, parent, scope) {
  console.error("Method binding is deprecated and will be removed in 5.0.0");

  var object = node.object;
  var prop = node.property;

  var temp = scope.generateTempBasedOnNode(node.object);
  if (temp) object = temp;

  var call = t.callExpression(t.memberExpression(t.memberExpression(object, prop), t.identifier("bind")), [object].concat(_toConsumableArray(node.arguments)));

  if (temp) {
    return t.sequenceExpression([t.assignmentExpression("=", temp, node.object), call]);
  } else {
    return call;
  }
}

function BindFunctionExpression(node, parent, scope) {
  console.error("Method binding is deprecated and will be removed in 5.0.0");

  var buildCall = function buildCall(args) {
    var param = scope.generateUidIdentifier("val");
    return t.functionExpression(null, [param], t.blockStatement([t.returnStatement(t.callExpression(t.memberExpression(param, node.callee), args))]));
  };

  var temp = scope.generateTemp("args");

  return t.sequenceExpression([t.assignmentExpression("=", temp, t.arrayExpression(node.arguments)), buildCall(node.arguments.map(function (node, i) {
    return t.memberExpression(temp, t.literal(i), true);
  }))]);
}