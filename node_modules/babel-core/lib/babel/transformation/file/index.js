"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

var _classCallCheck = function (instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } };

var convertSourceMap = _interopRequire(require("convert-source-map"));

var optionParsers = _interopRequireWildcard(require("./option-parsers"));

var shebangRegex = _interopRequire(require("shebang-regex"));

var TraversalPath = _interopRequire(require("../../traversal/path"));

var isFunction = _interopRequire(require("lodash/lang/isFunction"));

var isAbsolute = _interopRequire(require("path-is-absolute"));

var resolveRc = _interopRequire(require("../../tools/resolve-rc"));

var sourceMap = _interopRequire(require("source-map"));

var transform = _interopRequire(require("./../index"));

var generate = _interopRequire(require("../../generation"));

var defaults = _interopRequire(require("lodash/object/defaults"));

var includes = _interopRequire(require("lodash/collection/includes"));

var traverse = _interopRequire(require("../../traversal"));

var assign = _interopRequire(require("lodash/object/assign"));

var Logger = _interopRequire(require("./logger"));

var parse = _interopRequire(require("../../helpers/parse"));

var Scope = _interopRequire(require("../../traversal/scope"));

var slash = _interopRequire(require("slash"));

var util = _interopRequireWildcard(require("../../util"));

var path = _interopRequire(require("path"));

var each = _interopRequire(require("lodash/collection/each"));

var t = _interopRequireWildcard(require("../../types"));

var checkTransformerVisitor = {
  enter: function enter(node, parent, scope, state) {
    checkNode(state.stack, node, scope);
  }
};

function checkNode(stack, node, scope) {
  each(stack, function (pass) {
    if (pass.shouldRun || pass.ran) return;
    pass.checkNode(node, scope);
  });
}

var File = (function () {
  function File() {
    var opts = arguments[0] === undefined ? {} : arguments[0];

    _classCallCheck(this, File);

    this.dynamicImportedNoDefault = [];
    this.dynamicImportIds = {};
    this.dynamicImported = [];
    this.dynamicImports = [];

    this.usedHelpers = {};
    this.dynamicData = {};
    this.data = {};

    this.lastStatements = [];
    this.log = new Logger(this, opts.filename || "unknown");
    this.opts = this.normalizeOptions(opts);
    this.ast = {};

    this.buildTransformers();
  }

  File.helpers = ["inherits", "defaults", "create-class", "create-computed-class", "apply-constructor", "tagged-template-literal", "tagged-template-literal-loose", "interop-require", "to-array", "to-consumable-array", "sliced-to-array", "object-without-properties", "has-own", "slice", "bind", "define-property", "async-to-generator", "interop-require-wildcard", "typeof", "extends", "get", "set", "class-call-check", "object-destructuring-empty", "temporal-undefined", "temporal-assert-defined", "self-global"];
  File.options = require("./options");

  File.prototype.normalizeOptions = function normalizeOptions(opts) {
    opts = assign({}, opts);

    if (opts.filename && isAbsolute(opts.filename)) {
      opts = resolveRc(opts.filename, opts);
    }

    //

    for (var key in opts) {
      if (key[0] === "_") continue;

      var option = File.options[key];
      if (!option) this.log.error("Unknown option: " + key, ReferenceError);
    }

    for (var key in File.options) {
      var option = File.options[key];

      var val = opts[key];
      if (val == null) val = option["default"] || null;

      var optionParser = optionParsers[option.type];
      if (optionParser) {
        val = optionParser(key, val);
      }

      opts[key] = val;
    }

    if (opts.inputSourceMap) {
      opts.sourceMap = true;
    }

    // normalize windows path separators to unix
    opts.filename = slash(opts.filename);
    if (opts.sourceRoot) {
      opts.sourceRoot = slash(opts.sourceRoot);
    }

    if (opts.moduleId) {
      opts.moduleIds = true;
    }

    opts.basename = path.basename(opts.filename, path.extname(opts.filename));

    opts.ignore = util.arrayify(opts.ignore, util.regexify);
    opts.only = util.arrayify(opts.only, util.regexify);

    defaults(opts, {
      moduleRoot: opts.sourceRoot
    });

    defaults(opts, {
      sourceRoot: opts.moduleRoot
    });

    defaults(opts, {
      filenameRelative: opts.filename
    });

    defaults(opts, {
      sourceFileName: opts.filenameRelative,
      sourceMapName: opts.filenameRelative
    });

    //

    if (opts.externalHelpers) {
      this.set("helpersNamespace", t.identifier("babelHelpers"));
    }

    return opts;
  };

  File.prototype.isLoose = function isLoose(key) {
    return includes(this.opts.loose, key);
  };

  File.prototype.buildPlugins = function buildPlugins(stack) {};

  File.prototype.buildTransformers = function buildTransformers() {
    var file = this;

    var transformers = {};

    var secondaryStack = [];
    var stack = [];

    this.buildPlugins(stack);

    each(transform.transformers, function (transformer, key) {
      var pass = transformers[key] = transformer.buildPass(file);

      if (pass.canTransform()) {
        stack.push(pass);

        if (transformer.metadata.secondPass) {
          secondaryStack.push(pass);
        }

        if (transformer.manipulateOptions) {
          transformer.manipulateOptions(file.opts, file);
        }
      }
    });

    this.transformerStack = stack.concat(secondaryStack);
    this.transformers = transformers;
  };

  File.prototype.getModuleFormatter = function getModuleFormatter(type) {
    var ModuleFormatter = isFunction(type) ? type : transform.moduleFormatters[type];

    if (!ModuleFormatter) {
      var loc = util.resolve(type);
      if (loc) ModuleFormatter = require(loc);
    }

    if (!ModuleFormatter) {
      throw new ReferenceError("Unknown module formatter type " + JSON.stringify(type));
    }

    return new ModuleFormatter(this);
  };

  File.prototype.parseInputSourceMap = function parseInputSourceMap(code) {
    var opts = this.opts;

    if (opts.inputSourceMap !== false) {
      var inputMap = convertSourceMap.fromSource(code);
      if (inputMap) {
        opts.inputSourceMap = inputMap.toObject();
        code = convertSourceMap.removeComments(code);
      }
    }

    return code;
  };

  File.prototype.parseShebang = function parseShebang(code) {
    var shebangMatch = shebangRegex.exec(code);

    if (shebangMatch) {
      this.shebang = shebangMatch[0];

      // remove shebang
      code = code.replace(shebangRegex, "");
    }

    return code;
  };

  File.prototype.set = function set(key, val) {
    return this.data[key] = val;
  };

  File.prototype.setDynamic = function setDynamic(key, fn) {
    this.dynamicData[key] = fn;
  };

  File.prototype.get = function get(key) {
    var data = this.data[key];
    if (data) {
      return data;
    } else {
      var dynamic = this.dynamicData[key];
      if (dynamic) {
        return this.set(key, dynamic());
      }
    }
  };

  File.prototype.addImport = function addImport(source, name, noDefault) {
    if (!name) name = source;

    var id = this.dynamicImportIds[name];

    if (!id) {
      id = this.dynamicImportIds[name] = this.scope.generateUidIdentifier(name);

      var specifiers = [t.importDefaultSpecifier(id)];
      var declar = t.importDeclaration(specifiers, t.literal(source));
      declar._blockHoist = 3;

      this.dynamicImported.push(declar);
      if (noDefault) this.dynamicImportedNoDefault.push(declar);

      if (this.transformers["es6.modules"].canTransform()) {
        this.moduleFormatter.importSpecifier(specifiers[0], declar, this.dynamicImports);
      } else {
        this.dynamicImports.push(declar);
      }
    }

    return id;
  };

  File.prototype.isConsequenceExpressionStatement = function isConsequenceExpressionStatement(node) {
    return t.isExpressionStatement(node) && this.lastStatements.indexOf(node) >= 0;
  };

  File.prototype.attachAuxiliaryComment = function attachAuxiliaryComment(node) {
    var comment = this.opts.auxiliaryComment;
    if (comment) {
      var _node = node;
      if (!_node.leadingComments) _node.leadingComments = [];

      node.leadingComments.push({
        type: "Line",
        value: " " + comment
      });
    }
    return node;
  };

  File.prototype.addHelper = function addHelper(name) {
    if (!includes(File.helpers, name)) {
      throw new ReferenceError("Unknown helper " + name);
    }

    var program = this.ast.program;

    var declar = program._declarations && program._declarations[name];
    if (declar) return declar.id;

    this.usedHelpers[name] = true;

    var generator = this.get("helperGenerator");
    var runtime = this.get("helpersNamespace");
    if (generator) {
      return generator(name);
    } else if (runtime) {
      var id = t.identifier(t.toIdentifier(name));
      return t.memberExpression(runtime, id);
    } else {
      var ref = util.template(name);
      ref._compact = true;
      var uid = this.scope.generateUidIdentifier(name);
      this.scope.push({
        key: name,
        id: uid,
        init: ref
      });
      return uid;
    }
  };

  File.prototype.errorWithNode = function errorWithNode(node, msg) {
    var Error = arguments[2] === undefined ? SyntaxError : arguments[2];

    var loc = node.loc.start;
    var err = new Error("Line " + loc.line + ": " + msg);
    err.loc = loc;
    return err;
  };

  File.prototype.addCode = function addCode(code) {
    code = (code || "") + "";
    code = this.parseInputSourceMap(code);
    this.code = code;
    return this.parseShebang(code);
  };

  File.prototype.shouldIgnore = function shouldIgnore() {
    var opts = this.opts;

    var filename = opts.filename;
    var ignore = opts.ignore;
    var only = opts.only;

    if (only.length) {
      for (var i = 0; i < only.length; i++) {
        if (only[i].test(filename)) return false;
      }
      return true;
    } else if (ignore.length) {
      for (var i = 0; i < ignore.length; i++) {
        if (ignore[i].test(filename)) return true;
      }
    }

    return false;
  };

  File.prototype.parse = (function (_parse) {
    var _parseWrapper = function parse(_x) {
      return _parse.apply(this, arguments);
    };

    _parseWrapper.toString = function () {
      return _parse.toString();
    };

    return _parseWrapper;
  })(function (code) {
    var _this = this;

    if (this.shouldIgnore()) {
      return {
        code: code,
        map: null,
        ast: null
      };
    }

    code = this.addCode(code);

    var opts = this.opts;

    //

    var parseOpts = {
      filename: opts.filename,
      plugins: {}
    };

    var features = parseOpts.features = {};
    for (var key in this.transformers) {
      var transformer = this.transformers[key];
      features[key] = transformer.canTransform();
    }

    parseOpts.looseModules = this.isLoose("es6.modules");
    parseOpts.strictMode = features.strict;

    //

    return parse(parseOpts, code, function (tree) {
      _this.transform(tree);
      return _this.generate();
    });
  });

  File.prototype.setAst = function setAst(ast) {
    this.path = TraversalPath.get(null, null, ast, ast, "program", this);
    this.scope = this.path.scope;
    this.ast = ast;

    this.path.traverse({
      enter: function enter(node, parent, scope) {
        if (this.isScope()) {
          for (var key in scope.bindings) {
            scope.bindings[key].setTypeAnnotation();
          }
        }
      }
    });
  };

  File.prototype.transform = function transform(ast) {
    this.log.debug();

    this.setAst(ast);

    this.lastStatements = t.getLastStatements(ast.program);

    var modFormatter = this.moduleFormatter = this.getModuleFormatter(this.opts.modules);
    if (modFormatter.init && this.transformers["es6.modules"].canTransform()) {
      modFormatter.init();
    }

    this.checkNode(ast);

    this.call("pre");

    each(this.transformerStack, function (pass) {
      pass.transform();
    });

    this.call("post");
  };

  File.prototype.call = function call(key) {
    var stack = this.transformerStack;
    for (var i = 0; i < stack.length; i++) {
      var transformer = stack[i].transformer;
      if (transformer[key]) {
        transformer[key](this);
      }
    }
  };

  File.prototype.checkNode = (function (_checkNode) {
    var _checkNodeWrapper = function checkNode(_x2, _x3) {
      return _checkNode.apply(this, arguments);
    };

    _checkNodeWrapper.toString = function () {
      return _checkNode.toString();
    };

    return _checkNodeWrapper;
  })(function (node, scope) {
    if (Array.isArray(node)) {
      for (var i = 0; i < node.length; i++) {
        this.checkNode(node[i], scope);
      }
      return;
    }

    var stack = this.transformerStack;
    if (!scope) scope = this.scope;

    checkNode(stack, node, scope);

    scope.traverse(node, checkTransformerVisitor, {
      stack: stack
    });
  });

  File.prototype.mergeSourceMap = function mergeSourceMap(map) {
    var opts = this.opts;

    var inputMap = opts.inputSourceMap;

    if (inputMap) {
      map.sources[0] = inputMap.file;

      var inputMapConsumer = new sourceMap.SourceMapConsumer(inputMap);
      var outputMapConsumer = new sourceMap.SourceMapConsumer(map);
      var outputMapGenerator = sourceMap.SourceMapGenerator.fromSourceMap(outputMapConsumer);
      outputMapGenerator.applySourceMap(inputMapConsumer);

      var mergedMap = outputMapGenerator.toJSON();
      mergedMap.sources = inputMap.sources;
      mergedMap.file = inputMap.file;
      return mergedMap;
    }

    return map;
  };

  File.prototype.generate = (function (_generate) {
    var _generateWrapper = function generate() {
      return _generate.apply(this, arguments);
    };

    _generateWrapper.toString = function () {
      return _generate.toString();
    };

    return _generateWrapper;
  })(function () {
    var opts = this.opts;
    var ast = this.ast;

    var result = {
      code: "",
      map: null,
      ast: null
    };

    if (this.opts.returnUsedHelpers) {
      result.usedHelpers = Object.keys(this.usedHelpers);
    }

    if (opts.ast) result.ast = ast;
    if (!opts.code) return result;

    var _result = generate(ast, opts, this.code);
    result.code = _result.code;
    result.map = _result.map;

    if (this.shebang) {
      // add back shebang
      result.code = "" + this.shebang + "\n" + result.code;
    }

    result.map = this.mergeSourceMap(result.map);

    if (opts.sourceMap === "inline" || opts.sourceMap === "both") {
      result.code += "\n" + convertSourceMap.fromObject(result.map).toComment();
    }

    if (opts.sourceMap === "inline") {
      result.map = null;
    }

    return result;
  });

  return File;
})();

module.exports = File;