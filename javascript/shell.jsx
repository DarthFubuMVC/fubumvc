/** @jsx React.DOM */

var React = require('react');


var Router = require('react-router'); // or var Router = ReactRouter; in browsers
var Route = Router.Route, DefaultRoute = Router.DefaultRoute,
  Link=Router.Link, RouteHandler = Router.RouteHandler;

var Header = require('./header');



var App = React.createClass({
  mixins: [Router.State],

  getHandlerKey: function () {
    return 1;
  },

	render: function(){
		return (
			<div className="container">
				<div className="row">
					<Header />
				</div>
				<div className="row">
					<RouteHandler key={this.getHandlerKey()}/>
				</div>
			</div>
		);
	}
});

var Dashboard = React.createClass({
	render: function(){
		return (<h1>The Dashboard</h1>);
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

