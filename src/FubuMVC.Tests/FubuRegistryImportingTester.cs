using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuRegistryImportingTester
    {
        private FubuRegistry parent;
        private FubuRegistry import;

        [SetUp]
        public void SetUp()
        {
            parent = new FubuRegistry();
            import = new FubuRegistry();
        }

        [Test]
        public void register_non_fubu_service_in_imported_registry()
        {
            import.Services(s => s.AddService<ISomething>(new Something()));
            parent.Import(import, string.Empty);

            var graph = parent.BuildGraph();

            graph.Services.DefaultServiceFor<ISomething>().Value.ShouldBeOfType<Something>();
        }

        [Test]
        public void parent_registry_service_registration_is_performed_after_the_import()
        {
            import.Services(s => s.AddService<ISomething>(new Something()));
            parent.Import(import, string.Empty);

            parent.Services(x => x.ReplaceService<ISomething>(new DifferentSomething()));

            var graph = parent.BuildGraph();

            graph.Services.DefaultServiceFor<ISomething>().Value.ShouldBeOfType<DifferentSomething>();
        }

        [Test]
        public void override_a_fubu_service_in_imported_registry()
        {
            import.Services(s => s.ReplaceService<IOutputWriter>(new DifferentOutputWriter()));
            parent.Import(import, string.Empty);

            var graph = parent.BuildGraph();

            graph.Services.DefaultServiceFor<IOutputWriter>().Value.ShouldBeOfType<DifferentOutputWriter>();
        }

        [Test]
        public void adds_chains_from_import_without_prefix()
        {
            import.Route("a/m1").Calls<Action1>(x => x.M1());
            import.Route("a/m2").Calls<Action1>(x => x.M2());
            parent.Import(import, string.Empty);

            var graph = parent.BuildGraph();

            graph.BehaviorFor<Action1>(x => x.M1()).GetRoutePattern().ShouldEqual("a/m1");
            graph.BehaviorFor<Action1>(x => x.M2()).GetRoutePattern().ShouldEqual("a/m2");
        }

        [Test]
        public void adds_chains_from_import_with_a_prefix()
        {
            import.Route("a/m1").Calls<Action1>(x => x.M1());
            import.Route("a/m2").Calls<Action1>(x => x.M2());
            parent.Import(import, "import");

            var graph = parent.BuildGraph();

            graph.Behaviors.Each(b => Debug.WriteLine(b.FirstCallDescription()));

            graph.BehaviorFor<Action1>(x => x.M1()).GetRoutePattern().ShouldEqual("import/a/m1");
            graph.BehaviorFor<Action1>(x => x.M2()).GetRoutePattern().ShouldEqual("import/a/m2");
        }

        [Test]
        public void apply_policy_to_import_only_applies_to_the_chains_in_the_imported_registry()
        {
            import.Route("a/m1").Calls<Action1>(x => x.M1());
            import.Route("a/m2").Calls<Action1>(x => x.M2());

            import.Policies.WrapBehaviorChainsWith<Wrapper>();

            parent.Import(import, "import");

            parent.Route("b/m1").Calls<Action2>(x => x.M1());
            parent.Route("b/m2").Calls<Action2>(x => x.M2());

            var graph = parent.BuildGraph();

            // Chains from the import should have the wrapper
            var chain = graph.BehaviorFor<Action1>(x => x.M1());
            chain.IsWrappedBy(typeof(Wrapper)).ShouldBeTrue();

            graph.BehaviorFor<Action1>(x => x.M2()).IsWrappedBy(typeof(Wrapper)).ShouldBeTrue();
        
        
            // Chains from the parent should not have the wrapper
            graph.BehaviorFor<Action2>(x => x.M1()).IsWrappedBy(typeof(Wrapper)).ShouldBeFalse();
            graph.BehaviorFor<Action2>(x => x.M2()).IsWrappedBy(typeof(Wrapper)).ShouldBeFalse();
        }


        [Test]
        public void apply_policy_to_parent_only_applies_to_the_chains_in_the_imported_registry_and_the_parent()
        {
            import.Route("a/m1").Calls<Action1>(x => x.M1());
            import.Route("a/m2").Calls<Action1>(x => x.M2());

            

            parent.Import(import, "import");

            parent.Route("b/m1").Calls<Action2>(x => x.M1());
            parent.Route("b/m2").Calls<Action2>(x => x.M2());

            parent.Policies.WrapBehaviorChainsWith<Wrapper>();

            var graph = parent.BuildGraph();

            // Chains from the import should have the wrapper
            graph.BehaviorFor<Action1>(x => x.M1()).IsWrappedBy(typeof(Wrapper)).ShouldBeTrue();
            graph.BehaviorFor<Action1>(x => x.M2()).IsWrappedBy(typeof(Wrapper)).ShouldBeTrue();


            // Chains from the parent should also have the wrapper
            graph.BehaviorFor<Action2>(x => x.M1()).IsWrappedBy(typeof(Wrapper)).ShouldBeTrue();
            graph.BehaviorFor<Action2>(x => x.M2()).IsWrappedBy(typeof(Wrapper)).ShouldBeTrue();
        }

        [Test]
        public void import_conventions_are_applied_to_imported_registry()
        {
            var importConfig = new bool[3];

            importConfig.Each((b, i) => parent.ConfigureImports(x => importConfig[i] = import == x));

            parent.Import(import, "import");
            parent.BuildGraph();

            importConfig.Each(b => b.ShouldBeTrue());
        }


        public interface ISomething{}
        public class Something : ISomething{}
        public class DifferentSomething : ISomething{}

        public class DifferentOutputWriter : IOutputWriter
        {
            public void WriteFile(string contentType, string localFilePath, string displayName)
            {
                throw new NotImplementedException();
            }

            public void Write(string contentType, string renderedOutput)
            {
                throw new NotImplementedException();
            }

            public void RedirectToUrl(string url)
            {
                throw new NotImplementedException();
            }

            public IRecordedOutput Record(Action action)
            {
                throw new NotImplementedException();
            }

            public void Replay(IRecordedOutput output)
            {
                throw new NotImplementedException();
            }

            public void AppendCookie(HttpCookie cookie)
        	{
        		throw new NotImplementedException();
        	}

            public void AppendHeader(string key, string value)
            {
                throw new NotImplementedException();
            }

            public void Write(string contentType, Action<Stream> output)
            {
                throw new NotImplementedException();
            }

            public void WriteResponseCode(HttpStatusCode status, string description = null)
            {
                throw new NotImplementedException();
            }
        }

        public class Action1
        {
            public void M1(){}
            public void M2(){}
        }

        public class Action2
        {
            public void M1() { }
            public void M2() { }
        }

        public class Wrapper : BasicBehavior
        {
            public Wrapper() : base(PartialBehavior.Ignored)
            {
            }
        }
    }
}