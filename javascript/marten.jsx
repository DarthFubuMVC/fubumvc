var React = FubuDiagnostics.React;
var Router = require('react-router');

var MartenSessions = React.createClass({
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get('LightningQueues:queues', {}, data => {
			this.setState({sessions: data, loading: false});
		});
	},

	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}


		var rows = this.state.sessions.map(function(session){
			var sessionUrl = "#marten/session/" + session.request;
			var requestUrl = '#fubumvc/request-details/' + session.request;
			var chainUrl = "#/fubumvc/chain-details/" + session.hash;

			return (
				<tr>
					<td><a href={chainUrl}>{session.chain}</a></td>
					<td><a href={requestUrl}>{session.time}</a></td>
					<td style={{align: 'right'}}><a href={sessionUrl}>{session.request_count}</a></td>
				</tr>
			);
		});

		return (
			<table className="table table-striped" style={{width: 'auto'}}>
				<thead>
					<tr>
						<th>Chain</th>
						<th>Time (Local)</th>
						<th style={{align: 'right'}}>Command Count</th>
					</tr>
				</thead>
				<tbody>
					{rows}
				</tbody>
			</table>
		);
	}
});

var Args = React.createClass({
	render(){
		var args = [];
		for (var arg in this.props.args){
			var elem = arg + ": " + this.props.args[arg] + "; ";
			args.push(elem);
		}

		return (<p>{args}</p>);
	}
});

var MartenSession = React.createClass({
	mixins: [Router.State],

	getInitialState: function(){
		return {
			loading: true,
		}
	},

	componentDidMount: function(){
		var params = this.getParams();
		FubuDiagnostics.get('Marten:session_Id', params, data => {
			this.setState({loading: false, data: data});
		});
	},

	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var commands = this.state.data.map(function(cmd){
			if (cmd.success){
				return (
					<div>
						<pre>
							{cmd.sql}
						</pre>
						<Args args={cmd.args} />
						<hr />
					</div>
					
				);
			}
			else {
				return (
					<div>
						<pre>
							{cmd.sql}
						</pre>
						<pre className="bg_warning">
							{cmd.error}
						</pre>
						<Args args={cmd.args} />
						<hr />
					</div>

				);
			}
		});

		return (
			<div>{commands}</div>
		);
	}
});



FubuDiagnostics.addSection({
	title: 'Marten',
	description: 'Information about Marten Activity',
	key: 'marten',
	component: MartenSessions
}).add({
	title: 'Session Details',
	description: 'Session Details',
	key: 'session-details',
	route: 'Marten:session_Id',
	component: MartenSession
});