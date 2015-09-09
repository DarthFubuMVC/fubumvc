/** @jsx React.DOM */

var React = require('react');
var Router = require('react-router');
var HttpRequestHeader = require('./http-request-header');
var MessageExecutionHeader = require('./message-execution-header');
var _ = require('lodash');
var Description = require('./description');


var ActivityTable = React.createClass({
	render(){
		var rows = this.props.activities.map(a => {
			return (
				<tr>
					<td>{a.title}</td>
					<td style={{textAlign: "right"}}>{a.start}</td>
					<td style={{textAlign: "right"}}>{a.end}</td>
					<td style={{textAlign: "right"}}>{a.duration}</td>
					<td style={{textAlign: "right"}}>{a.inner_time}</td>
				</tr>

			);
		});

		return (
			<table className="table table-striped" style={{width: 'auto'}}>
				<tr>
					<th>Activity</th>
					<th style={{textAlign: "right"}}>Starting</th>
					<th style={{textAlign: "right"}}>Ending</th>
					<th style={{textAlign: "right"}}>Total Time</th>
					<th style={{textAlign: "right"}}>Inner Time</th>
				</tr>
				{rows}
			</table>
		);
	}
});

var LogTable = React.createClass({
	render(){
		var activities = {};
		this.props.activities.forEach(a => {
			activities[a.id] = a.title;
		});

		var rows = this.props.steps.map(step => {
			return (
				<tr>
					<td style={{verticalAlign: "top"}}>{activities[step.activity]}</td>
					<td style={{textAlign: "right", verticalAlign: "top"}}>{step.time}</td>
					<td style={{verticalAlign: "top"}}><Description {...step.log} /></td>
				</tr>
			);
		});

		return (
			<table className="table table-striped" style={{width: 'auto'}}>
				<tr>
					<th>Activity</th>
					<th>Request Time</th>
					<th>Log</th>
				</tr>
				{rows}
			</table>
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


		if (this.state.data.log == null){
			return (
				<div>Request not found.</div>
			);
		}

		var header = null;
		if (this.state.data.type == 'RoutedChain'){
			header = (<HttpRequestHeader {...this.state.data.log} />);
		}

		if (this.state.data.type == 'HandlerChain'){
			header = (<MessageExecutionHeader {...this.state.data.log} />);
		}

		return (
			<div>
				{header}
				<h3>Performance Timings per Behavior and/or Partial</h3>
				<ActivityTable {...this.state.data.log} />

				<h3>Log Activity</h3>
				<LogTable {...this.state.data.log} />



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

			