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

module.exports = ActiveSectionView;