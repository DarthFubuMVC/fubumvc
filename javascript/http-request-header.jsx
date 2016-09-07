import React from 'react'

module.exports = React.createClass({
    url(){
        var request = this.props.request;

        var url = request["owin.RequestScheme"] + "://" + request["owin.RequestHeaders"]["Host"][0] + request["owin.RequestPathBase"] + request["owin.RequestPath"];
        if (request["RequestQueryString"] && request["RequestQueryString"].length > 0){
            url = url + "?" + request["RequestQueryString"];
        }
        return url;
    },

    requestHeaders(){
        var request = this.props.request;
        var requestHeaders = [];
        for (var key in request["owin.RequestHeaders"]){
            var item = (
                <li key={key}><b>{key}: </b>{request["owin.RequestHeaders"][key].join(' / ')}</li>

            );
            requestHeaders.push(item);
        }
        return requestHeaders;
    },

    responseHeaders(){
        var request = this.props.request;
        var responseHeaders = [];
        for (var key in request["owin.ResponseHeaders"]){
            var item = (
                <li key={key}><b>{key}: </b>{request["owin.ResponseHeaders"][key].join(' / ')}</li>
            );
            responseHeaders.push(item);
        }
        return responseHeaders;
    },

    render(){
        var request = this.props.request;
        var requestHeaders = this.requestHeaders();
        var responseHeaders = this.responseHeaders();

        var chainUrl = '#/fubumvc/chain-details/' + this.props.chain;

        return (
            <div>
                <h4>General</h4>
                <ul>
                    <li key="chain"><b>Chain: </b><a href={chainUrl}>{this.props.title}</a></li>
                    <li key="time"><b>Time: </b>{this.props.time}</li>
                    <li key="executiontime"><b>Execution Time: </b>{this.props["execution_time"]} ms</li>
                    <li key="url"><b>Request URL: </b>{this.url()}</li>
                    <li key="method"><b>Request Method: </b>{request["owin.RequestMethod"]}</li>
                    <li key="status"><b>Status Code: </b>{request["owin.ResponseStatusCode"]}</li>
                </ul>
                <h4>Request Headers</h4>
                <ul>
                    {requestHeaders}
                </ul>
                <h4>Response Headers</h4>
                <ul>
                    {responseHeaders}
                </ul>
            </div>
        );
    }
});
