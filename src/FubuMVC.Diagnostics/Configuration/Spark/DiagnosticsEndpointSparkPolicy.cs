using System;
using System.Collections.Generic;
using System.Text;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Configuration.Policies;
using FubuMVC.Diagnostics.Endpoints;
using Spark.Web.FubuMVC.Extensions;
using Spark.Web.FubuMVC.Registration;

namespace FubuMVC.Diagnostics.Configuration.Spark
{
    public class DiagnosticsEndpointSparkPolicy : ISparkPolicy
    {
        private readonly List<Type> _markerTypes;

        public DiagnosticsEndpointSparkPolicy(params Type[] markerTypes)
        {
            _markerTypes = new List<Type>(markerTypes);
            _markerTypes.Fill(typeof (DiagnosticsEndpointMarker));
        }

        public bool Matches(ActionCall call)
        {
            return call.IsDiagnosticsCall();
        }

        public string BuildViewLocator(ActionCall call)
        {
            var strippedName = call.HandlerType.FullName.RemoveSuffix(DiagnosticsEndpointUrlPolicy.ENDPOINT);
            _markerTypes.Each(type => strippedName = strippedName.Replace(type.Namespace + ".", string.Empty));

            if (!strippedName.Contains("."))
            {
                return string.Empty;
            }

            var viewLocator = new StringBuilder();
            while (strippedName.Contains("."))
            {
                viewLocator.Append(strippedName.Substring(0, strippedName.IndexOf(".")));
                strippedName = strippedName.Substring(strippedName.IndexOf(".") + 1);

                var hasNext = strippedName.Contains(".");
                if (hasNext)
                {
                    viewLocator.Append("\\");
                }
            }

            return viewLocator.ToString();
        }

        public string BuildViewName(ActionCall call)
        {
            return call.HandlerType.Name.RemoveSuffix(DiagnosticsEndpointUrlPolicy.ENDPOINT);
        }
    }
}