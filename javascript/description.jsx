var React = require('react');
var DescriptionBody = require('./description-body');


var Description = React.createClass({
	render(){


		return (
			<div className="description">
				<h4>{this.props.title}</h4>
				<DescriptionBody {...this.props} />

			</div>
		);
	}
});

module.exports = Description;