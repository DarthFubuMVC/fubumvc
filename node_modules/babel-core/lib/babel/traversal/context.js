"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

var _classCallCheck = function (instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } };

var TraversalPath = _interopRequire(require("./path"));

var flatten = _interopRequire(require("lodash/array/flatten"));

var compact = _interopRequire(require("lodash/array/compact"));

var t = _interopRequireWildcard(require("../types"));

var TraversalContext = (function () {
  function TraversalContext(scope, opts, state, parentPath) {
    _classCallCheck(this, TraversalContext);

    this.shouldFlatten = false;
    this.parentPath = parentPath;
    this.scope = scope;
    this.state = state;
    this.opts = opts;
  }

  TraversalContext.prototype.flatten = function flatten() {
    this.shouldFlatten = true;
  };

  TraversalContext.prototype.visitNode = function visitNode(node, obj, key) {
    var iteration = TraversalPath.get(this.parentPath, this, node, obj, key);
    return iteration.visit();
  };

  TraversalContext.prototype.visit = function visit(node, key) {
    var nodes = node[key];
    if (!nodes) return;

    if (!Array.isArray(nodes)) {
      return this.visitNode(node, node, key);
    }

    // nothing to traverse!
    if (nodes.length === 0) {
      return;
    }

    for (var i = 0; i < nodes.length; i++) {
      if (nodes[i] && this.visitNode(node, nodes, i)) {
        return true;
      }
    }

    if (this.shouldFlatten) {
      node[key] = flatten(node[key]);

      if (t.FLATTENABLE_KEYS.indexOf(key) >= 0) {
        // we can safely compact this
        node[key] = compact(node[key]);
      }
    }
  };

  return TraversalContext;
})();

module.exports = TraversalContext;