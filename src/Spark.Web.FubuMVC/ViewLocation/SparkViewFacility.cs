using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using Spark.Web.FubuMVC.ViewCreation;

namespace Spark.Web.FubuMVC.ViewLocation
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly Func<Type, bool> _sparkActionEndPointConventionFilter;
        private readonly SparkViewFactory _viewFactory;
        private Func<string, string> _getActionNameFromCallConvention;

        public SparkViewFacility(SparkViewFactory viewFactory, 
            Func<Type, bool> sparkActionEndPointConventionFilter, 
            Func<string, string> getActionNameFromCallConvention)
        {
            _viewFactory = viewFactory;
            _sparkActionEndPointConventionFilter = sparkActionEndPointConventionFilter;
            _getActionNameFromCallConvention = getActionNameFromCallConvention;
        }

        #region IViewFacility Members

        public IEnumerable<IViewToken> FindViews(TypePool types)
        {
            IEnumerable<Type> actionTypes = types.TypesMatching(_sparkActionEndPointConventionFilter);

            var viewTokens = new List<IViewToken>();
            actionTypes.Each(actionType =>
                                 {
                                     var sparkBatchEntry = new SparkBatchEntry {ControllerType = actionType};
                                     IList<SparkViewDescriptor> descriptors = _viewFactory.CreateDescriptors(sparkBatchEntry, _getActionNameFromCallConvention);
                                     viewTokens.Add(new SparkViewToken(descriptors));
                                 });
            return viewTokens;
        }

        #endregion
    }
}