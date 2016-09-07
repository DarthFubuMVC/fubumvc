import React from 'react'
import _ from 'lodash'

function Detail(header, prop){
    this.header = header;
    this.prop = prop;

    this.toRow = function(data, index){
        return (
            <tr key={index}>
                <th>{this.header}</th>
                <td>{data[this.prop]}</td>
            </tr>
        );
    }
}

var AppDomain = React.createClass({
    getInitialState(){
        return {
            loading: true
        }
    },

    componentDidMount(){
        // TODO -- add parameters into this someday
        FubuDiagnostics.get('AppDomain:appdomain', {}, data => {
            _.assign(data, {loading: false});

            this.setState(data);
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        var assemblies = this.state.assemblies.map(function(a, i){
            return (
                <tr key={i}>
                    <td>{a.name}</td>
                    <td>{a.version}</td>
                    <td>{a.location}</td>
                </tr>
            );
        });

        var details = [
            new Detail('Reloaded at', 'reloaded'),
            new Detail('FubuMVC Path', 'fubuPath'),
            new Detail('AppDomain Base Directory', 'baseDirectory'),
            new Detail('AppDomain Bin Path', 'binPath'),
            new Detail('Configuration File', 'config')
        ];

        var detailRows = details.map((d, i) => {
            return d.toRow(this.state, i);
        });

        return (
            <div>
                <h3>Application Properties</h3>
                <table className="table table-striped details" style={{width: 'auto'}}>
                    <tbody>
                    {detailRows}
                    </tbody>
                </table>

                <h3>Loaded Assemblies</h3>
                <table className="table table-striped">
                    <tbody>
                    <tr>
                        <th>Name</th>
                        <th>Version</th>
                        <th>Location</th>
                    </tr>
                    {assemblies}
                    </tbody>
                </table>
            </div>
        );
    }
});

FubuDiagnostics.addSection({
    title: 'AppDomain',
    description: 'Properties and Assemblies of the current AppDomain',
    key: 'appdomain',
    component: AppDomain
});
