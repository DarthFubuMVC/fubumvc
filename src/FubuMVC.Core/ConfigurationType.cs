namespace FubuMVC.Core
{
    public static class ConfigurationType
    {
        public const string Settings = "Settings";
        public const string Discovery = "Discovery";
        public const string Explicit = "Explicit";
        public const string Policy = "Policy";
        public const string Reordering = "Reordering";
        public const string Navigation = "Navigation";
        public const string Instrumentation = "Instrumentation";
        public const string Services = "Services";
        public const string ByNavigation = "ByNavigation";

        public const string Attributes = "Attributes";
        public const string ModifyRoutes = "ModifyRoutes";
        public const string InjectNodes = "InjectNodes";
        public const string Conneg = "Conneg";
        public const string Attachment = "Attachment";

        public const string Import = "Import";

    }


    /*
     * 
     * graph.Services.AddService(this); <----- ConfigLog
     * 
     * 
    public BehaviorGraph BuildForImport(BehaviorGraph parent)
    {
        IEnumerable<IConfigurationAction> lightweightActions = _configurations[ConfigurationType.Settings]
            .Union(_configurations[ConfigurationType.Discovery])
            //.Union(_imports)
            .Union(_configurations[ConfigurationType.Explicit])
            .Union(_configurations[ConfigurationType.Policy])
            .Union(_configurations[ConfigurationType.Reordering]);

        BehaviorGraph graph = BehaviorGraph.ForChild(parent);

        lightweightActions.Each(x => graph.Log.RunAction(_registry, x));

        return graph;
    }
     * 
     * 
     * 
     * 
    public IEnumerable<IConfigurationAction> AllConfigurationActions()
    {
        return _configurations[ConfigurationType.Settings]
            .Union(_configurations[ConfigurationType.Discovery])
            //.Union(UniqueImports())
            .Union(_configurations[ConfigurationType.Explicit])
            .Union(_configurations[ConfigurationType.Policy])
            .Union(_configurations[ConfigurationType.Navigation])
            .Union(_configurations[ConfigurationType.ByNavigation])
            .Union(_configurations[ConfigurationType.Attributes])
            .Union(_configurations[ConfigurationType.ModifyRoutes])
            .Union(_configurations[ConfigurationType.InjectNodes])
            .Union(_configurations[ConfigurationType.Conneg])
            .Union(_configurations[ConfigurationType.Attachment])

            .Union(_configurations[ConfigurationType.Reordering])
            .Union(_configurations[ConfigurationType.Instrumentation]);
    }
     */
}