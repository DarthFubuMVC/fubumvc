using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Resources.Conneg
{
    public class ConnegGraph
    {
        public readonly IList<Type> Writers = new List<Type>();
        public readonly IList<Type> Readers = new List<Type>();

        public static ConnegGraph Build(BehaviorGraph behaviorGraph)
        {
            var graph = new ConnegGraph();

            TypePool typePool = behaviorGraph.Types();
            var writers = typePool
                .TypesMatching(
                    x =>
                        x.Closes(typeof (IMediaWriter<>)) && x.IsConcreteWithDefaultCtor() &&
                        !x.IsOpenGeneric());

            graph.Writers.AddRange(writers);


            var readers = typePool
                .TypesMatching(
                    x =>
                        x.Closes(typeof (IReader<>)) && x.IsConcreteWithDefaultCtor() &&
                        !x.IsOpenGeneric());

            graph.Readers.AddRange(readers);


            return graph;
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