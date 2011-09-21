using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Formatters;

namespace FubuMVC.Core.Rest.Conneg
{
    public enum FormatterUsage
    {
        all,
        none,
        selected
    }

    public class ConnegInputNode : BehaviorNode
    {
        private readonly Type _inputType;
        private readonly IList<IMediaReaderNode> _readers = new List<IMediaReaderNode>();

        public ConnegInputNode(Type inputType)
        {
            _inputType = inputType;
            AllowHttpFormPosts = true;
        }

        public Type InputType
        {
            get { return _inputType; }
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var objectDef = new ObjectDef(typeof (ConnegInputBehavior<>).MakeGenericType(InputType));

            var mediaReaderType = typeof (IMediaReader<>).MakeGenericType(InputType);


            var readerDependencies = new ListDependency(typeof(IEnumerable<>).MakeGenericType(mediaReaderType));
            readerDependencies.AddRange(createBuilderDependencies());

            objectDef.Dependency(readerDependencies);

            return objectDef;
        }

        private IEnumerable<ObjectDef> createBuilderDependencies()
        {
            foreach (var node in _readers)
            {
                yield return node.ToObjectDef();
            }

            if (AllowHttpFormPosts)
            {
                yield return new ObjectDef(typeof(ModelBindingMediaReader<>), InputType);
            }

            switch (_formatterUsage)
            {
                case FormatterUsage.selected:
                    var formatterDef = new ObjectDef(typeof (FormatterMediaReader<>), InputType);
                    var dependencies = formatterDef.EnumerableDependenciesOf<IFormatter>();
                    _selectedFormatterTypes.Each(t => dependencies.AddType(t));

                    yield return formatterDef;

                    break;

                case FormatterUsage.all:
                    yield return new ObjectDef(typeof(FormatterMediaReader<>), InputType);
                    break;
            }

            yield break;
        }

        private FormatterUsage _formatterUsage = FormatterUsage.all;
        
        public FormatterUsage FormatterUsage
        {
            get
            {
                return _formatterUsage;
            }
        }

        public void UseAllFormatters()
        {
            _formatterUsage = FormatterUsage.all;
            _selectedFormatterTypes.Clear();
        }

        public void UseNoFormatters()
        {
            _formatterUsage = FormatterUsage.none;
            _selectedFormatterTypes.Clear();
        }

        private readonly IList<Type> _selectedFormatterTypes = new List<Type>();

        public IEnumerable<Type> SelectedFormatterTypes
        {
            get
            {
                return _selectedFormatterTypes;
            }
        }

        public void UseFormatter<T>() where T : IFormatter
        {
            _formatterUsage = FormatterUsage.selected;
            _selectedFormatterTypes.Add(typeof(T));
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
    }

    public interface IMediaReaderNode
    {
        Type InputType { get;}
        ObjectDef ToObjectDef();
    }
}