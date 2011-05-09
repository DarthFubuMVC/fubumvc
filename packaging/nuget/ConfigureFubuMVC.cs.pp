using FubuMVC.Core;

namespace $rootnamespace$
{
    public class ConfigureFubuMVC : FubuRegistry
    {
        public ConfigureFubuMVC()
        {
	    // This line turns on the basic diagnostics and request tracing
            IncludeDiagnostics(true);

	    // All public methods from concrete classes ending in "Controller"
	    // in this assembly are assumed to be action methods
            Actions.IncludeClassesSuffixedWithController();
	    
            Routes
		.IgnoreControllerNamespaceEntirely()
	        .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Command"), "POST")
                .ConstrainToHttpMethod(action => action.Method.Name.StartsWith("Post"), "POST")
                .ConstrainToHttpMethod(action => action.Method.Name.StartsWith("Query"), "GET");
		
	    // Match views to action methods by 
	    Views.TryToAttachWithDefaultConventions();
        }
    }
}