"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _classCallCheck = function (instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } };

var util = _interopRequireWildcard(require("../util"));

var Logger = (function () {
  function Logger(file) {
    _classCallCheck(this, Logger);

    this.filename = file.opts.filename;
    this.file = file;
  }

  Logger.prototype._buildMessage = function _buildMessage(msg) {
    var parts = this.filename;
    if (msg) parts += ": " + msg;
    return parts;
  };

  Logger.prototype.debug = function debug(msg) {
    util.debug(this._buildMessage(msg));
  };

  Logger.prototype.deopt = function deopt(node, msg) {
    util.debug(this._buildMessage(msg));
  };

  return Logger;
})();

module.exports = Logger;