﻿using FakeTestNamespaceForAutoImport;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.View
{
    
    public class AutoImportModelNamespacesConventionTester
    {
        private FubuRegistry _registry;

        public AutoImportModelNamespacesConventionTester()
        {
            _registry = new FubuRegistry();
            _registry.Policies.Local.Add<AutoImportModelNamespacesConvention>();
            _registry.Actions.IncludeType<FakeController>();
            _registry.Actions.IncludeType<FakeAction>();
        }

        [Fact]
        public void when_auto_import_is_true_namespaces_are_added()
        {
            var graph = BehaviorGraph.BuildFrom(_registry);
            var commonViewNamespaces = graph.Settings.Get<CommonViewNamespaces>();
            commonViewNamespaces.Namespaces.ShouldContain("FakeTestNamespaceForAutoImport");
            commonViewNamespaces.Namespaces.ShouldContain(typeof(FakeAction).Namespace);
        }

        [Fact]
        public void when_auto_import_is_false_namespaces_are_not_added()
        {
            _registry.AlterSettings<CommonViewNamespaces>(x => x.DontAutoImportWhenNamespaceStartsWith("Fubu"));

            var graph = BehaviorGraph.BuildFrom(_registry);
            var commonViewNamespaces = graph.Settings.Get<CommonViewNamespaces>();
            commonViewNamespaces.Namespaces.ShouldContain("FakeTestNamespaceForAutoImport");
            commonViewNamespaces.Namespaces.ShouldNotContain(typeof(FakeAction).Namespace);
        }
    }

    public class FakeAction
    {
        public FakeViewModel Execute(FakeInputModel input)
        {
            return new FakeViewModel();
        }
    }

    public class FakeInputModel
    {
    }

    public class FakeViewModel
    {
    }


}


namespace FakeTestNamespaceForAutoImport
{
    public class FakeController
    {
        public FakeViewModel Execute(FakeInputModel input)
        {
            return new FakeViewModel();
        }
    }

    public class FakeInputModel
    {
    }

    public class FakeViewModel
    {
    }
}