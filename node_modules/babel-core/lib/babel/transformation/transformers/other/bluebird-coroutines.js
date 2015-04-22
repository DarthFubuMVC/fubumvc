"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

exports.manipulateOptions = manipulateOptions;
exports.__esModule = true;

var remapAsyncToGenerator = _interopRequire(require("../../helpers/remap-async-to-generator"));

var t = _interopRequireWildcard(require("../../../types"));

function manipulateOptions(opts) {
  opts.experimental = true;
  opts.blacklist.push("regenerator");
}

var optional = true;

exports.optional = optional;
exports.Function = function (node, parent, scope, file) {
  if (!node.async || node.generator) return;

  return remapAsyncToGenerator(node, t.memberExpression(file.addImport("bluebird", null, true), t.identifier("coroutine")), scope);
};