import React from 'react'

var MartenSessions = React.createClass({
    getInitialState(){
        return {
            loading: true
        }
    },

    componentDidMount(){
        FubuDiagnostics.get('Marten:sessions', {}, data => {
            this.setState({sessions: data, loading: false});
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        var rows = this.state.sessions.map(function(session, i){
            var sessionUrl = "#marten/session/" + session.request;
            var requestUrl = '#fubumvc/request-details/' + session.request;
            var chainUrl = "#/fubumvc/chain-details/" + session.hash;

            return (
                <tr key={i}>
                    <td><a href={chainUrl}>{session.chain}</a></td>
                    <td style={{textAlign: 'right'}}>{session.execution_time}</td>
                    <td><a href={requestUrl}>{session.time}</a></td>
                    <td style={{textAlign: 'right'}}><a href={sessionUrl}>{session.request_count}</a></td>
                </tr>
            );
        });

        return (
            <table className="table table-striped" style={{width: 'auto'}}>
                <thead>
                    <tr>
                        <th>Chain</th>
                        <th style={{align: 'right'}}>Execution Time (ms)</th>
                        <th>Time (Local)</th>
                        <th style={{align: 'right'}}>Command Count</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>
        );
    }
});

var Args = React.createClass({
    render(){
        var args = [];
        for (var arg in this.props.args){
            var elem = arg + ": " + this.props.args[arg] + "; ";
            args.push(elem);
        }

        return (<p>{args}</p>);
    }
});

var MartenSession = React.createClass({

    getInitialState(){
        return {
            loading: true,
        }
    },

    componentDidMount(){
        var params = this.getParams();
        FubuDiagnostics.get('Marten:session_Id', params, data => {
            this.setState({loading: false, data: data});
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        var commands = this.state.data.map(function(cmd){
            if (cmd.success){
                return (
                    <div>
                        <pre>
                            {cmd.sql}
                        </pre>
                        <Args args={cmd.args} />
                        <hr />
                    </div>

                );
            }
            else {
                return (
                    <div>
                        <pre>
                            {cmd.sql}
                        </pre>
                        <pre className="bg_warning">
                            {cmd.error}
                        </pre>
                        <Args args={cmd.args} />
                        <hr />
                    </div>

                );
            }
        });

        return (
            <div>{commands}</div>
        );
    }
});

FubuDiagnostics.addSection({
    title: 'Marten',
    description: 'Information about Marten Activity',
    key: 'marten'
}).add({
    title: 'Marten Sessions',
    description: 'Marten Request Count per Session',
    key: 'sessions',
    component: MartenSessions
}).add({
    title: 'Session Details',
    description: 'Session Details',
    key: 'session-details',
    route: 'Marten:session_Id',
    component: MartenSession
});
