var React = require('react');

/*


<h1>Listing of all LightningQueues QueueManagers / Queues</h1>

<br></br>

<h2>QueueManagers</h2>
<hr></hr>
<for each="var queueInfo in Model.QueueManagers">
  <ul style="float:left">
    <li>Storage Path: ${queueInfo.Path}</li>
    <li>Port: ${queueInfo.Port}</li>
    <li>Keep Outgoing History: ${queueInfo.EnableOutgoingMessageHistory}</li>
    <li>Keep Processed History: ${queueInfo.EnableProcessedMessageHistory}</li>
  </ul>
  <ul style="float:left">
    <li>Oldest Outgoing History: ${queueInfo.OldestMessageInOutgoingHistory}</li>
    <li>Oldest Processed History: ${queueInfo.OldestMessageInProcessedHistory}</li>
    <li>Max # in Outgoing History: ${queueInfo.NumberOfMessagesToKeepInOutgoingHistory}</li>
    <li>Max # in Processed History: ${queueInfo.NumberOfMessagesToKeepInProcessedHistory}</li>
    <li>Max # MessageId's to keep: ${queueInfo.NumberOfMessagIdsToKeep}</li>
  </ul>
  !{queueInfo.Queues}
  <hr></hr>
</for>



*/

var QueueManager = React.createClass({
	render: function(){
		var queueRows = this.props.Queues.map(q => {
			return (
				<tr>
					<td>{q.QueueName}</td>
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


FubuDiagnostics.addSection({
	title: 'LightningQueues',
	description: 'The active LightningQueues queues in this application',
	key: 'lq',
	component: AllQueues
});