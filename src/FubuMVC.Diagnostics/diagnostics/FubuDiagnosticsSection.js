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
