using System;
using System.Collections.Generic;

namespace Spark.Web.FubuMVC
{
    public class ActionContext
    {
        public ActionContext(string actionNamespace, string actionName, Action<IDictionary<string, object>> configuration = null)
        {
            ActionNamespace = actionNamespace;
            ActionName = actionName;
            Params = new Dictionary<string, object>();

            if(configuration != null)
            {
                configuration(Params);
            }
        }

        public string ActionNamespace { get; private set; }
        public string ActionName { get; private set; }

        public IDictionary<string, object> Params { get; private set; } 
    }
}