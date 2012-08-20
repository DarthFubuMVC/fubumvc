using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuCore;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Content
{
    public class ContentPlan : IContentSource
    {
        private readonly string _name;
        private readonly IList<IContentSource> _sources = new List<IContentSource>();
        private readonly MimeType _mimeType;

        public ContentPlan(string name, IEnumerable<AssetFile> files)
        {
            _name = name;
            _sources.AddRange(files.Select(InitialSourceForAssetFile));

            if (files.Any())
            {
                var mimeTypes = files.Select(x => x.MimeType).Distinct();
                if (mimeTypes.Count() > 1)
                {
                    throw new ArgumentOutOfRangeException("The list of files is a mix of MimeTypes");
                }

                _mimeType = mimeTypes.Single();
            }


        }

        public MimeType MimeType
        {
            get { return _mimeType; }
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

        public IEnumerable<IContentSource> GetAllSources()
        {
            return _sources.ToArray();
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
            return GetAllSources().FirstOrDefault(x => x.Files.Contains(file));
        }

        public void CombineAll()
        {
            if (GetAllSources().Count() > 1)
            {
                Combine(GetAllSources().ToList());
            }
        }

        public IContentSource Top()
        {
            CombineAll();
            return _sources.Single();
        }

        string IContentSource.GetContent(IContentPipeline pipeline)
        {
            return Top().GetContent(pipeline);
        }

        IEnumerable<AssetFile> IContentSource.Files
        {
            get
            {
                if (!_sources.Any()) return Enumerable.Empty<AssetFile>();

                return Top().Files;
            }
        }

        IEnumerable<IContentSource> IContentSource.InnerSources
        {
            get { return Top().InnerSources; }
        }
    }
}