
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





