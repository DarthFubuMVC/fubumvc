var React = require('react');


var MessageRow = React.createClass({
	render: function(){
		var url = '#fubumvc/chain-details/' + this.props.message.chain;

	
		return (
			<tr>
				<td><a href={url} title="View the chain visualization for this message type">{this.props.message.type}</a></td>
				<td>{this.props.message.query}</td>
				<td>{this.props.message.resource}</td>
			</tr>
		);
	}
});

var MessageTable = React.createClass({
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get('ClientMessages:clientmessages', {}, data => {
			_.assign(data, {loading: false});

			this.setState(data);
		});
	},

	render: function(){	
		if (this.state.loading){
			return (<p>Loading...</p>);
		}
		
		if (this.state.messages.length == 0){
			return (
				<h1>No client messages in this application!</h1>
			);
		}

		var rows = this.state.messages.map(function(r, i){
			return (
				<MessageRow message={r} />
			);
		});
		
		return (
			<table className="table table-striped">
				<thead>
					<tr>
						<th>Client Side Name</th>
						<th>Query Model</th>
						<th>Resource Model</th>
					</tr>
				</thead>
				<tbody>
					{rows}
				</tbody>
			</table>
		);
	}
});


FubuDiagnostics.section('fubumvc').add({
	title: 'Client Message Types',
	description: 'A list of all the message types available for aggregated querying',
	key: 'client-messages',
	component: MessageTable
});
