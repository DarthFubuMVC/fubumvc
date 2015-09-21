var React = require('react');

var PollingJobTable = React.createClass({
	getInitialState(){
		return {
			loading: true
		}
	},

	componentDidMount(){
		FubuDiagnostics.get('PollingJob:pollingjobs', {}, data => {
			this.setState({jobs: data, loading: false});
		});
	},



/*
            var dict = new Dictionary<string, object>
            {
                {"type", job.JobType.FullName},
                {"interval", job.Interval},
                {"performance", job.Chain.Performance.ToDictionary()},
                {"running", job.IsRunning()},
                {"execution", job.ScheduledExecution.ToString()}
            };


            if (job.Chain.Performance.LastExecution != null)
            {
                dict.Add("last", job.Chain.Performance.LastExecution.ToDictionary());
            }


*/

	render(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var rows = this.state.jobs.map(job => {

			return (
				<tr>
					<td title={job.type}>{job.title}</td>
					<td style={{textAlign: "right"}}>{job.interval}</td>
					<td>{job.execution}</td>
					<td style={{textAlign: "right"}}>{job.performance.hits}</td>
					<td style={{textAlign: "right"}}>{job.performance.average}</td>
					<td style={{textAlign: "right"}}>{job.performance.exceptions}</td>
					<td style={{textAlign: "right"}}>{job.performance.min}</td>
					<td style={{textAlign: "right"}}>{job.performance.max}</td>
				</tr>
			);
		});

		return (
			<table className="table table-striped">
				<thead>
					<tr>
						<th>Job Name</th>
						<th style={{textAlign: "right"}}>Interval (ms)</th>
						<th>Execution</th>
						<th style={{textAlign: "right"}}>Hits</th>
						<th style={{textAlign: "right"}}>Average Time</th>
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
    title: 'Polling Jobs',
    description: 'All configured polling jobs',
    key: 'pollingjobs',
	component: PollingJobTable

});