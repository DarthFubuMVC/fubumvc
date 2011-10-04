using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Resources.Media.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{
    public class ConnegInputNode : ConnegNode
    {
        private readonly IList<IMediaReaderNode> _readers = new List<IMediaReaderNode>();

        public ConnegInputNode(Type inputType) : base(inputType)
        {
            AllowHttpFormPosts = true;
        }

        public bool AllowHttpFormPosts { get; set; }

        public IEnumerable<IMediaReaderNode> Readers
        {
            get { return _readers; }
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
                yield return new ObjectDef(typeof (ModelBindingMediaReader<>), InputType);
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

        public void AddReader(IMediaReaderNode node)
        {
            if (node.InputType != InputType)
            {
                throw new ArgumentException("Mismatched input types", "node");
            }

            _readers.Add(node);
        }

        public void JsonOnly()
        {
            AllowHttpFormPosts = false;
            UseFormatter<JsonFormatter>();
        }

        public bool Equals(ConnegInputNode other)
        {
            if (other == null) return false;
            return other.InputType.Equals(InputType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ConnegInputNode);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}