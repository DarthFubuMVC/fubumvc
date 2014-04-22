using System;
using System.IO;
using System.Net;
using System.Web;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuRegistryImportingTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            parent = new FubuRegistry();
            import = new FubuRegistry();
        }

        #endregion

        private FubuRegistry parent;
        private FubuRegistry import;


        public interface ISomething
        {
        }

        public class Something : ISomething
        {
        }

        public class DifferentSomething : ISomething
        {
        }

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

            public void Write(string renderedOutput)
            {
                throw new NotImplementedException();
            }

            public void RedirectToUrl(string url)
            {
                throw new NotImplementedException();
            }

            public void AppendCookie(Cookie cookie)
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

            public void Flush()
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
            [UrlPattern("a/m1")]
            public void M1()
            {
            }

            [UrlPattern("a/m2")]
            public void M2()
            {
            }
        }

        public class Action2
        {
            [UrlPattern("b/m1")]
            public void M1()
            {
            }

            [UrlPattern("b/m2")]
            public void M2()
            {
            }
        }

        public class Wrapper : BasicBehavior
        {
            public Wrapper() : base(PartialBehavior.Ignored)
            {
            }
        }

        [Test]
        public void adds_chains_from_import_with_a_prefix()
        {
            import.Actions.IncludeType<Action1>();

            parent.Import(import, "import");

            var graph = BehaviorGraph.BuildFrom(parent);

            graph.BehaviorFor<Action1>(x => x.M1()).As<RoutedChain>().GetRoutePattern().ShouldEqual("import/a/m1");
            graph.BehaviorFor<Action1>(x => x.M2()).As<RoutedChain>().GetRoutePattern().ShouldEqual("import/a/m2");
        }

        [Test]
        public void adds_chains_from_import_without_prefix()
        {
            import.Actions.IncludeType<Action1>();
            parent.Import(import, string.Empty);

            var graph = BehaviorGraph.BuildFrom(parent);

            graph.BehaviorFor<Action1>(x => x.M1()).As<RoutedChain>().GetRoutePattern().ShouldEqual("a/m1");
            graph.BehaviorFor<Action1>(x => x.M2()).As<RoutedChain>().GetRoutePattern().ShouldEqual("a/m2");
        }

        [Test]
        public void apply_policy_to_import_only_applies_to_the_chains_in_the_imported_registry()
        {
            import.Actions.IncludeType<Action1>();

            import.Policies.Local.Add(policy => policy.Wrap.WithBehavior<Wrapper>());

            parent.Import(import, "import");

            parent.Actions.IncludeType<Action2>();

            var graph = BehaviorGraph.BuildFrom(parent);

            // Chains from the import should have the wrapper
            var chain = graph.BehaviorFor<Action1>(x => x.M1());
            chain.IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();

            graph.BehaviorFor<Action1>(x => x.M2()).IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();


            // Chains from the parent should not have the wrapper
            graph.BehaviorFor<Action2>(x => x.M1()).IsWrappedBy(typeof (Wrapper)).ShouldBeFalse();
            graph.BehaviorFor<Action2>(x => x.M2()).IsWrappedBy(typeof (Wrapper)).ShouldBeFalse();
        }


        [Test]
        public void apply_policy_to_parent_only_applies_to_the_chains_in_the_imported_registry_and_the_parent()
        {
            import.Actions.IncludeType<Action1>();


            parent.Import(import, "import");

            parent.Actions.IncludeType<Action2>();

            parent.Policies.Global.Add(policy => policy.Wrap.WithBehavior<Wrapper>());

            var graph = BehaviorGraph.BuildFrom(parent);

            // Chains from the import should have the wrapper
            graph.BehaviorFor<Action1>(x => x.M1()).IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();
            graph.BehaviorFor<Action1>(x => x.M2()).IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();


            // Chains from the parent should also have the wrapper
            graph.BehaviorFor<Action2>(x => x.M1()).IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();
            graph.BehaviorFor<Action2>(x => x.M2()).IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();
        }

        [Test]
        public void override_a_fubu_service_in_imported_registry()
        {
            import.Services(s => s.ReplaceService<IOutputWriter>(new DifferentOutputWriter()));
            parent.Import(import, string.Empty);

            var graph = BehaviorGraph.BuildFrom(parent);

            graph.Services.DefaultServiceFor<IOutputWriter>().Value.ShouldBeOfType<DifferentOutputWriter>();
        }

        [Test]
        public void parent_registry_service_registration_is_performed_after_the_import()
        {
            import.Services(s => s.AddService<ISomething>(new Something()));
            parent.Import(import, string.Empty);

            parent.Services(x => x.ReplaceService<ISomething>(new DifferentSomething()));

            var graph = BehaviorGraph.BuildFrom(parent);

            graph.Services.DefaultServiceFor<ISomething>().Value.ShouldBeOfType<DifferentSomething>();
        }

        [Test]
        public void register_non_fubu_service_in_imported_registry()
        {
            import.Services(s => s.AddService<ISomething>(new Something()));
            parent.Import(import, string.Empty);

            var graph = BehaviorGraph.BuildFrom(parent);

            graph.Services.DefaultServiceFor<ISomething>().Value.ShouldBeOfType<Something>();
        }
    }
}