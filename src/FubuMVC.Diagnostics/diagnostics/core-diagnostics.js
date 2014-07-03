
FubuDiagnostics.addSection({    
    title: 'FubuMVC',
    description: 'Core Diagnostics for FubuMVC Applications',
    key: 'fubumvc'
})
.add({
	title: 'Endpoints',
	description: 'something about Endpoints',
	key: 'endpoints',
	screen: new TextScreen('Endpoints!')
})
.add({
	title: 'Requests',
	description: 'something about Requests',
	key: 'requests',
	screen: new TextScreen('Requests!')
})
.add({
	title: 'Model Binding',
	description: 'something about Model Binding',
	key: 'model-binding',
	screen: new ServerScreen('fubumvc.modelbinding')
});

function TextScreen(text){
	this.activate = function(pane){
		$(pane).html(text);
	}
	
	this.deactivate = function(){
		
	}
}

FubuDiagnostics.addSection({
    title: 'StructureMap',
    description: 'Insight into the configuration and state of the application container',
    key: 'structuremap',
	screen: new TextScreen('StructureMap!')
});



FubuDiagnostics.addSection({
    title: 'OWIN',
    description: 'OWIN Middleware Chain',
    key: 'owin',
	screen: new TextScreen('OWIN!')
});



