using Bottles.Deployment;
using Bottles.Tests.Deployment.Parsing;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace Bottles.Tests
{
    [TestFixture]
    public class RecipeTester
    {
        [Test]
        public void append_recipe_pushes_hosts_from_one_to_another()
        {
            var recipe1 = new Recipe("1");
            var recipe2 = new Recipe("2");
        
            recipe1.HostFor("h1").RegisterValue<OneSettings>(x => x.Age, 1);
            recipe2.HostFor("h2").RegisterValue<TwoSettings>(x => x.City, "Jasper");

            recipe1.AppendBehind(recipe2);

            recipe1.Hosts.Select(x => x.Name).ShouldHaveTheSameElementsAs("h1", "h2");
        }

        [Test]
        public void append_recipe_appends_hosts()
        {
            var recipe1 = new Recipe("1");
            var recipe2 = new Recipe("2");

            recipe1.HostFor("h1").RegisterValue<OneSettings>(x => x.Age, 1);
            recipe2.HostFor("h1").RegisterValue<OneSettings>(x => x.Name, "Tommy");

            recipe1.AppendBehind(recipe2);

            var host = recipe1.HostFor("h1");

            var settings = host.GetDirective<OneSettings>();

            settings.Age.ShouldEqual(1);
            settings.Name.ShouldEqual("Tommy");
        }

        [Test]
        public void append_recipe_host_data_precedence_favors_the_dependee()
        {
            var recipe1 = new Recipe("1");
            var recipe2 = new Recipe("2");

            recipe1.HostFor("h1").RegisterValue<OneSettings>(x => x.Age, 1);
            recipe2.HostFor("h1").RegisterValue<OneSettings>(x => x.Name, "Tommy");
            recipe2.HostFor("h1").RegisterValue<OneSettings>(x => x.Age, 15);

            recipe1.AppendBehind(recipe2);

            var host = recipe1.HostFor("h1");

            var settings = host.GetDirective<OneSettings>();
            settings.Age.ShouldEqual(1);
        }
    }
}