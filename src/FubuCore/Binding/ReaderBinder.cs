using System;
using System.Collections.Generic;
using System.Data;
using FubuCore.Util;
using Microsoft.Practices.ServiceLocation;

namespace FubuCore.Binding
{
    public class ReaderBinder
    {
        private readonly IModelBinder _binder;
        private readonly IServiceLocator _services;
        private readonly Cache<string, string> _aliases = new Cache<string, string>(key => key);

        public ReaderBinder(IModelBinder binder, IServiceLocator services)
        {
            _binder = binder;
            _services = services;
        }

        public IEnumerable<T> Build<T>(Func<IDataReader> getReader) where T : new()
        {
            using (var reader = getReader())
            {
                return Build<T>(reader);
            }    
        }

        public IEnumerable<T> Build<T>(IDataReader reader)
        {
            var request = new DataReaderRequestData(reader, _aliases);
            var context = new BindingContext(request, _services);
                
            while (reader.Read())
            {
                yield return (T) _binder.Bind(typeof (T), context);
            }
        }

        public void SetAlias(string name, string alias)
        {
            _aliases[name] = alias;
        }
    }
}