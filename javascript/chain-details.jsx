/** @jsx React.DOM */

var React = require('react');
var _ = require('lodash');
var Router = require('react-router');
var {Grid, Col, Row} = require('react-bootstrap');

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

	return activeCells.map(cell => {
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
			<table className="details table table-striped" style={{width: 'auto'}}>
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
	mixins: [Router.State],

	getInitialState: function(){
		return {
			loading: true,
		}
	},

	componentDidMount: function(){
		var params = this.getParams();
		FubuDiagnostics.get('Chain:chain_details_Hash', params, data => {
			

			this.setState({loading: false, data: data});
		});
	},
	
	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		if (this.state.data['not-found']){
			return (<h1>Chain not found!</h1>);
		}

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


		
		var rows = toDetailRows(detailCells, this.state.data.details);
		if (this.state.data.route != null){
			var routeRow = (
				<tr>
					<th>Route: {this.state.data.details.route}</th>
					<td dangerouslySetInnerHTML={{__html: this.state.data.route}} />	
				</tr>
			);
		}

		var behaviorHeader = (
			<tr><td colSpan="2"><h4>Behaviors</h4></td></tr>
		);
		rows.push(behaviorHeader);
	
		this.state.data.nodes.forEach(function(node, i){
			var row = toBehaviorRow(node);
			rows.push(row);
		});
	
		return (
			<Row>

			<Col md={3} xs={3}>
			This is a visualization of a single behavior chain. We should put more information here some day.
			</Col>

			<Col md={9} xs={9}>

			<table className="details table table-striped" style={{width: 'auto'}}>
				<tbody>
				{rows}
				</tbody>
			</table>

			</Col>

			</Row>
		);
	}
});




FubuDiagnostics.section('fubumvc').add({
	title: 'Chain Details',
	description: 'BehaviorChain Visualization',
	key: 'chain-details',
	route: 'Chain:chain_details_Hash',
	component: EndpointDetails
});




