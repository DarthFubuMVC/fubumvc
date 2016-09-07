import React from 'react'

module.exports = React.createClass({
    render(){
        var request = this.props.request;

        var chainUrl = '#/fubumvc/chain-details/' + this.props.chain;

        var requestHeaders = [];
        for (var key in request.headers){
            var item = (
                <li key={key}><b>{key}: </b>{request.headers[key]}</li>
            );

            requestHeaders.push(item);
        }

        return (
            <div>
                <h4>General</h4>
                <ul>
                    <li key="chain"><b>Chain: </b><a href={chainUrl}>{this.props.title}</a></li>
                    <li key="time"><b>Time: </b>{this.props.time}</li>
                    <li key="execution-time"><b>Execution Time: </b>{this.props["execution_time"]} ms</li>
                </ul>

                <h4>Envelope Headers</h4>
                <ul>
                    {requestHeaders}
                </ul>
            </div>
        );
    }
});
