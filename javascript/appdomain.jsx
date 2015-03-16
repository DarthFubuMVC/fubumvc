/** @jsx React.DOM */

var React = require('react');
var _ = require('lodash');

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
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		// TODO -- add parameters into this someday
		FubuDiagnostics.get('AppDomain:appdomain', {}, data => {
			_.assign(data, {loading: false});

			this.setState(data);
		});
	},

	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var assemblies = this.state.assemblies.map(function(a, i){
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
		
		var detailRows = details.map(d => {
			return d.toRow(this.state);
		});

		return (
			<div>
				<h3>Application Properties</h3>
				<table className="table table-striped details" style={{width: 'auto'}}>
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
	component: AppDomain
});
