/** @jsx React.DOM */

var React = require('react');


var Router = require('react-router'); // or var Router = ReactRouter; in browsers
var Route = Router.Route, DefaultRoute = Router.DefaultRoute,
  Link=Router.Link, RouteHandler = Router.RouteHandler;

var Header = require('./header');
var Dashboard = require('./dashboard');


var App = React.createClass({
  mixins: [Router.State],

  getHandlerKey: function () {
    return 1;
  },

	render: function(){
		style = {
			paddingLeft: '25px'
		}
	
		return (
			<div className="container-fluid">
				<div className="row">
					<Header />
				</div>
				<div className="row"  style={style}>
					<RouteHandler key={this.getHandlerKey()}/>
				</div>
			</div>
		);
	}
});



var routes = (
  <Route name="app" path="/" handler={App}>
    <DefaultRoute handler={Dashboard}/>
  </Route>
);


module.exports = {
	start: function(){
		Router.run(routes, function (Handler) {
		  React.render(<Handler/>, document.body);
		});
	}
}

