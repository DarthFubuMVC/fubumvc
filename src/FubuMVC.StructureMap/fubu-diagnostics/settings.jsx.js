/** @jsx React.DOM */

var SettingsTable = React.createClass({
	render: function(){
		var rows = this.props.settings.map(function(s, i){
			var url = "#settings/setting-details/" + encodeURIComponent(s.type);
			
			return (
				<tr>
					<td title={s.type}><a href={url}>{s.name}</a></td>
					<td>{s.description}</td>
				</tr>
			);
		});
		
		return (
			<table className="table table-striped">
				<tr>
					<th>Type</th>
					<th>Description</th>
				</tr>
				{rows}
			</table>
		);
	}
});

var SettingsDetail = React.createClass({
	render: function(){
		if (this.props.data.type == 'json'){
			var json = JSON.stringify(this.props.data.body, null, 4);
		
			return (
				<div>
					<h3>{this.props.data.title}</h3>
					<pre>{json}</pre>
				</div>
			);
		}
		else{
			return (
				<div>
					<h3>{this.props.data.title}</h3>
					<div dangerouslySetInnerHTML={{
						__html: this.props.data.body
					  }}></div>
				</div>
			);
		}
	}
});

FubuDiagnostics.addSection({
    title: 'Settings',
    description: "All known 'Settings' types",
    key: 'settings',
	route: 'Settings:settings',
	screen: new ReactScreen({
		component: SettingsTable,
		route: 'Settings:settings'
	})

})
.add({
	title: 'Setting Details',
	description: '',
	key: 'setting-details',
	route: 'Settings:setting_Name',
	screen: new ReactScreen({
		component: SettingsDetail,
		route: 'Settings:setting_Name',
		options: {}
	})
});

