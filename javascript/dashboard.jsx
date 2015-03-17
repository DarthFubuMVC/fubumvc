/** @jsx React.DOM */

var React = require('react');
var {Grid, Col, Row} = require('react-bootstrap');


var SectionLinks = require('./section-links');

AllLinks = React.createClass({
	render: function(){
		var style = {
			marginLeft: '10px'
		}

		var items = FubuDiagnostics.sections.map(s => {
			var header = null;
			if (s.activeViews().length == 0){
				header = (
					<h4>
						<a href={s.anchor}>{s.title}</a>
						<small style={style}>{s.description}</small>
					</h4>
				);
			}
			else{
				header = (
					<h4>
						<span>{s.title}</span>
						<small style={style}>{s.description}</small>
					</h4>
				);
			}
		
			return (
				<div>
					{header}

					<SectionLinks section={s} />
					
					<hr />
				</div>
			);
		});
		
	
		return (	
			<div>{items}</div>
		);
	}
});


var Dashboard = React.createClass({
	render: function(){
		return (
			<Row>
				<Col xs={6} md={6} >
				  <h3>Getting Started</h3>
				  <p>You are viewing the Advanced Diagnostics package that provides detailed insight into the configuration and runtime of your application.</p>

				  <ol>
					<li>
					  <strong>
						<a href="#fubumvc/endpoints"> Explore your application</a>
					  </strong>
					  <p>The endpoint explorer allows you to sort and filter through the routes, endpoints, and chains within your application. You can also drill into the details and visualize the behavior chain.</p>
					</li>
					<li>
					  <strong>
						<a href="#fubumvc/requests">Explore the request history</a>
					  </strong>
					  <p>The requests explorer allows you to see the most recent requests that have been recorded within your application. You can drill into the details of each request to visualize the various steps that were taken to issue the response.</p>
					</li>
					<li>
					  <strong>
						<a href="http://fubuworld.com/fubumvc/">Browse the documentation</a>
					  </strong>
					  <p>Browse through our topics and read more about the our APIs.</p>
					</li>
					<li>
					  <strong>
						<a href="https://groups.google.com/forum/#!forum/fubumvc-devel">If you're really stuck</a>
					  </strong>
					  <p>Visit our user group to learn how to get plugged into our vibrant community. You'll get your questions answered in no time.</p>
					</li>
				  </ol>

				</Col>
				
				<Col xs={6} md={6}>
					<AllLinks />
				</Col>
			
			
			</Row>
		
		);
	}
});

module.exports = Dashboard;