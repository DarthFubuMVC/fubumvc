using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class RouteCollectionTester
    {
        private readonly BehaviorGraph theGraph = BehaviorGraph.BuildFrom(_ =>
        {
            _.Actions.DisableDefaultActionSource();
            _.Actions.IncludeType<FakeActions>();
            _.AlterSettings<ViewEngineSettings>(x => x.ExcludeViews(t => true));
        });

        private void assertHasRoutes(IEnumerable<RoutedChain> chains, params int[] numbers)
        {
            chains.Where(x => x.HandlerTypeIs<FakeActions>())
                .Select(x => int.Parse(x.GetRoutePattern())).OrderBy(x => x)
                .ShouldHaveTheSameElementsAs(numbers);
        }


        [Test]
        public void all_routes()
        {
            assertHasRoutes(theGraph.Routes, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }

        [Test]
        public void gets()
        {
            assertHasRoutes(theGraph.Routes.Gets, 2, 7);
        }

        [Test]
        public void heads()
        {
            assertHasRoutes(theGraph.Routes.Heads, 1, 6);
        }

        [Test]
        public void posts()
        {
            assertHasRoutes(theGraph.Routes.Posts, 3, 8);
        }

        [Test]
        public void puts()
        {
            assertHasRoutes(theGraph.Routes.Puts, 5, 10);
        }

        [Test]
        public void deletes()
        {
            assertHasRoutes(theGraph.Routes.Deletes, 4, 9);
        }

        [Test]
        public void resources()
        {
            assertHasRoutes(theGraph.Routes.Resources, 1, 2, 6, 7);
        }

        [Test]
        public void commands()
        {
            assertHasRoutes(theGraph.Routes.Commands, 3, 4, 5, 8, 9, 10);
        }
    }

    public class FakeActions
    {
        public string head_1()
        {
            return "1";
        }

        public string get_2()
        {
            return "1";
        }

        public string post_3()
        {
            return "1";
        }

        public string delete_4()
        {
            return "1";
        }

        public string put_5()
        {
            return "1";
        }

        public string head_6()
        {
            return "1";
        }

        public string get_7()
        {
            return "1";
        }

        public string post_8()
        {
            return "1";
        }

        public string delete_9()
        {
            return "1";
        }

        public string put_10()
        {
            return "1";
        }
    }
}