using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.PathBased;
using FubuTestingSupport;
using NUnit.Framework;
using SpecificationExtensions = FubuTestingSupport.SpecificationExtensions;
using FubuCore;

namespace FubuMVC.Tests.Resources.PathBased
{
    [TestFixture]
    public class ResourcePathBinderTester
    {
        private AggregateDictionary theAggregateDictionary;
        private Dictionary<string, object> theRouteValues;
        private StandardModelBinder theBinder;

        [SetUp]
        public void SetUp()
        {
            Assert.Fail("Use the new BindingScenario stuff");
            //theBinder = StandardModelBinder.Basic().As<StandardModelBinder>();

            theAggregateDictionary = new AggregateDictionary();

            var otherDictionary = new Dictionary<string, object>();
            for (int i = 0; i < 10; i++)
            {
                otherDictionary.Add("Part" + i, Guid.NewGuid().ToString());
            }  

            theAggregateDictionary.AddDictionary(RequestDataSource.Request.ToString(), otherDictionary);

            
            theRouteValues = new Dictionary<string, object>();

            theAggregateDictionary.AddDictionary(RequestDataSource.Route.ToString(), theRouteValues);

          
        }

        private ResourcePath findPathByBinding()
        {
            var locator = new SelfMockingServiceLocator();
            locator.Stub(theAggregateDictionary);
            var context = new BindingContext(new RequestData(theAggregateDictionary), locator, new NulloBindingLogger());

            return (ResourcePath)new ResourcePathBinder(theBinder).Bind(typeof(ResourcePath), context);
        }

        [Test]
        public void resource_binder_is_registered_by_default()
        {
            new FubuRegistry().BuildGraph().Services
                .ServicesFor<IModelBinder>()
                .Any(x => x.Type == typeof(ResourcePathBinder)).ShouldBeTrue();
        }

        [Test]
        public void matches_subclass_of_resource_path()
        {
            new ResourcePathBinder(theBinder).Matches(typeof(SpecialPath))
                .ShouldBeTrue();
        }

        [Test]
        public void matches_ResourcePath_itself()
        {
            new ResourcePathBinder(theBinder).Matches(typeof(ResourcePath))
                .ShouldBeTrue();
        }

        [Test]
        public void does_not_match_on_a_non_resource_path_class()
        {
            new ResourcePathBinder(theBinder).Matches(GetType())
                .ShouldBeFalse();
        }

        [Test]
        public void bind_with_one_part()
        {
            theRouteValues.Add("Part0", "file.js");

            findPathByBinding().Path.ShouldEqual("file.js");
        }

        [Test]
        public void bind_with_two_parts()
        {
            
            theRouteValues.Add("Part0", "f1");
            theRouteValues.Add("Part1", "file.js");

            findPathByBinding().Path.ShouldEqual("f1/file.js");
        }

        [Test]
        public void bind_with_three_parts()
        {

            theRouteValues.Add("Part0", "f1");
            theRouteValues.Add("Part1", "f2");
            theRouteValues.Add("Part2", "file.js");

            findPathByBinding().Path.ShouldEqual("f1/f2/file.js");
        }

        [Test]
        public void bind_with_four_parts()
        {

            theRouteValues.Add("Part0", "f1");
            theRouteValues.Add("Part1", "f2");
            theRouteValues.Add("Part2", "f3");
            theRouteValues.Add("Part3", "file.js");

            findPathByBinding().Path.ShouldEqual("f1/f2/f3/file.js");
        }

        [Test]
        public void bind_with_eight_parts()
        {

            theRouteValues.Add("Part0", "f1");
            theRouteValues.Add("Part1", "f2");
            theRouteValues.Add("Part2", "f3");
            theRouteValues.Add("Part3", "f4");
            theRouteValues.Add("Part4", "f5");
            theRouteValues.Add("Part5", "f6");
            theRouteValues.Add("Part6", "f7");
            theRouteValues.Add("Part7", "f8");
            theRouteValues.Add("Part8", "file.js");

            findPathByBinding().Path.ShouldEqual("f1/f2/f3/f4/f5/f6/f7/f8/file.js");
        }

        public class SpecialPath : ResourcePath
        {
            public SpecialPath(string path) : base(path)
            {
            }
        }
    }
}