using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using Spark.Web.FubuMVC.ViewCreation;

namespace Spark.Web.FubuMVC.ViewLocation
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly Func<Type, bool> _sparkActionEndPointConventionFilter;
        private readonly SparkViewFactory _viewFactory;

        public SparkViewFacility(SparkViewFactory viewFactory, Func<Type, bool> sparkActionEndPointConventionFilter)
        {
            _viewFactory = viewFactory;
            _sparkActionEndPointConventionFilter = sparkActionEndPointConventionFilter;
        }

        #region IViewFacility Members

        public IEnumerable<IViewToken> FindViews(TypePool types)
        {
            IEnumerable<Type> actionTypes = types.TypesMatching(_sparkActionEndPointConventionFilter);

            var viewTokens = new List<IViewToken>();
            actionTypes.Each(actionType =>
                                 {
                                     var sparkBatchEntry = new SparkBatchEntry {ControllerType = actionType};
                                     IList<SparkViewDescriptor> descriptors = _viewFactory.CreateDescriptors(sparkBatchEntry);
                                     viewTokens.Add(new SparkViewToken(descriptors));
                                 });
            return viewTokens;
        }

        #endregion
    }
}