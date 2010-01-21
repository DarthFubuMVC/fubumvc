using FubuMVC.Core.Util;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.UI.Configuration
{
    public class ElementRequest
    {
        private readonly Stringifier _stringifier;
        private bool _hasFetched;
        private object _rawValue;
        private IServiceLocator _services;

        public ElementRequest(object model, Accessor accessor, IServiceLocator services, Stringifier stringifier)
        {
            _stringifier = stringifier;
            Model = model;
            Accessor = accessor;
            _services = services;
        }

        public string ElementId { get; set; }


        public object RawValue
        {
            get
            {
                if (!_hasFetched)
                {
                    _rawValue = Accessor.GetValue(Model);
                    _hasFetched = true;
                }

                return _rawValue;
            }
        }

        public object Model { get; private set; }
        public Accessor Accessor { get; private set; }

        public AccessorDef ToAccessorDef()
        {
            return new AccessorDef
            {
                Accessor = Accessor,
                ModelType = Model.GetType()
            };
        }

        public T Get<T>()
        {
            return _services.GetInstance<T>();
        }

        public T Value<T>()
        {
            return (T) RawValue;
        }

        public string StringValue()
        {
            return _stringifier.GetString(Accessor.PropertyType, RawValue);
        }

        public bool ValueIsEmpty()
        {
            return RawValue == null || string.Empty.Equals(RawValue);
        }
    }
}