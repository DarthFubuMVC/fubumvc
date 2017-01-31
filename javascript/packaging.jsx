import React from 'react'

var Packaging = React.createClass({
    getInitialState(){
        return {
            loading: true
        }
    },

    componentDidMount(){
        FubuDiagnostics.get('PackageLog:package_logs', {}, data => {
            _.assign(data, {loading: false});

            this.setState(data);
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        var rows = this.state.logs.map((log, i) => {
            var row = (
                <tr key={i}>
                  <td>{log.Type}</td>
                  <td>{log.Description}</td>
                  <td>{log.Provenance}</td>
                </tr>
            );

            if (log.FullTraceText != '' && log.FullTraceText != null){
                return [row, (
                  <tr key={row.key + '-trace'}>
                    <td colSpan="4" className="bg-info">
                      {log.FullTraceText}
                    </td>
                  </tr>
                )]
            }

            return row;
        });

        rows = _.flatten(rows);

        return (
          <div>
          <h2>Package Loading Log</h2>
          <table className="table" style={{width: 'auto'}}>
            <thead>
              <tr>
                <th>Type</th>
                <th>Description</th>
                <th>Provenance</th>
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
    title: 'Package Loading',
    description: 'Bottle Loading Logs',
    key: 'package-loading',
    component: Packaging
});

