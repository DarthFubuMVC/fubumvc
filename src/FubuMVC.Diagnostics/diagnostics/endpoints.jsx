/** @jsx React.DOM */

var EndpointRow = React.createClass({
	render: function(){
		var hash = '#fubumvc/chain-details/' + this.props.endpoint.hash;
	
		return (
			<tr>
				<td><a href={hash}>Details</a></td>
				<td>{this.props.endpoint.title}</td>
				<td>{this.props.endpoint.actions}</td>
				<td>{this.props.endpoint.constraints}</td>
			</tr>
		);
	}
});

var EndpointTable = React.createClass({
	render: function(){
		var rows = this.props.endpoints.map(function(e, i){
			return (
				<EndpointRow endpoint={e} />
			);
		});
	
		return (
			<table className="table">
				<thead>
					<tr>
						<th>View Details</th>
						<th>Description</th>
						<th>Action(s)</th>
						<th>Http Method(s)</th>
					</tr>
				</thead>
				<tbody>
					{rows}
				</tbody>
			</table>
		);
	}
});

FubuDiagnostics.section('fubumvc').add({
	title: 'Endpoints',
	description: 'something about Endpoints',
	key: 'endpoints',
	screen: new ReactScreen({
		component: EndpointTable, 
		route: 'EndpointExplorer:endpoints'
	})
});




var TypeDisplay = React.createClass({
	render: function(){
		return (
			<span className="type-display" title={this.props.type.qualified-name}>{this.props.type.name}</span>
		);
	}
});



function Cell(title, att, toCell){
	this.title = title;
	this.att = att;
	this.toCell = toCell;
	
	if (this.toCell == null){
		this.toCell = function(data){
			return (
				<td>{data[this.att]}</td>
			);
		};
	}
}

function TypeCell(title, att){
	var cell = new Cell(title, att);
	
	cell.toCell = function(data){
		return (
			<TypeDisplay type={data[att]} />
		);
	}
	
	cell.shouldDisplay = function(data){
		return data[att] != null;
	}
	
	return cell;
}



var DetailsTable = React.createClass({
	render: function(){
		var rows = this.props.cells.map(function(cell, i){
			var td = cell.toCell(this.props.data);
			
			return (
				<tr><th>{cell.title}</th>{td}</tr>
			);
		});
	
		return (
			<table className="details table table-striped">
				<tr><th colspan="2"><span className="details-section">{this.props.label}</span></th></tr>
				{rows}
			</table>
		);
	}
});


var EndpointDetails = React.createClass({

	
	render: function(){
		var detailCells = [
			new Cell('Route', 'route'),
			new Cell('Http Verbs', 'constraints'),
			new Cell('Url Category', 'category'),
			new Cell('Origin', 'origin'),
			new TypeCell('Input Type', 'input'),
			new TypeCell('Resource Type', 'resource')
		
		];
	
		return (
			<DetailsTable label="Details" 
				cells={detailCells} 
				data={this.props.data} />
		);
	}
});


FubuDiagnostics.section('fubumvc').add({
	title: 'Chain Details',
	description: 'BehaviorChain Visualization',
	key: 'chain-details',
	route: 'Chain:chain_details_Hash',
	screen: new ReactScreen({
		component: EndpointDetails,
		route: 'Chain:chain_details_Hash',
		options: {}
	})
});




