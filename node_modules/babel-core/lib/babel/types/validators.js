"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

/**
 * Check if the input `node` is a reference to a bound variable.
 */

exports.isReferenced = isReferenced;

/**
 * Check if the input `node` is an `Identifier` and `isReferenced`.
 */

exports.isReferencedIdentifier = isReferencedIdentifier;

/**
 * Check if the input `name` is a valid identifier name
 * and isn't a reserved word.
 */

exports.isValidIdentifier = isValidIdentifier;

/**
 * Description
 */

exports.isLet = isLet;

/**
 * Description
 */

exports.isBlockScoped = isBlockScoped;

/**
 * Description
 */

exports.isVar = isVar;

/**
 * Description
 */

exports.isSpecifierDefault = isSpecifierDefault;

/**
 * Description
 */

exports.isScope = isScope;

/**
 * Description
 */

exports.isImmutable = isImmutable;
exports.__esModule = true;

var isString = _interopRequire(require("lodash/lang/isString"));

var esutils = _interopRequire(require("esutils"));

var t = _interopRequireWildcard(require("./index"));

function isReferenced(node, parent) {
  // yes: PARENT[NODE]
  // yes: NODE.child
  // no: parent.CHILD
  if (t.isMemberExpression(parent)) {
    if (parent.property === node && parent.computed) {
      return true;
    } else if (parent.object === node) {
      return true;
    } else {
      return false;
    }
  }

  // yes: { [NODE]: "" }
  // no: { NODE: "" }
  if (t.isProperty(parent) && parent.key === node) {
    return parent.computed;
  }

  // no: var NODE = init;
  // yes: var id = NODE;
  if (t.isVariableDeclarator(parent)) {
    return parent.id !== node;
  }

  // no: function NODE() {}
  // no: function foo(NODE) {}
  if (t.isFunction(parent)) {
    for (var i = 0; i < parent.params.length; i++) {
      var param = parent.params[i];
      if (param === node) return false;
    }

    return parent.id !== node;
  }

  // no: export { foo as NODE };
  if (t.isExportSpecifier(parent, { name: node })) {
    return false;
  }

  // no: import { NODE as foo } from "foo";
  if (t.isImportSpecifier(parent, { id: node })) {
    return false;
  }

  // no: class NODE {}
  if (t.isClass(parent)) {
    return parent.id !== node;
  }

  // yes: class { [NODE](){} }
  if (t.isMethodDefinition(parent)) {
    return parent.key === node && parent.computed;
  }

  // no: NODE: for (;;) {}
  if (t.isLabeledStatement(parent)) {
    return false;
  }

  // no: try {} catch (NODE) {}
  if (t.isCatchClause(parent)) {
    return parent.param !== node;
  }

  // no: function foo(...NODE) {}
  if (t.isRestElement(parent)) {
    return false;
  }

  // no: [NODE = foo] = [];
  // yes: [foo = NODE] = [];
  if (t.isAssignmentPattern(parent)) {
    return parent.right === node;
  }

  // no: [NODE] = [];
  // no: ({ NODE }) = [];
  if (t.isPattern(parent)) {
    return false;
  }

  // no: import NODE from "bar";
  if (t.isImportSpecifier(parent)) {
    return false;
  }

  // no: import * as NODE from "foo";
  if (t.isImportBatchSpecifier(parent)) {
    return false;
  }

  // no: class Foo { private NODE; }
  if (t.isPrivateDeclaration(parent)) {
    return false;
  }

  return true;
}

function isReferencedIdentifier(node, parent, opts) {
  return t.isIdentifier(node, opts) && t.isReferenced(node, parent);
}

function isValidIdentifier(name) {
  return isString(name) && esutils.keyword.isIdentifierName(name) && !esutils.keyword.isReservedWordES6(name, true);
}

function isLet(node) {
  return t.isVariableDeclaration(node) && (node.kind !== "var" || node._let);
}

function isBlockScoped(node) {
  return t.isFunctionDeclaration(node) || t.isClassDeclaration(node) || t.isLet(node);
}

function isVar(node) {
  return t.isVariableDeclaration(node, { kind: "var" }) && !node._let;
}

function isSpecifierDefault(specifier) {
  return specifier["default"] || t.isIdentifier(specifier.id) && specifier.id.name === "default";
}

function isScope(node, parent) {
  if (t.isBlockStatement(node)) {
    if (t.isLoop(parent.block, { body: node })) {
      return false;
    }

    if (t.isFunction(parent.block, { body: node })) {
      return false;
    }
  }

  return t.isScopable(node);
}

function isImmutable(node) {
  if (t.isLiteral(node)) {
    if (node.regex) {
      // regexes are mutable
      return false;
    } else {
      // immutable!
      return true;
    }
  } else if (t.isIdentifier(node)) {
    if (node.name === "undefined") {
      // immutable!
      return true;
    } else {
      // no idea...
      return false;
    }
  }

  return false;
}