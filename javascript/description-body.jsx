var React = require('react');

var Child = React.createClass({
	render(){
		return (
			<div className="desc-child">
				<div className="desc-child-title">
					<b>{this.props.name}</b>
					<i>{this.props.title}</i>
				</div>
				<div className="desc-child-body">
					<DescriptionBody {...this.props} />
				</div>
			</div>
		);
	}
});

var DescriptionBody = React.createClass({
	render(){
		var properties = this.renderProperties();
		var children = this.renderChildren();
		var lists = this.renderLists();

		var description = null;
		if (this.props.description){
			description = (<p className="short-desc">{this.props.description}</p>);
		}

		return (
			<div className="description-body">
				{description}
				{properties}
				{children}
				{lists}
			</div>
		);
	},

	renderChildren(){
		var children = [];
		if (this.props.children){
			for (var key in children){
				var child = (<Child name={key} {...children[key]} />);
				children.push(child);
			}
		}

		return children;
	},

	renderLists(){
		var lists = [];
		if (this.props.lists){
			for (var key in this.props.lists){
				var listTitle = (<div className="desc-list-name">{key}</div>);
				lists.push(listTitle);

				this.props.lists[key].forEach(i => {
					var title = (<div className="desc-bullet-item-title">{i.title}</div>);
					lists.push(title);

					var body = (
						<div className="desc-bullet-item-body">
							<DescriptionBody {...i} />
						</div>
					);

					lists.push(body);
				});

				
			}
		}

		return lists;
	},

	renderProperties(){
		var properties = null;
		if (this.props.properties){
			var propRows = [];
			for (var key in this.props.properties){
				var row = (<tr><th>{key}</th><td>{this.props.properties[key]}</td></tr>);
				propRows.push(row);
			}

			properties = (
				<table className="table desc-properties">
					{propRows}
				</table>
			)
		}

		return properties;
	}
});

module.exports = DescriptionBody;