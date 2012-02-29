using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.AspNet;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using FubuMVC.Tests.Behaviors;
using FubuTestingSupport;
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

        public class FakeJsonBehavior : ConnegOutputBehavior<Output>
        {
            public FakeJsonBehavior(IJsonWriter writer, IFubuRequest request, IRequestData data) : base(null, request, null, new IMediaWriter<Output>[0])
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
            def.DependencyByValue(typeof (IFubuRequest), request);
            var jsonWriter = def.DependencyByType(typeof (IJsonWriter), typeof (AjaxAwareJsonWriter));
            jsonWriter.DependencyByType(typeof (IOutputWriter), typeof (OutputWriter));
            jsonWriter.DependencyByType(typeof(IRequestData), typeof(InMemoryRequestData));
            def.DependencyByType(typeof (IRequestData), typeof (InMemoryRequestData));

            var container =
                new Container(x =>
                {
                    x.For<IFileSystem>().Use<FileSystem>();
                    x.For<IHttpWriter>().Use<NulloHttpWriter>();
                    x.For<IActionBehavior>().Use(new ObjectDefInstance(def));
                });

            var jsonBehavior = container.GetInstance<IActionBehavior>().ShouldBeOfType<FakeJsonBehavior>();
            jsonBehavior.Writer.ShouldBeOfType<AjaxAwareJsonWriter>();
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

        [Test]
        public void build_an_object_with_a_list_dependency()
        {
            var def = new ObjectDef(typeof (ClassWithSomethings));


            var listDependency = def.ListDependenciesOf<ISomething>();
                
            listDependency.AddType(typeof (SomethingA));
            listDependency.AddType(typeof (SomethingB));
            listDependency.AddValue(new SomethingC());

            var instance = new ObjectDefInstance(def);

            var container = new Container();
            var @object = container.GetInstance<ClassWithSomethings>(instance);

            @object.Somethings[0].ShouldBeOfType<SomethingA>();
            @object.Somethings[1].ShouldBeOfType<SomethingB>();
            @object.Somethings[2].ShouldBeOfType<SomethingC>();
        }

        // IOutputWriter writer, IFubuRequest request
    }

    public interface ISomething{}
    public class SomethingA : ISomething{}
    public class SomethingB : ISomething{}
    public class SomethingC : ISomething{}

    public class ClassWithSomethings
    {
        private readonly IList<ISomething> _somethings;

        public ClassWithSomethings(IList<ISomething> somethings)
        {
            _somethings = somethings;
        }

        public IList<ISomething> Somethings
        {
            get { return _somethings; }
        }
    }
}