using System;
using FubuMVC.Core;
using FubuMVC.HelloWorld.Services;
using FubuMVC.StructureMap.Bootstrap;
using StructureMap;

namespace FubuMVC.HelloWorld
{
    public class Global : FubuStructureMapApplication
    {
        public override FubuRegistry GetMyRegistry()
        {
            return new HelloWorldFubuRegistry();
        }

        protected override void InitializeStructureMap(IInitializationExpression ex)
        {
            ex.For<IHttpSession>().Use<CurrentHttpContextSession>();
        }
    }
}