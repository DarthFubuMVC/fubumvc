import React from 'react'

var HtmlScreen = React.createClass({
    getInitialState(){
        return {
            html: 'Loading...'
        }
    },

    componentDidMount(){
        // TODO -- add parameters into this someday
        FubuDiagnostics.getText(this.props.route, {}, data => {
            this.setState({html: data});
        });
    },

    render(){
        return (
            <div dangerouslySetInnerHTML={{__html: this.state.html}}></div>
        );
    }
});

module.exports = HtmlScreen;
