using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using FubuMVC.Tests.Behaviors;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.StructureMapIoC
{
    [TestFixture]
    public class ObjectDefInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public class FakeJsonBehavior : RenderJsonBehavior<Output>
        {
            public FakeJsonBehavior(IJsonWriter writer, IFubuRequest request, IRequestData data)
                : base(writer, request)
            {
                Writer = writer;
                Request = request;
                Data = data;
            }

            public IJsonWriter Writer { get; set; }
            public IFubuRequest Request { get; set; }
            public IRequestData Data { get; set; }
        }

        [Test]
        public void build_an_object_with_dependencies()
        {
            var request = new InMemoryFubuRequest();


            var def = new ObjectDef(typeof (FakeJsonBehavior));
            def.Child(typeof (IFubuRequest), request);
            var jsonWriter = def.Child(typeof (IJsonWriter), typeof (JsonWriter));
            jsonWriter.Child(typeof (IOutputWriter), typeof (HttpResponseOutputWriter));
            jsonWriter.Child(typeof(IRequestData), typeof(InMemoryRequestData));
            def.Child(typeof (IRequestData), typeof (InMemoryRequestData));

            var container =
                new Container(x => { x.For<IActionBehavior>().Use(new ObjectDefInstance(def)); });

            var jsonBehavior = container.GetInstance<IActionBehavior>().ShouldBeOfType<FakeJsonBehavior>();
            jsonBehavior.Writer.ShouldBeOfType<JsonWriter>();
            jsonBehavior.Request.ShouldBeTheSameAs(request);
        }

        [Test]
        public void build_an_object_with_no_children()
        {
            Type type = typeof (TestBehavior);
            var def = new ObjectDef
            {
                Type = type
            };

            var container =
                new Container(x => { x.For<IActionBehavior>().Use(new ObjectDefInstance(def)); });

            container.GetInstance<IActionBehavior>().ShouldBeOfType<TestBehavior>();
        }

        [Test]
        public void name_of_the_instance_comes_from_the_name_of_the_inner_object_def()
        {
            var def = new ObjectDef(typeof (FakeJsonBehavior));
            var instance = new ObjectDefInstance(def);

            instance.Name.ShouldEqual(def.Name);
        }

        // IOutputWriter writer, IFubuRequest request
    }
}