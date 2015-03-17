/** @jsx React.DOM */

var React = FubuDiagnostics.React;


var WhatDoIHave = React.createClass({
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get('StructureMap:whatdoihave', {}, data => {
			this.setState({text: data, loading: false});
		});
	},


	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		return (<pre>{this.state.text}</pre>);
	}
});

module.exports = WhatDoIHave;