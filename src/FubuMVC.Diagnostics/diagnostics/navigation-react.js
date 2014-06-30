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



var SectionLinks = React.createClass({
	render: function(){
		var items = this.props.section.activeViews().map(function(view, i){
			return (
				<div>
					<dt><a href={view.url}>{view.title}</a></dt>
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

FubuDiagnostics.components.AllLinks = React.createClass({
	render: function(){
		var items = FubuDiagnostics.sections.map(function(s, i){
			var header = null;
			if (s.activeViews().length == 0){
				header = (
					<h4>
						<a href={s.url}>{s.title}</a>
						<small className="small">{s.description}</small>
					</h4>
				);
			}
			else{
				header = (
					<h4>
						<span>{s.title}</span>
						<small className="small">{s.description}</small>
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

var screen = new ReactScreen(FubuDiagnostics.components.AllLinks);
FubuDiagnostics.start(navBar, screen);



