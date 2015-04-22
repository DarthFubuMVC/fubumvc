"use strict";

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

var each = _interopRequire(require("lodash/collection/each"));

each(["BindMemberExpression", "BindFunctionExpression"], function (type) {
  exports[type] = function () {
    throw new ReferenceError("Trying to render non-standard playground node " + JSON.stringify(type));
  };
});