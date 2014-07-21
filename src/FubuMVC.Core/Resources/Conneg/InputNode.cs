using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{

    public interface IInputNode : IMayHaveInputType
    {
        /// <summary>
        /// Returns 
        /// </summary>
        IEnumerable<IReader> Readers();

        /// <summary>
        /// The mimetypes that can be read
        /// </summary>
        IEnumerable<string> Mimetypes { get; }

        /// <summary>
        /// Explicitly add reader for an IFormatter
        /// </summary>
        /// <param name="formatter"></param>
        void Add(IFormatter formatter);

        /// <summary>
        /// Explicitly add a reader by type.  Type must
        /// be an open generic type that closes to IReader<T>
        /// </summary>
        /// <param name="readerType"></param>
        void Add(Type readerType);

        /// <summary>
        /// Explicitly add an IReader.  Must implement IReader<T>
        /// where T is the resource type of this chain
        /// </summary>
        /// <param name="reader"></param>
        void Add(IReader reader);

        /// <summary>
        /// Remove all explicit readers
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Is there a reader for this mimetype?
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        bool CanRead(MimeType mimeType);

        /// <summary>
        /// Is there a reader for this mimetype?
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        bool CanRead(string mimeType);
    }

    public class InputNode : BehaviorNode, IInputNode, DescribesItself
    {
        private readonly Type _inputType;
        private readonly IList<IReader> _readers = new List<IReader>();
        private ConnegSettings _settings;
        private readonly Lazy<IEnumerable<IReader>> _allReaders; 

        public InputNode(Type inputType)
        {
            _inputType = inputType;

            _allReaders = new Lazy<IEnumerable<IReader>>(() => {
                var settings = _settings ?? new ConnegSettings();

                settings.ApplyRules(this);

                return _readers;
            });
        }

        public IEnumerable<IReader> Readers()
        {
            return _allReaders.Value;
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

            var collection = typeof (ReaderCollection<>).CloseAndBuildAs<object>(this, _inputType);
            var collectionType = typeof (IReaderCollection<>).MakeGenericType(_inputType);

            def.DependencyByValue(collectionType, collection);

            return def;
        }


        public void ClearAll()
        {
            _readers.Clear();
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                return _allReaders.Value.SelectMany(x => x.Mimetypes);
            }
        }

        public IEnumerable<IReader> Explicits
        {
            get
            {
                return _readers;
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
            description.Title = "Conneg Input";
            description.ShortDescription =
                "Performs content negotiation and model resolution from the request for the type " + InputType().Name;

            description.AddList("Readers", _readers);
        }


        public void UseSettings(ConnegSettings settings)
        {
            _settings = settings;
        }
    }
}