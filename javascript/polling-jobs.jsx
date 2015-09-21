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

                {"request", _request},
                {"time", Time.ToShortTimeString()},
                {"execution_time", ExecutionTime},
                {"title", Title()},
                {"id", Id.ToString()},
                {"exceptions", HadException}
            };
*/

	render(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var rows = this.state.jobs.map(job => {
			var url = '#/fubumvc/chain-details/' + job.chain;


			var last = (<td></td>);
			if (job.last){
				var clazz = 'success';
				if (job.last.exceptions){
					clazz = 'danger';
				}


				last = (
					<td className={clazz}>{job.last.time} in {job.last.execution_time} (ms)</td>
				);
			}

			return (
				<tr>
					<td title={job.type}><a href={url}>{job.title}</a></td>
					<td style={{textAlign: "right"}}>{job.interval}</td>
					<td>{job.execution}</td>
					{last}
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
						<th>Last Execution</th>
						<th style={{textAlign: "right"}}>Hits</th>
						<th style={{textAlign: "right"}}>Average Time (ms)</th>
						<th style={{textAlign: "right"}}>Exception %</th>
						<th style={{textAlign: "right"}}>Min Time (ms)</th>
						<th style={{textAlign: "right"}}>Max Time (ms)</th>
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