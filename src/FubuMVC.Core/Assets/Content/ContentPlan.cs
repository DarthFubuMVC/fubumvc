using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuCore;
using System.Linq;

namespace FubuMVC.Core.Assets.Content
{
    public class ContentPlan
    {
        private readonly string _name;
        private readonly IList<IContentSource> _sources = new List<IContentSource>();

        public ContentPlan(string name, IEnumerable<AssetFile> files)
        {
            _name = name;
            _sources.AddRange(files.Select(InitialSourceForAssetFile));
        }

        public void AcceptVisitor(IContentPlanVisitor visitor)
        {
            _sources.Each(s => acceptVisitorForSource(visitor, s));
        }

        private static void acceptVisitorForSource(IContentPlanVisitor visitor, IContentSource source)
        {
            visitor.Push(source);

            source.InnerSources.Each(s => acceptVisitorForSource(visitor, s));

            visitor.Pop();
        }


        public string Name
        {
            get { return _name; }
        }

        public static IContentSource InitialSourceForAssetFile(AssetFile file)
        {
            if (file.FullPath.IsNotEmpty())
            {
                return new FileRead(file);
            }

            throw new ArgumentOutOfRangeException("Don't know how to determine a content source for an AssetFile without a FullPath (yet)");
        }

        public IEnumerable<IContentSource> AllSources
        {
            get
            {
                return _sources;
            }
        }

        public Combination Combine(IEnumerable<IContentSource> sources)
        {
            int index = findIndexOfSource(sources.First());
            _sources.RemoveAll(x => sources.Contains(x));

            var combination = new Combination(sources);
            _sources.Insert(index, combination);

            return combination;
        }

        public IContentSource ApplyTransform(IContentSource source, Type transformerType)
        {
            int index = findIndexOfSource(source);

            _sources.Remove(source);
            var transformer = typeof (Transform<>).CloseAndBuildAs<IContentSource>(source, transformerType);
            _sources.Insert(index, transformer);

            return transformer;
        }

        private int findIndexOfSource(IContentSource source)
        {
            var index = _sources.IndexOf(source);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("Source {0} is not in the top level of this plan".ToFormat(source));
            }
            return index;
        }

        public IContentSource FindForFile(AssetFile file)
        {
            return AllSources.FirstOrDefault(x => x.Files.Contains(file));
        }
    }
}