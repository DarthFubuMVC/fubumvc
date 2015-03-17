/** @jsx React.DOM */

var React = require('react');


var Router = require('react-router'); // or var Router = ReactRouter; in browsers
var Route = Router.Route, DefaultRoute = Router.DefaultRoute,
  Link=Router.Link, RouteHandler = Router.RouteHandler;

var SectionLinks = require('./section-links');


var ActiveSectionView = React.createClass({
	mixins: [ Router.State ],

	render: function(){
		var path = this.getPathname();
		var sectionKey = path.split('/')[1];
		var activeSection = FubuDiagnostics.section(sectionKey);



		return (
			<div style={{marginLeft: '300px'}}>
				<h2>{activeSection.title} <small>{activeSection.description}</small></h2>
				<SectionLinks section={activeSection} />
			</div>
		);
	}
});

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


		if (view.route){
			var route = FubuDiagnostics.routes[view.route];
			if (route.params.length > 0){
				this.hasParameters = true;
				route.params.forEach(x => {
					this.url = this.url + '/:' + x.Name;
				});
			}
		}

		this.route = (<Route name={routeName} path={this.url} handler={component}/>);
		

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

		this.component = section.component || ActiveSectionView;
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
		var routes = this.views.map(view => view.route);
		if (this.component){
			var sectionRoute = (<Route name={this.key} path={this.url} handler={this.component} />);
			routes.push(sectionRoute);
		}

		return routes;
	}
}

module.exports = FubuDiagnosticsSection;