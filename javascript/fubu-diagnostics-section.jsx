/** @jsx React.DOM */

var React = require('react');


var Router = require('react-router'); // or var Router = ReactRouter; in browsers
var Route = Router.Route, DefaultRoute = Router.DefaultRoute,
  Link=Router.Link, RouteHandler = Router.RouteHandler;

var SectionLinks = require('./section-links');


class FubuDiagnosticsSection {
	constructor(section){
		this.title = section.title;
		this.description = section.description;
		this.key = section.key;
		
		this.url = '/' + this.key;
		this.views = [];
		this.anchor = '#' + this.key;

		this.component = section.component || require('./active-section-view');
	}
	
	add(data){
		var FubuDiagnosticsView = require('./fubu-diagnostics-view');
		var view = new FubuDiagnosticsView(data, this);
		this.views.push(view);

		return this;
	}
	
	activeViews(){
		return this.views.filter(v => !v.hasParameters);
	}
	
	toRoutes(){
		var React = require('react'); // workaround for babel+webpack problem

		var Router = require('react-router'); // or var Router = ReactRouter; in browsers
		var Route = Router.Route;

		var routes = this.views.map(view => view.route);
		if (this.component){
			var sectionRoute = (<Route name={this.key} path={this.url} handler={this.component} />);
			routes.push(sectionRoute);
		}

		return routes;
	}
}

module.exports = FubuDiagnosticsSection;