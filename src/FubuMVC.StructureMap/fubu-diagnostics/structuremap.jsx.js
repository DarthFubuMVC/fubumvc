/** @jsx React.DOM */
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


function StructureMapSearch(){
	var self = this;
	
	self.options = null;
	
	self.find = function(query){
		query = query.toLowerCase();
	
		var matches = _.filter(self.options, function(o){
			try {
				return o.display.toLowerCase().indexOf(query) > -1;
			}
			catch (e){
				return false;
			}
		});

		return matches;
	}
	
	self.findMatches = function(query, callback){
		if (self.options == null){
			FubuDiagnostics.get('StructureMap:search_options', {}, function(data){
				if (typeof data == 'string'){
					data = JSON.parse(data);
				}
			
				self.options = data;
				callback(self.find(query));
			});
		}
		else{
			callback(self.find(query));
		}
	}
	
	return self;
}


FubuDiagnostics.StructureMapSearch = new StructureMapSearch();


var SearchBox = React.createClass({
	componentDidMount: function(){
	
		var element = this.getDOMNode();
		$(element).typeahead({
		  minLength: 5,
		  highlight: true
		},
		{
		  name: 'structuremap',
		  displayKey: 'value',
		  
		  source: FubuDiagnostics.StructureMapSearch.findMatches,
		  templates: {
			empty: '<div class="empty-message">No matches found</div>',
			suggestion: _.template('<div><p><%= display%> - <small><%= type%></small></p><p><small><%= value%></small></p></div>')
		  }
		});
		
		$(element).on('typeahead:selected', function(jquery, option){
			var url = 'structuremap/search-results/' + option.type + '/' + option.value;
			FubuDiagnostics.navigateTo(url);
		});
	},
	
	componentWillUnmount: function(){
		var element = this.getDOMNode();
		$(element).typeahead('destroy');
	},

	render: function(){
		return (
			<input type="search" name="search" ref="input" className="form-control typeahead structuremap-search" placeholder="Search the application container" />
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




var InstanceRow = React.createClass({
	render: function(){
		var url = '#structuremap/buildplan/' + this.props._key;
		var familyUrl = '#structuremap/search-results/Plugin-Type/' + this.props.pluginType;
	
		return (
			<tr>
				<td><a href={familyUrl}>{this.props.pluginType}</a></td>
				<td><a href={url}>{this.props.name}</a></td>
				<td>{this.props.lifecycle}</td>
				<td>{this.props.returnedType}</td>
				<td>{this.props.description}</td>
			</tr>		
		);
	}
});

var InstanceResults = React.createClass({
	render: function(){
		var items = this.props.instances.map(function(instance, i){
			instance._key = instance.key;
			return InstanceRow(instance);
		});
		
		return (
			<table className="table table-striped">
				<tr>
					<th>Plugin Type</th>
					<th>Name</th>
					<th>Lifecycle</th>
					<th>Returned Type</th>
					<th>Description</th>
				</tr>
				{items}
			</table>
		);
	}
});


function BuildPluginTypeData(pluginType){
	var items = [];
	
	if (pluginType.defaultInstance){
		items.push(pluginType.defaultInstance);
	}
	else{
		items.push({
			pluginType: pluginType.pluginType,
			lifecycle: pluginType.lifecycle,
			returnedType: '',
			name: '',
			description: ''
		});
	}
	
	items = items.concat(pluginType.others);
	
	if (pluginType.missingName){
		pluginType.missingName.name = '(missing named instance)';
		items.push(pluginType.missingName);
	}
	
	if (pluginType.fallback){
		pluginType.fallback.name = '(fallback)';
		items.push(pluginType.fallback);
	}
	
	for (var i = 1; i < items.length; i++){
		items[i].pluginType = '';
	}
	
	return items;
}


var SearchTitle = React.createClass({
	render: function(){
		var type = this.props.search.Type.replace('-', ' ');
		
		return (
			<h3>Search Results: {type}/{this.props.search.Value}</h3>
		);
	}
});



var SearchResults = React.createClass({
	render: function(){
		var instances = [];
	
		if (this.props.data.search.Type == 'Returned-Type'){
			instances = this.props.data.instances;
		}
		else{
			instances = _.flatten(_.map(this.props.data.pluginTypes, BuildPluginTypeData));
		}
		
		return (
			<div>
				<SearchBox />
			
				<SearchTitle search={this.props.data.search} />
				<InstanceResults instances={instances} />
			</div>
		);

	}
});

FubuDiagnostics.addSection({
    title: 'StructureMap',
    description: 'Insight into the configuration and state of the application container',
    key: 'structuremap',

})
.add({
	title: 'Summary',
	description: 'Assemblies and Namespaces in the Container',
	key: 'summary',
	screen: new ReactScreen({
		component: Summary,
		route: 'StructureMap:summary'
	})
})
.add({
	title: 'StructureMap Search Results',
	description: "Interactive version of StructureMap's WhatDoIHave()",
	key: 'search-results',
	route: 'StructureMap:search_Type_Value',
	screen: new ReactScreen({
		component: SearchResults,
		route: 'StructureMap:search_Type_Value',
		options: {}
	})
})
.add({
	title: 'What do I have?',
	description: "StructureMap's textual WhatDoIHave() diagnostics",
	key: 'whatdoihave',
	screen: new TextScreen('StructureMap:whatdoihave')
})
.add({
	title: 'Build Plan',
	description: 'How StructureMap will build this Instance',
	key: 'buildplan',
	route: 'StructureMap:build_plan_PluginType_Name',
	screen: new TextScreen('StructureMap:build_plan_PluginType_Name')
});
