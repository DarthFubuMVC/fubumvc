using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Registration
{
    // TODO -- move this to StructureMap
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SingletonAttribute : InstanceAttribute
    {
        public override void Alter(IConfiguredInstance instance)
        {
            instance.SetLifecycleTo<SingletonLifecycle>();
        }
    }
}