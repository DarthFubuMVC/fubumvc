using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Media.Formatters;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public class InputNode : BehaviorNode
    {
        private readonly Type _inputType;
        private readonly ReaderChain _readers = new ReaderChain();

        public InputNode(Type inputType)
        {
            _inputType = inputType;

            AllowsHttpFormPosts = true;
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
            dependency.AddRange(Readers.OfType<IContainerModel>().Select(x => x.ToObjectDef(DiagnosticLevel.None)));

            def.Dependency(dependency);

            return def;
        }

        public ReaderChain Readers
        {
            get { return _readers; }
        }

        public bool AllowsHttpFormPosts
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
    }
}