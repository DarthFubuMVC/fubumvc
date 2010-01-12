using System.Diagnostics;
using System.Reflection;

namespace FubuMVC.Core.Registration.DSL
{
    public class AppliesToExpression
    {
        private readonly TypePool _pool;

        public AppliesToExpression(TypePool pool)
        {
            _pool = pool;
        }

        public AppliesToExpression ToThisAssembly()
        {
            return ToAssembly(findTheCallingAssembly());
        }

        private static Assembly findTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Assembly callingAssembly = null;
            for (int i = 0; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Assembly assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }

        public AppliesToExpression ToAssembly(Assembly assembly)
        {
            _pool.AddAssembly(assembly);
            return this;
        }

        public AppliesToExpression ToAssemblyContainingType<T>()
        {
            return ToAssembly(typeof (T).Assembly);
        }

        public AppliesToExpression ToAssembly(string assemblyName)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            return ToAssembly(assembly);
        }
    }
}