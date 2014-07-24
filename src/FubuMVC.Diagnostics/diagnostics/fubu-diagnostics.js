


var FubuDiagnostics = {
	currentScreen: {
		deactivate: function(){}
	},
	
	components: {},
	
	showScreen: function(screen, element, section, params){
		$('#home-view').hide();
		$('.left-content').hide();
		
		// optional html content
		// in the left pane
		var count = $('#' + element.key).show().length; 
		
		if (count == 0){
			$('#left-pane').hide();
			$('#main-pane-holder')
				.removeClass('col-xs-9 col-md-9')
				.addClass('col-xs-12 col-md-12');
		}
		else {
			$('#left-pane').show();
			$('#main-pane-holder')
				.removeClass('col-xs-12 col-md-12')
				.addClass('col-xs-9 col-md-9');
		}
		
		this.currentScreen.deactivate();
		this.activeSection = section;
		
		var pane = document.getElementById('main-pane');

		screen.activate(pane, params);
		
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
	
	navigateTo: function(hash){
		this.router.navigate(hash, {trigger: true, replace: true});
	},

    start: function(navBar) {
		this.navBar = navBar;
		this.defaultScreen = {
			activate: function(){
				$('#home-view').show();
				$('#main-row').hide();
			},
			
			deactivate: function(){
				$('#home-view').hide();
				$('#main-row').show();
			}
		};

		var router = new Backbone.Router();
		router.route('*actions', 'defaultRoute', function(){
			FubuDiagnostics.showScreen(FubuDiagnostics.defaultScreen, {title: 'Welcome to FubuMVC!', description: 'The .Net web framework that gets out of your way'}, {});
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
        return _.find(this.sections, function(s){
			return s.key == key;
		});
    },

    addView: function(view) {
		this.lastSection.add(view);
    },

	activeSection: {},
	
	
	cache: {},
	
	get: function(key, params, callback){
		var url = this.toUrl(key, params);
		
		$.get(url, callback);
	},
	
	toUrl: function(key, params){
		var route = this.routes[key];
		var url = route.url;
		_.each(route.params, function(param){
			url = url.replace('{' + param + '}', params[param]);
		});

		return url;
	},
	
	// TODO -- add cached ability

};



