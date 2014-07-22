function FubuDiagnosticsView(view, section){
	view.anchor = '#' + view.url;
	
	if (view.route != null && typeof view.route == 'string'){
		view.route = FubuDiagnostics.routes[view.route];
	}
	
	view.url = section.key + '/' + view.key;

	if (view.route){
		_.each(view.route.params, function(param, i){
			view.url = view.url + '/:' + param;
		});
		
	}
	
	if (view.applyRoutes == null){
		view.applyRoutes = function(router, section){
			var key = section.key + '/' + view.key;
		
			router.route(view.url, key, function(){
				var params = {};
				if (view.route){
					for (var i = 0; i < view.route.params.length; i++){
						params[view.route.params[i]] = arguments[i];
					}
				}

				FubuDiagnostics.showScreen(view.screen, view, section, params);
			});
		}
	}
	

	
	view.anchor = '#' + view.url;
	
	view.hasParameters = function(){
		if (view.route == null) return false;
		
		return view.route.params.length > 0;
	}

}

function FubuDiagnosticsSection(section){
	$.extend(section, {
		views: [],
		
		anchor: '#' + section.key,
		
		add: function(view){
			this.views.push(view);
			FubuDiagnosticsView(view, section);

			return this;
		},

		addRoutes: function(router){
			if (this.screen == null){
				this.screen = new ReactScreen({
					component: FubuDiagnostics.components.SectionLinks, 
					subject: {section: this}
				});
			}

			var section = this;
			_.each(this.views, function(view){
				view.applyRoutes(router, section);
			});
		
			router.route(this.key, this.key, function(){
				FubuDiagnostics.showScreen(section.screen, section, section);
			});
			

		},
		
		activeViews: function(){
			return _.filter(this.views, function(v){
				return !v.hasParameters();
			});
		},
		
		
	});
}	
