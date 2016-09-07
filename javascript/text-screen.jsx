import React from 'react'

var TextScreen = React.createClass({
    getInitialState(){
        return {
            text: 'Loading...'
        }
    },

    componentDidMount(){
        FubuDiagnostics.get(route, {}, data => {
            this.setState({text: data});
        });
    },

    render(){
        return (
            <pre>{this.state.text}</pre>
        );
    }
});

module.exports = TextScreen;
