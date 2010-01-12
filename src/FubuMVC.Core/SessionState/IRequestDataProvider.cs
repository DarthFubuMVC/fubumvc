using System.Collections.Generic;
using System.Reflection;
using System.Web;

namespace FubuMVC.Core.SessionState
{
    public interface IRequestDataProvider
    {
        void Store<TFlashModel>(TFlashModel flashModel) where TFlashModel : class;
        TFlashModel Load<TFlashModel>() where TFlashModel : class, new();
        void Clear();
    }

    public class RequestDataProvider : IRequestDataProvider
    {
        public const string REQUESTDATA_PREFIX_KEY = "fubuRequestData__";
        private readonly HttpContextBase _httpContext;

        public RequestDataProvider(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }


        public void Store<TFlashModel>(TFlashModel flashModel) where TFlashModel : class
        {
            if (_httpContext.Session == null) return;

            Clear();

            typeof (TFlashModel)
                .GetProperties()
                .Each(property => StorePropertyValue(property, flashModel));
        }

        public TFlashModel Load<TFlashModel>() where TFlashModel : class, new()
        {
            if (_httpContext.Session == null) return null;

            var flashModel = new TFlashModel();

            typeof (TFlashModel).GetProperties()
                .Each(property => LoadPropertyValue(property, flashModel));

            return flashModel;
        }

        public void Clear()
        {
            if (_httpContext.Session == null || _httpContext.Session.Keys == null) return;

            var keysToBeRemoved = new List<string>();
            _httpContext.Session.Keys.Each(key =>
            {
                if (key.ToString().StartsWith(REQUESTDATA_PREFIX_KEY))
                    keysToBeRemoved.Add(key.ToString());
            });

            keysToBeRemoved.ForEach(key => _httpContext.Session.Remove(key));
        }


        private void StorePropertyValue(PropertyInfo propertyInfo, object flashModel)
        {
            object value = propertyInfo.GetValue(flashModel, new object[] {});
            string key = REQUESTDATA_PREFIX_KEY + propertyInfo.Name;
            _httpContext.Session.Add(key, value);
        }

        private void LoadPropertyValue<TFlashModel>(PropertyInfo propertyInfo, TFlashModel flashModel)
            where TFlashModel : class
        {
            string key = REQUESTDATA_PREFIX_KEY + propertyInfo.Name;
            object value = _httpContext.Session[key];

            if (value == null)
                throw new KeyNotFoundException(string.Format("THere was no data found for property {0} in {1}",
                                                             propertyInfo.Name, typeof (TFlashModel).FullName));

            propertyInfo.SetValue(flashModel, value, new object[] {});
        }
    }
}