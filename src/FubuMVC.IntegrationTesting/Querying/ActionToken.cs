using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.IntegrationTesting.Querying
{
    public class ActionToken
    {
        public ActionToken()
        {
        }



        public ActionToken(ActionCall call)
        {
            MethodName = call.Method.Name;
            HandlerType = new TypeToken(call.HandlerType);

            if (call.HasInput)
            {
                InputType = new TypeToken(call.InputType());
            }

            if (call.HasOutput)
            {
                OutputType = new TypeToken(call.OutputType());
            }
        }

        public TypeToken InputType { get; set; }
        public TypeToken OutputType { get; set; }
        public string MethodName { get; set; }
        public TypeToken HandlerType { get; set; }
    
        public string Description
        {
            get
            {
                return "{0}.{1}()".ToFormat(HandlerType.Name, MethodName);
            }
        }
    }
}