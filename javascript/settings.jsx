/** @jsx React.DOM */

var React = FubuDiagnostics.React;
var Router = require('react-router');

var SettingsTable = React.createClass({
	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		FubuDiagnostics.get('Settings:settings', {}, data => {
			_.assign(data, {loading: false});

			this.setState(data);
		});
	},

	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}

		var rows = this.state.settings.map(function(s, i){
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
	mixins: [Router.State],

	getInitialState: function(){
		return {
			loading: true
		}
	},

	componentDidMount: function(){
		var params = this.getParams();
		FubuDiagnostics.get('Settings:setting_Name', params, data => {
			this.setState({data: data, loading: false});
		});
	},


	render: function(){
		if (this.state.loading){
			return (<p>Loading...</p>);
		}


		if (this.state.data.type == 'json'){
			var json = JSON.stringify(this.state.data.body, null, 4);
		
			return (
				<div>
					<h3>{this.state.data.title}</h3>
					<pre>{json}</pre>
				</div>
			);
		}
		else{
			return (
				<div>
					<h3>{this.state.data.title}</h3>
					<div dangerouslySetInnerHTML={{
						__html: this.state.data.body
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
	component: SettingsTable
})
.add({
	title: 'Setting Details',
	description: '',
	key: 'setting-details',
	route: 'Settings:setting_Name',
	component: SettingsDetail
});

