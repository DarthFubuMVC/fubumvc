using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Media.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{
    public abstract class ConnegNode : BehaviorNode
    {
        private readonly Type _inputType;
        private readonly IList<Type> _selectedFormatterTypes = new List<Type>();
        private FormatterUsage _formatterUsage = FormatterUsage.all;

        protected ConnegNode(Type inputType)
        {
            _inputType = inputType;
        }

        public Type InputType
        {
            get { return _inputType; }
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }

        public FormatterUsage FormatterUsage
        {
            get { return _formatterUsage; }
        }

        public IEnumerable<Type> SelectedFormatterTypes
        {
            get { return _selectedFormatterTypes; }
        }

        protected abstract Type formatterActionType();

        protected IEnumerable<ObjectDef> createFormatterObjectDef()
        {
            if (_formatterUsage != FormatterUsage.none)
            {
                var formatterDef = new ObjectDef(formatterActionType(), InputType);

                if (_formatterUsage == FormatterUsage.selected)
                {
                    var dependencies = formatterDef.EnumerableDependenciesOf<IFormatter>();
                    _selectedFormatterTypes.Each(t => dependencies.AddType(t));
                }

                yield return formatterDef;
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

        public void UseFormatter<T>() where T : IFormatter
        {
            _formatterUsage = FormatterUsage.selected;
            _selectedFormatterTypes.Fill(typeof (T));
        }

        protected abstract IEnumerable<ObjectDef> createBuilderDependencies();
        protected abstract Type getReaderWriterType();
        protected abstract Type behaviorType();

        protected override sealed ObjectDef buildObjectDef()
        {
            var objectDef = new ObjectDef(behaviorType().MakeGenericType(InputType));

            var mediaReaderType = getReaderWriterType().MakeGenericType(InputType);


            var readerDependencies = new ListDependency(typeof (IEnumerable<>).MakeGenericType(mediaReaderType));
            readerDependencies.AddRange(createBuilderDependencies());

            objectDef.Dependency(readerDependencies);

            return objectDef;
        }

        public bool Equals(ConnegNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._inputType, _inputType) && Equals(other.GetType(), GetType());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ConnegNode)) return false;
            return Equals((ConnegNode) obj);
        }

        public override int GetHashCode()
        {
            return (_inputType != null ? _inputType.GetHashCode() : 0);
        }
    }
}