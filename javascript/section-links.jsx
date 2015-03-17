/** @jsx React.DOM */

var React = require('react');


var SectionLinks = React.createClass({
	render: function(){
		var items = this.props.section.activeViews().map(view => {
			return (
				<div>
					<dt><a href={view.anchor}>{view.title}</a></dt>
					<dd> {view.description}</dd>
				</div>
			);
		});
	
		return (

			<dl className="dl-horizontal">
				{items}
			</dl>

		);

	}
});

module.exports = SectionLinks;
