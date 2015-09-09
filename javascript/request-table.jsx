/** @jsx React.DOM */

var React = require('react');

var RequestRow = React.createClass({
	render: function(){
		var details = '#fubumvc/request-details/' + this.props.request.id;

		return (
			<tr>
				<td><a href={details}>Details</a></td>
				<td>{this.props.request.time}</td>
				<td>{this.props.request.url}</td>
				<td>{this.props.request.method}</td>
				<td>{this.props.request.status}</td>
				<td>{this.props.request.description}</td>
				<td>{this.props.request.contentType}</td>
				<td style={{textAlign: "right"}}>{this.props.request.duration}</td>
			</tr>
		);
	}
});

var RequestTable = React.createClass({
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get('Requests:requests', {}, data => {
			_.assign(data, {loading: false});

			this.setState(data);
		});
	},

	render: function(){	
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var rows = this.state.requests.map(function(r, i){
			return (
				<RequestRow request={r} />
			);
		});
		
		return (
			<table className="table table-striped" style={{width: 'auto'}}>
				<thead>
					<tr>
						<th>Details</th>
						<th>Time (Local)</th>
						<th>Url</th>
						<th>Method</th>
						<th>Status</th>
						<th>Description</th>
						<th>Content Type</th>
						<th>Duration (ms)</th>
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
	title: 'Requests',
	description: 'A history of the most recent requests handled by this application',
	key: 'requests',
	component: RequestTable
});
