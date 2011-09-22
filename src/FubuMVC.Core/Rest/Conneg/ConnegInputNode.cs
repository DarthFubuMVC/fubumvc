using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Formatters;

namespace FubuMVC.Core.Rest.Conneg
{
    public class ConnegInputNode : ConnegNode
    {
        private readonly IList<IMediaReaderNode> _readers = new List<IMediaReaderNode>();

        public ConnegInputNode(Type inputType) : base(inputType)
        {
            AllowHttpFormPosts = true;
        }

        protected override Type behaviorType()
        {
            return typeof (ConnegInputBehavior<>);
        }

        protected override Type getReaderWriterType()
        {
            return typeof (IMediaReader<>);
        }

        protected override IEnumerable<ObjectDef> createBuilderDependencies()
        {
            foreach (var node in _readers)
            {
                yield return node.ToObjectDef();
            }

            if (AllowHttpFormPosts)
            {
                yield return new ObjectDef(typeof(ModelBindingMediaReader<>), InputType);
            }

            foreach (var objectDef in createFormatterObjectDef())
            {
                yield return objectDef;
            }
        }

       
        protected override Type formatterActionType()
        {
            return typeof (FormatterMediaReader<>);
        }

        public bool AllowHttpFormPosts { get; set; }

        public void AddReader(IMediaReaderNode node)
        {
            if (node.InputType != InputType)
            {
                throw new ArgumentException("Mismatched input types", "node");
            }

            _readers.Add(node);
        }

        public IEnumerable<IMediaReaderNode> Readers
        {
            get
            {
                return _readers;
            }
        }

        public void JsonOnly()
        {
            AllowHttpFormPosts = false;
            UseFormatter<JsonFormatter>();
        }
    }
}