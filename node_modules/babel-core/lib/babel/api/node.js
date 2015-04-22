"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

exports.register = register;
exports.polyfill = polyfill;
exports.transformFile = transformFile;
exports.transformFileSync = transformFileSync;
exports.__esModule = true;

var isFunction = _interopRequire(require("lodash/lang/isFunction"));

var _transformation = require("../transformation");

var transform = _interopRequire(_transformation);

var fs = _interopRequire(require("fs"));

var _util = require("../util");

var util = _interopRequireWildcard(_util);

exports._util = util;
exports.canCompile = _util.canCompile;
exports.acorn = _interopRequire(require("acorn-babel"));
exports.transform = _interopRequire(_transformation);
exports.traverse = _interopRequire(require("../traversal"));
exports.buildExternalHelpers = _interopRequire(require("../tools/build-external-helpers"));
exports.version = require("../../../package").version;

var t = _interopRequireWildcard(require("../types"));

exports.types = t;

function register(opts) {
  var callback = require("./register/node");
  if (opts != null) callback(opts);
  return callback;
}

function polyfill() {
  require("../polyfill");
}

function transformFile(filename, opts, callback) {
  if (isFunction(opts)) {
    callback = opts;
    opts = {};
  }

  opts.filename = filename;

  fs.readFile(filename, function (err, code) {
    if (err) return callback(err);

    var result;

    try {
      result = transform(code, opts);
    } catch (err) {
      return callback(err);
    }

    callback(null, result);
  });
}

function transformFileSync(filename) {
  var opts = arguments[1] === undefined ? {} : arguments[1];

  opts.filename = filename;
  return transform(fs.readFileSync(filename), opts);
}