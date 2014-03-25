using AssemblyPackage;
using Bottles;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class AccessOverridesFinderIntegratedTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            PackageRegistry.LoadPackages(x => {
                x.Assembly(typeof(AssemblyPackage.Address).Assembly);
            }, false);

            PackageRegistry.PackageAssemblies.Single()
                .ShouldEqual(typeof (AssemblyPackage.Address).Assembly);

            theGraph = BehaviorGraph.BuildFrom(x => {
            });
        }

        [Test]
        public void registers_the_accessor_rules()
        {
            theGraph.Services.DefaultServiceFor<AccessorRules>()
                .Value.ShouldBeTheSameAs(theGraph.Settings.Get<AccessorRules>());
        }

        [Test]
        public void finds_overrides_from_the_application()
        {
            var rules = theGraph.Settings.Get<AccessorRules>();

            rules.AllRulesFor<Target1, ColorRule>(x => x.Name)
                .ShouldHaveTheSameElementsAs(new ColorRule("orange"));
        }

        [Test,Explicit]
        public void finds_overrides_from_package_assemblies_too()
        {
            var rules = theGraph.Settings.Get<AccessorRules>();
            rules.AllRulesFor<AssemblyPackage.Address, ElementRule>(x => x.Address1)
                .ShouldHaveTheSameElementsAs(new ElementRule("1"), new ElementRule("2"));
        }
    }

    public class Target1Overrides : OverridesFor<Target1>
    {
        public Target1Overrides()
        {
            Property(x => x.Name).Add(new ColorRule("orange"));
            Property(x => x.Age).Add(new ColorRule("blue"));
        }
    }

    public class Target1
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Target2
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public interface IRule
    {

    }

    public class ColorRule : IRule
    {
        private readonly string _color;

        public ColorRule(string color)
        {
            _color = color;
        }

        public string Color
        {
            get { return _color; }
        }

        protected bool Equals(ColorRule other)
        {
            return string.Equals(_color, other._color);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ColorRule)obj);
        }

        public override int GetHashCode()
        {
            return (_color != null ? _color.GetHashCode() : 0);
        }
    }
}