var React = require('react');

module.exports = React.createClass({
	render(){
		var rows = this.props.executions.map(x => {
			var clazz = '';
			if (x.warn){
				clazz = 'warning';
			}

			var exceptionText = '';
			if (x.exceptions){
				exceptionText = 'Exception(s)!';
				clazz = 'danger';
			}

			var url = "#/fubumvc/request-details/" + x.id;

			return (
				<tr className={clazz}>
					<td><a href={url}>{x.time}</a></td>
					<td style={{textAlign: "right"}}>{x.execution_time}</td>
				</tr>

			);
		});

		return (
			<table className="table table-striped" style={{width: 'auto'}}>
				<tr>
					<th>Time</th>
					<th style={{textAlign: "right"}}>Execution Time</th>
				</tr>
				{rows}
			</table>
		);
	}
});