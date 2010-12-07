using System.Linq;
using FubuFastPack.Domain;

namespace FubuFastPack.Persistence
{
    public interface IQueryExpression<T> where T : DomainEntity
    {
        IQueryable<T> Apply(IQueryable<T> input);
    }
}