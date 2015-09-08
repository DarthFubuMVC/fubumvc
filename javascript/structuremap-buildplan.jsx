/** @jsx React.DOM */

var React = FubuDiagnostics.React;
var Router = require('react-router');

var BuildPlanView = React.createClass({
	mixins: [Router.State],

	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		var params = this.getParams();
		FubuDiagnostics.getText('StructureMap:build_plan_PluginType_Name', params, data => {
			this.setState({text: data, loading: false});
		});
	},


	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var params = this.getParams();

		return (

			<div>
				<h3>PluginType: {params.PluginType}, Instance {params.Name}</h3>
				<pre>{this.state.text}</pre>
			</div>
		);
	}
});


module.exports = BuildPlanView;