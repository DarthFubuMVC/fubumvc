function ReactScreen(component, data){
	this.component = component(data);

	this.activate = function(pane){
		this.deactivate = function(){
			React.unmountComponentAtNode(pane);
		};
	
		React.renderComponent(this.component, pane);
	};
}