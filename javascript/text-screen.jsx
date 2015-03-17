/** @jsx React.DOM */

var React = require('react');


var TextScreen = React.createClass({
	getInitialState: function(){
		return {
			text: 'Loading...'
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get(route, {}, data => {
			this.setState({text: data});
		});
	},

	render: function(){
		return (
			<pre>{this.state.text}</pre>
		);
	}
});


module.exports = TextScreen;