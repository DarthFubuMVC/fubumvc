import React from 'react'
import {Route} from 'react-router'

class FubuDiagnosticsView {
    constructor(view, section){
        this.url = section.key + '/' + view.key;
        this.key = view.key;

        var routeName = section.key + ':' + view.key;

        this.anchor = '#' + this.url;
        this.hasParameters = false;
        this.description = view.description;
        this.title = view.title;


        var component = view.component;
        if (view.hasOwnProperty('render')){
            component = React.createClass({
                render: view.render
            });
        }

        if (!component){
            throw new Error("You need to either specify a React in view.component or pass in a render() function");
        }

        if (view.route){
            var route = FubuDiagnostics.routes[view.route];
            if (route.params && route.params.length > 0){
                this.hasParameters = true;
                route.params.forEach(x => {
                    this.url = this.url + '/:' + x.Name;
                });
            }
        }
        this.route = (<Route key={this.key} name={routeName} path={this.url} component={component}/>);
    }
}

module.exports = FubuDiagnosticsView;
