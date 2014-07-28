/** @jsx React.DOM */

var EndpointRow = React.createClass({
	render: function(){
		var hash = '#fubumvc/chain-details/' + this.props.endpoint.hash;
	
		return (
			<tr>
				<td><a href={hash}>Details</a></td>
				<td>{this.props.endpoint.title}</td>
				<td>{this.props.endpoint.actions}</td>
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
			<span className="type-display" title={this.props.type['qualified-name']}>{this.props.type.name}</span>
		);
	}
});



function Cell(title, att, toCell){
	this.title = title;
	this.toCell = toCell;
	
	if (this.toCell == null){
		this.toCell = function(data){
			return (
				<td>{data[att]}</td>
			);
		};
	}
	
	this.applies = function(data){
		return data[att] != null;
	}
	
	this.shouldDisplay = function(data){
		return data[att] != null;
	}
}

function ArrayCell(title, att){
	var toCell = function(data){
		var display = _(data[att]).join(', ');

		return (
			<td>{display}</td>
		);
	};

	var cell = new Cell(title, att, toCell);
	
	cell.shouldDisplay = function(data){
		var actual = data[att];
		
		return actual != null && actual.length > 0;
	}
	
	return cell;
}

function TypeCell(title, att){
	var cell = new Cell(title, att);
	
	cell.toCell = function(data){
		return (
			<td><TypeDisplay type={data[att]} /></td>
		);
	}
	

	
	return cell;
}


function toDetailRows(cells, data){
	var activeCells = _.filter(cells, function(c, i){
		return c.shouldDisplay(data);
	});

	return activeCells.map(function(cell, i){
		var td = cell.toCell(data);
		
		return (
			<tr><th>{cell.title}</th>{td}</tr>
		);
	});
}


var DetailsTable = React.createClass({
	render: function(){
		var data = this.props.data;
		var activeCells = _.filter(this.props.cells, function(c, i){
			return c.shouldDisplay(data);
		});
	
		var rows = activeCells.map(function(cell, i){
			var td = cell.toCell(data);
			
			return (
				<tr><th>{cell.title}</th>{td}</tr>
			);
		});
	
		return (
			<table className="details table table-striped">
				<tbody>
				{rows}
				</tbody>
			</table>
		);
	}
});

function toBehaviorRow(node){
	return (
		<tr>
			<th>
				<p>{node.title}</p>
				<p><small className="small">Category: {node.category}</small></p>
			</th>
			<td dangerouslySetInnerHTML={{__html: node.details}} />	
		</tr>
	);
}



var EndpointDetails = React.createClass({

	
	render: function(){
		var detailCells = [
			new Cell('Route', 'route'),
			new Cell('Url Category', 'category'),
			new Cell('Origin', 'origin'),
			new TypeCell('Input Type', 'input'),
			new TypeCell('Resource Type', 'resource'),
			new ArrayCell('Accepts', 'accepts'),
			new ArrayCell('Content-Type', 'content-type'),
			new ArrayCell('Tags', 'tags'),
		
		];
		
		var rows = toDetailRows(detailCells, this.props.data.details);
		if (this.props.data.route != null){
			var routeRow = (
				<tr>
					<th>Route: {this.props.data.details.route}</th>
					<td dangerouslySetInnerHTML={{__html: this.props.data.route}} />	
				</tr>
			);
		}

		var behaviorHeader = (
			<tr><td colSpan="2"><h4>Behaviors</h4></td></tr>
		);
		rows.push(behaviorHeader);
	
		this.props.data.nodes.forEach(function(node, i){
			var row = toBehaviorRow(node);
			rows.push(row);
		});
	
		return (
			<table className="details table table-striped">
				<tbody>
				{rows}
				</tbody>
			</table>
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




