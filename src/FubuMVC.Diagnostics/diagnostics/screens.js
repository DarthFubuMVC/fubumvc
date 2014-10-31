// TODO -- need to add an option for cached!
function ReactScreen(config){
	var self = this;
	

	self.buildComponent = function(pane, data){
		self.component = config.component(data);
		React.unmountComponentAtNode(pane);
		React.renderComponent(self.component, pane);
		
		self.deactivate = function(){
			React.unmountComponentAtNode(pane);
		};
	}
	
	if (config.options){
		self.constructData = function(data){
			return $.extend({}, config.options, {data: data});
		}
	}
	else {
		self.constructData = function(data){
			return data;
		}
	}
	
	if (config.subject != null){
		self.activate = function(pane){
			self.buildComponent(pane, config.subject);
		}
	}
	else if (config.route != null){
		self.activate = function(pane, params){
			self.pane = pane;
		
			FubuDiagnostics.get(config.route, params, function (data) {
			    if (typeof data == 'string') {
			        data = JSON.parse(data);
			    }


				data = self.constructData(data);
				self.buildComponent(pane, data);
			});
		};
	}
	
	return self;

}

function ServerScreen(route){
	this.activate = function(pane, params){
		FubuDiagnostics.get(route, params, function(data){
			$(pane).html(data);
		});
	}
	
	this.deactivate = function(){
	
	}
}

function TextScreen(route){
	this.route = route;

	this.activate = function(pane, params){
		FubuDiagnostics.get(route, params, function(data){
			$('<pre class="text-display"></pre>').html(data).appendTo(pane);
		});
	}
	
	this.deactivate = function(){
	
	}
}