

var FubuDiagnostics = {
    start: function(navBar) {
		this.navBar = navBar;

		this.router = new Backbone.Router();

		this.router.route('*actions', 'defaultRoute', function(){
			alert('default route');
		});
		
		_.each(this.sections, function(s){
			s.addRoutes(this.router);
		});
		
		Backbone.history.start();
    },

	refreshNavigation: function(){
		this.navBar.setProps(this);
	},
	
    sections: [],
        
    addSection: function(section) {
        this.sections.push(section);
            
		$.extend(section, {
			views: [],
			add: function(view){
				this.views.push(view);

				view.url = '#' + this.key + '/' + view.key;
				return this;
			},
			url: '#' + section.key,
			addRoutes: function(router){
				// nothing yet
			}
		});
			
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
