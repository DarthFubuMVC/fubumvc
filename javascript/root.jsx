/** @jsx React.DOM */

var React = require('react');
var _ = require('lodash');
var $ = require('jquery');

var Router = require('react-router'); // or var Router = ReactRouter; in browsers
var Route = Router.Route, DefaultRoute = Router.DefaultRoute,
  Link=Router.Link, RouteHandler = Router.RouteHandler;



var FubuDiagnosticsSection = require('./fubu-diagnostics-section');


_.assign(FubuDiagnostics, {
    cache: {},

    // TODO -- cache if there are no params?
    get: function (key, params, callback) {
        var url = this.toUrl(key, params);
        $.get(url, callback);
    },

    toUrl: function (key, params) {
        var route = this.routes[key];
        var url = route.url;
        _.each(route.params, function (param) {
            url = url.replace('{' + param.Name + '}', params[param.Name]);
        });

        return url;
    },

    sections: [],

    addSection: function(data){
		var section = new FubuDiagnosticsSection(data);
		this.sections.push(section);

		return section;
	},

	section: function(key){
		return _.find(this.sections, s => s.key == key);
	}
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

require('./endpoint-explorer');
require('./chain-details');
require('./request-table.jsx');
require('./request-details.jsx');
require('./packaging');




var Shell = require('./shell');

$(document).ready(() => Shell.start());
