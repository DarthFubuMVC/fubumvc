using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ServiceModel.Syndication;
using FubuCore.Reflection;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media.Atom
{
    public interface IFeedItem<T>
    {
        void ConfigureItem(SyndicationItem item, IValues<T> target);
    }

    public class FeedItem<T> : IFeedItem<T>
    {
        private readonly IList<Action<IValues<T>, SyndicationItem>> _modifications
            = new List<Action<IValues<T>, SyndicationItem>>();

        public FeedItem()
        {
        }

        public FeedItem(Action<FeedItem<T>> configure)
        {
            configure(this);
        }

        private Action<IValues<T>, SyndicationItem> alter
        {
            set { _modifications.Add(value); }
        }

        void IFeedItem<T>.ConfigureItem(SyndicationItem item, IValues<T> target)
        {
            _modifications.Each(x => x(target, item));
        }

        private void modify<TArg>(Expression<Func<T, TArg>> property, Action<SyndicationItem, TArg> modification)
        {
            var accessor = ReflectionHelper.GetAccessor(property);
            alter = (target, item) =>
            {
                var value = target.ValueFor(accessor);

                if (value != null)
                {
                    modification(item, (TArg) value);
                }
            };
        }


        public void Title(Expression<Func<T, string>> expression)
        {
            modify(expression, (item, value) => item.Title = value.ToContent());
        }

        public void Id(Expression<Func<T, object>> expression)
        {
            modify(expression, (item, value) => item.Id = value.ToString());
        }

        public void UpdatedByProperty(Expression<Func<T, DateTime>> property)
        {
            var accessor = ReflectionHelper.GetAccessor(property);
            alter = (target, item) => item.LastUpdatedTime = (DateTime) target.ValueFor(accessor);
        }
    }
}