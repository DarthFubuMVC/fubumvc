import React from 'react'

var MessageRow = React.createClass({
    render(){
        var details = '#fubumvc/request-details/' + this.props.id;

        return (
            <tr key={this.props.id}>
                <td><a href={details}>Details</a></td>
                <td>{this.props.time}</td>
                <td>{this.props.message}</td>
                <td>{this.props.request.headers.attempts}</td>
                <td style={{textAlign: "right"}}>{this.props.execution_time}</td>
                <td>{this.props.request.headers["received-at"]}</td>
                <td>{this.props.request.headers["reply-uri"]}</td>
            </tr>
        );
    }
});

var MessageTable = React.createClass({
    getInitialState(){
        return {
            loading: true
        }
    },

    componentDidMount(){
        FubuDiagnostics.get('Requests:messages', {}, data => {
            this.setState({messages: data, loading: false});
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        var rows = this.state.messages.map((m, i) => {
            return (
                <MessageRow key={i} {...m} />
            );
        });

        return (
            <table className="table table-striped" style={{width: 'auto'}}>
                <thead>
                    <tr>
                        <th>Details</th>
                        <th>Time (Local)</th>
                        <th>Message Type</th>
                        <th>Attempts</th>
                        <th style={{textAlign: "right"}}>Duration (ms)</th>
                        <th>Received At</th>
                        <th>Reply Uri</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>
        );
    }
});

FubuDiagnostics.section('servicebus').add({
    title: 'Message History',
    description: 'A history of the most recent service bus messages handled by this application',
    key: 'messages',
    component: MessageTable
});
