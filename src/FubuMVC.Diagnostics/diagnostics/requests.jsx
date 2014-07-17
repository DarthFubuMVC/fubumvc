/** @jsx React.DOM */

var RequestRow = React.createClass({
	render: function(){
		var details = '#fubumvc/request-details/' + this.props.request.id;

	
		return (
			<tr>
				<td><a href={details}>Details</a></td>
				<td>{this.props.request.time}</td>
				<td>{this.props.request.endpoint}</td>
				<td>{this.props.request.method}</td>
				<td>{this.props.request.status}</td>
				<td>{this.props.request.description}</td>
				<td>{this.props.request['content-type']}</td>
				<td>{this.props.request.duration}</td>
			</tr>
		);
	}
});

var RequestTable = React.createClass({
	render: function(){	
		var rows = this.props.requests.map(function(r, i){
			return (
				<RequestRow request={r} />
			);
		});
		
		return (
			<table className="table table-striped">
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
	description: 'something about Requests',
	key: 'requests',
	screen: new ReactScreen({
		component: RequestTable, 
		route: 'Requests:requests'
	})
});
