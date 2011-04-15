using System.Diagnostics;
using Bottles.Deployment;
using Bottles.Deployment.Writing;
using Bottles.Tests.Deployment.Writing;
using NUnit.Framework;

namespace Bottles.Tests.Deployment.Parsing
{

    public class OneSettings : IDirective
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class TwoSettings : IDirective
    {
        public string City { get; set; }
        public bool IsLocal { get; set; }
    }

    public class ThreeSettings : IDirective
    {
        public int Threshold { get; set; }
        public string Direction { get; set; }
    }

    [TestFixture]
    public class read_a_single_recipe
    {
        [SetUp]
        public void SetUp()
        {
            var writer = new ProfileWriter("profile2");

            var host = writer.RecipeFor("r1").HostFor("h1");
            
            host.AddDirective(new SimpleSettings{
                One = "one",
                Two = "two"
            });

            host.AddDirective(new OneSettings()
            {
                Name = "Jeremy",
                Age = 37
            });

            host.AddReference(new BottleReference()
            {
                Name = "bottle1"
            });

            host.AddReference(new BottleReference()
            {
                Name = "bottle2",
                Relationship = "binaries"
            });

            writer.Flush();


        }

        [Test]
        public void TESTNAME()
        {
            Debug.WriteLine("go");
        }
    }
}