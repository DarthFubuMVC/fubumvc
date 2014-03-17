using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model
{
    public abstract class ViewFacility<T> : IViewFacility where T : ITemplateFile
    {
        private readonly IList<BottleViews<T>> _bottles = new List<BottleViews<T>>();
        private List<T> _views;

        public abstract Func<IFubuFile, T> CreateBuilder(SettingsCollection settings);

        public abstract FileSet FindMatching(SettingsCollection settings);

        public void Fill(ViewEngines viewEngines, BehaviorGraph graph)
        {
            var builder = CreateBuilder(graph.Settings);
            var match = FindMatching(graph.Settings);

            graph.Files.AllFolders.Each(folder => {
                var bottle = new BottleViews<T>(folder, builder, viewEngines, match);
                _bottles.Add(bottle);
            });

            _views = _bottles.SelectMany(x => x.AllViews()).ToList();
        }

        public IEnumerable<BottleViews<T>> Bottles
        {
            get { return _bottles; }
        }

        public IEnumerable<IViewToken> AllViews()
        {
            return _views.OfType<IViewToken>();
        } 

    }
}