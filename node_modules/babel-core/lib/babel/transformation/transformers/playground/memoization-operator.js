"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

exports.__esModule = true;

var build = _interopRequire(require("../../helpers/build-conditional-assignment-operator-transformer"));

var t = _interopRequireWildcard(require("../../../types"));

var playground = true;

exports.playground = playground;
build(exports, {
  is: (function (_is) {
    var _isWrapper = function is(_x) {
      return _is.apply(this, arguments);
    };

    _isWrapper.toString = function () {
      return _is.toString();
    };

    return _isWrapper;
  })(function (node) {
    var is = t.isAssignmentExpression(node, { operator: "?=" });
    if (is) t.assertMemberExpression(node.left);
    return is;
  }),

  build: function build(node, file) {
    console.error("The memoization operator is deprecated and will be removed in 5.0.0");
    return t.unaryExpression("!", t.callExpression(t.memberExpression(file.addHelper("has-own"), t.identifier("call")), [node.object, node.property]), true);
  }
});