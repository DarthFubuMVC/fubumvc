/** @jsx React.DOM */




var TopSectionMenu = React.createClass({
	render: function(){
		var sections = this.props.data.map(function(section, i){
			return (
				<li><a href={section.anchor} title={section.description}>{section.title}</a></li>
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
		if (this.props.data.activeViews == null){
			return (
				<span></span>
			);
		}
	
		var items = this.props.data.activeViews().map(function(view, i){
			return (
				<li><a href={view.anchor} title={view.description}>{view.title}</a></li>
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
		<div>
		  <ActiveSectionMenu data={this.props.data.activeSection} />,
		  <TopSectionMenu data={this.props.data.sections} />
		  </div>
		);
	}
});

var navBar = React.renderComponent(
  <NavigationBar data={FubuDiagnostics} />,
  document.getElementById('top-navbar')
);



var SectionLinks = React.createClass({
	render: function(){
		var items = this.props.section.activeViews().map(function(view, i){
			return (
				<div>
					<dt><a href={view.anchor}>{view.title}</a></dt>
					<dd>{view.description}</dd>
				</div>
			);
		});
	
		return (

			<dl className="dl-horizontal">
				{items}
			</dl>

		);

	}
});

FubuDiagnostics.components.SectionLinks = SectionLinks;

AllLinks = React.createClass({
	render: function(){
		var items = FubuDiagnostics.sections.map(function(s, i){
			var header = null;
			if (s.activeViews().length == 0){
				header = (
					<h4>
						<a href={s.anchor}>{s.title}</a>
						<small className="section-subtitle">{s.description}</small>
					</h4>
				);
			}
			else{
				header = (
					<h4>
						<span>{s.title}</span>
						<small className="section-subtitle">{s.description}</small>
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

React.renderComponent(
  <AllLinks data={FubuDiagnostics} />,
  document.getElementById('all-links')
);

FubuDiagnostics.start(navBar);



