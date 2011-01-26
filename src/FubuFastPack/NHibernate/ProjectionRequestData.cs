using System;
using FubuCore.Binding;
using FubuCore.Util;

namespace FubuFastPack.NHibernate
{
    public class ProjectionRequestData : IRequestData
    {
        private readonly Cache<string, int> _positions = new Cache<string, int>();
        private object[] _objects;

        public void AddPropertyName(string propertyName)
        {
            _positions[propertyName] = _positions.Count;
        }

        public void SetObjects(object[] objects)
        {
            _objects = objects;
        }

        public object Value(string key)
        {
            return _objects[_positions[key]];
        }

        public bool Value(string key, Action<object> callback)
        {
            _positions.WithValue(key, i => callback(_objects[i]));
            return true;
        }
    }
}