var React = require('react');
var Router = require('react-router');

var PerformanceSummary = React.createClass({
	getInitialState(){
		return {
			loading: true,
		}
	},

	componentDidMount(){
		FubuDiagnostics.get('Performance:performance', null, data => {
			this.setState({loading: false, chains: data});
		});
	},

	render(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var rows = this.state.chains.map(chain => {
			var url = "#/fubumvc/chain-details/" + chain.hash;

			return (
				<tr>
					<td><a href={url}>{chain.title}</a></td>
					<td style={{textAlign: "right"}}>{chain.hits}</td>
					<td style={{textAlign: "right"}}>{chain.total}</td>
					<td style={{textAlign: "right"}}>{chain.average}</td>
					<td style={{textAlign: "right"}}>{chain.exceptions}</td>
					<td style={{textAlign: "right"}}>{chain.min}</td>
					<td style={{textAlign: "right"}}>{chain.max}</td>
				</tr>
			);
		});

		return (
			<table className="table table-striped" style={{width: 'auto'}}>
				<thead>
					<tr>
						<th>Chain</th>
						<th style={{textAlign: "right"}}>Hits</th>
						<th style={{textAlign: "right"}}>Total</th>
						<th style={{textAlign: "right"}}>Average</th>
						<th style={{textAlign: "right"}}>Exception %</th>
						<th style={{textAlign: "right"}}>Min Time</th>
						<th style={{textAlign: "right"}}>Max Time</th>
					</tr>
				</thead>
				<tbody>
					{rows}
				</tbody>
			</table>
		);
	}
});


FubuDiagnostics.addSection({    
    title: 'Performance',
    description: 'Performance statistics for all behavior chains in the running system',
    key: 'performance'
}).add({
	title: 'Performance Summary',
	description: 'Performance overview for all behavior chains',
	key: 'summary',
	component: PerformanceSummary
});