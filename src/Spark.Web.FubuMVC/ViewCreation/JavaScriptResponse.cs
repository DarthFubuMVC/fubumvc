using System;
using FubuMVC.Core.Registration.Nodes;
using System.Reflection;
namespace Spark.Web.FubuMVC.ViewCreation
{
    public class JavaScriptResponse
    {
        public string ViewName { get; set; }
        
        public static JavaScriptResponse GetResponse(ActionCall call)
        {
            var controller = Activator.CreateInstance(call.HandlerType, null);

            var output =
                call.HandlerType.InvokeMember(
                call.Method.Name,
                BindingFlags.InvokeMethod, null, controller, new object[] { }) as JavaScriptResponse;

            return output;
        }

    }
}
