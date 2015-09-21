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
        $.ajax({
            method: 'GET',
            url: url,
            dataType: 'json',
            accept: 'application/json',
            success: function(data){
                try {
                    callback(data);
                }
                catch (e){
                    console.error(e);
                }
            },
            error: function(jqXHR, textStatus, errorThrown){
                console.error(errorThrown);
            }
        });
    },

     getText: function (key, params, callback) {
        var url = this.toUrl(key, params);
        $.ajax({
            method: 'GET',
            url: url,
            dataType: 'text',
            accept: 'text/plain',
            success: function(data){
                try {
                    callback(data);
                }
                catch (e){
                    console.error(e);
                }
            },
            error: function(jqXHR, textStatus, errorThrown){
                console.error(errorThrown);
            }
        });
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
	},

	TextScreen: require('./text-screen'),
	HtmlScreen: require('./html-screen'),
	React: React
});




require('./appdomain');
require('./assets');
require('./structuremap');
require('./service-bus');
require('./message-table');
require('./polling-jobs');


FubuDiagnostics.addSection({    
    title: 'HTTP',
    description: 'Core Diagnostics for HTTP Requests and Handlers',
    key: 'fubumvc'
}).add({
	title: 'Model Binding',
	description: 'All the configured model binding converters, property binders, and custom model binders',
	key: 'model-binding',
	component: require('./model-binding')
});

require('./endpoint-explorer');
require('./partial-explorer');
require('./chain-details');
require('./request-table.jsx');
require('./request-details.jsx');
require('./packaging');
require('./client-messages');
require('./performance');


var Shell = require('./shell');

$(document).ready(() => Shell.start());
