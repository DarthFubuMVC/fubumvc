/** @jsx React.DOM */

var AssetsTable = React.createClass({
	render: function(){

		var rows = this.props.assets.map(function(a, i){
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
	route: 'Asset:assets',
	screen: new ReactScreen({
		component: AssetsTable,
		route: 'Asset:assets'
	})

});