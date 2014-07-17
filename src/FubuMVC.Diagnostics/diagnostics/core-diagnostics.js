
FubuDiagnostics.addSection({    
    title: 'FubuMVC',
    description: 'Core Diagnostics for FubuMVC Applications',
    key: 'fubumvc'
})
.add({
	title: 'Model Binding',
	description: 'something about Model Binding',
	key: 'model-binding',
	screen: new ServerScreen('ModelBinding:binding_all')
})
.add({
	title: 'Package Loading',
	description: 'something about Package Loading',
	key: 'package-loading',
	screen: new ServerScreen('PackageLog:package_logs')
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



