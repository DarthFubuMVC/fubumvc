using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.Conventions
{
    public class RegisterAllSettings : ServiceRegistry
    {
        public RegisterAllSettings(BehaviorGraph graph)
        {
            graph.Settings.ForAllSettings((type, o) => SetServiceIfNone(type, ObjectDef.ForValue(o)));
        }
    }


}