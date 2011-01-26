using NHibernate.Criterion;

namespace FubuFastPack.NHibernate
{
    public interface IWhere
    {
        ICriterion Create();
    }
}