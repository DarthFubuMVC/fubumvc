
FubuDiagnostics.addSection({    
    title: 'FubuMVC',
    description: 'Core Diagnostics for FubuMVC Applications',
    key: 'fubumvc'
})
.add({
	title: 'Endpoints',
	description: 'something about Endpoints',
	key: 'endpoints'
})
.add({
	title: 'Requests',
	description: 'something about Requests',
	key: 'requests'
})
.add({
	title: 'Model Binding',
	description: 'something about Model Binding',
	key: 'model-binding'
});



FubuDiagnostics.addSection({
    title: 'StructureMap',
    description: 'Insight into the configuration and state of the application container',
    key: 'structuremap'
});



FubuDiagnostics.addSection({
    title: 'OWIN',
    description: 'OWIN Middleware Chain',
    key: 'owin'
});