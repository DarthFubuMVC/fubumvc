/** @jsx React.DOM */

var React = require('react');


var Router = require('react-router'); // or var Router = ReactRouter; in browsers
var Route = Router.Route, DefaultRoute = Router.DefaultRoute,
  Link=Router.Link, RouteHandler = Router.RouteHandler;

class FubuDiagnosticsView {
	constructor(view, section){
		this.url = section.key + '/' + view.key;
		this.key = view.key;
		this.anchor = '#' + this.url;
		this.hasParameters = false;
		this.component = view.component;
		this.description = view.description;
		this.title = view.title;
		
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
	
	add(view){
		this.views.push(new FubuDiagnosticsView(view, this));
	}
	
	activeViews(){
		return this.views.filter(v => !v.hasParameters);
	}
	
	toRoutes(){
		var handler = (<SectionLinks section={this}/>);
		return [ ( <Route key={this.key} path={this.key} handler={handler}/> ) ];
	}
}

module.exports = FubuDiagnosticsSection;