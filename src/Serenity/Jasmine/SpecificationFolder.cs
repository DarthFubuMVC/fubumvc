using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;

namespace Serenity.Jasmine
{
    public interface ISpecNode
    {
        SpecPath Path();
        IEnumerable<Specification> AllSpecifications { get; }
        IEnumerable<ISpecNode> AllNodes { get; }
        string FullName { get; }

        ISpecNode Parent();
    }

    public class SpecificationFolder : ISpecNode
    {
        private readonly Cache<string, SpecificationFolder> _children;
        private readonly string _name;
        private readonly SpecificationFolder _parent;
        private readonly IList<Specification> _specifications = new List<Specification>();

        public SpecificationFolder(string name)
        {
            _name = name;

            _children = new Cache<string, SpecificationFolder>(path => new SpecificationFolder(path, this));
        }

        public SpecificationFolder(string name, SpecificationFolder parent) : this(name)
        {
            _parent = parent;
        }

        public SpecificationFolder Parent
        {
            get { return _parent; }
        }

        public string FullName
        {
            get { return _parent == null ? _name : _parent.FullName + "/" + _name; }
        }

        ISpecNode ISpecNode.Parent()
        {
            return Parent;
        }

        public IEnumerable<Specification> Specifications
        {
            get { return _specifications; }
        }

        public IEnumerable<Specification> AllSpecifications
        {
            get
            {
                foreach (var folder in _children)
                {
                    foreach (var spec in folder.AllSpecifications)
                    {
                        yield return spec;
                    }
                }

                foreach (var spec in _specifications)
                {
                    yield return spec;
                }
            }
        }

        public IEnumerable<ISpecNode> AllNodes
        {
            get
            {
                foreach (var folder in _children)
                {
                    yield return folder;

                    foreach (var node in folder.AllNodes)
                    {
                        yield return node;
                    }
                }

                foreach (var spec in _specifications)
                {
                    yield return spec;
                }

            }
        }

        public SpecPath Path()
        {
            var list = new List<string>{
                _name
            };

            var parent = Parent;
            while (parent != null)
            {
                list.Add(parent._name);
                parent = parent.Parent;
            }

            list.Reverse();

            return new SpecPath(list);
        }

        public SpecificationFolder ChildFolderFor(string name)
        {
            return ChildFolderFor(new SpecPath(name));
        }

        public SpecificationFolder ChildFolderFor(SpecPath path)
        {
            return path.Parts.Count == 1
                       ? _children[path.TopFolder]
                       : _children[path.TopFolder].ChildFolderFor(path.ChildPath());
        }


        public void AddSpecs(IEnumerable<AssetFile> assetFiles)
        {
            var specs = assetFiles.Select(x => new Specification(x){
                Parent = this
            });
            _specifications.AddRange(specs);
        }
    }
}