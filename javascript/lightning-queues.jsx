import React from 'react'
var _ = require('lodash');

var QueueManager = React.createClass({
    render(){
        var queueRows = this.props.Queues.map((q, i) => {
            var url = "#lq/messages/" + q.Port + "/" + q.QueueName;

            return (
                <tr key={i}>
                    <td><a href={url}>{q.QueueName}</a></td>
                    <td>{q.Port}</td>
                    <td style={{textAlign: "right"}}>{q.NumberOfMessages}</td>
                </tr>
            );
        });

        return (
        <div key={this.props.Port}>
          <ul style={{float:'left'}}>
            <li>Storage Path: {this.props.Path}</li>
            <li>Port: {this.props.Port}</li>
          </ul>
          <table className="table table-striped" style={{width: 'auto', marginLeft: '25px'}}>
              <tbody>
            <tr>
                <th>Queue</th>
                <th>Port</th>
                <th style={{textAlign: "right"}}>Number of Messages</th>
            </tr>
            {queueRows}
            </tbody>
          </table>
          <hr></hr>
          </div>
        );
    }
});

var AllQueues = React.createClass({
    getInitialState(){
        return {
            loading: true
        }
    },

    componentDidMount(){
        FubuDiagnostics.get('LightningQueues:queues', {}, data => {
            this.setState({queues: data, loading: false});
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        var managers = this.state.queues.QueueManagers.map((qm, i) => {
            return (<QueueManager key={i} {...qm} />);
        });

        return (
            <div>
            <h2>QueueManagers</h2>
            <hr></hr>
            {managers}
            </div>
        );
    }
});


var QueueDetails = React.createClass({

    getInitialState(){
        return {
            loading: true,
        }
    },

    componentDidMount(){
        var params = this.props.params;
        FubuDiagnostics.get('LightningQueues:messages_Port_QueueName', params, data => {
            this.setState({loading: false, data: data});
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        var rows = this.state.data.Messages.map((msg, i) => {
            // For whatever reason, LQ is sending us two different id's separated by a 'q', so this matches the route
            var url = "#lq/message-details/" + this.state.data.Port + "/" + this.state.data.QueueName + "/" + msg.id;

            return (
                <tr key={i}>
                    <td><a href={url}>{msg.id}</a></td>
                    <td>{msg.headers['message-type']}</td>
                    <td>{msg.sentat}</td>
                    <td>{msg.destination}</td>
                </tr>
            );
        });

        return (
            <div>
            <p><a href="#/lq">Back to LightningQueues...</a></p>

            <h1>Messages in {this.state.data.QueueName} ({this.state.data.Port}) queue</h1>
            <table className="table">
                <tbody>
                <tr>
                    <th>Message Id</th>
                    <th>Type</th>
                    <th>Sent At</th>
                    <th>Destination</th>
                </tr>

                {rows}
                </tbody>
            </table>
            <br></br>
            </div>
        );
    }
});

var MessageDetails = React.createClass({

    getInitialState(){
        return {
            loading: true,
        }
    },

    componentDidMount(){
        var params = this.props.params;
        FubuDiagnostics.get('LightningQueues:message_Port_QueueName_SourceInstanceId_MessageId', params, data => {
            this.setState({loading: false, data: data});
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        if (this.state.data.NotFound){
            return (
                <div>This message cannot be found</div>
            );
        }

        var keys = _.keys(this.state.data.Headers);
        var headers = keys.map((key, i) => {
            return (
                <li key={i}><b>{key}</b> = {this.state.data.Headers[key]}</li>
            );
        });

        var queueUrl = "#/lq/messages/" + this.state.data.Port + "/" + this.state.data.QueueName;

        return (
            <div>
                <h1>Message Id: {this.state.data.MessageId.MessageIdentifier}</h1>

                <br></br>
                <h3>Queue: <a href={queueUrl}>{this.state.data.QueueName}</a></h3>
                <h3>Sent At: {this.state.data.SentAt}</h3>
                <br></br>

                <h3>Headers</h3>
                <ul>
                    {headers}
                </ul>

                <h3>Message Payload</h3>
                <pre>{this.state.data.Payload}</pre>

            </div>

        );
    }
});

FubuDiagnostics.addSection({
    title: 'LightningQueues',
    description: 'The active LightningQueues queues in this application',
    key: 'lq'
}).add({
    title: 'LightningQueues',
    description: 'The active LightningQueues queues in this application',
    key: 'summary',
    component: AllQueues
}).add({
    title: 'Queue Messages',
    description: 'Queued Messages',
    key: 'messages',
    route: 'LightningQueues:messages_Port_QueueName',
    component: QueueDetails
}).add({
    title: 'Message Details',
    description: 'Message Details',
    key: 'message-details',
    route: 'LightningQueues:message_Port_QueueName_SourceInstanceId_MessageId',
    component: MessageDetails
});
