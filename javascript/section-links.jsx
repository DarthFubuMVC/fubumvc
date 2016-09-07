import React from 'react'

const SectionLinks = (props) => {
    return (
        <dl className="dl-horizontal">
            {props.section.activeViews().map(view =>{
                return (
                    <div key={view.key}>
                        <dt><a href={view.anchor}>{view.title}</a></dt>
                        <dd> {view.description}</dd>
                    </div>
                )
            })}
        </dl>
    )
};
module.exports = SectionLinks;
