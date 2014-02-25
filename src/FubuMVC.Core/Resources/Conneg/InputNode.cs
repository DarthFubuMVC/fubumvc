using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{
    public class InputNode : BehaviorNode, IMayHaveInputType, DescribesItself
    {
        private readonly Type _inputType;
        private readonly IList<IReader> _readers = new List<IReader>(); 

        public InputNode(Type inputType)
        {
            _inputType = inputType;
        }

        // TODO -- use ConnegSettings here
        public IEnumerable<IReader> Readers
        {
            get
            {
                return _readers;
            }
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
            def.DependencyByValue(this);

            return def;
        }


        public void ClearAll()
        {
            _readers.Clear();
        }

        public void JsonOnly()
        {
            ClearAll();

            // TODO -- this will have to change
            Add(new JsonSerializer());

        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                return _readers.SelectMany(x => x.Mimetypes);
            }
        }

        public bool CanRead(MimeType mimeType)
        {
            return Mimetypes.Contains(mimeType.ToString());
        }

        public bool CanRead(string mimeType)
        {
            return Mimetypes.Contains(mimeType);
        }

        public Type InputType()
        {
            return _inputType;
        }

        void DescribesItself.Describe(Description description)
        {
            description.ShortDescription =
                "Performs content negotiation and model resolution from the request for the type " + InputType().Name;

            description.AddList("Readers", _readers);
        }


        public IEnumerable<string> MimeTypes()
        {
            return Readers.SelectMany(x => x.Mimetypes).Distinct();
        } 

    }
}