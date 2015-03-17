
/** @jsx React.DOM */

var React = require('react');



var Packaging = React.createClass({
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get('PackageLog:package_logs', {}, data => {
			_.assign(data, {loading: false});

			this.setState(data);
		});
	},


	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var rows = this.state.logs.map(log => {
			var row = (
		        <tr>
		          <td>{log.Type}</td>
		          <td>{log.Description}</td>
		          <td>{log.Provenance}</td>
		        </tr>
			);

			if (log.FullTraceText != '' && log.FullTraceText != null){
				return [row, (
		          <tr>
		            <td colSpan="4" className="bg-info">
		              {log.FullTraceText}
		            </td>
		          </tr>
				)]
			}

			return row;
		});

		rows = _.flatten(rows);

		return (
		  <div>
		  <h2>Package Loading Log</h2>
		  <table className="table" style={{width: 'auto'}}>
		    <thead>
		      <tr>
		        <th>Type</th>
		        <th>Description</th>
		        <th>Provenance</th>
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
	title: 'Package Loading',
	description: 'Bottle Loading Logs',
	key: 'package-loading',
	component: Packaging
});

