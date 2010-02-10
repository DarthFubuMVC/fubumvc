using System.Collections.Generic;
using FubuMVC.Core.Models;

namespace FubuMVC.Core.Registration.DSL
{
    public class ModelsExpression
    {
        private readonly IList<IConfigurationAction> _configurationActions;

        public ModelsExpression(IList<IConfigurationAction> configurationActions)
        {
            _configurationActions = configurationActions;
        }

        public ModelsExpression ConvertUsing<T>() where T : IConverterFamily
        {
            _configurationActions.Add(
                new LambdaConfigurationAction(graph => graph.Services.AddService<IConverterFamily, T>()));
            return this;
        }
    }
}