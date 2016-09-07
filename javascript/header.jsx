import React from 'react'
import {Navbar, Nav, DropdownButton, NavItem} from 'react-bootstrap'

function Image(props) {
    const { active, activeHref, activeKey, ...rest } = props
        return <img {...rest} />
}

function Anchor(props) {
    const { active, activeHref, activeKey, ...rest } = props
        return <a {...rest} />
}

function StrippedDownButton(props) {
    const { active, activeHref, activeKey, ...rest } = props
    return <DropdownButton {...rest} />
}

var Header = React.createClass({

    render(){
        var path = this.props.location;
        var sectionLinks = [];
        if (path != '/' && path){
            var sectionKey = path.split('/')[1];
            var activeSection = FubuDiagnostics.section(sectionKey);
            sectionLinks = activeSection.activeViews().map(view => {
                return (
                    <NavItem key={sectionKey} href={view.anchor} title={view.description}>{view.title}</NavItem>
                );
            });
        }

        var sectionItems = FubuDiagnostics.sections.map((section) => {
            var onclick = () => {
                window.location = section.anchor;
            }

            return (<NavItem key={section.key} onClick={onclick} href={section.anchor} title={section.description}>{section.title}</NavItem>);
        });

        return (
            <div>
                <Navbar inverse={true} id="top-nav">
                    <Nav>
                        <Image src="/_fubu/icon" style={{marginTop: "5px", marginRight: '20px'}}/>
                    </Nav>
                    <Nav>
                        <Anchor className="navbar-brand" href="#/">FubuMVC Diagnostics</Anchor>
                        {sectionLinks}
                    </Nav>
                    <Nav>
                        <StrippedDownButton id="top-nav-dropbutton" key={1} title="Sections" style={{float: 'right'}}>
                          {sectionItems}
                        </StrippedDownButton>
                    </Nav>
                </Navbar>
            </div>
        );
    }
});

module.exports = Header;
