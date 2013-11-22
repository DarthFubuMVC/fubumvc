using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;

namespace AutoImportTarget
{
    public class AutoImportRegistry : FubuRegistry
    {
    
    }

    [AutoImport]
    public class FindHandlers : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<FooHandler>();
        }
    }

    [AutoImport]
    public class WrapWithFoo : Policy
    {
        public WrapWithFoo()
        {
            Wrap.WithBehavior<FooWrapper>();
        }
    }

    public class FooWrapper : WrappingBehavior
    {
        protected override void invoke(Action action)
        {
            action();
        }
    }

    public class FooHandler
    {
        public string get_hello()
        {
            return "Hello";
        }
    }
}