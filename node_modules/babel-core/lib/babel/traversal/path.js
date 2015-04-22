"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

var _createClass = (function () { function defineProperties(target, props) { for (var key in props) { var prop = props[key]; prop.configurable = true; if (prop.value) prop.writable = true; } Object.defineProperties(target, props); } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _classCallCheck = function (instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } };

var traverse = _interopRequire(require("./index"));

var includes = _interopRequire(require("lodash/collection/includes"));

var Scope = _interopRequire(require("./scope"));

var t = _interopRequireWildcard(require("../types"));

var TraversalPath = (function () {
  function TraversalPath(parent, container) {
    _classCallCheck(this, TraversalPath);

    this.container = container;
    this.parent = parent;
    this.data = {};
  }

  TraversalPath.get = function get(parentPath, context, parent, container, key) {
    var _container;

    var targetNode = container[key];
    var paths = (_container = container, !_container._paths && (_container._paths = []), _container._paths);
    var path;

    for (var i = 0; i < paths.length; i++) {
      var pathCheck = paths[i];
      if (pathCheck.node === targetNode) {
        path = pathCheck;
        break;
      }
    }

    if (!path) {
      path = new TraversalPath(parent, container);
      paths.push(path);
    }

    path.setContext(parentPath, context, key);

    return path;
  };

  TraversalPath.getScope = function getScope(node, parent, scope) {
    var ourScope = scope;

    // we're entering a new scope so let's construct it!
    if (t.isScope(node, parent)) {
      ourScope = new Scope(node, parent, scope);
    }

    return ourScope;
  };

  TraversalPath.prototype.insertBefore = function insertBefore(node) {};

  TraversalPath.prototype.insertAfter = function insertAfter(node) {};

  TraversalPath.prototype.setData = function setData(key, val) {
    return this.data[key] = val;
  };

  TraversalPath.prototype.getData = function getData(key) {
    return this.data[key];
  };

  TraversalPath.prototype.setScope = function setScope() {
    this.scope = TraversalPath.getScope(this.node, this.parent, this.context.scope);
  };

  TraversalPath.prototype.setContext = function setContext(parentPath, context, key) {
    this.shouldSkip = false;
    this.shouldStop = false;

    this.parentPath = parentPath || this.parentPath;
    this.context = context;
    this.state = context.state;
    this.opts = context.opts;
    this.key = key;

    this.setScope();
  };

  TraversalPath.prototype.remove = function remove() {
    this._refresh(this.node, []);
    this.container[this.key] = null;
    this.flatten();
  };

  TraversalPath.prototype.skip = function skip() {
    this.shouldSkip = true;
  };

  TraversalPath.prototype.stop = function stop() {
    this.shouldStop = true;
    this.shouldSkip = true;
  };

  TraversalPath.prototype.flatten = function flatten() {
    this.context.flatten();
  };

  TraversalPath.prototype._refresh = function _refresh(oldNode, newNodes) {};

  TraversalPath.prototype.refresh = function refresh() {
    var node = this.node;
    this._refresh(node, [node]);
  };

  TraversalPath.prototype.call = function call(key) {
    var node = this.node;
    if (!node) return;

    var opts = this.opts;
    var fn = opts[key] || opts;
    if (opts[node.type]) fn = opts[node.type][key] || fn;

    var replacement = fn.call(this, node, this.parent, this.scope, this.state);

    if (replacement) {
      this.node = replacement;
    }
  };

  TraversalPath.prototype.isBlacklisted = function isBlacklisted() {
    var blacklist = this.opts.blacklist;
    return blacklist && blacklist.indexOf(this.node.type) > -1;
  };

  TraversalPath.prototype.visit = function visit() {
    if (this.isBlacklisted()) return false;

    this.call("enter");

    if (this.shouldSkip) {
      return this.shouldStop;
    }

    var node = this.node;
    var opts = this.opts;

    if (node) {
      if (Array.isArray(node)) {
        // traverse over these replacement nodes we purposely don't call exitNode
        // as the original node has been destroyed
        for (var i = 0; i < node.length; i++) {
          traverse.node(node[i], opts, this.scope, this.state, this);
        }
      } else {
        traverse.node(node, opts, this.scope, this.state, this);
        this.call("exit");
      }
    }

    return this.shouldStop;
  };

  TraversalPath.prototype.get = function get(key) {
    return TraversalPath.get(this, this.context, this.node, this.node, key);
  };

  TraversalPath.prototype.isReferencedIdentifier = function isReferencedIdentifier(opts) {
    return t.isReferencedIdentifier(this.node, this.parent, opts);
  };

  TraversalPath.prototype.isReferenced = function isReferenced() {
    return t.isReferenced(this.node, this.parent);
  };

  TraversalPath.prototype.isScope = function isScope() {
    return t.isScope(this.node, this.parent);
  };

  TraversalPath.prototype.getBindingIdentifiers = function getBindingIdentifiers() {
    return t.getBindingIdentifiers(this.node);
  };

  _createClass(TraversalPath, {
    node: {
      get: function () {
        return this.container[this.key];
      },
      set: function (replacement) {
        if (!replacement) return this.remove();

        var oldNode = this.node;
        var isArray = Array.isArray(replacement);
        var replacements = isArray ? replacement : [replacement];

        // inherit comments from original node to the first replacement node
        var inheritTo = replacements[0];
        if (inheritTo) t.inheritsComments(inheritTo, oldNode);

        // replace the node
        this.container[this.key] = replacement;

        // potentially create new scope
        this.setScope();

        // refresh scope with new/removed bindings
        this._refresh(oldNode, replacements);

        var file = this.scope && this.scope.file;
        if (file) {
          for (var i = 0; i < replacements.length; i++) {
            file.checkNode(replacements[i], this.scope);
          }
        }

        // we're replacing a statement or block node with an array of statements so we better
        // ensure that it's a block
        if (isArray) {
          if (includes(t.STATEMENT_OR_BLOCK_KEYS, this.key) && !t.isBlockStatement(this.container)) {
            t.ensureBlock(this.container, this.key);
          }

          this.flatten();
          // TODO: duplicate internal path metadata across the new node paths
        }
      }
    }
  });

  return TraversalPath;
})();

module.exports = TraversalPath;

for (var i = 0; i < t.TYPES.length; i++) {
  (function () {
    var type = t.TYPES[i];
    var typeKey = "is" + type;
    TraversalPath.prototype[typeKey] = function (opts) {
      return t[typeKey](this.node, opts);
    };
  })();
}

// todo