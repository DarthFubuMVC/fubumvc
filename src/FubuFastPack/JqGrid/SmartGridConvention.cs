using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Security;

namespace FubuFastPack.JqGrid
{
    public class SmartGridConvention : IConfigurationAction
    {
        private readonly TypePool _types;

        public SmartGridConvention(TypePool types)
        {
            _types = types;
        }

        public void Configure(BehaviorGraph graph)
        {
            _types.TypesMatching(t => t.IsConcreteTypeOf<ISmartGrid>()).Each(t =>
            {
                buildChain(graph, t);
                addHarnessToServices(graph, t);
            });
        }

        private void addHarnessToServices(BehaviorGraph graph, Type type)
        {
            var key = type.NameForGrid();

            var harnessType = typeof (SmartGridHarness<>).MakeGenericType(type);
            var objectDef = graph.Services.AddService<ISmartGridHarness>(harnessType);
            objectDef.Name = key;
        }

        private void buildChain(BehaviorGraph graph, Type t)
        {
            var chain = graph.AddChain();
            chain.Origin = "SmartGridConvention";
            chain.Route = new RouteDefinition("_griddata/" + t.NameForGrid().ToLower());

            var call = typeof (GridActionCall<>).CloseAndBuildAs<ActionCall>(t);
            chain.AddToEnd(call);

            t.GetAllAttributes<AuthorizationAttribute>().Each(att => att.Alter(call));

            chain.AddToEnd(new RenderJsonNode(typeof (GridResults)));
        }
    }
}