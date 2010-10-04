using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using Spark.Web.FubuMVC.ViewCreation;

namespace Spark.Web.FubuMVC.ViewLocation
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly Func<Type, bool> _actionEndPointFilter;
        private readonly SparkViewFactory _viewFactory;
        private Func<Type, string> _getViewLocatorNameFromActionType;

        public SparkViewFacility(SparkViewFactory viewFactory, 
            Func<Type, bool> actionEndPointFilter,
            Func<Type, string> getViewLocatorNameFromActionType)
        {
            _viewFactory = viewFactory;
            _actionEndPointFilter = actionEndPointFilter;
            _getViewLocatorNameFromActionType = getViewLocatorNameFromActionType;
        }

        #region IViewFacility Members

        public IEnumerable<IViewToken> FindViews(TypePool types)
        {
            IEnumerable<Type> actionTypes = types.TypesMatching(_actionEndPointFilter);

            var viewTokens = new List<IViewToken>();
            actionTypes.Each(actionType =>
                                 {
                                     var sparkBatchEntry = new SparkBatchEntry { ControllerType = actionType };
                                     string viewLocatorName = _getViewLocatorNameFromActionType(actionType);
                                     IList<SparkViewDescriptor> descriptors = _viewFactory.CreateDescriptors(sparkBatchEntry, viewLocatorName);
                                     viewTokens.Add(new SparkViewToken(descriptors));
                                 });
            return viewTokens;
        }

        #endregion
    }
}