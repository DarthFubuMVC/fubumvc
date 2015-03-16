/** @jsx React.DOM */

var React = require('react');
var _ = require('lodash');
var $ = require('jquery');

var Router = require('react-router'); // or var Router = ReactRouter; in browsers
var Route = Router.Route, DefaultRoute = Router.DefaultRoute,
  Link=Router.Link, RouteHandler = Router.RouteHandler;

FubuDiagnostics.HtmlScreen = React.createClass({
	getInitialState: function(){
		return {
			html: 'Loading...'
		}
	},

	componentDidMount: function(){
		// TODO -- add parameters into this someday
		FubuDiagnostics.get(this.props.route, {}, data => {
			this.setState({html: data});
		});
	},

	render: function(){
		return (
			<div dangerouslySetInnerHTML={this.state.html}></div>
		);
	}
});

FubuDiagnostics.TextScreen = React.createClass({
	getInitialState: function(){
		return {
			text: 'Loading...'
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get(route, {}, data => {
			this.setState({text: data});
		});
	},

	render: function(){
		return (
			<pre>{this.state.text}</pre>
		);
	}
});

var FubuDiagnosticsSection = require('./fubu-diagnostics-section');

FubuDiagnostics.sections = [];
FubuDiagnostics.addSection = function(data){
	var section = new FubuDiagnosticsSection(data);
	this.sections.push(section);

	return section;
}


FubuDiagnostics.addSection({    
    title: 'FubuMVC',
    description: 'Core Diagnostics for FubuMVC Applications',
    key: 'fubumvc'
});



var Shell = require('./shell');

$(document).ready(() => Shell.start());
