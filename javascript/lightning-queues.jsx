var React = require('react');
var Router = require('react-router');
var _ = require('lodash');

var QueueManager = React.createClass({
	render: function(){
		var queueRows = this.props.Queues.map(q => {
			var url = "#lq/lq-messages/" + q.Port + "/" + q.QueueName;

			return (
				<tr>
					<td><a href={url}>{q.QueueName}</a></td>
					<td>{q.Port}</td>
					<td style={{textAlign: "right"}}>{q.NumberOfMessages}</td>
				</tr>
			);
		});

		return (
		<div>
		  <ul style={{float:'left'}}>
		    <li>Storage Path: {this.props.Path}</li>
		    <li>Port: {this.props.Port}</li>
		    <li>Keep Outgoing History: {this.props.EnableOutgoingMessageHistory}</li>
		    <li>Keep Processed History: {this.props.EnableProcessedMessageHistory}</li>
		  </ul>
		  <ul style={{float:'left', marginRight: '25px'}}>
		    <li>Oldest Outgoing History: {this.props.OldestMessageInOutgoingHistory}</li>
		    <li>Oldest Processed History: {this.props.OldestMessageInProcessedHistory}</li>
		    <li>Max # in Outgoing History: {this.props.NumberOfMessagesToKeepInOutgoingHistory}</li>
		    <li>Max # in Processed History: {this.props.NumberOfMessagesToKeepInProcessedHistory}</li>
		    <li>Max # MessageIds to keep: {this.props.NumberOfMessagIdsToKeep}</li>
		  </ul>
		  <table className="table table-striped" style={{width: 'auto', marginLeft: '25px'}}>
		  	<tr>
		  		<th>Queue</th>
		  		<th>Port</th>
		  		<th style={{textAlign: "right"}}>Number of Messages</th>
		  	</tr>
		  	{queueRows}
		  </table>
		  <hr></hr>
		  </div>
		);
	}
});

var AllQueues = React.createClass({
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get('LightningQueues:queues', {}, data => {
			this.setState({queues: data, loading: false});
		});
	},

	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var managers = this.state.queues.QueueManagers.map(qm => {
			return (<QueueManager {...qm} />);
		});

		return (
			<div>
			<h2>QueueManagers</h2>
			<hr></hr>
			{managers}
			</div>
		);
	}
});

/*

        public string id { get; set; }
        public string status { get; set; }
        public string sentat { get; set; }
        public string sourceinstanceid { get; set; }
        public IDictionary<string, string> headers { get; set; }


*/
var QueueDetails = React.createClass({
	mixins: [Router.State],

	getInitialState: function(){
		return {
			loading: true,
		}
	},

	componentDidMount: function(){
		var params = this.getParams();
		FubuDiagnostics.get('LightningQueues:messages_Port_QueueName', params, data => {
			this.setState({loading: false, data: data});
		});
	},

	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var rows = this.state.data.Messages.map(msg => {
			return (
				<tr>
					<td>{msg.id}</td>
					<td>{msg.headers['message-type']}</td>
					<td>{msg.status}</td>
					<td>{msg.sentat}</td>
				</tr>
			);
		});

		return (
			<div>
			<h1>Messages in {this.state.data.QueueName} ({this.state.data.Port}) queue</h1>
			<table className="table">
				<tr>
					<th>Message Id</th>
					<th>Type</th>
					<th>Status</th>
					<th>Sent At</th>
				</tr>

				{rows}
			</table>
			<br></br>
			</div>
		);
	}
});


FubuDiagnostics.addSection({
	title: 'LightningQueues',
	description: 'The active LightningQueues queues in this application',
	key: 'lq',
	component: AllQueues
}).add({
	title: 'Queue Messages',
	description: 'Queued Messages',
	key: 'messages',
	route: 'LightningQueues:messages_Port_QueueName',
	component: QueueDetails
});