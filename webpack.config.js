
module.exports = {
  entry: {
    "root": './javascript/root.jsx',
	"lq": './javascript/lightning-queues.jsx'
  },
  output: {
    path: __dirname + '/src/DiagnosticsHarness/fubu-diagnostics',
    //path: __dirname + '/src/FubuMVC.Core/fubu-diagnostics',
    filename: "[name].js",
    publicPath: '/client/public/javascript/',
    pathinfo: true
  },
  resolve: {
    // Allow to omit extensions when requiring these files
    extensions: ['', '.js', '.jsx']
  },
  module: {
    loaders: [
      { test: /\.css$/, loader: "style!css" },
      { test: /\.jsx$/, loader: 'jsx?harmony'},
      { test: /\.js$/, exclude: /(node_modules)|(-data\.js$)/, loader: "babel"}
    ]
  },
  externals: {
      // require("jquery") is external and available
      //  on the global var jQuery
      "jquery": "jQuery"
  },

  devtool: 'eval'
}
