/** @jsx React.DOM */

var React = require('react');
var _ = require('lodash');
var $ = require('jquery');

var Router = require('react-router'); // or var Router = ReactRouter; in browsers
var Route = Router.Route, DefaultRoute = Router.DefaultRoute,
  Link=Router.Link, RouteHandler = Router.RouteHandler;

_.assign(FubuDiagnostics, {
    cache: {},

    get: function (key, params, callback) {
        var url = this.toUrl(key, params);

        $.get(url, callback);
    },

    toUrl: function (key, params) {
        var route = this.routes[key];
        var url = route.url;
        _.each(route.params, function (param) {
            url = url.replace('{' + param + '}', params[param]);
        });

        return url;
    },

    // TODO -- add cached ability
});



var TextScreen = React.createClass({
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

require('./appdomain');
require('./assets');


FubuDiagnostics.addSection({    
    title: 'FubuMVC',
    description: 'Core Diagnostics for FubuMVC Applications',
    key: 'fubumvc'
}).add({
	title: 'Model Binding',
	description: 'something about Model Binding',
	key: 'model-binding',
	component: require('./model-binding')
});

/*
.add({
	title: 'Package Loading',
	description: 'something about Package Loading',
	key: 'package-loading',
	component: (<HtmlScreen route="PackageLog:package_logs"/> )
});
*/



var Shell = require('./shell');

$(document).ready(() => Shell.start());
