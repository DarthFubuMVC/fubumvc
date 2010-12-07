using FluentNHibernate;
using NHibernate.Cfg;

namespace FubuFastPack.NHibernate
{
    public interface IConfigurationModifier
    {
        void ApplyProperties(Configuration configuration);
        void Configure(PersistenceModel model);
        void Configure(Configuration configuration);
    }
}