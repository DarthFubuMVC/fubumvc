/** @jsx React.DOM */

var React = require('react');

var PartialRow = React.createClass({
	render: function(){
		var hash = '#fubumvc/chain-details/' + this.props.endpoint.hash;
	
		return (
			<tr>
				<td><a href={hash}>Details</a></td>
				<td>{this.props.endpoint.title}</td>
				<td>{this.props.endpoint.actions}</td>
			</tr>
		);
	}
});

var PartialTable = React.createClass({
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get('EndpointExplorer:partials', {}, data => {
			_.assign(data, {loading: false});

			this.setState(data);
		});
	},

	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var rows = this.state.endpoints.map(function(e, i){
			return (
				<PartialRow endpoint={e} />
			);
		});
	
		return (
			<div>

			<h3>Partial Chains</h3>

			<table className="table">
				<thead>
					<tr>
						<th>View Details</th>
						<th>Description</th>
						<th>Action(s)</th>
					</tr>
				</thead>
				<tbody>
					{rows}
				</tbody>
			</table>

			</div>
		);
	}
});

FubuDiagnostics.section('fubumvc').add({
	title: 'Partials',
	description: 'All the configured partial chains in this application',
	key: 'partials',
	component: PartialTable
});