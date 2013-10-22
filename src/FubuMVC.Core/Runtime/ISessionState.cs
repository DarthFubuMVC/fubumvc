using System.Web;
using FubuCore.Util;

namespace FubuMVC.Core.Runtime
{
    public interface ISessionState
    {
        T Get<T>() where T : class;
        T Get<T>(string key) where T : class;

        void Remove<T>();
        void Remove(string key);

        void Set<T>(T value);
        void Set<T>(string key, T value);
    }

    public class BasicSessionState : ISessionState
    {
        private readonly Cache<string, object> _objects = new Cache<string, object>(t => null);

        private string getKey<T>()
        {
            return typeof (T).FullName;
        }

        public T Get<T>() where T : class
        {
            return Get<T>(getKey<T>());
        }

        public T Get<T>(string key) where T : class
        {
            return (T)_objects[key];
        }

        public void Remove<T>()
        {
            Remove(getKey<T>());
        }

        public void Remove(string key)
        {
            _objects.Remove(key);
        }

        public void Set<T>(string key, T value)
        {
            _objects[key] = value;
        }

        public void Set<T>(T value)
        {
            Set(getKey<T>(), value);
        }
    }


    public class SimpleSessionState : ISessionState
    {
        private readonly HttpSessionStateBase _session;

        public SimpleSessionState(HttpContextBase httpContext)
        {
            _session = httpContext.Session;
        }

        private string getKey<T>()
        {
            return typeof(T).FullName;
        }

        public T Get<T>() where T : class
        {
            return Get<T>(getKey<T>());
        }

        public T Get<T>(string key) where T : class
        {
            return _session[key] as T;
        }

        public void Remove<T>()
        {
            Remove(getKey<T>());
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }

        public void Set<T>(string key, T value)
        {
            _session[key] = value;
        }

        public void Set<T>(T value)
        {
            Set(getKey<T>(), value);
        }
    }
}