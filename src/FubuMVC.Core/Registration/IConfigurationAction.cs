using System;

namespace FubuMVC.Core.Registration
{
    public interface IConfigurationAction
    {
        void Configure(BehaviorGraph graph);
    }
}