import React from 'react';
import _ from 'lodash'
import {Grid, Col, Row} from 'react-bootstrap'
import DescriptionBody from './description-body'
import ChainPerformanceHistory from './chain-performance-history'

var TypeDisplay = React.createClass({
    render(){
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
                <td key={this.title}>{data[att]}</td>
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
            <tr key={cell.title}><th>{cell.title}</th>{td}</tr>
        );
    });
}

function toBehaviorRow(node){
    return (
        <tr key={node.title}>
            <th>
                <p>{node.title}</p>
                <p><small className="small">Category: {node.category}</small></p>
            </th>
            <td><DescriptionBody key={node.title} {...node} /></td>
        </tr>
    );
}

var ChainDetails = React.createClass({

    getInitialState(){
        return {
            loading: true,
        }
    },

    componentDidMount(){
        var params = this.props.params;
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
            <tr key={this.state.data.route.title} className="route-data">
                <th>
                    <p>{this.state.data.route.title}</p>
                </th>
                <td><DescriptionBody {...this.state.data.route} /></td>
            </tr>
        );
    },

    buildPerformanceSummary(rows){
        var performanceHeader = (
            <tr key='PerformanceHeader'><td colSpan="2"><h4>Performance Summary</h4></td></tr>
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

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        if (this.state.data['not-found']){
            return (<h1>Chain not found!</h1>);
        }

        var rows = this.buildDetails();

        if (this.state.data.route != null){

            rows.push(this.buildRoute(rows.length));
        }

        rows = this.buildPerformanceSummary(rows);

        var behaviorHeader = (
            <tr key='BehaviorsHeader'><td colSpan="2"><h4>Behaviors</h4></td></tr>
        );
        rows.push(behaviorHeader);

        this.state.data.nodes.forEach(function(node, i){
            var row = toBehaviorRow(node, i);
            rows.push(row);
        });
        return (
            <Row>

            <Col key="desc" md={3} xs={3}>
            This is a visualization of a single behavior chain. We should put more information here some day.
            </Col>

            <Col key="details-body" md={9} xs={9}>

            <table className="details table table-striped" style={{width: 'auto'}}>
                <tbody>
                {rows}
                </tbody>
            </table>

            <ChainPerformanceHistory key="chain-perf-history" executions={this.state.data.executions} />

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




