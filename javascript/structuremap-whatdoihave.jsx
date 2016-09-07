import React from 'react'

var WhatDoIHave = React.createClass({
    getInitialState(){
        return {
            loading: true
        }
    },

    componentDidMount(){
        FubuDiagnostics.getText('StructureMap:whatdoihave', {}, data => {
            this.setState({text: data, loading: false});
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }
        return (<pre>{this.state.text}</pre>);
    }
});

module.exports = WhatDoIHave;
