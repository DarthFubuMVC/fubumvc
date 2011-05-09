using FubuCore.Binding;
using FubuMVC.StructureMap;
using StructureMap;

namespace FubuCore.Testing.Binding
{
    public class InMemoryBindingContext : BindingContext
    {
        private readonly InMemoryRequestData _data;
        private readonly IContainer _container;

        public InMemoryBindingContext() : this(new InMemoryRequestData(), new Container())
        {
        }

        private InMemoryBindingContext(InMemoryRequestData data, IContainer container)
            : base(data, new StructureMapServiceLocator(container))
        {
            _data = data;
            _container = container;
        }

        public InMemoryBindingContext WithData(string key, object value)
        {
            this[key] = value;
            return this;
        }

        public InMemoryBindingContext WithPropertyValue(object value)
        {
            PropertyValue = value;
            return this;
        }

        public InMemoryRequestData Data { get { return _data; } }
        public IContainer Container { get { return _container; } }

        public object this[string key] { get { return _data[key]; } set { _data[key] = value; } }
    }
}