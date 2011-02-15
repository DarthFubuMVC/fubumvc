using System;
using FubuCore;
using FubuFastPack.Binding;
using FubuMVC.Core;

namespace FubuFastPack
{
    public class FubuFastPackRegistryExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services(x => x.SetServiceIfNone<IObjectConverter, FastPackObjectConverter>());
        }
    }
}