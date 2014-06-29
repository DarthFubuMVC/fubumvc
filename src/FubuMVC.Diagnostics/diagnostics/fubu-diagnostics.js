

var FubuDiagnostics = {
    start: function(navBar) {
		this.navBar = navBar;
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
				view.section = this;
			},
			url: '#' + section.key
		});
			
        this.lastSection = section;
    },
    
    section: function(key) {
        
    },

    addView: function(view) {
		this.lastSection
    },

	activeSection: {views: []},
};
