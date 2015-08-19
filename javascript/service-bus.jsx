/** @jsx React.DOM */

var React = require('react');

var HtmlScreen = require('./html-screen');

var ChannelGraph = React.createClass({
	render: function(){
		return (
			<HtmlScreen route="ChannelGraph:channels" />

		);
	}
});

var Subscriptions = React.createClass({
	render: function(){
		return (
			<HtmlScreen route="Subscriptions:subscriptions" />
		);
	}
});

var Messages = React.createClass({
	render: function(){
		return (
			<HtmlScreen route="Messages:messages" />
		);
	}
});

var Tasks = React.createClass({
	render: function(){
		return (
			<HtmlScreen route="Tasks:tasks" />
		);
	}
})

FubuDiagnostics.addSection({
	title: 'Messaging',
	description: 'The configuration and state of the messaging features',
	key: 'servicebus'
}).add({
	title: 'Service Bus',
	description: 'Everything about this service bus node',
	key: 'channelgraph',
	component: ChannelGraph
}).add({
	title: 'Subscriptions',
	description: 'The current state and configuration of the subscription storage',
	key: 'subscriptions',
	component: Subscriptions
}).add({
	title: 'Message Handlers',
	description: 'All of the message handlers in this application',
	key: 'message-handlers',
	component: Messages
}).add({
	title: 'Permanent Tasks',
	description: 'Current state of the persistent tasks in the service bus',
	key: 'persistent-tasks',
	component: Tasks
});