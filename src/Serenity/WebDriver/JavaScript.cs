using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using FubuCore;
using OpenQA.Selenium;
using Serenity.WebDriver.JavaScriptBuilders;

namespace Serenity.WebDriver
{
    public class JavaScript : DynamicObject
    {
        const string Return_Template = "__result = {0};\n__callback(__result);";
        const string Void_Template = "{0};\n__callback();";

        public string Statement { get; private set; }
        public int JQueryCheckCount { get; set; }
        public TimeSpan JQueryCheckInterval { get; set; }
        public bool CheckForJQuery { get; set; }

        private IList<IWebElement> Arguments { get; set; }

        protected static IList<IJavaScriptBuilder> JavaScriptBuilders { get; private set; }

        static JavaScript()
        {
            JavaScriptBuilders = new ReadOnlyCollection<IJavaScriptBuilder>(new IJavaScriptBuilder[]
            {
                new NullObjectJavaScriptBuilder(),
                new StringJavaScriptBuilder(),
                new WebElementJavaScriptBuilder(),
                new DefaultJavaScriptBuilder()
            });
        }

        public JavaScript(string statement) : this(statement, new IWebElement[0]) { }

        public JavaScript(string statement, params IWebElement[] arguments)
        {
            Statement = statement;
            Arguments = new List<IWebElement>(arguments);
            JQueryCheckCount = 3;
            JQueryCheckInterval = TimeSpan.FromMilliseconds(100.0);
            CheckForJQuery = false;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var javascriptFriendlyName = JavaScriptFriendlyName(binder.Name);
            result = CreateWith(
                AppendFunction(javascriptFriendlyName, args),
                CheckForJQuery,
                JQueryCheckCount,
                JQueryCheckInterval,
                Arguments.Union(args.Where(x => x is IWebElement).Cast<IWebElement>()).ToArray());
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var javascriptFriendlyName = JavaScriptFriendlyName(binder.Name);
            result = CreateWith(
                "{0}.{1}".ToFormat(Statement, javascriptFriendlyName),
                CheckForJQuery,
                JQueryCheckCount,
                JQueryCheckInterval,
                Arguments.ToArray());
            return true;
        }

        public object ExecuteAndGet(IWebDriver driver)
        {
            return ExecuteAndGet((IJavaScriptExecutor) driver);
        }

        public object ExecuteAndGet(IJavaScriptExecutor executor)
        {
            return ExecuteAndGetPrivate(executor, true);
        }

        private object ExecuteAndGetPrivate(IJavaScriptExecutor executor, bool returnValue)
        {

            var statement = StatementWithArguments(returnValue);

            var executeTemplate = returnValue
                ? Return_Template.ToFormat(statement)
                : Void_Template.ToFormat(statement);

            var script = ScriptWithoutJQueryCheck(executeTemplate);

            if (CheckForJQuery)
            {
                script = ScriptWithJQueryCheck(executeTemplate, JQueryCheckCount, JQueryCheckInterval);
            }

            var result = executor.ExecuteAsyncScript(script, Arguments.Cast<object>().ToArray());

            CheckForReferenceError(result);

            return result;
        }

        private string ScriptWithJQueryCheck(string executeTemplate, int checkCount, TimeSpan checkInterval)
        {
            const string checkJQueryTemplate = "var __callback = arguments[arguments.length - 1], __checkCount = 0, __result;\n" +
                                    "function __checkForJQuery() {{\n" +
                                    "  __checkCount++;\n" +
                                    "  if(!window.jQuery && __checkCount < {1}) {{\n" +
                                    "    window.setTimeout(__checkForJQuery, {2});\n" +
                                    "  }}\n" +
                                    "  else {{\n" +
                                    "    try {{\n" +
                                    "      {0}\n" +
                                    "    }} catch(err) {{\n" +
                                    "      var errstr = err.toString();\n" +
                                    "      __callback(errstr);\n" +
                                    "    }}\n" +
                                    "  }}\n" +
                                    "}}\n" +
                                    "if (!window.jQuery) {{\n" +
                                    "  window.setTimeout(__checkForJQuery, {2});\n" +
                                    "}}\n" +
                                    "else {{" +
                                    "  {0}" +
                                    " }}";

            return checkJQueryTemplate.ToFormat(executeTemplate, checkCount, checkInterval.TotalMilliseconds);
        }

        private string ScriptWithoutJQueryCheck(string executeTemplate)
        {

            const string basicTemplate = "var __callback = arguments[arguments.length - 1];\n {0}";
            return basicTemplate.ToFormat(executeTemplate);
        }

        private void CheckForReferenceError(object result)
        {
            var resultStr = result as string;

            if (!string.IsNullOrWhiteSpace(resultStr) && resultStr.StartsWith("ReferenceError"))
            {
                throw new InvalidOperationException(resultStr);
            }
        }

        public T ExecuteAndGet<T>(IWebDriver driver)
        {
            return ExecuteAndGet<T>((IJavaScriptExecutor) driver);
        }

        public T ExecuteAndGet<T>(IJavaScriptExecutor executor)
        {
            return (T) ExecuteAndGet(executor);
        }

        public void Execute(IWebDriver driver)
        {
            Execute((IJavaScriptExecutor) driver);
        }

        public void Execute(IJavaScriptExecutor executor)
        {
            ExecuteAndGetPrivate(executor, false);
        }

        private string StatementWithArguments(bool returnValue)
        {
            if (Arguments == null || Arguments.Count == 0)
            {
                return Statement;
            }

            var argumentVariables = Arguments.Select((element, index) => new
            {
                ParameterName = "__element__argument__{0}".ToFormat(index),
                ParameterRetrieval = "arguments[{0}]".ToFormat(index)
            }).ToList();

            var correctedStatement = argumentVariables
                .Select(x => x.ParameterName)
                .Aggregate(Statement, (statement, arg) => WebElementJavaScriptBuilder.MarkerRgx.Replace(statement, arg, 1));

            return "(function({0}) {{ {1}{2} }})({3})".ToFormat(
                argumentVariables.Select(x => x.ParameterName).Join(", "),
                returnValue ? "return " : "",
                correctedStatement,
                argumentVariables.Select(x => x.ParameterRetrieval).Join(", "));
        }

        public dynamic ModifyStatement(string format)
        {
            return CreateWith(
                format.ToFormat(Statement),
                CheckForJQuery,
                JQueryCheckCount,
                JQueryCheckInterval,
                Arguments.ToArray());
        }

        public override string ToString()
        {
            return Statement;
        }

        private string JavaScriptFriendlyName(string name)
        {
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }

        private string AppendFunction(string func, params object[] args)
        {
            var argsString = args == null
                ? ""
                : args
                    .Reverse()
                    .SkipWhile(arg => arg == null)
                    .Reverse()
                    .Select(arg => JavaScriptBuilders.First(x => x.Matches(arg)).Build(arg))
                    .Join(", ");

            return "{0}.{1}({2})".ToFormat(Statement, func, argsString);
        }

        public static dynamic Create(string javaScript)
        {
            return new JavaScript(javaScript);
        }

        public static dynamic CreateJQuery(string selector)
        {
            return new JavaScript("$(\"" + selector + "\")") {CheckForJQuery = true};
        }

        public static dynamic CreateWithJQueryCheck(string javaScript)
        {
            return new JavaScript(javaScript) {CheckForJQuery = true};
        }

        public static dynamic CreateWith(string statement, bool checkForJQuery, int jQueryCheckCount, TimeSpan jQueryCheckInterval, params IWebElement[] arguments)
        {
            var js = new JavaScript(statement, arguments);
            js.CheckForJQuery = checkForJQuery;
            js.JQueryCheckCount = jQueryCheckCount;
            js.JQueryCheckInterval = jQueryCheckInterval;
            return js;
        }

        public static dynamic JQueryFrom(IWebElement element)
        {
            return new JavaScript("$({0})".ToFormat(WebElementJavaScriptBuilder.Marker), element)
            {
                CheckForJQuery = true
            };
        }

        public static dynamic Function(JavaScript body)
        {
            return Function(Enumerable.Empty<string>(), body);
        }

        public static dynamic Function(IEnumerable<string> args, JavaScript body)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            return CreateWith(
                "function({0}) {{ {1} }}".ToFormat(args.Join(", "), body.Statement),
                body.CheckForJQuery,
                body.JQueryCheckCount,
                body.JQueryCheckInterval);
        }

        public static implicit operator By(JavaScript source)
        {
            return (JavaScriptBy) source;
        }

        public static implicit operator OpenQA.Selenium.By(JavaScript source)
        {
            return (JavaScriptBy) source;
        }

        public static implicit operator JavaScriptBy(JavaScript source)
        {
            return new JavaScriptBy(source);
        }

        public static implicit operator JavaScript(JavaScriptBy source)
        {
            return (JavaScript) source.JavaScript;
        }
    }
}