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
				<td align="right">{this.props.request.duration}</td>
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

var HeaderTable = React.createClass({
	render: function(){
		if (this.props.headers.length == 0){
			return (
				<div></div>
			);
		}
	
		var rows = this.props.headers.map(function(header, i){
			return (
				<tr><th>{header.key}</th><td>{header.values}</td></tr>
			);
		});
	
		return (
			<div>
			<h3>{this.props.label}</h3>
			<table className="table table-striped details">
				{rows}
			</table>
			</div>
		);
	}
});

var PropTable = React.createClass({
	render: function(){
		if (this.props.data.length == 0){
			return (
				<div></div>
			);
		}
	
		var rows = this.props.data.map(function(x, i){
			return (
				<tr><th>{x.key}</th><td>{x.value}</td></tr>
			);
		});
		
		return (
			<div>
			<h3>{this.props.label}</h3>
			<table className="table table-striped">
				{rows}
			</table>
			</div>
		);
	}
});


var TabGroup = React.createClass({
	render: function(){
		var tabs = this.props.options.map(function(opt, i){
			if (opt.key == this.props.active){
				return (
					<li className="active"><a href={opt.anchor}>{opt.title}</a></li>
				);
			}
			else {
				return (
					<li><a href={opt.anchor}>{opt.title}</a></li>
				);
			}
		});
		
		return (
			<ul className="nav nav-tabs" role="tablist">
				{tabs}
			</ul>
		);
	}
});

var RequestDetails = React.createClass({
	render: function(){
		if (this.props.data.request == null){
			return (
				<div>Request not found.</div>
			);
		}
		
		var request = this.props.data.request;

		var status = request.status;
		if (request.description){
			status = status + '(' + request.description + ')';
		}
				
		var chainUrl = '#fubumvc/chain-details/' + request.hash;

		var logRows = request.logs.map(function(log, i){
			return (
				<tr>
					<td>{log.behavior}</td>
					<td>{log.time}</td>
					<td dangerouslySetInnerHTML={{__html: log.html}} />	
				</tr>
			);
		});
		
		return (
			<div>
				<table className="table table-striped">
						<tr><th>Chain</th><td><a href={chainUrl}>{request.title}</a></td><th>Status</th><td>{status}</td></tr>
						<tr><th>Url</th><td>{request.endpoint}</td><th>Request Time</th><td>{request.time}</td></tr>
						<tr><th>Method</th><td>{request.method}</td><th>Execution Time (ms)</th><td>{request.duration}</td></tr>
				</table>
				

				<HeaderTable label="Request Headers" headers={request['request-headers']}/>
				
				<PropTable label="Querystring" data={request.querystring} />
				<PropTable label="Form Data" data={request.form} />
				
				<HeaderTable label="Response Headers" headers={request['response-headers']}/>
				
				<table className="table table-striped">
					<thead>
						<th>Behavior</th>
						<th>Time</th>
						<th>Log</th>
					</thead>
					<tbody>
						{logRows}
					</tbody>
				</table>
			
			</div>
		);
	}
});


FubuDiagnostics.section('fubumvc').add({
	title: 'Request Details',
	description: 'Http Request Visualization',
	key: 'request-details',
	route: 'Requests:request_Id',
	screen: new ReactScreen({
		component: RequestDetails,
		route: 'Requests:request_Id',
		options: {}
	})
});

			
		



