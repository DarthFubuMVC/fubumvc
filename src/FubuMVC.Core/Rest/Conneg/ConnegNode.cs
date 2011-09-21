using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Rest.Media.Formatters;

namespace FubuMVC.Core.Rest.Conneg
{
    public abstract class ConnegNode : BehaviorNode
    {
        private readonly Type _inputType;
        private FormatterUsage _formatterUsage = FormatterUsage.all;
        private readonly IList<Type> _selectedFormatterTypes = new List<Type>();

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
            get
            {
                return _formatterUsage;
            }
        }

        public IEnumerable<Type> SelectedFormatterTypes
        {
            get
            {
                return _selectedFormatterTypes;
            }
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
                    GenericEnumerableExtensions.Each<Type>(_selectedFormatterTypes, t => dependencies.AddType(t));
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
            _selectedFormatterTypes.Add(typeof(T));
        }
    }
}