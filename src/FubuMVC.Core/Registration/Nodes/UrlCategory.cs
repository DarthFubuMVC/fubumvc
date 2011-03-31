using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Registration.Nodes
{
    /// <summary>
    /// Categorizes a BehaviorChain endpoint for IUrlRegistry,
    /// IEndpointService, and IAuthorizationPreviewService.  
    /// Especially useful when there is multiple BehaviorChain's
    /// in the system with the same input model
    /// </summary>
    public class UrlCategory
    {
        public UrlCategory()
        {
            Creates = new List<Type>();
        }

        /// <summary>
        /// Url category.  Examples are "New", "Find", "View", "Edit"
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Registers this behavior chain for calls to 
        /// IUrlRegistry.UrlForNew(type) methods
        /// </summary>
        public IList<Type> Creates { get; private set; }
    }
}