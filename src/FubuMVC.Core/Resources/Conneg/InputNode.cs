using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{
    public class InputNode : BehaviorNode, IMayHaveInputType, DescribesItself
    {
        private readonly Type _inputType;
        private readonly ReaderChain _chain = new ReaderChain();
        private readonly IList<IReader> _readers = new List<IReader>(); 

        public InputNode(Type inputType)
        {
            _inputType = inputType;

            AllowHttpFormPosts = true;
        }

        // TODO -- use ConnegSettings here
        public IEnumerable<IReader> SelectReaders()
        {
            return _readers;
        }

        public void Add(IFormatter formatter)
        {
            var reader = typeof (FormatterReader<>).CloseAndBuildAs<IReader>(formatter, _inputType);
            _readers.Add(reader);
        }

        public void Add(Type readerType)
        {
            if (!readerType.Closes(typeof (IReader<>)))
            {
                throw new ArgumentOutOfRangeException("readerType", "readerType must close IReader<T> where T is the input type for this chain");
            }

            var reader = readerType.CloseAndBuildAs<IReader>(_inputType);
            _readers.Add(reader);
        }

        public void Add(IReader reader)
        {
            var readerType = typeof (IReader<>).MakeGenericType(_inputType);
            if (!reader.GetType().CanBeCastTo(readerType))
            {
                throw new ArgumentOutOfRangeException("reader", "reader must be of type " + readerType.GetFullName());
            }

            _readers.Add(reader);
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var def = new ObjectDef(typeof (InputBehavior<>), _inputType);

            var readerType = typeof(IReader<>).MakeGenericType(_inputType);
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(readerType);
            var dependency = new ListDependency(enumerableType);
            dependency.AddRange(Readers.OfType<IContainerModel>().Select(x => x.ToObjectDef()));

            def.Dependency(dependency);

            return def;
        }

        public ReaderChain Readers
        {
            get { return _chain; }
        }

        public bool AllowHttpFormPosts
        {
            get { return Readers.Any(x => x is ModelBind); }
            set
            {
                var binders = Readers.Where(x => x is ModelBind).ToList();

                if (value && !binders.Any())
                {
                    Readers.AddToEnd(new ModelBind(_inputType));
                }

                if (!value)
                {
                    binders.Each(x => x.Remove());
                }
            }
        }

        public ReadWithFormatter AddFormatter<T>() where T : IFormatter
        {
            var formatter = new ReadWithFormatter(_inputType, typeof(T));
            var existing = Readers.FirstOrDefault(x => x.Equals(formatter)) as ReadWithFormatter;
            if (existing != null)
            {
                return existing;
            }

            Readers.AddToEnd(formatter);

            return formatter;
        }

        public Reader AddReader<T>()
        {
            var readerType = typeof(IReader<>).MakeGenericType(_inputType);
            if (!typeof(T).CanBeCastTo(readerType))
            {
                throw new ArgumentOutOfRangeException("{0} can not be cast to {1}", typeof(T).FullName, readerType.FullName);
            }

            var reader = new Reader(typeof(T));

            Readers.AddToEnd(reader);

            return reader;
        }

        public void ClearAll()
        {
            Readers.SetTop(null);
        }

        public void JsonOnly()
        {
            ClearAll();
            AddFormatter<JsonFormatter>();
        }

        public bool UsesFormatter<T>()
        {
            return _chain.OfType<ReadWithFormatter>().Any(x => x.FormatterType == typeof (T));
        }

        public Type InputType()
        {
            return _inputType;
        }

        void DescribesItself.Describe(Description description)
        {
            description.ShortDescription =
                "Performs content negotiation and model resolution from the request for the type " + InputType().Name;

            description.AddList("Readers", _chain);
        }


        public IEnumerable<string> MimeTypes()
        {
            return Readers.SelectMany(x => x.Mimetypes).Distinct();
        } 

    }
}