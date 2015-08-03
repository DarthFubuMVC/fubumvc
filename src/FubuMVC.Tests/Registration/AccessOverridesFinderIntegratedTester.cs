using AssemblyPackage;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using Shouldly;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class AccessOverridesFinderIntegratedTester
    {
        [Test]
        public void accessor_rules_mechanics()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                var container = runtime.Get<IContainer>();

                container.DefaultRegistrationIs<AccessorRules, AccessorRules>();

                var accessorRules = container.GetInstance<AccessorRules>();
                
                accessorRules
                    .AllRulesFor<Target1, ColorRule>(x => x.Name)
                    .ShouldHaveTheSameElementsAs(new ColorRule("orange"));
            }
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
            return Equals((ColorRule) obj);
        }

        public override int GetHashCode()
        {
            return (_color != null ? _color.GetHashCode() : 0);
        }
    }
}