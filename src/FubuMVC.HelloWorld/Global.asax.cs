using System;
using System.Web;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.HelloWorld.Services;
using FubuMVC.StructureMap.Bootstrap;
using StructureMap;
using FubuMVC.StructureMap;

namespace FubuMVC.HelloWorld
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            FubuApplication
                .For<HelloWorldFubuRegistry>()
                .StructureMap(() =>
                {
                    return new Container(x => x.For<IHttpSession>().Use<CurrentHttpContextSession>());    
                })
                .Bootstrap(RouteTable.Routes);
        }

    }
}