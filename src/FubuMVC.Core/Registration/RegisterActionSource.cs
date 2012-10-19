using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration
{
    [ConfigurationType(ConfigurationType.Settings)]
    public class RegisterActionSource : IConfigurationAction, DescribesItself
    {
        private readonly IActionSource _source;

        public RegisterActionSource(IActionSource source)
        {
            _source = source;
        }

        public void Configure(BehaviorGraph graph)
        {
            graph.Settings.Alter<ActionSources>(x => x.AddSource(_source));
        }

        public void Describe(Description description)
        {
            description.Title = "Adding an Action Source";
            description.Children["Source"] = Description.For(_source);
        }
    }
}