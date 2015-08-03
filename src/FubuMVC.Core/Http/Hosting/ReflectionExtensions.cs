namespace FubuMVC.Core.Http.Hosting
{
    public static class ReflectionExtensions
    {
        public static void SetProperty(this object target, string name, object value)
        {
            var property = target.GetType().GetProperty(name);
            property.SetValue(target, value);
        }

        public static object GetProperty(this object target, string name)
        {
            var property = target.GetType().GetProperty(name);
            return property.GetValue(target);
        }

        public static object Call(this object target, string methodName, params object[] args)
        {
            var method = target.GetType().GetMethod(methodName);
            return method.Invoke(target, args);
        }
    }
}