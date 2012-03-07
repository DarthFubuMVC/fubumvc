using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Util;

namespace FubuMVC.Core.Http
{
    public static class RequestDataExtensions
    {
        public static void AddValues(this IRequestData request, RequestDataSource source, IKeyValues values)
        {
            request.AddValues(source.ToString(), values);
        }

        public static IValueSource ValuesFor(this IRequestData request, RequestDataSource source)
        {
            return request.ValuesFor(source.ToString());
        }
    }
}