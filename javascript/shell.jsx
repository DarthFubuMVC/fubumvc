import React from 'react'
import ReactDOM from 'react-dom'
import _ from 'lodash'
import {Router, Route, IndexRoute, hashHistory} from 'react-router'

import Header from './header'
import Dashboard from './dashboard'
import {Grid, Row} from 'react-bootstrap'

var count = 0;

var App = React.createClass({

  getHandlerKey() {
    count++;
    return count;
  },

    render(){
        var style = {
            paddingLeft: '25px'
        }

        return (
            <Grid fluid={true}>
                <Row>
                    <Header />
                </Row>
                <Row  style={style}>
                    {React.cloneElement(this.props.children, { key: this.getHandlerKey() })}
                </Row>
            </Grid>
        );
    }
});

module.exports = {
    start(){
        var sectionRoutes = _.flatten(FubuDiagnostics.sections.map((section, i) => section.toRoutes(i)));

        ReactDOM.render((
            <Router key={"router"} history={hashHistory}>
                <Route key={"rootroute"} name="app" path="/" component={App}>
                    <IndexRoute key={"indexroute"} component={Dashboard} />
                    {sectionRoutes}
                </Route>
            </Router>), document.getElementById('diagnostics'));
    }
}

