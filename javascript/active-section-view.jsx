import React from 'react'

import SectionLinks from './section-links'

var ActiveSectionView = React.createClass({

    render(){
        var path = this.props.location;
        var sectionKey = path.split('/')[1];
        var activeSection = FubuDiagnostics.section(sectionKey);

        return (
            <div style={{marginLeft: '300px'}}>
                <h2>{activeSection.title} <small>{activeSection.description}</small></h2>
                <SectionLinks section={activeSection} />
            </div>
        );
    }
});

module.exports = ActiveSectionView;
