using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using StructureMap.Graph.Scanning;

namespace FubuMVC.Core.ServiceBus.Web
{
    [Description("Finds actions for classes that implement ISendMessages")]
    public class SendsMessageActionSource : IActionSource
    {
        public Task<ActionCall[]> FindActions(Assembly applicationAssembly)
        {
            return TypeRepository.FindTypes(applicationAssembly,
                TypeClassification.Concretes | TypeClassification.Closed, t => t.CanBeCastTo<ISendMessages>())
                .ContinueWith(t =>
                {
                    return t.Result.SelectMany(type =>
                    {
                        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .Where(ActionSource.IsCandidate);

                        return methods.Select(method => new ActionCall(type, method));
                    }).ToArray();
                });
        }
    }
}