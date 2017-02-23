using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Navigation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Navigation
{
    
    public class MenuItemAttributes_integrated_Tester
    {

        [Fact]
        public void puts_the_navigation_graph_in_the_right_order()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            registry.Import<NavigationRegistryExtension>();

            using (var runtime = registry.ToRuntime())
            {
                var resolver = runtime.Get<IMenuResolver>();

                resolver.MenuFor(new NavigationKey("Root")).AllNodes().Select(x => x.Key.Key)
                    .ShouldHaveTheSameElementsAs("Three", "Two", "One", "Four");

                runtime.Get<NavigationGraph>()
                    .FindNode(new NavigationKey("Two")).ShouldBeOfType<MenuNode>().Previous.Key.Key.ShouldBe("Three");

            }


        }

        public class Controller1
        {
            [MenuItem("One", AddChildTo = "Two")]
            public void One(Input1 input){}

            [MenuItem("Two", AddChildTo = "Root")]
            public void Two(Input1 input){}

            [MenuItem("Three", AddBefore = "Two")]
            public void Three(Input1 input){}

            [MenuItem("Four", AddChildTo = "Root")]
            public void Four(Input1 input) { }


            public class Input1 { }
        }
    }

    
}