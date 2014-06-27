using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Bottles;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model
{
    public abstract class ViewFacility<T> : IViewFacility, IFubuRegistryExtension where T : class,ITemplateFile
    {
        private readonly IList<BottleViews<T>> _bottles = new List<BottleViews<T>>();
        private List<T> _views = new List<T>();

        public abstract Func<IFubuFile, T> CreateBuilder(SettingsCollection settings);

        public abstract FileSet FindMatching(SettingsCollection settings);

        public void Configure(FubuRegistry registry)
        {
            registry.AlterSettings<ViewEngineSettings>(x => x.AddFacility(this));
            registry.Services(registerServices);

            registry.AlterSettings<CommonViewNamespaces>(addNamespacesForViews);

            registry.AlterSettings<AssetSettings>(registerWatchSettings);
        }

        protected abstract void registerWatchSettings(AssetSettings settings);

        protected abstract void addNamespacesForViews(CommonViewNamespaces namespaces);

        protected abstract void registerServices(ServiceRegistry services);

        public virtual void Fill(ViewEngineSettings settings, BehaviorGraph graph)
        {
            var builder = CreateBuilder(graph.Settings);
            var match = FindMatching(graph.Settings);

            // HAS TO BE SHALLOW
            match.DeepSearch = false;

            graph.Files.AllFolders.Each(folder => {
                var bottle = new BottleViews<T>(this, folder, builder, settings, match);
                _bottles.Add(bottle);
            });

            LayoutAttachment = PackageRegistry.Timer.RecordTask("Attaching Layouts for " + GetType().Name,
                () => {
                    AttachLayouts(settings);
                });

            _views = _bottles.SelectMany(x => x.AllViews()).ToList();


        }

        protected virtual void precompile(BehaviorGraph graph)
        {
            // do nothing
        }

        public Task LayoutAttachment { get; private set; }
        public void AttachViewModels(ViewTypePool types, ITemplateLogger logger)
        {
            _bottles.Each(x => x.AttachViewModels(types, logger));
        }

        public abstract void ReadSharedNamespaces(CommonViewNamespaces namespaces);

        public void AttachLayouts(ViewEngineSettings settings)
        {
            _bottles.Each(x => x.AttachLayouts(settings.ApplicationLayoutName));
        }

        public ViewEngineSettings Settings { get; set; }
        public Type TemplateType { get { return typeof (T); } }

        public IEnumerable<BottleViews<T>> Bottles
        {
            get { return _bottles; }
        }

        public IEnumerable<IViewToken> AllViews()
        {
            return _views.OfType<IViewToken>();
        }

        public IEnumerable<T> AllTemplates()
        {
            return _views;
        } 

        public ITemplateFile FindInShared(string viewName)
        {
            foreach (var bottle in _bottles)
            {
                var view = bottle.FindInShared(viewName);
                if (view != null) return view;
            }

            return null;
        }

        public T FindPartial(T template, string name)
        {
            var partialName = "_" + name;
            T returnValue = null;

            // first look in the same bottle.
            var bottle = _bottles.FirstOrDefault(x => x.Provenance == template.Origin);
            
            if (bottle != null)
            {
                returnValue = bottle.AllViews().FirstOrDefault(x => x.Name() == partialName);
            }

            if (returnValue == null)
            {
                returnValue = AllViews().FirstOrDefault(x => x.Name() == partialName) as T;
            }

            return returnValue;
        }

        public IEnumerable<string> SharedPaths()
        {
            return _bottles.SelectMany(x => x.SharedPaths()).ToArray();
        }


    }
}