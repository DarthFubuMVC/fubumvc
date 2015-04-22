"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

/**
 * Return a list of binding identifiers associated with
 * the input `node`.
 */

exports.getBindingIdentifiers = getBindingIdentifiers;

/**
 * Description
 */

exports.getLastStatements = getLastStatements;

/**
 * Description
 */

exports.getSpecifierName = getSpecifierName;

/**
 * Description
 */

exports.getSpecifierId = getSpecifierId;
exports.__esModule = true;

var object = _interopRequire(require("../helpers/object"));

var t = _interopRequireWildcard(require("./index"));

function getBindingIdentifiers(node) {
  var search = [].concat(node);
  var ids = object();

  while (search.length) {
    var id = search.shift();
    if (!id) continue;

    var keys = t.getBindingIdentifiers.keys[id.type];

    if (t.isIdentifier(id)) {
      ids[id.name] = id;
    } else if (t.isImportSpecifier(id)) {
      search.push(id.name || id.id);
    } else if (t.isExportDeclaration(id)) {
      if (t.isDeclaration(node.declaration)) {
        search.push(node.declaration);
      }
    } else if (keys) {
      for (var i = 0; i < keys.length; i++) {
        var key = keys[i];
        search = search.concat(id[key] || []);
      }
    }
  }

  return ids;
}

getBindingIdentifiers.keys = {
  UnaryExpression: ["argument"],
  AssignmentExpression: ["left"],
  ImportBatchSpecifier: ["name"],
  VariableDeclarator: ["id"],
  FunctionDeclaration: ["id"],
  FunctionExpression: ["id"],
  ClassDeclaration: ["id"],
  ClassExpression: ["id"],
  SpreadElement: ["argument"],
  RestElement: ["argument"],
  UpdateExpression: ["argument"],
  SpreadProperty: ["argument"],
  Property: ["value"],
  ComprehensionBlock: ["left"],
  AssignmentPattern: ["left"],
  PrivateDeclaration: ["declarations"],
  ComprehensionExpression: ["blocks"],
  ImportDeclaration: ["specifiers"],
  VariableDeclaration: ["declarations"],
  ArrayPattern: ["elements"],
  ObjectPattern: ["properties"]
};
function getLastStatements(node) {
  var nodes = [];

  var add = function add(node) {
    nodes = nodes.concat(getLastStatements(node));
  };

  if (t.isIfStatement(node)) {
    add(node.consequent);
    add(node.alternate);
  } else if (t.isFor(node) || t.isWhile(node)) {
    add(node.body);
  } else if (t.isProgram(node) || t.isBlockStatement(node)) {
    add(node.body[node.body.length - 1]);
  } else if (node) {
    nodes.push(node);
  }

  return nodes;
}

function getSpecifierName(specifier) {
  return specifier.name || specifier.id;
}

function getSpecifierId(specifier) {
  if (specifier["default"]) {
    return t.identifier("default");
  } else {
    return specifier.id;
  }
}