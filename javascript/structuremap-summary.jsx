/** @jsx React.DOM */

var React = FubuDiagnostics.React;
var SearchBox = require('./structuremap-searchbox');

var AssemblySummaryItem = React.createClass({
	render: function(){
		var url = '#structuremap/search-results/Assembly/' + this.props.name;

		return (
		  <li className="list-group-item">
			<span className="badge">{this.props.count}</span>
			<b><a href={url}>{this.props.name}</a></b>
		  </li>
		);
	}
});

var NamespaceSummaryItem = React.createClass({
	render: function(){
	  var url = '#structuremap/search-results/Namespace/' + this.props.name;
	
	  return (
		  <li className="list-group-item">
			<span className="badge">{this.props.count}</span>
			<a href={url}>{this.props.name}</a>
		  </li>
	  );
	}
});



var Summary = React.createClass({
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get('StructureMap:summary', {}, data => {
			_.assign(data, {loading: false});

			this.setState(data);
		});
	},

	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var items = [];

		this.state.assemblies.forEach(function(assem, i){	
			var item = (<AssemblySummaryItem {...assem}/>);

			items.push(item);

			assem.namespaces.forEach(function(ns){
				var assemItem = (<NamespaceSummaryItem {...ns} />);
				items.push(assemItem);
			});
			
		});

		return (
			<div>

			<SearchBox />
			
			<hr />
			
			<ul className="list-group">
				{items}
			</ul>
			
			</div>
		);

	}
});

module.exports = Summary;
