using System;
using FubuCore;
using FubuCore.Util;
using FubuFastPack.Domain;
using FubuFastPack.Persistence;

namespace FubuFastPack.Binding
{
    public class DomainEntityConverterFamily : IObjectConverterFamily
    {
        private readonly Lazy<IRepository> _repository;

        public DomainEntityConverterFamily(Func<IRepository> source)
        {
            _repository = new Lazy<IRepository>(source);
        }

        // Matches any type deriving from DomainEntity
        // CanBeCastTo<> is an extension method in FubuCore as well
        public bool Matches(Type type, IObjectConverter converter)
        {
            return type.CanBeCastTo<DomainEntity>();
        }

        // In this case we find the correct object by looking it up by Id
        // from our repository
        public Func<string, object> CreateConverter(Type type, Cache<Type, Func<string, object>> converters)
        {
            return text =>
            {
                if (text.IsEmpty()) return null;

                var guid = new Guid(text);
                return _repository.Value.Find(type, guid);
            };
        }
    }
}