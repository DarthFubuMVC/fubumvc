using System;
using FubuMVC.Core;

namespace FubuMVC.Media
{
    public class MediaBottleRegistrations : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services<ResourcesServiceRegistry>();
        }
    }
}