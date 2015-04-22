"use strict";

var _interopRequireWildcard = function (obj) { return obj && obj.__esModule ? obj : { "default": obj }; };

var _interopRequire = function (obj) { return obj && obj.__esModule ? obj["default"] : obj; };

exports.ImportSpecifier = ImportSpecifier;
exports.ExportSpecifier = ExportSpecifier;
exports.ExportBatchSpecifier = ExportBatchSpecifier;
exports.ExportDeclaration = ExportDeclaration;
exports.ImportDeclaration = ImportDeclaration;
exports.ImportBatchSpecifier = ImportBatchSpecifier;
exports.__esModule = true;

var each = _interopRequire(require("lodash/collection/each"));

var t = _interopRequireWildcard(require("../../types"));

function ImportSpecifier(node, print) {
  if (t.isSpecifierDefault(node)) {
    print(t.getSpecifierName(node));
  } else {
    return ExportSpecifier.apply(this, arguments);
  }
}

function ExportSpecifier(node, print) {
  print(node.id);
  if (node.name) {
    this.push(" as ");
    print(node.name);
  }
}

function ExportBatchSpecifier() {
  this.push("*");
}

function ExportDeclaration(node, print) {
  this.push("export ");

  var specifiers = node.specifiers;

  if (node["default"]) {
    this.push("default ");
  }

  if (node.declaration) {
    print(node.declaration);
    if (t.isStatement(node.declaration)) return;
  } else {
    if (specifiers.length === 1 && t.isExportBatchSpecifier(specifiers[0])) {
      print(specifiers[0]);
    } else {
      this.push("{");
      if (specifiers.length) {
        this.space();
        print.join(specifiers, { separator: ", " });
        this.space();
      }
      this.push("}");
    }

    if (node.source) {
      this.push(" from ");
      print(node.source);
    }
  }

  this.ensureSemicolon();
}

function ImportDeclaration(node, print) {
  var _this = this;

  this.push("import ");

  if (node.isType) {
    this.push("type ");
  }

  var specfiers = node.specifiers;
  if (specfiers && specfiers.length) {
    var foundImportSpecifier = false;

    each(node.specifiers, function (spec, i) {
      if (+i > 0) {
        _this.push(", ");
      }

      var isDefault = t.isSpecifierDefault(spec);

      if (!isDefault && spec.type !== "ImportBatchSpecifier" && !foundImportSpecifier) {
        foundImportSpecifier = true;
        _this.push("{ ");
      }

      print(spec);
    });

    if (foundImportSpecifier) {
      this.push(" }");
    }

    this.push(" from ");
  }

  print(node.source);
  this.semicolon();
}

function ImportBatchSpecifier(node, print) {
  this.push("* as ");
  print(node.name);
}