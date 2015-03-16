/** @jsx React.DOM */

var React = require('react');


var Router = require('react-router'); // or var Router = ReactRouter; in browsers
var Route = Router.Route, DefaultRoute = Router.DefaultRoute,
  Link=Router.Link, RouteHandler = Router.RouteHandler;

var SectionLinks = require('./section-links');


class FubuDiagnosticsView {
	constructor(view, section){
		this.url = section.key + '/' + view.key;
		this.key = view.key;

		var routeName = section.key + ':' + view.key;

		
		this.anchor = '#' + this.url;
		this.hasParameters = false;
		this.description = view.description;
		this.title = view.title;


		var component = view.component;
		if (view.hasOwnProperty('render')){
			component = React.createClass({
				render: view.render
			});
		}

		if (!component){
			throw new Error("You need to either specify a React in view.component or pass in a render() function");
		}

		this.route = (<Route name={routeName} path={this.url} handler={component}/>);
		
		if (view.route){
			var route = FubuDiagnostics.routes[view.route];
			if (route.params.length > 0){
				this.hasParameters = true;
				route.params.forEach(x => {
					this.url = this.url + '/:' + x.Name;
				});
			}
		}

	}
}

class FubuDiagnosticsSection {
	constructor(section){
		this.title = section.title;
		this.description = section.description;
		this.key = section.key;
		
		this.url = '/' + this.key;
		this.views = [];
		this.anchor = '#' + this.key;
	}
	
	add(data){
		var view = new FubuDiagnosticsView(data, this);
		this.views.push(view);

		return this;
	}
	
	activeViews(){
		return this.views.filter(v => !v.hasParameters);
	}
	
	toRoutes(){
		return this.views.map(view => view.route);
	}
}

module.exports = FubuDiagnosticsSection;