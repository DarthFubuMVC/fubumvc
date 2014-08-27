/** @jsx React.DOM */

function Detail(header, prop){
	this.header = header;
	this.prop = prop;
	
	this.toRow = function(data){
		return (
			<tr>
				<th>{this.header}</th>
				<td>{data[this.prop]}</td>
			</tr>
		);
	}
}

var AppDomain = React.createClass({
	render: function(){
		var assemblies = this.props.assemblies.map(function(a, i){
			return (
				<tr>
					<td>{a.name}</td>
					<td>{a.version}</td>
					<td>{a.location}</td>
				</tr>
			);
		});
		
		var details = [
			new Detail('Reloaded at', 'reloaded'),
			new Detail('FubuMVC Path', 'fubuPath'),
			new Detail('AppDomain Base Directory', 'baseDirectory'),
			new Detail('AppDomain Bin Path', 'binPath'),
			new Detail('Configuration File', 'config')
		];
		
		var props = this.props;
		var detailRows = details.map(function(d, i){
			return d.toRow(props);
		});

		return (
			<div>
				<h3>Application Properties</h3>
				<table className="table table-striped details">
					{detailRows}
				</table>
			
				<h3>Loaded Assemblies</h3>
				<table className="table table-striped">
					<tr>
						<th>Name</th>
						<th>Version</th>
						<th>Location</th>
					</tr>
					{assemblies}
				</table>
			</div>
		
		);
	}
});

FubuDiagnostics.addSection({
    title: 'AppDomain',
    description: 'Properties and Assemblies of the current AppDomain',
    key: 'appdomain',
	screen: new ReactScreen({
		component: AppDomain,
		route:'AppDomain:appdomain'
	})
});
