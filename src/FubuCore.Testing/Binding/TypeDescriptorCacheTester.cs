using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class TypeDescriptorCacheTester
    {
        private TypeDescriptorCache _cache;

        private class Item
        {
            public string Property1 { get; set; }
            public int Property2 { get; set; }
            public string NonWriteProp { get { return string.Empty; } }
        }

        [SetUp]
        public void SetUp()
        {
            _cache = new TypeDescriptorCache();
        }

        [Test]
        public void for_each_property_invokes_action_on_each_property()
        {
            var props = new List<string>();
            _cache.ForEachProperty(typeof(Item), p=>props.Add(p.Name));
            props.ShouldHaveCount(2);
            props.ShouldContain("Property1");
            props.ShouldContain("Property2");
            props.ShouldNotContain("NonWriteProp");
        }

        [Test]
        public void get_properties_for_returns_type_properties()
        {
            var props = new List<string>();
            _cache.ForEachProperty(typeof(Item), p => props.Add(p.Name));
            var itemProperties = _cache.GetPropertiesFor<Item>();
            itemProperties.Keys.ToList().ShouldHaveTheSameElementsAs(props as IEnumerable<string>);
        }

        [Test]
        public void clear_all_removes_all_property_dictionaries()
        {
            //NOTE: how can this be verified?
            _cache.ClearAll();
        }
    }
}