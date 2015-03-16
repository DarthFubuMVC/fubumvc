/** @jsx React.DOM */

var React = require('react');

var rb = require('react-bootstrap');
var Navbar = rb.Navbar;
var Nav = rb.Nav;
var DropdownButton = rb.DropdownButton;
var MenuItem = rb.MenuItem;
var NavItem = rb.NavItem;



var Header = React.createClass({
	render: function(){
		return (
		
			<div>
				<Navbar inverse={true} id="top-nav">
					<Nav>
						<a className="navbar-brand" href="#/">FubuMVC Diagnostics</a>

				        <NavItem eventKey="4" href="#/">Application Name Here?</NavItem>
					</Nav>
					<Nav right={true}>
				          <NavItem eventKey="1" href="#/language">Fixtures and Grammars</NavItem>
				          <NavItem eventKey="2" href="#/docs">Documentation</NavItem>
					</Nav>
				</Navbar>

			</div>
		);
	}
});

module.exports = Header;