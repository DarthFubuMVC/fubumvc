using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Resources.Conneg
{
    public class ConnegGraph
    {
        public readonly IList<Type> Writers = new List<Type>();
        public readonly IList<Type> Readers = new List<Type>();

        public static ConnegGraph Build()
        {
            var graph = new ConnegGraph();

            TypePool typePool = TypePool.AppDomainTypes();
            var writers = typePool
                .TypesMatching(
                    x =>
                        x.Closes(typeof (IMediaWriter<>)) && x.IsConcreteWithDefaultCtor() &&
                        !x.IsOpenGeneric());

            graph.Writers.AddRange(writers);


            var readers = typePool
                .TypesMatching(
                    x =>
                        x.Closes(typeof(IReader<>)) && x.IsConcreteWithDefaultCtor() &&
                        !x.IsOpenGeneric());

            graph.Readers.AddRange(readers);


            return graph;

        }

        public IEnumerable<Type> ReaderTypesFor(Type inputType)
        {
            return
                Readers.Where(
                    x => x.FindInterfaceThatCloses(typeof(IReader<>)).GetGenericArguments().Single() == inputType);
        }

        public IEnumerable<Type> WriterTypesFor(Type resourceType)
        {
            return
                Writers.Where(
                    x => x.FindInterfaceThatCloses(typeof(IMediaWriter<>)).GetGenericArguments().Single() == resourceType);
        }
    }
}