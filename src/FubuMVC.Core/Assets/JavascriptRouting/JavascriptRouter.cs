using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.Assets.JavascriptRouting
{
    // Completely covered by integration tests
    public abstract class JavascriptRouter : IJavascriptRouter
    {
        private readonly IList<JavascriptRoute> _routes = new List<JavascriptRoute>();


        public IEnumerable<JavascriptRoute> Routes()
        {
            return _routes;
        }

        public ChainExpression Get(string name)
        {
            return new ChainExpression(this, name, "GET");
        }

        public ChainExpression Post(string name)
        {
            return new ChainExpression(this, name, "POST");
        }

        public ChainExpression Put(string name)
        {
            return new ChainExpression(this, name, "PUT");
        }

        public ChainExpression Delete(string name)
        {
            return new ChainExpression(this, name, "DELETE");
        }

        public class ChainExpression
        {
            private readonly JavascriptRoute _route;

            public ChainExpression(JavascriptRouter parent, string name, string method)
            {
                _route = new JavascriptRoute
                {
                    Name = name,
                    Method = method
                };

                parent._routes.Add(_route);
            }

            public void InputType<T>()
            {
                _route.Finder = x => x.FindUniqueByType(typeof (T), _route.Method);
            }

            public void Action<T>(Expression<Action<T>> expression)
            {
                _route.Finder = x => x.Find(expression, _route.Method);
            }
        }
    }
}