/** @jsx React.DOM */

var React = require('react');

var AssetsTable = React.createClass({
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		// TODO -- add parameters into this someday
		FubuDiagnostics.get('Asset:assets', {}, data => {
			_.assign(data, {loading: false});

			this.setState(data);
		});
	},

	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var rows = this.state.assets.map(a => {
			return (
				<tr>
					<td>{a.file}</td>
					<td>{a.url}</td>
					<td>{a.mimetype}</td>
					<td>{a.provenance}</td>
					<td>{a.cdn}</td>
					
				</tr>
			);
		});
		
		return (
			<table className="table table-striped">
				<tr>
					<th>File</th>
					<th>Url</th>
					<th>Mimetype</th>
					<th>Provenance</th>
					<th>CDN</th>
					
				</tr>
				{rows}
			</table>
		);
	}
});	


FubuDiagnostics.addSection({
    title: 'Assets',
    description: 'All known client side assets',
    key: 'assets',
	component: AssetsTable

});