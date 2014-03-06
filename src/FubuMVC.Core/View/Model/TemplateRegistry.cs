using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.View.Model
{
    // TODO: Reconsider this
    [MarkedForTermination("Adds little value")]
    public class TemplateRegistry<T> : ITemplateRegistry<T> where T : ITemplateFile
    {
        private readonly Cache<string, T> _templates = new Cache<string, T>();

        public TemplateRegistry() : this(Enumerable.Empty<T>()) {}
        public TemplateRegistry(IEnumerable<T> templates)
        {
            templates.ToList().Each(Register);
        }

        public void Register(T template)
        {
            _templates.Fill(template.FilePath, template);
        } 

        public IEnumerable<T> ByNameUnderDirectories(string name, IEnumerable<string> directories)
        {
            return directories
                .SelectMany(local => this.Where(x => x.Name() == name && x.DirectoryPath() == local));
        }

        public T FirstByName(string name)
        {
            return this.FirstOrDefault(x => x.Name() == name);
        }

        public IEnumerable<T> ByOrigin(string origin)
        {
            return this.Where(x => x.Origin == origin);
        }

        // TODO: UT
        public IEnumerable<TDescriptor> DescriptorsWithViewModels<TDescriptor>() where TDescriptor : ViewDescriptor<T>
        {
            return this.Where(t => t.Descriptor is TDescriptor)
                .Select(x => x.Descriptor.As<TDescriptor>())
                .Where(x => x.Template.HasViewModel());
        }

        public IEnumerable<T> FromHost()
        {
            return this.Where(x => x.FromHost());
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _templates.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Obsolete("Replace with the existing ViewBag")]
    public interface ITemplateRegistry<out T> : IEnumerable<T> where T : ITemplateFile
    {
        IEnumerable<T> ByNameUnderDirectories(string name, IEnumerable<string> directories);
        IEnumerable<T> ByOrigin(string origin);
        IEnumerable<T> FromHost();
    }
}