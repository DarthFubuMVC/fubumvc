using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.SessionState;
using FubuCore.Util;

namespace FubuMVC.Core.Runtime
{
    public interface ISessionState
    {
        T Get<T>() where T : class;
        void Set<T>(T value);
    }

    public class BasicSessionState : ISessionState
    {
        private readonly Cache<Type, object> _objects = new Cache<Type,object>(t => null);

        public T Get<T>() where T : class
        {
            return (T) _objects[typeof(T)];
        }

        public void Set<T>(T value)
        {
            _objects[typeof (T)] = value;
        }
    }

    //// TODO -- throw that ugly wrapper in here and unit test
    //public class BinaryCopySessionState : ISessionState
    //{
    //    private HttpSessionState _session;
    //    private BinaryFormatter _formatter;

    //    public BinaryCopySessionState()
    //    {
    //        _session = HttpContext.Current.Session;
    //        _formatter = new BinaryFormatter();
    //    }

    //    public T Get<T>()
    //    {
    //        var key = getKey<T>();
            
    //    }

    //    private string getKey<T>()
    //    {
    //        return typeof (T).FullName;
    //    }

    //    public void Set<T>(T value)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class SimpleSessionState : ISessionState
    {
        private readonly HttpSessionState _session;

        public SimpleSessionState()
        {
            _session = HttpContext.Current.Session;
        }

        private string getKey<T>()
        {
            return typeof(T).FullName;
        }


        public T Get<T>() where T : class
        {
            return _session[getKey<T>()] as T;
        }

        public void Set<T>(T value)
        {
            _session[getKey<T>()] = value;
        }
    }
}