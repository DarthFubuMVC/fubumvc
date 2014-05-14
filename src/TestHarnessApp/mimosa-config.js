exports.config = {
  "modules": [
    "copy",
    "server",
    "jshint",
    "csslint",
    "require",
    "minify-js",
    "minify-css",
    "live-reload",
    "bower",
    "less"
  ],
  "server": {
    "path": "server.js",
    "views": {
      "compileWith": "html",
      "extension": "html"
    }
  }
}