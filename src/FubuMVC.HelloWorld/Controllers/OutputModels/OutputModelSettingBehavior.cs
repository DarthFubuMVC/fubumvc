using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using StructureMap;

namespace FubuMVC.HelloWorld.Controllers.OutputModels
{
    public class OutputModelSettingBehavior<TOutputModel> : BasicBehavior
        where TOutputModel : class
    {
        private readonly IFubuRequest _request;
        private readonly IContainer _container;

        public OutputModelSettingBehavior(IFubuRequest request, IContainer container)
            : base(PartialBehavior.Executes)
        {
            _request = request;
            _container = container;
        }

        protected override DoNext performInvoke()
        {
            BindSettingsProperties();

            return DoNext.Continue;
        }

        public void BindSettingsProperties()
        {
            var viewModel = _request.Find<TOutputModel>().First();
            _container.BuildUp(viewModel);
        }
    }

public class OutputModelSettingBehaviorConfiguration : IConfigurationAction
{
    public void Configure(BehaviorGraph graph)
    {
        graph.Actions()
            .Where(x => x.HasOutput &&
                        x.OutputType().GetProperties()
                            .Any(p => p.Name.EndsWith("Settings")))
            .Each(x => x.AddAfter(new Wrapper(
                typeof (OutputModelSettingBehavior<>)
                .MakeGenericType(x.OutputType()))));
    }
}

}