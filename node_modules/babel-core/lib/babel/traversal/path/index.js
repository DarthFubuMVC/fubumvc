"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

var _createClass = (function () { function defineProperties(target, props) { for (var key in props) { var prop = props[key]; prop.configurable = true; if (prop.value) prop.writable = true; } Object.defineProperties(target, props); } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _classCallCheck = function (instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } };

var isBoolean = _interopRequire(require("lodash/lang/isBoolean"));

var isNumber = _interopRequire(require("lodash/lang/isNumber"));

var isRegExp = _interopRequire(require("lodash/lang/isRegExp"));

var isString = _interopRequire(require("lodash/lang/isString"));

var traverse = _interopRequire(require("../index"));

var includes = _interopRequire(require("lodash/collection/includes"));

var assign = _interopRequire(require("lodash/object/assign"));

var Scope = _interopRequire(require("../scope"));

var t = _interopRequireWildcard(require("../../types"));

var TraversalPath = (function () {
  function TraversalPath(parent, container) {
    _classCallCheck(this, TraversalPath);

    this.container = container;
    this.parent = parent;
    this.data = {};
  }

  TraversalPath.get = function get(parentPath, context, parent, container, key, file) {
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

    path.setContext(parentPath, context, key, file);

    return path;
  };

  TraversalPath.getScope = function getScope(path, scope, file) {
    var ourScope = scope;

    // we're entering a new scope so let's construct it!
    if (path.isScope()) {
      ourScope = new Scope(path, scope, file);
    }

    return ourScope;
  };

  TraversalPath.prototype.insertBefore = function insertBefore(node) {};

  TraversalPath.prototype.insertAfter = function insertAfter(node) {};

  TraversalPath.prototype.setData = function setData(key, val) {
    return this.data[key] = val;
  };

  TraversalPath.prototype.getData = function getData(key, def) {
    var val = this.data[key];
    if (!val && def) val = this.data[key] = def;
    return val;
  };

  TraversalPath.prototype.setScope = function setScope(file) {
    this.scope = TraversalPath.getScope(this, this.context && this.context.scope, file);
  };

  TraversalPath.prototype.setContext = function setContext(parentPath, context, key, file) {
    this.shouldSkip = false;
    this.shouldStop = false;

    this.parentPath = parentPath || this.parentPath;
    this.key = key;

    if (context) {
      this.context = context;
      this.state = context.state;
      this.opts = context.opts;
    }

    this.setScope(file);
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

  TraversalPath.prototype.errorWithNode = function errorWithNode(msg) {
    var Error = arguments[1] === undefined ? SyntaxError : arguments[1];

    var loc = this.node.loc.start;
    var err = new Error("Line " + loc.line + ": " + msg);
    err.loc = loc;
    return err;
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
    var _this = this;

    var node = this.node;
    var container = node[key];
    if (Array.isArray(container)) {
      return container.map(function (_, i) {
        return TraversalPath.get(_this, _this.context, node, container, i);
      });
    } else {
      return TraversalPath.get(this, this.context, node, node, key);
    }
  };

  TraversalPath.prototype.has = function has(key) {
    return !!this.node[key];
  };

  TraversalPath.prototype.is = function is(key) {
    return this.has(key);
  };

  TraversalPath.prototype.isnt = function isnt(key) {
    return !this.has(key);
  };

  TraversalPath.prototype.getTypeAnnotation = function getTypeAnnotation() {
    if (this.typeInfo) {
      return this.typeInfo;
    }

    var info = this.typeInfo = {
      inferred: false,
      annotation: null
    };

    var type = this.node.typeAnnotation;

    if (!type) {
      info.inferred = true;
      type = this.inferType(this);
    }

    if (type) {
      if (t.isTypeAnnotation(type)) type = type.typeAnnotation;
      info.annotation = type;
    }

    return info;
  };

  TraversalPath.prototype.resolve = function resolve() {
    if (this.isVariableDeclarator()) {
      if (this.get("id").isIdentifier()) {
        return this.get("init").resolve();
      } else {}
    } else if (this.isIdentifier()) {
      var binding = this.scope.getBinding(this.node.name);
      if (!binding || !binding.constant) return;

      if (binding.path === this) {
        return this;
      } else {
        return binding.path.resolve();
      }
    } else if (this.isMemberExpression()) {
      // this is dangerous, as non-direct target assignments will mutate it's state
      // making this resolution inaccurate

      var targetKey = this.toComputedKey();
      if (!t.isLiteral(targetKey)) return;
      var targetName = targetKey.value;

      var target = this.get("object").resolve();
      if (!target || !target.isObjectExpression()) return;

      var props = target.get("properties");
      for (var i = 0; i < props.length; i++) {
        var prop = props[i];
        if (!prop.isProperty()) continue;

        var key = prop.get("key");

        // { foo: obj }
        var match = prop.isnt("computed") && key.isIdentifier({ name: targetName });

        // { "foo": "obj" } or { ["foo"]: "obj" }
        if (!match) match = key.isLiteral({ value: targetName });

        if (match) return prop.get("value");
      }
    } else {
      return this;
    }
  };

  TraversalPath.prototype.inferType = function inferType(path) {
    path = path.resolve();
    if (!path) return;

    if (path.isRestElement() || path.parentPath.isRestElement() || path.isArrayExpression()) {
      return t.genericTypeAnnotation(t.identifier("Array"));
    }

    if (path.parentPath.isTypeCastExpression()) {
      return path.parentPath.node.typeAnnotation;
    }

    if (path.isTypeCastExpression()) {
      return path.node.typeAnnotation;
    }

    if (path.isObjectExpression()) {
      return t.genericTypeAnnotation(t.identifier("Object"));
    }

    if (path.isFunction()) {
      return t.identifier("Function");
    }

    if (path.isLiteral()) {
      var value = path.node.value;
      if (isString(value)) return t.stringTypeAnnotation();
      if (isNumber(value)) return t.numberTypeAnnotation();
      if (isBoolean(value)) return t.booleanTypeAnnotation();
    }

    if (path.isCallExpression()) {
      var callee = path.get("callee").resolve();
      if (callee && callee.isFunction()) return callee.node.returnType;
    }
  };

  TraversalPath.prototype.isScope = function isScope() {
    return t.isScope(this.node, this.parent);
  };

  TraversalPath.prototype.isReferencedIdentifier = function isReferencedIdentifier(opts) {
    return t.isReferencedIdentifier(this.node, this.parent, opts);
  };

  TraversalPath.prototype.isReferenced = function isReferenced() {
    return t.isReferenced(this.node, this.parent);
  };

  TraversalPath.prototype.isBlockScoped = function isBlockScoped() {
    return t.isBlockScoped(this.node);
  };

  TraversalPath.prototype.isVar = function isVar() {
    return t.isVar(this.node);
  };

  TraversalPath.prototype.isScope = function isScope() {
    return t.isScope(this.node, this.parent);
  };

  TraversalPath.prototype.isTypeGeneric = function isTypeGeneric(genericName) {
    var opts = arguments[1] === undefined ? {} : arguments[1];

    var typeInfo = this.getTypeAnnotation();
    var type = typeInfo.annotation;
    if (!type) return false;

    if (type.inferred && opts.inference === false) {
      return false;
    }

    if (!t.isGenericTypeAnnotation(type) || !t.isIdentifier(type.id, { name: genericName })) {
      return false;
    }

    if (opts.requireTypeParameters && !type.typeParameters) {
      return false;
    }

    return true;
  };

  TraversalPath.prototype.getBindingIdentifiers = function getBindingIdentifiers() {
    return t.getBindingIdentifiers(this.node);
  };

  TraversalPath.prototype.traverse = (function (_traverse) {
    var _traverseWrapper = function traverse(_x, _x2) {
      return _traverse.apply(this, arguments);
    };

    _traverseWrapper.toString = function () {
      return _traverse.toString();
    };

    return _traverseWrapper;
  })(function (opts, state) {
    traverse(this.node, opts, this.scope, state, this);
  });

  /**
   * Match the current node if it matches the provided `pattern`.
   *
   * For example, given the match `React.createClass` it would match the
   * parsed nodes of `React.createClass` and `React["createClass"]`.
   */

  TraversalPath.prototype.matchesPattern = function matchesPattern(pattern, allowPartial) {
    var parts = pattern.split(".");

    // not a member expression
    if (!this.isMemberExpression()) return false;

    var search = [this.node];
    var i = 0;

    while (search.length) {
      var node = search.shift();

      if (allowPartial && i === parts.length) {
        return true;
      }

      if (t.isIdentifier(node)) {
        // this part doesn't match
        if (parts[i] !== node.name) return false;
      } else if (t.isLiteral(node)) {
        // this part doesn't match
        if (parts[i] !== node.value) return false;
      } else if (t.isMemberExpression(node)) {
        if (node.computed && !t.isLiteral(node.property)) {
          // we can't deal with this
          return false;
        } else {
          search.push(node.object);
          search.push(node.property);
          continue;
        }
      } else {
        // we can't deal with this
        return false;
      }

      // too many parts
      if (++i > parts.length) {
        return false;
      }
    }

    return true;
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

assign(TraversalPath.prototype, require("./evaluation"));
assign(TraversalPath.prototype, require("./conversion"));

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

// otherwise it's a request for a destructuring declarator and i'm not
// ready to resolve those just yet