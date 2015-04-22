"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

exports.canCompile = canCompile;
exports.resolve = resolve;
exports.list = list;
exports.regexify = regexify;
exports.arrayify = arrayify;
exports.booleanify = booleanify;

//

exports.template = template;
exports.parseTemplate = parseTemplate;
exports.__esModule = true;

require("./patch");

var buildDebug = _interopRequire(require("debug/node"));

var cloneDeep = _interopRequire(require("lodash/lang/cloneDeep"));

var isBoolean = _interopRequire(require("lodash/lang/isBoolean"));

var messages = _interopRequireWildcard(require("./messages"));

var contains = _interopRequire(require("lodash/collection/contains"));

var traverse = _interopRequire(require("./traversal"));

var isString = _interopRequire(require("lodash/lang/isString"));

var isRegExp = _interopRequire(require("lodash/lang/isRegExp"));

var isEmpty = _interopRequire(require("lodash/lang/isEmpty"));

var parse = _interopRequire(require("./helpers/parse"));

var path = _interopRequire(require("path"));

var each = _interopRequire(require("lodash/collection/each"));

var has = _interopRequire(require("lodash/object/has"));

var fs = _interopRequire(require("fs"));

var t = _interopRequireWildcard(require("./types"));

var _util = require("util");

exports.inherits = _util.inherits;
exports.inspect = _util.inspect;
var debug = buildDebug("babel");

exports.debug = debug;

function canCompile(filename, altExts) {
  var exts = altExts || canCompile.EXTENSIONS;
  var ext = path.extname(filename);
  return contains(exts, ext);
}

canCompile.EXTENSIONS = [".js", ".jsx", ".es6", ".es"];

function resolve(loc) {
  try {
    return require.resolve(loc);
  } catch (err) {
    return null;
  }
}

function list(val) {
  return val ? val.split(",") : [];
}

function regexify(val) {
  if (!val) return new RegExp(/.^/);
  if (Array.isArray(val)) val = val.join("|");
  if (isString(val)) return new RegExp(val);
  if (isRegExp(val)) return val;
  throw new TypeError("illegal type for regexify");
}

function arrayify(val) {
  if (!val) return [];
  if (isBoolean(val)) return [val];
  if (isString(val)) return list(val);
  if (Array.isArray(val)) return val;
  throw new TypeError("illegal type for arrayify");
}

function booleanify(val) {
  if (val === "true") return true;
  if (val === "false") return false;
  return val;
}

var templateVisitor = {
  enter: function enter(node, parent, scope, nodes) {
    if (t.isExpressionStatement(node)) {
      node = node.expression;
    }
    if (t.isIdentifier(node) && has(nodes, node.name)) {
      this.skip();
      return nodes[node.name];
    }
  }
};
function template(name, nodes, keepExpression) {
  var ast = exports.templates[name];
  if (!ast) throw new ReferenceError("unknown template " + name);

  if (nodes === true) {
    keepExpression = true;
    nodes = null;
  }

  ast = cloneDeep(ast);

  if (!isEmpty(nodes)) {
    traverse(ast, templateVisitor, null, nodes);
  }

  if (ast.body.length > 1) return ast.body;

  var node = ast.body[0];

  if (!keepExpression && t.isExpressionStatement(node)) {
    return node.expression;
  } else {
    return node;
  }
}

function parseTemplate(loc, code) {
  var ast = parse({ filename: loc }, code).program;
  ast = traverse.removeProperties(ast);
  return ast;
}

function loadTemplates() {
  var templates = {};

  var templatesLoc = path.join(__dirname, "transformation/templates");
  if (!fs.existsSync(templatesLoc)) {
    throw new ReferenceError(messages.get("missingTemplatesDirectory"));
  }

  each(fs.readdirSync(templatesLoc), function (name) {
    if (name[0] === ".") return;

    var key = path.basename(name, path.extname(name));
    var loc = path.join(templatesLoc, name);
    var code = fs.readFileSync(loc, "utf8");

    templates[key] = parseTemplate(loc, code);
  });

  return templates;
}

try {
  exports.templates = require("../../templates.json");
} catch (err) {
  if (err.code !== "MODULE_NOT_FOUND") throw err;
  exports.templates = loadTemplates();
}