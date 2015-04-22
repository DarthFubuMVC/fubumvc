"use strict";

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

exports.save = save;
exports.load = load;
exports.get = get;
exports.__esModule = true;

var path = _interopRequire(require("path"));

var os = _interopRequire(require("os"));

var fs = _interopRequire(require("fs"));

var FILENAME = process.env.BABEL_CACHE_PATH || path.join(os.tmpdir(), "babel.json");
var data = {};

function save() {
  fs.writeFileSync(FILENAME, JSON.stringify(data, null, "  "));
}

function load() {
  if (process.env.BABEL_DISABLE_CACHE) return;

  process.on("exit", save);

  var sigint = (function (_sigint) {
    var _sigintWrapper = function sigint() {
      return _sigint.apply(this, arguments);
    };

    _sigintWrapper.toString = function () {
      return _sigint.toString();
    };

    return _sigintWrapper;
  })(function () {
    process.removeListener("SIGINT", sigint);
    save();
    process.kill(process.pid, "SIGINT");
  });

  process.on("SIGINT", sigint);

  if (!fs.existsSync(FILENAME)) return;

  try {
    data = JSON.parse(fs.readFileSync(FILENAME));
  } catch (err) {
    return;
  }
}

function get() {
  return data;
}