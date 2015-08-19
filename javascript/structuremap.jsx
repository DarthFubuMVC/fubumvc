/** @jsx React.DOM */

var React = require('react');
var Router = require('react-router');

require('./settings');
var Summary = require('./structuremap-summary');
var WhatDoIHave = require('./structuremap-whatdoihave');
var BuildPlanView = require('./structuremap-buildplan');
var SearchResults = require('./structuremap-searchresults');


FubuDiagnostics.addSection({
    title: 'StructureMap',
    description: 'Insight into the configuration and state of the application container',
    key: 'structuremap'
})
.add({
	title: 'Summary',
	description: 'Assemblies and Namespaces in the Container',
	key: 'summary',
	component: Summary
})
.add({
	title: 'StructureMap Search Results',
	description: "Interactive version of StructureMap's WhatDoIHave()",
	key: 'search-results',
	route: 'StructureMap:search_Type_Value',
	component: SearchResults
})
.add({
	title: 'What do I have?',
	description: "StructureMap's textual WhatDoIHave() diagnostics",
	key: 'whatdoihave',
	component: WhatDoIHave
})
.add({
	title: 'Build Plan',
	description: 'How StructureMap will build this Instance',
	key: 'buildplan',
	route: 'StructureMap:build_plan_PluginType_Name',
	component:  BuildPlanView
});
