using FubuFastPack.Domain;

namespace FubuFastPack.Querying
{
    public interface IDataRestriction
    {
    }

    public interface IDataRestriction<T> : IDataRestriction where T : DomainEntity
    {
        void Apply(IDataSourceFilter<T> filter);
    }

}