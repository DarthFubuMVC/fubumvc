using FubuMVC.Core.Util;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.UI.Configuration
{
    public class ElementRequest
    {
        private readonly Stringifier _stringifier;
        private bool _hasFetched;
        private object _rawValue;

        public ElementRequest(object model, Accessor accessor, IServiceLocator services, Stringifier stringifier)
        {
            _stringifier = stringifier;
            Model = model;
            Accessor = accessor;
            Services = services;
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
        public IServiceLocator Services { get; private set; }

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
            return Services.GetInstance<T>();
        }

        public T Value<T>()
        {
            return (T) RawValue;
        }

        public string StringValue()
        {
            return _stringifier.GetString(Accessor.PropertyType, RawValue);
        }
    }
}