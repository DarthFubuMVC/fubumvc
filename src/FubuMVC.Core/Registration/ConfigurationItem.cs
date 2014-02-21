using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    public interface IConfigurationItem
    {
        void Seal();
        void Register(ServiceGraph graph);
    }

    public class ConfigurationItem<T> : IConfigurationItem where T : class
    {
        private T _value;
        private Task<T> _task;
        private readonly List<Action<T>> _alterations = new List<Action<T>>();

        public ConfigurationItem()
        {
        }

        public ConfigurationItem(T value)
        {
            _value = value;
        }

        public ConfigurationItem(Task<T> task)
        {
            _task = task;
        }

        public void Seal()
        {
            if (_task != null)
            {
                var inner = _task;
                _task = Task.Factory.StartNew(() =>
                {
                    _alterations.Each(x => x(inner.Result));

                    return inner.Result;
                });
            }
            else if (_value != null)
            {
                _alterations.Each(x => x(_value));
                _task = _value.ToTask();
            }
            else
            {
                _task = Task.Factory.StartNew(() => {
                    var value = (T)SettingsCollection.SettingsProvider.Value.SettingsFor(typeof (T));

                    _alterations.Each(x => x(value));

                    return value;
                });
            }
        
        }

        public void Register(ServiceGraph graph)
        {
            graph.SetServiceIfNone(typeof(T), ObjectDef.ForValue(Result));
        }

        public T Result
        {
            get
            {
                return _task.Result;
            }
        }

        public void Replace(T value)
        {
            _task = null;
            _value = value;
        }

        public void Replace(Task<T> source)
        {
            _task = source;
            _value = null;
        }

        public void Alter(Action<T> alteration)
        {
            _alterations.Add(alteration);
        }

        public void Use(Action<T> action)
        {
            
        }

    }
}