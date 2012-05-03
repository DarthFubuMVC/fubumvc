using System;
using FubuMVC.Core.Registration.Nodes;
using FubuCore;

namespace FubuMVC.Core.Registration
{
    public interface IConfigurationAction
    {
        void Configure(BehaviorGraph graph);
    }


}