using FluentNHibernate;
using NHibernate.Cfg;

namespace FubuFastPack.NHibernate
{
    public interface IConfigurationSource
    {
        Configuration Configuration();
        PersistenceModel Model();
    }
}