using System;
using System.Linq.Expressions;
using FubuCore;
using OpenQA.Selenium;
using FubuCore.Reflection;

namespace Serenity.Fixtures.Grammars
{
    public class GestureConfig
    {
        public string CellName = "value";
        public Type CellType = typeof(string);
        public string Template;
        public string Description;
        public Func<IWebElement> Finder;
        public Action BeforeClick = () => { };
        public Action AfterClick = () => { };
        public string FinderDescription;
        public string Label = string.Empty;
        public Func<ISearchContext> FindContext = () => { return null; };

        public static GestureConfig ByName(Func<ISearchContext> findContext, string name)
        {
            return new GestureConfig{
                Finder = () =>
                {
                    var context = findContext();
                    return context.FindElement(By.Name(name)) ?? context.FindElement(By.Id(name));
                },
                FinderDescription = "Name/Id = '{0}'".ToFormat(name),
                CellName = name,
                Label = name,
                FindContext = findContext
            };
        }

        public static GestureConfig ByProperty<T>(Func<ISearchContext> findContext, Expression<Func<T, object>> expression)
        {
            var accessor = expression.ToAccessor();
            return ByName(findContext, accessor.Name);
        }
    }
}