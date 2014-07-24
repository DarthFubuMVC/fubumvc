/** @jsx React.DOM */
var AssemblySummaryItem = React.createClass({
	render: function(){
		var url = '#structuremap/assembly/' + this.props.name;

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
	  var url = '#structuremap/namespace/' + this.props.name;
	
	  return (
		  <li className="list-group-item">
			<span className="badge">{this.props.count}</span>
			<a href={url}>{this.props.name}</a>
		  </li>
	  );
	}
});


var Summary = React.createClass({
	render: function(){
		var items = [];
		
		this.props.assemblies.forEach(function(assem, i){		
			items.push(AssemblySummaryItem(assem));
			
			assem.namespaces.forEach(function(ns){
				items.push(NamespaceSummaryItem(ns));
			});
			
		});
		
		return (
			<ul className="list-group">
				{items}
			</ul>
		);
	}
});

FubuDiagnostics.addSection({
    title: 'StructureMap',
    description: 'Insight into the configuration and state of the application container',
    key: 'structuremap',
	screen: new ReactScreen({
		component: Summary,
		route: 'StructureMap:summary'
	})
});

