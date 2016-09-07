import React from 'react'
import {Route} from 'react-router'
import FubuDiagnosticsView from './fubu-diagnostics-view'

class FubuDiagnosticsSection {
    constructor(section){
        this.title = section.title;
        this.description = section.description;
        this.key = section.key;

        this.url = '/' + this.key;
        this.views = [];
        this.anchor = '#' + this.key;

        this.component = section.component || require('./active-section-view');
    }

    add(data){
        var view = new FubuDiagnosticsView(data, this);
        this.views.push(view);
        return this;
    }

    activeViews(){
        return this.views.filter(v => !v.hasParameters);
    }

    toRoutes(index){
        var routes = this.views.map(view => view.route);
        if (this.component){
            var sectionRoute = (<Route key={index} name={this.key} path={this.url} component={this.component} />);
            routes.push(sectionRoute);
        }
        return routes;
    }
}

module.exports = FubuDiagnosticsSection;
