using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Json;
using FubuMVC.Core.Registration;
using StructureMap.Graph.Scanning;

namespace FubuMVC.Core.Resources.Conneg
{
    public class ConnegGraph
    {
        public readonly IList<Type> Writers = new List<Type>();
        public readonly IList<Type> Readers = new List<Type>{typeof(AggregatedQueryReader)};

        public static Task<ConnegGraph> Build(BehaviorGraph behaviorGraph)
        {
            var graph = new ConnegGraph();

            var writers = TypeRepository.FindTypes(behaviorGraph.AllAssemblies(),
                TypeClassification.Concretes | TypeClassification.Closed, x => x.Closes(typeof (IMediaWriter<>)))
                .ContinueWith(t => graph.Writers.AddRange(t.Result));

            var readers = TypeRepository.FindTypes(behaviorGraph.AllAssemblies(),
                TypeClassification.Concretes | TypeClassification.Closed, x => x.Closes(typeof(IReader<>)))
                .ContinueWith(t => graph.Readers.AddRange(t.Result));

            return Task.WhenAll(writers, readers).ContinueWith(t => graph);
        }

        public IEnumerable<Type> ReaderTypesFor(Type inputType)
        {
            var matchingInterface = typeof (IReader<>).MakeGenericType(inputType);

            return Readers.Where(x => x.CanBeCastTo(matchingInterface));
        }

        public IEnumerable<Type> WriterTypesFor(Type resourceType)
        {
            var matchingInterface = typeof (IMediaWriter<>).MakeGenericType(resourceType);

            return Writers.Where(x => x.CanBeCastTo(matchingInterface));
        }
    }
}