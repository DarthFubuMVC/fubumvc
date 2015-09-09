/** @jsx React.DOM */

var React = require('react');

var HtmlScreen = React.createClass({
	getInitialState: function(){
		return {
			html: 'Loading...'
		}
	},

	componentDidMount: function(){
		// TODO -- add parameters into this someday
		FubuDiagnostics.getText(this.props.route, {}, data => {
			this.setState({html: data});
		});
	},

	render: function(){
		return (
			<div dangerouslySetInnerHTML={{__html: this.state.html}}></div>
		);
	}
});

module.exports = HtmlScreen;