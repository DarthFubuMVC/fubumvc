﻿using System;
using FubuMVC.Core.Security;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.Extensibility
{



    public class ExtensionsExpression
    {
        private readonly FubuRegistry _registry;

        public ExtensionsExpression(FubuRegistry registry)
        {
            _registry = registry;
        }

        private ExtensionsExpression register(Action<ContentExtensionGraph> configure)
        {
            _registry.Services(x => x.SetServiceIfNone(new ContentExtensionGraph()));

            _registry.Configure(graph =>
            {
                var contentGraph = graph.Services.DefaultServiceFor<ContentExtensionGraph>().Value as ContentExtensionGraph;
                configure(contentGraph);
            });

            return this;
        }

        public ExtensionsExpression For<T>(string tag, IContentExtension<T> extension) where T : class
        {
            return register(g => g.Register(tag, extension));
        }

        public ExtensionsExpression For<T>(IContentExtension<T> extension) where T : class
        {
            return register(g => g.Register(extension));
        }

        public ExtensionsExpression For<T>(Func<IFubuPage<T>, object> func) where T : class
        {
            return For(new LambdaExtension<T>(func));
        }

        public ExtensionsExpression For<T>(string tag, Func<IFubuPage<T>, object> func) where T : class
        {
            return For(tag, new LambdaExtension<T>(func));
        }

        // TODO -- will need something more generic later.  Maybe.
        public ExtensionsExpression OnlyForRoles(params string[] roles)
        {
            Func<bool> filter = () => PrincipalRoles.IsInRole(roles);
            return register(g => g.FilterLast(filter));
        }
    }
}