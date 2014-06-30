function ReactScreen(component, data){
	this.component = component(data);

	this.activate = function(pane){
		this.deactivate = function(){
			React.unmountComponentAtNode(pane);
		};
	
		React.renderComponent(this.component, pane);
	};
}

function FubuDiagnosticsSection(section){
	$.extend(section, {
		views: [],
		
		add: function(view){
			this.views.push(view);

			view.url = '#' + this.key + '/' + view.key;
			
			if (view.addRoute == null){
				view.addRoute = function(section, router){
					router.route(this.url, section.key + '/' + this.key, function(){
						// TODO - have this also send the route parameters
						FubuDiagnostics.showScreen(this.screen, this);
					});
				}
			}
			
			return this;
		},
		
		url: '#' + section.key,
		
		addRoutes: function(router){
			if (this.screen == null){
				this.screen = new ReactScreen(FubuDiagnostics.components.SectionLinks, {section: this});
			}

			var section = this;
		
			router.route(this.key, this.key, function(){
				FubuDiagnostics.showScreen(section.screen, section);
			});
			
			_.each(this.activeViews(), function(view){
				view.addRoute(section, router);
			});
		},
		
		activeViews: function(){
			return this.views;
		},
		
		
	});
}	

var FubuDiagnostics = {
	currentScreen: {
		deactivate: function(){}
	},
	
	components: {},
	
	showScreen: function(screen, element){
		this.currentScreen.deactivate();

		var pane = document.getElementById('main-pane');

		screen.activate(pane);
		
		this.currentScreen = screen;
		
		// set title and description
		$('#main-heading').html(element.title);
		$('#main-description').html(element.description);
	},
	

    start: function(navBar, defaultScreen) {
		this.navBar = navBar;
		this.defaultScreen = defaultScreen;

		var router = new Backbone.Router();
		router.route('*actions', 'defaultRoute', function(){
			FubuDiagnostics.showScreen(defaultScreen, {title: 'FubuMVC Diagnostics', description: 'Visualization and insight into a running FubuMVC application'});
		});
		
		
		_.each(this.sections, function(s){
			s.addRoutes(router);
		});
		
		this.router = router;
		
		Backbone.history.start();
    },

	refreshNavigation: function(){
		this.navBar.setProps(this);
	},
	
    sections: [],
        

		
    addSection: function(section) {
        this.sections.push(section);
            
		FubuDiagnosticsSection(section);
			
        this.lastSection = section;
		
		return section;
    },
    
    section: function(key) {
        
    },

    addView: function(view) {
		this.lastSection.add(view);
    },

	activeSection: {views: []},
};
