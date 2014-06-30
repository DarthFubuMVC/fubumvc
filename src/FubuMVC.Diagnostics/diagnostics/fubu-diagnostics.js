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
		
		anchor: '#' + section.key,
		
		add: function(view){
			this.views.push(view);
			view.url = this.key + '/' + view.key;
			if (view.modifyUrl){
				view.url = view.modifyUrl(url);
			};
			
			view.anchor = '#' + view.url;
			

			return this;
		},

		addRoutes: function(router){
			if (this.screen == null){
				this.screen = new ReactScreen(FubuDiagnostics.components.SectionLinks, {section: this});
			}

			var section = this;
			_.each(this.activeViews(), function(view){
				var key = section.key + '/' + view.key;

				router.route(view.url, key, function(){
					// TODO - have this also send the route parameters
					FubuDiagnostics.showScreen(view.screen, view, section);
				});
			});
		
			router.route(this.key, this.key, function(){
				FubuDiagnostics.showScreen(section.screen, section, section);
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
	
	showScreen: function(screen, element, section){
		this.currentScreen.deactivate();
		this.activeSection = section;
		
		var pane = document.getElementById('main-pane');

		screen.activate(pane);
		
		this.currentScreen = screen;
		
		// set title and description
		$('#main-heading').html(element.title);
		$('#main-description').html(element.description);
		
		this.refreshNavigation(section);
	},
	
	refreshNavigation: function(section){
		this.activeSection = section;
		this.navBar.setProps(this);
	},
	

    start: function(navBar, defaultScreen) {
		this.navBar = navBar;
		this.defaultScreen = defaultScreen;

		var router = new Backbone.Router();
		router.route('*actions', 'defaultRoute', function(){
			FubuDiagnostics.showScreen(defaultScreen, {title: 'FubuMVC Diagnostics', description: 'Visualization and insight into a running FubuMVC application'}, {});
		});
		
		
		_.each(this.sections, function(s){
			s.addRoutes(router);
		});
		
		this.router = router;
		
		Backbone.history.start();
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

	activeSection: {},
};
