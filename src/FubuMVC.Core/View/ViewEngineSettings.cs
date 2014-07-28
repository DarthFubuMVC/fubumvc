using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bottles;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Attachment;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Core.View
{
    [Title("View Engine Registration and Settings")]
    public class ViewEngineSettings : DescribesItself
    {
        public static readonly IFileSystem FileSystem = new FileSystem();

        private readonly IList<Func<IViewToken, bool>> _excludes = new List<Func<IViewToken, bool>>();
        private readonly List<IViewFacility> _facilities = new List<IViewFacility>();
        private readonly IList<ViewTokenPolicy> _viewPolicies = new List<ViewTokenPolicy>();

        public void Describe(Description description)
        {
            description.ShortDescription = "Registers active View Engines and governs the behavior of view attachment";

            description.Properties["Shared Layout Folders"] = SharedLayoutFolders.Join(", ");
            description.Properties["Application Layout Name"] = ApplicationLayoutName;

            description.AddList("Facilities", _facilities);
            if (_viewPolicies.Any())
            {
                description.AddList("Policies", _viewPolicies);
            }
        }

        /// <summary>
        /// List of folder names that should be treated as shared layout folders of views.
        /// Default is ["Shared"]
        /// </summary>
        public readonly IList<string> SharedLayoutFolders = new List<string>{"Shared"};

        /// <summary>
        /// The name of the default layout file for the application.
        /// The default is "Application"
        /// </summary>
        public string ApplicationLayoutName = "Application";

        /// <summary>
        /// All of the registered view engines in this application
        /// </summary>
        public IEnumerable<IViewFacility> Facilities
        {
            get { return _facilities; }
        }


        /// <summary>
        ///   Define a view activation policy for views matching the filter.
        ///   <seealso cref = "IfTheInputModelOfTheViewMatches" />
        /// </summary>
        public PageActivationExpression IfTheViewMatches(Func<IViewToken, bool> filter)
        {
            return new PageActivationExpression(this, filter);
        }

        /// <summary>
        ///   Define a view activation policy by matching on the input type of a view.
        ///   A view activation element implements <see cref = "IPageActivationAction" /> and takes part in setting up a View instance correctly
        ///   at runtime.
        /// </summary>
        public PageActivationExpression IfTheInputModelOfTheViewMatches(Func<Type, bool> filter)
        {
            Func<IViewToken, bool> combined = viewToken => { return filter(viewToken.ViewModel); };

            return IfTheViewMatches(combined);
        }

        public Task<ViewBag> BuildViewBag(BehaviorGraph graph)
        {
            return PackageRegistry.Timer.RecordTask("Building the View Bag", () => {
                var viewFinders = _facilities.Select(x =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        x.Fill(this, graph);
                        return x.AllViews();
                    });
                });

                var views = viewFinders.SelectMany(x => x.Result).ToList();
                _viewPolicies.Each(x => x.Alter(views));

                var logger = TemplateLogger.Default();
                var types = new ViewTypePool(graph);

                // Attaching the view models

                _facilities.Each(x => x.AttachViewModels(types, logger));

                return new ViewBag(views);
            });
        }

        public bool IsExcluded(IViewToken token)
        {
            return _excludes.Any(x => x(token));
        }

        public bool IsSharedFolder(string folder)
        {
            return SharedLayoutFolders.Contains(Path.GetFileNameWithoutExtension(folder));
        }

        /// <summary>
        /// Programmatically add a new view facility.  This method is generally called
        /// by each Bottle and should not be necessary by users
        /// </summary>
        /// <param name="facility"></param>
        public void AddFacility(IViewFacility facility)
        {
            Type typeOfFacility = facility.GetType();
            if (_facilities.Any(f => f.GetType() == typeOfFacility)) return;

            facility.Settings = this;

            _facilities.Add(facility);
        }

        /// <summary>
        /// Add a new ViewTokenPolicy to alter or configure the behavior of a view
        /// at configuation time
        /// </summary>
        /// <param name="policy"></param>
        public void AddPolicy(ViewTokenPolicy policy)
        {
            _viewPolicies.Add(policy);
        }

        /// <summary>
        /// Exclude discovered views from being used with the view attachment.  Helpful for being able
        /// to run FubuMVC simultaneously with ASP.Net MVC or some other web framework in the same
        /// application
        /// </summary>
        /// <param name="filter"></param>
        public void ExcludeViews(Func<IViewToken, bool> filter)
        {
            _excludes.Add(filter);
        }

        public T FindPartial<T>(T template, string name) where T : class, ITemplateFile
        {
            var facility = _facilities.Single(x => x.TemplateType == typeof (T)).As<ViewFacility<T>>();
            return facility.FindPartial(template, name);
        }


        private readonly string[] _ignoredFolders = new[] { "bin", "obj", "fubu-content", "node_modules" };
        public bool FolderShouldBeIgnored(string folder)
        {
            var segment = folder.Replace('\\', '/').Trim('/').Split('/').Last();

            return _ignoredFolders.Any(x => x.EqualsIgnoreCase(segment));
        }
    }

    /// <summary>
    /// Used to create a policy altering views represented by an IViewToken
    /// </summary>
    public class ViewTokenPolicy : DescribesItself
    {
        private readonly Action<IViewToken> _alteration;
        private readonly string _description;
        private readonly Func<IViewToken, bool> _filter;

        public ViewTokenPolicy(Func<IViewToken, bool> filter, Action<IViewToken> alteration, string description)
        {
            _filter = filter;
            _alteration = alteration;
            _description = description;
        }

        #region DescribesItself Members

        public void Describe(Description description)
        {
            description.Title = _description;
        }

        #endregion

        public void Alter(IEnumerable<IViewToken> views)
        {
            views.Where(_filter).Each(_alteration);
        }

        public override string ToString()
        {
            return string.Format("ViewTokenPolicy: {0}", _description);
        }


    }
}