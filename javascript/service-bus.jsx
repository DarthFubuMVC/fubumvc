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
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get('Messages:messages', {}, data => {
			this.setState({messages: data, loading: false});
		});
	},

	render(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var rows = this.state.messages.map(msg => {
			var url = "#/fubumvc/chain-details/" + msg.hash;

			return (
				<tr>
					<td><a href={url} title={msg.full_name}>{msg.message_type}</a></td>
					<td>{msg.handlers}</td>
				</tr>
			);
		});

		return (
			<table className="table table-striped" style={{width: 'auto'}}>
				<thead>
					<tr>
						<th>Message Type</th>
						<th>Handlers</th>
					</tr>
				</thead>
				<tbody>
					{rows}
				</tbody>
			</table>
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