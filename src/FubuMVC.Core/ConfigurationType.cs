namespace FubuMVC.Core
{
    /// <summary>
    /// Constants for the various configuration type categories of an IConfigurationAction
    /// </summary>
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


}