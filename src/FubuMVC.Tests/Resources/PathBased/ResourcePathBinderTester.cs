using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.PathBased;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using SpecificationExtensions = Shouldly.SpecificationExtensions;
using FubuCore;

namespace FubuMVC.Tests.Resources.PathBased
{
    [TestFixture]
    public class ResourcePathBinderTester
    {
        private Dictionary<string, string> theRouteValues;
        private StandardModelBinder theBinder;
        private RequestData theRequestData;

        [SetUp]
        public void SetUp()
        {
            theBinder = new StandardModelBinder(new BindingRegistry(), new TypeDescriptorCache());

            theRequestData = new RequestData();

            var otherDictionary = new Dictionary<string, string>();
            for (int i = 0; i < 10; i++)
            {
                otherDictionary.Add("Part" + i, Guid.NewGuid().ToString());
            }  

            theRequestData.AddValues(RequestDataSource.Request, new DictionaryKeyValues(otherDictionary));

            
            theRouteValues = new Dictionary<string, string>();

            theRequestData.AddValues(RequestDataSource.Route.ToString(), new DictionaryKeyValues(theRouteValues));

          
        }

        private ResourcePath findPathByBinding()
        {
            var locator = new SelfMockingServiceLocator();
            locator.Stub<IRequestData>(theRequestData);
            var context = new BindingContext(theRequestData, locator, new NulloBindingLogger());

            return (ResourcePath)new ResourcePathBinder().Bind(typeof(ResourcePath), context);
        }


        [Test]
        public void matches_subclass_of_resource_path()
        {
            new ResourcePathBinder().Matches(typeof(SpecialPath))
                .ShouldBeTrue();
        }

        [Test]
        public void matches_ResourcePath_itself()
        {
            new ResourcePathBinder().Matches(typeof(ResourcePath))
                .ShouldBeTrue();
        }

        [Test]
        public void does_not_match_on_a_non_resource_path_class()
        {
            new ResourcePathBinder().Matches(GetType())
                .ShouldBeFalse();
        }

        [Test]
        public void bind_with_one_part()
        {
            theRouteValues.Add("Part0", "file.js");

            findPathByBinding().Path.ShouldBe("file.js");
        }

        [Test]
        public void bind_with_two_parts()
        {
            
            theRouteValues.Add("Part0", "f1");
            theRouteValues.Add("Part1", "file.js");

            findPathByBinding().Path.ShouldBe("f1/file.js");
        }

        [Test]
        public void bind_with_three_parts()
        {

            theRouteValues.Add("Part0", "f1");
            theRouteValues.Add("Part1", "f2");
            theRouteValues.Add("Part2", "file.js");

            findPathByBinding().Path.ShouldBe("f1/f2/file.js");
        }

        [Test]
        public void bind_with_four_parts()
        {

            theRouteValues.Add("Part0", "f1");
            theRouteValues.Add("Part1", "f2");
            theRouteValues.Add("Part2", "f3");
            theRouteValues.Add("Part3", "file.js");

            findPathByBinding().Path.ShouldBe("f1/f2/f3/file.js");
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

            findPathByBinding().Path.ShouldBe("f1/f2/f3/f4/f5/f6/f7/f8/file.js");
        }

        public class SpecialPath : ResourcePath
        {
            public SpecialPath(string path) : base(path)
            {
            }
        }
    }
}