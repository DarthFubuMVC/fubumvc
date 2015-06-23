using System;
using System.Linq.Expressions;
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
        public Func<ISearchContext, IWebElement> Finder;

        public static GestureConfig ByName(string name)
        {
            return new GestureConfig{
                Finder = driver =>
                {
                    return driver.FindElementOrNull(By.Name(name)) ?? driver.FindElementOrNull(By.Id(name));
                },
                CellName = name,
                Description = "Name/Id=" + name
            };
        }

        public static GestureConfig ByProperty<T>(Expression<Func<T, object>> expression)
        {
            var accessor = expression.ToAccessor();
            return ByName(accessor.Name);
        }
    }
}
