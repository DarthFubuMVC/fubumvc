/** @jsx React.DOM */

var React = FubuDiagnostics.React;
var Router = require('react-router');
var _ = require('lodash');
var SearchBox = require('./structuremap-searchbox');

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

			return (<InstanceRow {...instance} />);
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
	mixins: [Router.State],

	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		var params = this.getParams();
		FubuDiagnostics.get('StructureMap:search_Type_Value', params, data => {
			_.assign(data, {loading: false});

			this.setState(data);
		});
	},


	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var instances = [];
	
		if (this.state.search.Type == 'Returned-Type'){
			instances = this.state.instances;
		}
		else{
			instances = _.flatten(_.map(this.state.pluginTypes, BuildPluginTypeData));
		}
		
		return (
			<div>
				<SearchBox />
			
				<SearchTitle search={this.state.search} />
				<InstanceResults instances={instances} />
			</div>
		);

	}
});



module.exports = SearchResults;
