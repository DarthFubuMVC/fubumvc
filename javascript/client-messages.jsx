import React from 'react'

var MessageTable = React.createClass({
    getInitialState(){
        return {
            loading: true
        }
    },

    componentDidMount(){
        FubuDiagnostics.get('ClientMessages:clientmessages', {}, data => {
            _.assign(data, {loading: false});

            this.setState(data);
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        if (this.state.messages.length == 0){
            return (
                <h1>No client messages in this application!</h1>
            );
        }

        var rows = this.state.messages.map(function(r, i){
            var url = '#fubumvc/chain-details/' + props.message.chain;
            return (
                <tr key={this.props.message.chain}>
                    <td><a href={url} title="View the chain visualization for this message type">{this.props.message.type}</a></td>
                    <td>{this.props.message.query}</td>
                    <td>{this.props.message.resource}</td>
                </tr>
            )
        });

        return (
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>Client Side Name</th>
                        <th>Query Model</th>
                        <th>Resource Model</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>
        );
    }
});


FubuDiagnostics.section('fubumvc').add({
    title: 'Client Message Types',
    description: 'A list of all the message types available for aggregated querying',
    key: 'client-messages',
    component: MessageTable
});
