/** @jsx React.DOM */

var React = require('react');
var Router = require('react-router');


var HeaderTable = React.createClass({
	render: function(){
		if (!(this.props.headers) || this.props.headers.length == 0){
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
			<table className="table table-striped details"  style={{width: 'auto'}}>
				{rows}
			</table>
			</div>
		);
	}
});

var PropTable = React.createClass({
	render: function(){
		if (!(this.props.data) || this.props.data.length == 0){
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
			<table className="table table-striped" style={{width: 'auto'}}>
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
	mixins: [Router.State],

	getInitialState: function(){
		return {
			loading: true,
		}
	},

	componentDidMount: function(){
		var params = this.getParams();
		FubuDiagnostics.get('Requests:request_Id', params, data => {
			this.setState({loading: false, data: data});
		});
	},

	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}


		if (this.state.data.request == null){
			return (
				<div>Request not found.</div>
			);
		}
		
		var request = this.state.data.request;

		var status = request.status;
		if (request.description){
			status = status + '(' + request.description + ')';
		}
				
		var chainUrl = '#fubumvc/chain-details/' + request.hash;

		var logRows = request.logs.map(log => {
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
				<table className="table table-striped"  style={{width: 'auto'}}>
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
	component: RequestDetails
});

			