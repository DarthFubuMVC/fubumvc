import React from 'react'

var EndpointRow = React.createClass({
    render(){
        var hash = '#fubumvc/chain-details/' + this.props.endpoint.hash;

        return (
            <tr>
                <td><a href={hash}>Details</a></td>
                <td>{this.props.endpoint.title}</td>
                <td>{this.props.endpoint.actions}</td>
            </tr>
        );
    }
});

var EndpointTable = React.createClass({
    getInitialState(){
        return {
            loading: true
        }
    },

    componentDidMount(){
        FubuDiagnostics.get('EndpointExplorer:endpoints', {}, data => {
            _.assign(data, {loading: false});

            this.setState(data);
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        var rows = this.state.endpoints.map(function(e, i){
            return (
                <EndpointRow key={i} endpoint={e} />
            );
        });

        return (
            <div>

            <h3>HTTP Endpoints</h3>

            <table className="table">
                <thead>
                    <tr>
                        <th>View Details</th>
                        <th>Description</th>
                        <th>Action(s)</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>

            </div>
        );
    }
});

FubuDiagnostics.section('fubumvc').add({
    title: 'Endpoints',
    description: 'All the configured endpoint routes, partials, and message handlers in this application',
    key: 'endpoints',
    component: EndpointTable
});
