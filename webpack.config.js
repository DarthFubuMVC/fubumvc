
module.exports = {
  entry: {
    "root": ['./javascript/root.js'],
  },
  output: {
    path: __dirname + '/src/FubuMVC.Core/fubu-diagnostics',
    filename: "[name].js",
    publicPath: '/client/public/javascript/',
    pathinfo: true,
	libraryTarget: 'var',
	library: 'FubuDiagnostics'
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
  devtool: 'eval'
}
