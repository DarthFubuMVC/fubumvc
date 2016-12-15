using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Registration.Conventions;
using Shouldly;
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

            public Task<IRecordedOutput> Record(Func<Task> inner)
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
            public string M1()
            {
                return "Hello";
            }

            [UrlPattern("a/m2")]
            public string M2()
            {
                return "Bye";
            }
        }

        public class Action2
        {
            [UrlPattern("b/m1")]
            public string M1()
            {
                return "Hello";
            }

            [UrlPattern("b/m2")]
            public string M2()
            {
                return "Bye";
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

            graph.ChainFor<Action1>(x => x.M1()).As<RoutedChain>().GetRoutePattern().ShouldBe("import/a/m1");
            graph.ChainFor<Action1>(x => x.M2()).As<RoutedChain>().GetRoutePattern().ShouldBe("import/a/m2");
        }

        [Test]
        public void adds_chains_from_import_without_prefix()
        {
            import.Actions.IncludeType<Action1>();
            parent.Import(import, string.Empty);

            var graph = BehaviorGraph.BuildFrom(parent);

            graph.ChainFor<Action1>(x => x.M1()).As<RoutedChain>().GetRoutePattern().ShouldBe("a/m1");
            graph.ChainFor<Action1>(x => x.M2()).As<RoutedChain>().GetRoutePattern().ShouldBe("a/m2");
        }

        [Test]
        public void apply_policy_to_import_only_applies_to_the_chains_in_the_imported_registry()
        {
            import.Actions.IncludeType<Action1>();

            import.Policies.Local.Configure(g => g.WrapAllWith<Wrapper>());

            parent.Import(import, "import");

            parent.Actions.IncludeType<Action2>();

            var graph = BehaviorGraph.BuildFrom(parent);

            // Chains from the import should have the wrapper
            var chain = graph.ChainFor<Action1>(x => x.M1());
            chain.IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();

            graph.ChainFor<Action1>(x => x.M2()).IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();


            // Chains from the parent should not have the wrapper
            graph.ChainFor<Action2>(x => x.M1()).IsWrappedBy(typeof (Wrapper)).ShouldBeFalse();
            graph.ChainFor<Action2>(x => x.M2()).IsWrappedBy(typeof (Wrapper)).ShouldBeFalse();
        }


        [Test]
        public void apply_policy_to_parent_only_applies_to_the_chains_in_the_imported_registry_and_the_parent()
        {
            import.Actions.IncludeType<Action1>();


            parent.Import(import, "import");

            parent.Actions.IncludeType<Action2>();

            parent.Policies.Global.Configure(g => g.WrapAllWith<Wrapper>());

            var graph = BehaviorGraph.BuildFrom(parent);

            // Chains from the import should have the wrapper
            graph.ChainFor<Action1>(x => x.M1()).IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();
            graph.ChainFor<Action1>(x => x.M2()).IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();


            // Chains from the parent should also have the wrapper
            graph.ChainFor<Action2>(x => x.M1()).IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();
            graph.ChainFor<Action2>(x => x.M2()).IsWrappedBy(typeof (Wrapper)).ShouldBeTrue();
        }


    }
}