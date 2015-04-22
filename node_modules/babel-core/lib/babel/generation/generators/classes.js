"use strict";

exports.ClassDeclaration = ClassDeclaration;
exports.ClassBody = ClassBody;
exports.MethodDefinition = MethodDefinition;
exports.__esModule = true;

function ClassDeclaration(node, print) {
  this.push("class");

  if (node.id) {
    this.space();
    print(node.id);
  }

  print(node.typeParameters);

  if (node.superClass) {
    this.push(" extends ");
    print(node.superClass);
    print(node.superTypeParameters);
  }

  if (node["implements"]) {
    this.push(" implements ");
    print.join(node["implements"], { separator: ", " });
  }

  this.space();
  print(node.body);
}

exports.ClassExpression = ClassDeclaration;

function ClassBody(node, print) {
  if (node.body.length === 0) {
    this.push("{}");
  } else {
    this.push("{");
    this.newline();

    this.indent();
    print.sequence(node.body);
    this.dedent();

    this.rightBrace();
  }
}

function MethodDefinition(node, print) {
  if (node["static"]) {
    this.push("static ");
  }

  this._method(node, print);
}