using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Resources.Conneg
{
    [TestFixture]
    public class ConnegGraphTester
    {
        private readonly ConnegGraph graph = ConnegGraph.Build(new BehaviorGraph { ApplicationAssembly = Assembly.GetExecutingAssembly() });

        [Test]
        public void build_conneg_graph_for_the_app_domain()
        {;

            graph.Writers.ShouldContain(typeof(Resource1Writer1));
            graph.Writers.ShouldContain(typeof(Resource1Writer2));
            graph.Writers.ShouldContain(typeof(Resource1Writer3));
            graph.Writers.ShouldContain(typeof(Resource2Writer1));
            graph.Writers.ShouldContain(typeof(Resource2Writer2));
            graph.Writers.ShouldContain(typeof(Resource2Writer3));

            graph.Readers.ShouldContain(typeof(Input1Reader1));
            graph.Readers.ShouldContain(typeof(Input1Reader2));
            graph.Readers.ShouldContain(typeof(Input2Reader));
        }

        [Test]
        public void find_writers_for_resource_type()
        {
            graph.WriterTypesFor(typeof(Resource2)).ShouldHaveTheSameElementsAs(typeof(Resource2Writer1), typeof(Resource2Writer2), typeof(Resource2Writer3));
        }

        [Test]
        public void find_readers_for_input_type()
        {
            graph.ReaderTypesFor(typeof (Input2)).Single()
                .ShouldEqual(typeof (Input2Reader));
        }

        
    }

    public class Input1{}
    public class Input2{}

    public class Resource1{}
    public class Resource2{}


    public class Resource1Writer1 : Projection<Resource1>{}
    public class Resource1Writer2 : Projection<Resource1>{}
    public class Resource1Writer3 : Projection<Resource1>{}
    public class Resource2Writer1 : Projection<Resource2>{}
    public class Resource2Writer2 : Projection<Resource2>{}
    public class Resource2Writer3 : Projection<Resource2>{}


    public class Input1Reader1 : IReader<Input1>
    {
        public IEnumerable<string> Mimetypes { get; private set; }
        public Input1 Read(string mimeType, IFubuRequestContext context)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Input1Reader2 : Input1Reader1{}


    public class Input2Reader : IReader<Input2>
    {
        public IEnumerable<string> Mimetypes { get; private set; }
        public Input2 Read(string mimeType, IFubuRequestContext context)
        {
            throw new System.NotImplementedException();
        }
    }

}