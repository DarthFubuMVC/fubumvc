using Gate;

namespace FubuMVC.OwinHost
{
    public static class EnvironmentExtensions
    {
        public static string ContentType(this Environment environment)
        {
            string value;
            return (environment.Headers != null && environment.Headers.TryGetValue("Content-Type", out value))
                       ? value
                       : null;
        }

        public static T Get<T>(this Environment environment, string name)
        {
            object value;
            return environment.TryGetValue(name, out value) ? (T)value : default(T);
        }
    }
}