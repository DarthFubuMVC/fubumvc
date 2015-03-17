/** @jsx React.DOM */

var React = FubuDiagnostics.React;


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
			var url = '#/structuremap/search-results/' + option.type + '/' + option.value;
			window.location = url;
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

module.exports = SearchBox;