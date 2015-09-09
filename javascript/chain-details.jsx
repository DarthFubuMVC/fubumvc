/** @jsx React.DOM */

var React = require('react');
var _ = require('lodash');
var Router = require('react-router');
var {Grid, Col, Row} = require('react-bootstrap');
var DescriptionBody = require('./description-body');
var ChainPerformanceHistory = require('./chain-performance-history');

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
		return data.hasOwnProperty(att);
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
		if (!data.hasOwnProperty(att)) return false;

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
	
	cell.shouldDisplay = function(data){
		if (!data.hasOwnProperty(att)) return false;

		var actual = data[att];
		
		return actual != null && actual.length > 0;
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




function toBehaviorRow(node){
	return (
		<tr>
			<th>
				<p>{node.title}</p>
				<p><small className="small">Category: {node.category}</small></p>
			</th>
			<td><DescriptionBody {...node} /></td>	
		</tr>
	);
}



var ChainDetails = React.createClass({
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

	buildDetails(){
		var detailCells = [
			new Cell('Title', 'title'),
			new Cell('Route', 'route'),
			new TypeCell('Input Type', 'input'),
			new TypeCell('Resource Type', 'resource'),
			new ArrayCell('Accepts', 'accepts'),
			new ArrayCell('Content-Type', 'content-type'),
			new ArrayCell('Tags', 'tags'),
		];

		return toDetailRows(detailCells, this.state.data.details);
	},

	buildRoute(){
		return (
			<tr className="route-data">
				<th>
					<p>{this.state.data.route.title}</p>
				</th>
				<td><DescriptionBody {...this.state.data.route} /></td>	
			</tr>
		);
	},

	buildPerformanceSummary(rows){
		var performanceHeader = (
			<tr><td colSpan="2"><h4>Performance Summary</h4></td></tr>
		);
		rows.push(performanceHeader);

		var cells = [
			new Cell('Hits', 'hits'),
			new Cell('Total Execution Time', 'total'),
			new Cell('Average Execution Time', 'average'),
			new Cell('Exception %', 'exceptions'),
			new Cell('Min Time', 'min'),
			new Cell('Max Time', 'max')
		];

		var perfRows = toDetailRows(cells, this.state.data.performance);
		return rows.concat(perfRows);
	},

	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		if (this.state.data['not-found']){
			return (<h1>Chain not found!</h1>);
		}

		var rows = this.buildDetails();

		if (this.state.data.route != null){

			rows.push(this.buildRoute());
		}

		rows = this.buildPerformanceSummary(rows);



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

			<ChainPerformanceHistory executions={this.state.data.executions} />

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
	component: ChainDetails
});




