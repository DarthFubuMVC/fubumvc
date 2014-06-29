/** @jsx React.DOM */




var TopSectionMenu = React.createClass({
	render: function(){
		var sections = this.props.data.map(function(section, i){
			return (
				<li><a href={section.url} title={section.description}>{section.title}</a></li>
			);
		});
	
		return (
          <ul className="nav navbar-nav navbar-right">
            <li className="dropdown">
              <a href="#" className="dropdown-toggle" data-toggle="dropdown">
                Sections <span className="caret"></span>
              </a>
              <ul className="dropdown-menu" role="menu">
                {sections}
              </ul>
            </li>
          </ul>
		);
	}
});


var ActiveSectionMenu = React.createClass({
	render: function(){
		var items = this.data.views.map(function(view, i){
			return (
				<li><a href={view.url} title={view.description}>{section.title}</a></li>
			);
		});
		
		return (
          <ul className="nav navbar-nav">
            {items}
          </ul>
		);
	}
});

var NavigationBar = React.createClass({
	render: function(){
		return (
		  <ActiveSectionMenu data={this.props.data.activeSection} />,
		  <TopSectionMenu data={this.props.data.sections} />
		);
	}
});


var navBar = React.renderComponent(
  <NavigationBar data={FubuDiagnostics} />,
  document.getElementById('top-navbar')
);

FubuDiagnostics.start(navBar);



