using System;

namespace FubuMVC.Core.Registration
{
    public interface IConfigurationAction
    {
        void Configure(BehaviorGraph graph);
    }

    [Obsolete("Kill this!")]
    public interface IKnowMyConfigurationType
    {
        string DetermineConfigurationType();
    }
}