using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Event;

namespace FubuFastPack.NHibernate
{
    public static class ConfigurationExtensions
    {
        public static void AddPreInsertListener(this Configuration configuration, IPreInsertEventListener listener)
        {
            var newArray = new[]{listener};

            if (configuration.EventListeners.PreInsertEventListeners == null)
            {
                configuration.EventListeners.PreInsertEventListeners = newArray;
            }
            else
            {
                configuration.EventListeners.PreInsertEventListeners =
                    configuration.EventListeners.PreInsertEventListeners.Union(newArray).ToArray();
            }
        }

        public static void AddPreUpdateListener(this Configuration configuration, IPreUpdateEventListener listener)
        {
            var newArray = new[]{listener};

            if (configuration.EventListeners.PreUpdateEventListeners == null)
            {
                configuration.EventListeners.PreUpdateEventListeners = newArray;
            }
            else
            {
                configuration.EventListeners.PreUpdateEventListeners =
                    configuration.EventListeners.PreUpdateEventListeners.Union(newArray).ToArray();
            }
        }

        public static void AddPreLoadListener(this Configuration configuration, IPreLoadEventListener listener)
        {
            var newArray = new[] { listener };

            if (configuration.EventListeners.PreLoadEventListeners == null)
            {
                configuration.EventListeners.PreLoadEventListeners = newArray;
            }
            else
            {
                configuration.EventListeners.PreLoadEventListeners =
                    configuration.EventListeners.PreLoadEventListeners.Union(newArray).ToArray();
            }
        }

        public static Type PersistedTypeByName(this Configuration configuration, string typeName)
        {
            return configuration.ClassMappings.Select(x => x.MappedClass).FirstOrDefault(x => x.Name.ToLowerInvariant() == typeName.ToLowerInvariant() );
        }

    }
}