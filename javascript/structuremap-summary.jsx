import React from 'react'
import SearchBox from './structuremap-searchbox'

var AssemblySummaryItem = React.createClass({
    render(){
        var url = '#structuremap/search-results/Assembly/' + this.props.name;

        return (
          <li className="list-group-item">
            <span className="badge">{this.props.count}</span>
            <b><a href={url}>{this.props.name}</a></b>
          </li>
        );
    }
});

var NamespaceSummaryItem = React.createClass({
    render(){
      var url = '#structuremap/search-results/Namespace/' + this.props.name;

      return (
          <li className="list-group-item">
            <span className="badge">{this.props.count}</span>
            <a href={url}>{this.props.name}</a>
          </li>
      );
    }
});



var Summary = React.createClass({
    getInitialState(){
        return {
            loading: true
        }
    },

    componentDidMount(){
        FubuDiagnostics.get('StructureMap:summary', {}, data => {
            _.assign(data, {loading: false});

            this.setState(data);
        });
    },

    render(){
        if (this.state.loading){
            return (<p>Loading...</p>);
        }

        var items = [];

        this.state.assemblies.forEach(function(assem, i){
            var item = (<AssemblySummaryItem key={assem.name} {...assem}/>);

            items.push(item);

            assem.namespaces.forEach(function(ns, i){
                var assemItem = (<NamespaceSummaryItem key={ns.name} {...ns} />);
                items.push(assemItem);
            });

        });

        return (
            <div>

            <SearchBox />

            <hr />

            <ul className="list-group">
                {items}
            </ul>

            </div>
        );
    }
});

module.exports = Summary;
