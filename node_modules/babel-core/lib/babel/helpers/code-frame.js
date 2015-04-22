"use strict";

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

var lineNumbers = _interopRequire(require("line-numbers"));

var repeating = _interopRequire(require("repeating"));

var jsTokens = _interopRequire(require("js-tokens"));

var esutils = _interopRequire(require("esutils"));

var chalk = _interopRequire(require("chalk"));

var ary = _interopRequire(require("lodash/function/ary"));

var defs = {
  string: chalk.red,
  punctuator: chalk.white.bold,
  curly: chalk.green,
  parens: chalk.blue.bold,
  square: chalk.yellow,
  name: chalk.white,
  keyword: chalk.cyan,
  number: chalk.magenta,
  regex: chalk.magenta,
  comment: chalk.grey,
  invalid: chalk.inverse
};

var newline = /\r\n|[\n\r\u2028\u2029]/;

var highlight = function highlight(text) {
  var tokenType = function tokenType(match) {
    var token = jsTokens.matchToToken(match);
    if (token.type === "name" && esutils.keyword.isReservedWordES6(token.value)) {
      return "keyword";
    }

    if (token.type === "punctuator") {
      switch (token.value) {
        case "{":
        case "}":
          return "curly";
        case "(":
        case ")":
          return "parens";
        case "[":
        case "]":
          return "square";
      }
    }

    return token.type;
  };

  return text.replace(jsTokens, function (match) {
    var type = tokenType(arguments);
    if (type in defs) {
      var colorize = ary(defs[type], 1);
      return match.split(newline).map(colorize).join("\n");
    }
    return match;
  });
};

module.exports = function (lines, lineNumber, colNumber) {
  colNumber = Math.max(colNumber, 0);

  if (chalk.supportsColor) {
    lines = highlight(lines);
  }

  lines = lines.split(newline);

  var start = Math.max(lineNumber - 3, 0);
  var end = Math.min(lines.length, lineNumber + 3);

  if (!lineNumber && !colNumber) {
    start = 0;
    end = lines.length;
  }

  return "\n" + lineNumbers(lines.slice(start, end), {
    start: start + 1,
    before: "  ",
    after: " | ",
    transform: function transform(params) {
      if (params.number !== lineNumber) {
        return;
      }
      if (colNumber) {
        params.line += "\n" + params.before + "" + repeating(" ", params.width) + "" + params.after + "" + repeating(" ", colNumber - 1) + "^";
      }
      params.before = params.before.replace(/^./, ">");
    }
  }).join("\n");
};